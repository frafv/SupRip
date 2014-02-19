using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SupRip
{
	internal class SubtitleImage
	{
		private class NoSubtitleTextException : Exception
		{
		}
		/// <summary>
		/// Text line
		/// </summary>
		private class TextLine
		{
			/// <summary>
			/// Text line number
			/// </summary>
			public int Num
			{
				get;
				set;
			}
			/// <summary>
			/// Text line height in pixels
			/// </summary>
			public int Height
			{
				get
				{
					return this.End - this.Start;
				}
			}
			/// <summary>
			/// Text line Y start
			/// </summary>
			public int Start
			{
				get;
				set;
			}
			/// <summary>
			/// Text line Y end
			/// </summary>
			public int End
			{
				get;
				set;
			}

#if DEBUG
			public LinkedList<PointF> debugPoints = new LinkedList<PointF>();
			public LinkedList<PointF[]> debugLines = new LinkedList<PointF[]>();
#endif

			/// <summary>
			/// Text line - image block
			/// </summary>
			/// <param name="n">Number</param>
			/// <param name="s">Y start</param>
			/// <param name="e">Y end</param>
			public TextLine(int n, int s, int e)
			{
				this.Num = n;
				this.Start = s;
				this.End = e;
			}

			/// <summary>
			/// Split image into text lines - image blocks
			/// </summary>
			/// <param name="workArray">Pixels bitmap</param>
			/// <returns></returns>
			internal static TextLine[] Find(int[,] nextArray)
			{
				var list = new List<TextLine>();
				int columnHeight = nextArray.GetLength(0);
				int lineWidth = nextArray.GetLength(1);
				int line = 0;
				int line1, line2;
				for (int num = 0; line < columnHeight; num++)
				{
					while (line < columnHeight && nextArray[line, 0] == lineWidth)
						line++;
					line1 = line;
					while (line < columnHeight && nextArray[line, 0] != lineWidth)
						line++;
					line2 = line;
					if (line2 != line1)
						list.Add(new SubtitleImage.TextLine(num, line1, line2));
				}
				TextLine textLine = null;
				int i = 0;
				while (i < list.Count)
				{
					if (list[i].Height < minLineHeight)
					{
						if (list.Count == 1)
						{
							throw new Exception("Could only find one line of text, and it's smaller than " + minLineHeight + " pixels");
						}
						if (i == 0)
						{
							list[1].Start = list[0].Start;
						}
						else
						{
							if (i == list.Count - 1)
							{
								textLine.End = list[i].End;
							}
							else
							{
								int prevSpace = list[i].Start - textLine.End;
								int nextSpace = list[i + 1].Start - list[i].End;
								if (prevSpace < minLineSpace || nextSpace / prevSpace > 2)
								{
									textLine.End = list[i].End;
								}
								else if (nextSpace < minLineSpace || prevSpace / nextSpace > 2)
								{
									list[i + 1].Start = list[i].Start;
								}
								else if (prevSpace < nextSpace)
								{
									textLine.End = list[i].End;
								}
								else
								{
									list[i + 1].Start = list[i].Start;
								}
							}
						}
						list.RemoveAt(i);
					}
					else
					{
						textLine = list[i];
						i++;
					}
				}
				return list.ToArray();
			}

			private static float GetNextLeft(byte[,] workArray, int[,] nextArray, int line, ref int left, byte pixelLevel1, byte pixelLevel2)
			{
				left = -nextArray[line, left];
				if (left >= workArray.GetLength(1)) return left;
				byte b1 = workArray[line, left];
				byte b2 = workArray[line, left - 1];
				if (b1 > pixelLevel1 && b2 > pixelLevel1)
					return left - (float)(b1 - pixelLevel2) / (float)(b1 - b2);
				else
					return left - (float)(b1 - pixelLevel1) / (float)(b1 - b2);
			}

			private static float GetNextRight(byte[,] workArray, int[,] nextArray, int line, ref int right, byte pixelLevel1, byte pixelLevel2)
			{
				right = nextArray[line, right] - 1;
				if (right >= workArray.GetLength(1) - 1) return right;
				byte b1 = workArray[line, right];
				byte b2 = workArray[line, right + 1];
				if (b1 > pixelLevel1 && b2 > pixelLevel1)
					return right + (float)(b1 - pixelLevel2) / (float)(b1 - b2);
				else
					return right + (float)(b1 - pixelLevel1) / (float)(b1 - b2);
			}

			internal Space[] FindSpaces(byte[,] workArray, int[,] nextArray, byte pixelLevel1, byte pixelLevel2)
			{
				var spaces = new LinkedList<Space>();
				int lineHeight = this.Height;
				int lineWidth = nextArray.GetLength(1);
				int start = this.Start - 1;
				spaces.AddLast(new Space(lineHeight + 1)).Value.SetRange(0, 0.0f, 0.0f);
				spaces.AddLast(new Space(lineHeight + 1)).Value.SetRange(0, 0.0f, lineWidth - 1);
				for (int i = 1; i <= lineHeight; i++)
				{
					var spaceNode = spaces.First;
					spaceNode.Value.SetRange(i, 0.0f, 0.0f);
					spaceNode = spaceNode.Next;
					while (spaceNode != null)
					{
						var space = spaceNode.Value;
						var spaceLeft = spaceNode.Previous.Value;
						float left = space.Left[i - 1];
						int ileft = (int)Math.Ceiling(left);
						if (nextArray[i + start, ileft] > 0.0f)
						{
							//First space found at left
							if (left - Space.smoothStep >= spaceLeft.Left[i] && nextArray[i + start, ileft - 1] > 0.0f)
							{
								left -= Space.smoothStep;
								ileft = (int)Math.Ceiling(left);
							}
						}
						else
						{
							//Find first space
							left = GetNextLeft(workArray, nextArray, i + start, ref ileft, pixelLevel1, pixelLevel2);
							//End of space
							if (left >= space.Right[i - 1] + Space.smoothStep)
							{
								//Delete space
								var old = spaceNode;
								spaceNode = spaceNode.Next;
								spaces.Remove(old);
								if (spaces.First.Next == null)
									return new Space[] { };
								continue;
							}
							//Need left smooth
							if (left > space.Left[i - 1] + Space.smoothStep)
							{
								space.LeftSmooth(left, i);
							}
						}
						//Get Last space
						int iright = ileft;
						float right = GetNextRight(workArray, nextArray, i + start, ref iright, pixelLevel1, pixelLevel2);
						//Space enough long
						if (right >= space.Right[i - 1] - Space.smoothStep)
						{
							right = Math.Min(space.Right[i - 1] + Space.smoothStep, right);
						}
						else
						{
							//Next first space
							int inextLeft = iright + 1;
							float nextLeft = GetNextLeft(workArray, nextArray, i + start, ref inextLeft, pixelLevel1, pixelLevel2);
							//Space need to split
							if (nextLeft <= space.Right[i - 1])
							{
								var spaceRight = spaceNode.Next != null ? spaceNode.Next.Value : null;
								//Check right space including
								bool inRight = spaceRight != null && nextLeft >= spaceRight.Left[i - 1] && space.Right[i - 1] <= spaceRight.Right[i - 1];
								//Check left space including
								bool inLeft = left >= spaceLeft.Left[i] && right <= spaceLeft.Right[i];
								//Split space
								if (!inRight) //space not contains in right - else go to left
								{
									if (!inLeft) //space not contains in left and right - split
									{
										var add = new Space(lineHeight + 1);
										space.CopyTo(add, 0, i);
										spaces.AddAfter(spaceNode, add);
										//Need left smooth new space
										add.LeftSmooth(nextLeft, i);
									}
									else //space contains in left only - go to right
									{
										space.LeftSmooth(nextLeft, i);
										continue;
									}
								}
							}
							//Need right smooth
							space.RightSmooth(right, i);
						}
						space.SetRange(i, left, right);
						spaceNode = spaceNode.Next;
					}
				}
				return spaces.Skip(1).ToArray();
			}

			private IEnumerable<SubtitleLetter> FindLetter(Space space, Space next, byte[,] workArray, int[,] nextArray)
			{
				int lineWidth = nextArray.GetLength(1);
				float spaceLeft = 0;
				float spaceRight = lineWidth - 1;
				float spaceWidth = lineWidth;
				int letterLeft = lineWidth;
				int letterRight = -1;
				int start = -1;
				int end = -1;

				if (space == null && this.Num != 0)
				{
					Rectangle rectangle = new Rectangle(1, this.Start - 20, 10, 25);
					yield return new SubtitleLetter(rectangle, "\r\n");
				}
				for (int line = 1; line <= this.Height; line++)
				{
					float left1 = space != null ? space.Left[line] : 0.0f;
					int ileft1 = (int)Math.Ceiling(left1);
					float right1 = space != null ? space.Right[line] : 0.0f;
					int iright1 = (int)Math.Floor(right1);
					float width1 = right1 - left1;
					float left2 = next != null ? next.Left[line] : lineWidth;
					int ileft2 = (int)Math.Ceiling(left2);
					if (left1 > spaceLeft) spaceLeft = left1;
					if (right1 < spaceRight) spaceRight = right1;
					if (width1 < spaceWidth) spaceWidth = width1;
					if (nextArray[line + this.Start - 1, ileft1] >= ileft2)
						continue;
					if (ileft2 > letterRight) letterRight = ileft2;
					if (iright1 < letterLeft) letterLeft = iright1;
					if (start < 0) start = line;
					end = line;
				}
				if (spaceRight - spaceLeft >= AppOptions.charSplitTolerance && spaceWidth > (float)AppOptions.minimumSpaceCharacterWidth)
				{
					var rectangle = new RectangleF(spaceLeft, this.Start + 4, spaceRight - spaceLeft + 1.0f, this.Height - 10);
					yield return new SubtitleLetter(rectangle, " ");
				}
				if (end > start)
				{
					Rectangle rectangle = new Rectangle(letterLeft + 1, start + this.Start - 1, letterRight - letterLeft - 1, end - start + 1);
#if DEBUG
					if (space != null)
					{
						float x1, x2; bool e;
						SubtitleLetter.CalcAngle(space.Right.Skip(start).Take(rectangle.Height).ToArray(), true, out x1, out x2, out e);
						lock (this.debugLines)
						{
							this.debugLines.AddLast(new PointF[]{new PointF(x1, rectangle.Top), new PointF(x2, rectangle.Bottom-1)});
						}
					}
					if (next != null)
					{
						float x3, x4; bool e;
						SubtitleLetter.CalcAngle(next.Left.Skip(start).Take(rectangle.Height).ToArray(), false, out x3, out x4, out e);
						lock (this.debugLines)
						{
							this.debugLines.AddLast(new PointF[] { new PointF(x3, rectangle.Top), new PointF(x4, rectangle.Bottom - 1) });
						}
					}
#endif
					yield return SubtitleLetter.Extract(workArray, rectangle,
						space != null ? space.Right.Skip(start).Take(rectangle.Height).ToArray() : null,
						next != null ? next.Left.Skip(start).Take(rectangle.Height).ToArray() : null,
						(rectangle.Top + rectangle.Bottom) - (this.End + this.Start) >> 1);
				}
			}

			public SubtitleLetter[] FindLetters(byte[,] workArray, int[,] nextArray, byte pixelLevel1, byte pixelLevel2)
			{
				var spaces = FindSpaces(workArray, nextArray, pixelLevel1, pixelLevel2);
#if DEBUG
				foreach (var _p in spaces.SelectMany(_l => Enumerable.Range(1, _l.Height - 1).SelectMany(_k => new PointF[] {
					new PointF(_l.Left[_k], _k + this.Start - 1), new PointF(_l.Right[_k], _k + this.Start - 1) })))
					this.debugPoints.AddLast(_p);
#endif
				var list = ParallelEnumerable.Range(-1, spaces.Length + 1).AsOrdered().SelectMany(index =>
					FindLetter(index >= 0 ? spaces[index] : null, index < spaces.Length - 1 ? spaces[index + 1] : null, workArray, nextArray)).ToList();
				var first = this.Num != 0 && list.Count > 1 ? list[1] : list.FirstOrDefault();
				var last = list.LastOrDefault();
				if (first != null && first.Text == " ") list.Remove(first);
				if (last != null && last.Text == " ") list.RemoveAt(list.Count - 1);
				var letters = list.ToArray();
				AdjustItalic(letters);
				return letters;
			}

			private double GetItalicAngle(SubtitleLetter[] letters)
			{
				if (!Double.IsNaN(SubtitleImage.italicAngle))
					return SubtitleImage.italicAngle;

				var angles = letters.Where(letter => !Double.IsNaN(letter.ExactAngle)).Select(letter => letter.ExactAngle).OrderBy(angle => angle).ToArray();
				int k = 0;
				double delta = 1.0 / SubtitleLetter.minAngleDiv;
				double maxAngle = 0.0;
				int maxCount = 0;
				while (k < angles.Length)
				{
					double start = angles[k];
					double end = start + delta;
					int count = 1 + angles.Skip(k + 1).TakeWhile(angle => angle <= end).Count();
					double sum = angles.Skip(k).Take(count).Sum();
					end = angles[k + count - 1];
					if (count > maxCount || count == maxCount && Math.Abs((end + start) / 2.0) < Math.Abs(maxAngle))
					{
						maxCount = count;
						maxAngle = sum;
					}
					if (k + count >= angles.Length) break;
					k++;
					k += angles.Skip(k).TakeWhile(angle => angle == start).Count();
				}
				if (Math.Abs(maxAngle) < delta) return Double.NaN;

				lock(SubtitleImage.italicSync)
				{
					if (!Double.IsNaN(SubtitleImage.italicAngle))
						return SubtitleImage.italicAngle;

					SubtitleImage.angleList += maxAngle;
					SubtitleImage.angleCount += maxCount;
					if (SubtitleImage.angleCount >= 20)
					{
						SubtitleImage.italicAngle = SubtitleImage.angleList / SubtitleImage.angleCount;
						return SubtitleImage.italicAngle;
					}
					return maxAngle / maxCount;
				}
			}

			public void AdjustItalic(SubtitleLetter[] letters)
			{
				double angle = GetItalicAngle(letters);
				if (angle == 0.0 || Double.IsNaN(angle)) return;
				var italic = new List<SubtitleLetter>();
				int wordStart = 0;
				int canStart = letters.Length;
				bool hasRegilar = false;
				for (int k = 0; k < letters.Length; k++)
				{
					var letter = letters[k];
					if (letter.Text == " " || letter.Text == "\r\n")
					{
						if (letter.Text == "\r\n")
							canStart = letters.Length;
						wordStart = k + 1;
						continue;
					}
					if (Double.IsNaN(letter.ExactAngle) || letter.ExactAngle == 0.0 || Math.Abs(letter.ExactAngle - angle) >= 1.0 / SubtitleLetter.minAngleDiv)
					{
						if (letter.ExactAngle == 0.0)
							hasRegilar = true;
						continue;
					}
					int wordEnd = k;
					for (int n = k + 1; n < letters.Length; n++)
					{
						var letter2 = letters[n];
						if (letter2.Text == " " || letter2.Text == "\r\n")
							break;
						wordEnd = n;
					}
					var word = letters.Skip(wordStart).Take(wordEnd - wordStart + 1).ToArray();
					int count1 = word.Count(l => !Double.IsNaN(l.ExactAngle) && Math.Abs(l.ExactAngle) < 1.0 / SubtitleLetter.minAngleDiv);
					int count2 = word.Count(l => !Double.IsNaN(l.ExactAngle) && Math.Abs(l.ExactAngle - angle) < 1.0 / SubtitleLetter.minAngleDiv);
					if (count2 > count1)
					{
						if (canStart < wordStart)
							italic.AddRange(letters.Skip(canStart).Take(wordStart - canStart));
						italic.AddRange(word);
						canStart = wordEnd + 1;
					}
					else
						hasRegilar = true;
					k = wordEnd;
				}
				if (italic.Any() && !hasRegilar)
					italic = letters.Where(l => l.Text != "\r\n").ToList();
				italic.AsParallel().ForAll(letter =>
				{
					letter.Angle = angle;
					letter.ApplyAngle();
				});
			}
		}

		public const int pixelLimitAsSet = 60;
		public const int minLineHeight = 20;
		public const int minLineSpace = 5;

		public byte maxPixelLevel = 255;
		public byte minPixelLevel = 0;

		private Rectangle subtitleBorders;
		public Bitmap subtitleBitmap;
		/// <summary>
		/// Pixels bitmap
		/// </summary>
		private byte[,] subtitleArray;
		private int[,] nextArray;
		private SubtitleImage.TextLine[] textLines;
		public SubtitleLetter[] letters;
#if DEBUG
		public LinkedList<PointF> debugPoints;
		public LinkedList<PointF[]> debugLines;
#endif
		private static double angleList;
		private static int angleCount;
		private static object italicSync = new object();
		public static double italicAngle = Double.NaN;
		public SubtitleImage(Bitmap source)
		{
			this.subtitleBitmap = new Bitmap(source.Width + 20, source.Height, PixelFormat.Format32bppArgb);
			Graphics graphics = Graphics.FromImage(this.subtitleBitmap);
			graphics.FillRectangle(new SolidBrush(Color.Black), 0, 0, source.Width + 20, source.Height);
			graphics.DrawImage(source, new Point(10, 0));
			graphics.Dispose();
			this.CreateSubtitleArray();
		}

		private static void FindNextRange(byte[,] image, int [,] nextArray, int line, byte pixelLevel1, byte pixelLevel2)
		{
			int lineWidth = nextArray.GetLength(1);
			int next = lineWidth;
			int pixel = image[line, lineWidth - 1] >= pixelLevel1 ? -1 : 1;
			int j2 = lineWidth + pixel;
			nextArray[line, lineWidth - 1] = next * pixel;
			for (int j = lineWidth - 2; j >= 0; j--)
			{
				if ((pixel == -1) != (image[line, j] >= pixelLevel1))
				{
					next = j + 1;
					pixel = -pixel;
				}
				if (image[line, j] > pixelLevel2)
				{
					if (j2 > j + 1 && j2 < next)
					{
						for (int j3 = j + 1; j3 < j2; j3++)
							nextArray[line, j3] = j2;
						next = j + 1;
					}
					j2 = j;
				}
				nextArray[line, j] = next * pixel;
			}
		}

		private void CreateSubtitleArray()
		{
			int lineWidth = this.subtitleBitmap.Size.Width;
			int columnHeight = this.subtitleBitmap.Height;
			BitmapData bitmapData = this.subtitleBitmap.LockBits(new Rectangle(0, 0, lineWidth, columnHeight), ImageLockMode.ReadOnly, this.subtitleBitmap.PixelFormat);
			byte[] array = new byte[lineWidth * columnHeight * 4];
			IntPtr scan = bitmapData.Scan0;
			Marshal.Copy(scan, array, 0, array.Length);
			this.subtitleBitmap.UnlockBits(bitmapData);
			this.subtitleArray = new byte[columnHeight, lineWidth];
			this.nextArray = new int[this.subtitleArray.GetLength(0), this.subtitleArray.GetLength(1)];
			this.maxPixelLevel = 0;
			this.minPixelLevel = 255;
			var maxPixelSync = new object();
			ParallelEnumerable.Range(0, columnHeight).ForAll(i =>
			{
				int num = i * lineWidth * 4;
				for (int j = 0; j < lineWidth; j++)
				{
					byte b = (byte)((int)(((int)array[num++] + (int)array[num++] + (int)array[num++]) * (int)array[num++]) / 768);
					this.subtitleArray[i, j] = b < pixelLimitAsSet / 2 ? (byte)0 : b;
					if (b > this.maxPixelLevel || b < this.minPixelLevel)
						lock(maxPixelSync)
						{
							if (b > this.maxPixelLevel)
								this.maxPixelLevel = b;
							if (b < this.minPixelLevel)
								this.minPixelLevel = b;
						}
				}
			});
			if (this.maxPixelLevel == this.minPixelLevel)
			{
				this.minPixelLevel = 0;
				this.maxPixelLevel = 255;
			}
			byte pixelLevel1 = (byte)((maxPixelLevel - minPixelLevel) >> 2 + minPixelLevel);
			byte pixelLevel2 = (byte)(maxPixelLevel - ((maxPixelLevel - minPixelLevel) >> 2));
			ParallelEnumerable.Range(0, columnHeight).ForAll(i =>
			{
				FindNextRange(this.subtitleArray, this.nextArray, i, pixelLevel1, pixelLevel2);
			});
			if (AppOptions.contrast != 0)
			{
				double fact = 0.5 + (double)(AppOptions.contrast / 5);
				ParallelEnumerable.Range(0, columnHeight).ForAll(k =>
				{
					for (int l = 0; l < lineWidth; l++)
					{
						this.subtitleArray[k, l] = this.Logistic(this.subtitleArray[k, l], fact);
					}
				});
			}
			DateTime now = DateTime.Now;
			this.textLines = TextLine.Find(this.nextArray);

			this.letters = this.textLines.SelectMany(line => line.FindLetters(this.subtitleArray, this.nextArray, pixelLevel1, pixelLevel2)).ToArray();
#if DEBUG
			this.debugPoints = new LinkedList<PointF>(this.textLines.SelectMany(_l => _l.debugPoints));
			this.debugLines = new LinkedList<PointF[]>(this.textLines.SelectMany(_l => _l.debugLines));
#endif
			//this.alternativeLetters = this.FindLetters(this.uncorrectedArray, true);
			Debugger.lettersTime += (DateTime.Now - now).TotalMilliseconds;
		}

		private byte Logistic(byte x, double fact)
		{
			double num = ((double)x - 128.0) * 5.0 / 128.0;
			double num2 = 1.0 / (1.0 + Math.Pow(Math.E, -num * fact));
			return (byte)(num2 * 256.0);
		}

		private void FindBorders()
		{
			if (this.subtitleBitmap == null)
			{
				throw new Exception("Trying to use FindBorders on a null image");
			}
			if (this.subtitleBitmap.PixelFormat != PixelFormat.Format32bppArgb)
			{
				throw new Exception("Pixel format isn't Format32bppArgb");
			}
			BitmapData bitmapData = this.subtitleBitmap.LockBits(new Rectangle(0, 0, this.subtitleBitmap.Width, this.subtitleBitmap.Height), ImageLockMode.ReadOnly, this.subtitleBitmap.PixelFormat);
			byte[] array = new byte[this.subtitleBitmap.Size.Width * this.subtitleBitmap.Size.Height * 4];
			IntPtr scan = bitmapData.Scan0;
			Marshal.Copy(scan, array, 0, array.Length);
			this.subtitleBitmap.UnlockBits(bitmapData);
			int num = -1;
			int num2 = -1;
			int num3 = -1;
			int num4 = -1;
			int num5 = 20;
			for (int i = 0; i < this.subtitleBitmap.Size.Height; i += num5)
			{
				if (this.LineContainsPixels(array, this.subtitleBitmap.Size.Width, i, 200))
				{
					if (num5 == 1)
					{
						num = i;
						break;
					}
					if (num5 == 4)
					{
						i -= num5;
						num5 = 1;
					}
					else
					{
						i -= num5 * 2;
						num5 = 4;
					}
				}
			}
			num5 = 4;
			for (int j = this.subtitleBitmap.Size.Height - 1; j > num; j -= num5)
			{
				if (this.LineContainsPixels(array, this.subtitleBitmap.Size.Width, j, 200))
				{
					if (num5 == 1)
					{
						num2 = j;
						break;
					}
					j += num5;
					num5 = 1;
				}
			}
			num5 = 4;
			for (int k = 0; k < this.subtitleBitmap.Size.Width; k += num5)
			{
				if (this.ColumnContainsPixels(array, this.subtitleBitmap.Size.Width, k, 200))
				{
					if (num5 == 1)
					{
						num3 = k;
						break;
					}
					k -= num5;
					num5 = 1;
				}
			}
			num5 = 4;
			for (int l = this.subtitleBitmap.Size.Width - 1; l > num3; l -= num5)
			{
				if (this.ColumnContainsPixels(array, this.subtitleBitmap.Size.Width, l, 200))
				{
					if (num5 == 1)
					{
						num4 = l;
						break;
					}
					l += num5;
					num5 = 1;
				}
			}
			if (num == -1 || num2 == -1 || num3 == -1 || num4 == -1)
			{
				throw new SubtitleImage.NoSubtitleTextException();
			}
			this.subtitleBorders = new Rectangle(num3, num, num4 - num3, num2 - num);
		}
		private bool LineContainsPixels(byte[] bytes, int w, int line, int limit)
		{
			for (int i = line * w * 4; i < (line + 1) * w * 4; i += 4)
			{
				if (((int)bytes[i] > limit || (int)bytes[i + 1] > limit || (int)bytes[i + 2] > limit) && bytes[i + 3] > 0)
				{
					return true;
				}
			}
			return false;
		}
		private bool ColumnContainsPixels(byte[] bytes, int w, int column, int limit)
		{
			for (int i = column * 4; i < bytes.Length; i += w * 4)
			{
				if (((int)bytes[i] > limit || (int)bytes[i + 1] > limit || (int)bytes[i + 2] > limit) && bytes[i + 3] > 0)
				{
					return true;
				}
			}
			return false;
		}

		public string GetText()
		{
			if (this.letters == null)
			{
				return "";
			}
			StringBuilder stringBuilder = new StringBuilder(100);
			bool flag = false;
			foreach (SubtitleLetter current in this.letters)
			{
				if (current.Angle != 0.0 && !flag)
				{
					stringBuilder.Append("<i>");
					flag = true;
				}
				if (current.Angle == 0.0 && flag)
				{
					stringBuilder.Append("</i>");
					flag = false;
				}
				if (current.Text != null)
				{
					stringBuilder.Append(current.Text);
				}
				else
				{
					stringBuilder.Append("¤");
				}
			}
			if (flag)
			{
				stringBuilder.Append("</i>");
			}
			return stringBuilder.ToString();
		}

		public Bitmap GetBitmap()
		{
			int num = this.subtitleBitmap.Height;
			int num2 = this.subtitleBitmap.Width;
			Bitmap bitmap = new Bitmap(num2, num, PixelFormat.Format32bppArgb);
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
			IntPtr scan = bitmapData.Scan0;
			byte[] array = new byte[this.subtitleBitmap.Width * this.subtitleBitmap.Height * 4];
			int num3 = 0;
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num2; j++)
				{
					byte level = this.subtitleArray[i, j];
					array[num3++] = level;
					array[num3++] = level;
					array[num3++] = level;
					array[num3++] = 255;
				}
			}
			Marshal.Copy(array, 0, scan, array.Length);
			bitmap.UnlockBits(bitmapData);
			return bitmap;
		}
		public void FixSpaces()
		{
			bool flag = false;
			LinkedList<SubtitleLetter> linkedList = new LinkedList<SubtitleLetter>();
			SubtitleLetter subtitleLetter = null;
			foreach (SubtitleLetter current in this.letters)
			{
				if (current.Text == " " && flag && current.Coords.Width + 6 < AppOptions.minimumSpaceCharacterWidth * 3 / 2 + 3)
				{
					linkedList.AddLast(current);
				}
				if (AppOptions.IsNarrowCharacter(current.Text))
				{
					if (subtitleLetter != null && subtitleLetter.Text == " " && subtitleLetter.Coords.Width + 6 < AppOptions.minimumSpaceCharacterWidth * 3 / 2 + 3)
					{
						linkedList.AddLast(subtitleLetter);
					}
					flag = true;
				}
				else
				{
					flag = false;
				}
				subtitleLetter = current;
			}
			this.letters = this.letters.Where(letter => !linkedList.Contains(letter)).ToArray();
		}
	}
}
