using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
namespace SupRip
{
	internal class SubtitleImage
	{
		private class EndOfImageReachedException : Exception
		{
		}
		private class NoSubtitleTextException : Exception
		{
		}
		private class TextLine
		{
			private int num;
			private int start;
			private int end;
			public double angle;
			public int Num
			{
				get
				{
					return this.num;
				}
				set
				{
					this.num = value;
				}
			}
			public int Height
			{
				get
				{
					return this.end - this.start;
				}
			}
			public int Start
			{
				get
				{
					return this.start;
				}
				set
				{
					this.start = value;
				}
			}
			public int End
			{
				get
				{
					return this.end;
				}
				set
				{
					this.end = value;
				}
			}
			public TextLine(int n, int s, int e)
			{
				this.num = n;
				this.start = s;
				this.end = e;
			}
		}
		private enum Anchor
		{
			Center,
			Top,
			Bottom
		}
		private enum Side
		{
			Left,
			Right
		}
		public const int pixelLimitAsSet = 60;
		private Rectangle subtitleBorders;
		public Bitmap subtitleBitmap;
		private byte[,] subtitleArray;
		private byte[,] uncorrectedArray;
		private int height;
		private int width;
		private SubtitleImage.TextLine[] textLines;
		public LinkedList<SubtitleLetter> letters;
		public LinkedList<SubtitleLetter> alternativeLetters;
		public SortedList<int, Space> debugLocations;
		public LinkedList<Point> debugPoints;
		private static LinkedList<double> angleList;
		private static int angleCount;
		private static double italicAngle;
		public SubtitleImage(Bitmap source)
		{
			this.subtitleBitmap = new Bitmap(source.Width + 20, source.Height, PixelFormat.Format32bppArgb);
			Graphics graphics = Graphics.FromImage(this.subtitleBitmap);
			graphics.FillRectangle(new SolidBrush(Color.Black), 0, 0, source.Width + 20, source.Height);
			graphics.DrawImage(source, new Point(10, 0));
			graphics.Dispose();
			this.CreateSubtitleArray();
		}
		private void CreateSubtitleArray()
		{
			BitmapData bitmapData = this.subtitleBitmap.LockBits(new Rectangle(0, 0, this.subtitleBitmap.Width, this.subtitleBitmap.Height), ImageLockMode.ReadOnly, this.subtitleBitmap.PixelFormat);
			byte[] array = new byte[this.subtitleBitmap.Size.Width * this.subtitleBitmap.Size.Height * 4];
			IntPtr scan = bitmapData.Scan0;
			Marshal.Copy(scan, array, 0, array.Length);
			this.subtitleBitmap.UnlockBits(bitmapData);
			this.width = this.subtitleBitmap.Size.Width;
			this.height = this.subtitleBitmap.Size.Height;
			this.subtitleArray = new byte[this.subtitleBitmap.Size.Height, this.subtitleBitmap.Size.Width];
			int num = 0;
			for (int i = 0; i < this.subtitleBitmap.Size.Height; i++)
			{
				for (int j = 0; j < this.subtitleBitmap.Size.Width; j++)
				{
					byte b = (byte)((int)((array[num++] + array[num++] + array[num++]) * array[num++]) / 768);
					if (b < 30)
					{
						this.subtitleArray[i, j] = 0;
					}
					else
					{
						this.subtitleArray[i, j] = b;
					}
				}
			}
			if (AppOptions.contrast != 0)
			{
				double fact = 0.5 + (double)(AppOptions.contrast / 5);
				for (int k = 0; k < this.subtitleBitmap.Size.Height; k++)
				{
					for (int l = 0; l < this.subtitleBitmap.Size.Width; l++)
					{
						this.subtitleArray[k, l] = this.Logistic(this.subtitleArray[k, l], fact);
					}
				}
			}
			DateTime now = DateTime.Now;
			this.textLines = this.FindTextLines(this.subtitleArray);
			this.AdjustItalicLines();
			this.letters = this.FindLetters(this.subtitleArray, false);
			this.alternativeLetters = this.FindLetters(this.uncorrectedArray, true);
			Debugger.lettersTime += (DateTime.Now - now).TotalMilliseconds;
		}
		private byte Logistic(byte x, double fact)
		{
			double num = ((double)x - 128.0) * 5.0 / 128.0;
			double num2 = 1.0 / (1.0 + Math.Pow(2.7182818284590451, -num * fact));
			return (byte)(num2 * 256.0);
		}
		private SubtitleImage.TextLine[] FindTextLines(byte[,] workArray)
		{
			LinkedList<SubtitleImage.TextLine> linkedList = new LinkedList<SubtitleImage.TextLine>();
			int length = workArray.GetLength(0);
			workArray.GetLength(1);
			int num = 0;
			int num2 = 0;
			int num4;
			while (true)
			{
				int num3 = num;
				while (num3 < length && !this.LineContainsPixels(workArray, num3))
				{
					num3++;
				}
				if (num3 == length)
				{
					goto IL_98;
				}
				num4 = num3;
				while (num3 < length && this.LineContainsPixels(workArray, num3))
				{
					num3++;
				}
				if (num3 == length)
				{
					break;
				}
				num = num3;
				linkedList.AddLast(new SubtitleImage.TextLine(num2, num4, num));
				num2++;
			}
			num = length - 1;
			if (num - num4 > 20)
			{
				linkedList.AddLast(new SubtitleImage.TextLine(num2, num4, num));
			}
			IL_98:
			SubtitleImage.TextLine[] array = new SubtitleImage.TextLine[linkedList.Count];
			linkedList.CopyTo(array, 0);
			int num5 = array.Length;
			SubtitleImage.TextLine textLine = null;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Height < 20)
				{
					if (array.Length == 1)
					{
						throw new Exception("Could only find one line of text, and it's smaller than 20 pixels");
					}
					if (i == 0)
					{
						array[1].Start = array[0].Start;
					}
					else
					{
						if (i == array.Length - 1)
						{
							textLine.End = array[i].End;
						}
						else
						{
							int num6 = array[i].Start - textLine.End;
							int num7 = array[i + 1].Start - array[i].End;
							if (num6 < 5 || num7 / num6 > 2)
							{
								textLine.End = array[i].End;
							}
							else
							{
								if (num7 < 5 || num6 / num7 > 2)
								{
									array[i + 1].Start = array[i].Start;
								}
								else
								{
									if (num6 < num7)
									{
										textLine.End = array[i].End;
									}
									else
									{
										array[i + 1].Start = array[i].Start;
									}
								}
							}
						}
					}
					array[i] = null;
					num5--;
				}
				else
				{
					textLine = array[i];
				}
			}
			int num8 = 0;
			SubtitleImage.TextLine[] array2 = new SubtitleImage.TextLine[num5];
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j] != null)
				{
					array2[num8] = array[j];
					num8++;
				}
			}
			return array2;
		}
		private double FindLineAngle(byte[,] image, SubtitleImage.TextLine line)
		{
			int length = image.GetLength(1);
			int[] array = new int[20];
			double result;
			if (SubtitleImage.italicAngle != 0.0)
			{
				for (int i = 0; i < length; i++)
				{
					if (!this.ColumnContainsPixels(this.subtitleArray, i, line.Start, line.End))
					{
						array[0]++;
					}
					if (!this.ColumnContainsPixels(this.subtitleArray, i, SubtitleImage.italicAngle, line.Start, line.End))
					{
						array[1]++;
					}
				}
				if (array[1] > array[0])
				{
					result = SubtitleImage.italicAngle;
				}
				else
				{
					result = 0.0;
				}
			}
			else
			{
				if (SubtitleImage.angleList == null)
				{
					SubtitleImage.angleList = new LinkedList<double>();
				}
				int num = 0;
				int num2 = -1;
				for (int j = 0; j < 20; j++)
				{
					if (j <= 0 || j >= 5)
					{
						for (int k = 0; k < length; k++)
						{
							if (!this.ColumnContainsPixels(this.subtitleArray, k, (double)j / 30.0, line.Start, line.End))
							{
								array[j]++;
							}
						}
						if (array[j] > num)
						{
							num = array[j];
							num2 = j;
						}
					}
				}
				if (num2 != 0 && SubtitleImage.angleCount < 20)
				{
					SubtitleImage.angleList.AddLast((double)num2 / 30.0);
					SubtitleImage.angleCount++;
				}
				if (SubtitleImage.angleCount == 20)
				{
					foreach (double num3 in SubtitleImage.angleList)
					{
						SubtitleImage.italicAngle += num3;
					}
					SubtitleImage.italicAngle /= 20.0;
				}
				result = (double)num2 / 30.0;
			}
			return result;
		}
		private void FindSpaces(int yStart, int yEnd, bool partial, ref SortedList<int, Space> spaces)
		{
			int[] array = new int[this.subtitleBitmap.Size.Width];
			for (int i = 0; i < this.subtitleBitmap.Size.Width; i++)
			{
				array[i] = this.ColumnFilledPixels(this.subtitleArray, i, yStart, yEnd);
			}
			int num = 1;
			bool flag = false;
			int num2 = 0;
			for (int j = 0; j < this.subtitleBitmap.Size.Width; j++)
			{
				if (flag && array[j] < num)
				{
					num2 = j;
					flag = false;
				}
				if ((!flag && array[j] >= num) || (!flag && j == this.subtitleBitmap.Size.Width - 1))
				{
					if (j - num2 >= AppOptions.charSplitTolerance || j < AppOptions.charSplitTolerance || j >= this.subtitleBitmap.Size.Width - AppOptions.charSplitTolerance)
					{
						int num3 = j;
						if (j == this.subtitleBitmap.Size.Width - 1)
						{
							num3++;
						}
						Rectangle rectangle = new Rectangle(num2, yStart, num3 - num2, yEnd - yStart);
						bool flag2 = false;
						if (partial)
						{
							foreach (KeyValuePair<int, Space> current in spaces)
							{
								Space value = current.Value;
								if (this.Intersects(rectangle, value.Rect))
								{
									flag2 = true;
									break;
								}
							}
						}
						if (!flag2)
						{
							Space space = new Space(rectangle, partial);
							spaces.Add(space.Hash, space);
						}
					}
					flag = true;
				}
			}
		}
		private bool Intersects(Rectangle r1, Rectangle r2)
		{
			return r2.Left <= r1.Right && r2.Right >= r1.Left && r2.Top <= r2.Bottom && r2.Bottom >= r2.Top;
		}
		private bool Intersects(Rectangle r1, SortedList<int, Space> rList)
		{
			foreach (KeyValuePair<int, Space> current in rList)
			{
				if (this.Intersects(r1, current.Value.Rect))
				{
					return true;
				}
			}
			return false;
		}
		private void ExtendPartialSpace(int yStart, int yEnd, Space r)
		{
			if (!r.Partial)
			{
				return;
			}
			if (r.Rect.Top == yStart)
			{
				int num = r.Rect.Bottom;
				while (num < yEnd && !this.LineContainsPixels(this.subtitleArray, num, r.Rect.Left, r.Rect.Right))
				{
					num++;
				}
				r.Resize(0, 0, 0, num - r.Rect.Bottom);
			}
			if (r.Rect.Bottom == yEnd)
			{
				int num = r.Rect.Top;
				while (num >= yStart && !this.LineContainsPixels(this.subtitleArray, num, r.Rect.Left, r.Rect.Right))
				{
					num--;
				}
				r.Resize(0, r.Rect.Top - num, 0, 0);
			}
			if (r.Rect.Top <= yStart && r.Rect.Bottom >= yEnd - 1)
			{
				r.Partial = false;
			}
		}
		private void ExtendPartialSpaces(int yStart, int yEnd, ref SortedList<int, Space> spaces)
		{
			foreach (KeyValuePair<int, Space> current in spaces)
			{
				Space arg_18_0 = current.Value;
				this.ExtendPartialSpace(yStart, yEnd, current.Value);
			}
		}
		private void CleanupSpaces(ref SortedList<int, Space> spaces)
		{
			int num = -10000;
			LinkedList<int> linkedList = new LinkedList<int>();
			foreach (KeyValuePair<int, Space> current in spaces)
			{
				if (current.Value.Partial)
				{
					linkedList.AddLast(current.Key);
				}
				else
				{
					if (current.Value.Rect.Left - num < 4)
					{
						linkedList.AddLast(current.Key);
					}
					num = current.Value.Rect.Right;
				}
			}
			foreach (int current2 in linkedList)
			{
				spaces.Remove(current2);
			}
		}
		private void FindDiagonalBreaks(int yStart, int yEnd, ref SortedList<int, Space> rectangles)
		{
			SortedList<int, Space> sortedList = new SortedList<int, Space>();
			int charSplitTolerance = AppOptions.charSplitTolerance;
			foreach (KeyValuePair<int, Space> current in rectangles)
			{
				Space value = current.Value;
				if (value.Partial)
				{
					bool flag = false;
					if (value.Rect.Bottom == yEnd)
					{
						for (double num = 0.1; num < 0.5; num += 0.1)
						{
							int num2 = value.Rect.Right;
							while (num2 > value.Rect.Right - charSplitTolerance && !this.ColumnContainsPixels(this.subtitleArray, num2, num, yStart, value.Rect.Top + 2, SubtitleImage.Anchor.Bottom))
							{
								num2--;
							}
							if (num2 <= value.Rect.Right - charSplitTolerance)
							{
								Space space = new Space(value.Rect.Right, yStart, 0, yEnd - yStart - 1, false, Space.SpaceType.TopRight, value.Rect.Top - yStart + 2, num);
								sortedList.Add(space.Hash, space);
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							for (double num3 = -0.1; num3 > -0.5; num3 -= 0.1)
							{
								int num4 = value.Rect.Left - 1;
								while (num4 < value.Rect.Left + charSplitTolerance - 1 && !this.ColumnContainsPixels(this.subtitleArray, num4, num3, yStart, value.Rect.Top + 2, SubtitleImage.Anchor.Bottom))
								{
									num4++;
								}
								if (num4 >= value.Rect.Left + charSplitTolerance - 1)
								{
									Space space2 = new Space(value.Rect.Left, yStart, 0, yEnd - yStart - 1, false, Space.SpaceType.TopLeft, value.Rect.Top - yStart + 2, num3);
									sortedList.Add(space2.Hash, space2);
									break;
								}
							}
						}
					}
					else
					{
						if (value.Rect.Top == yStart)
						{
							for (double num5 = -0.1; num5 > -0.5; num5 -= 0.1)
							{
								int num6 = value.Rect.Right;
								while (num6 > value.Rect.Right - charSplitTolerance && !this.ColumnContainsPixels(this.subtitleArray, num6, num5, value.Rect.Bottom - 2, yEnd, SubtitleImage.Anchor.Top))
								{
									num6--;
								}
								if (num6 <= value.Rect.Right - charSplitTolerance)
								{
									Space space3 = new Space(value.Rect.Right, yStart, 0, yEnd - yStart - 1, false, Space.SpaceType.BottomRight, value.Rect.Bottom - yStart - 2, num5);
									sortedList.Add(space3.Hash, space3);
									break;
								}
							}
							if (!flag)
							{
								for (double num7 = 0.1; num7 < 0.5; num7 += 0.1)
								{
									int num8 = value.Rect.Left - 1;
									while (num8 < value.Rect.Left + charSplitTolerance - 1 && !this.ColumnContainsPixels(this.subtitleArray, num8, num7, value.Rect.Bottom - 2, yEnd, SubtitleImage.Anchor.Top))
									{
										num8++;
									}
									if (num8 >= value.Rect.Left + charSplitTolerance - 1)
									{
										Space space4 = new Space(value.Rect.Left, yStart, 0, yEnd - yStart - 1, false, Space.SpaceType.BottomLeft, value.Rect.Bottom - yStart - 2, num7);
										sortedList.Add(space4.Hash, space4);
										break;
									}
								}
							}
						}
					}
				}
			}
			foreach (KeyValuePair<int, Space> current2 in sortedList)
			{
				rectangles.Add(current2.Value.Hash + 1, current2.Value);
			}
		}
		private void CorrectItalics(int yStart, int yEnd, double angle)
		{
			byte[,] array = new byte[this.height, this.width];
			for (int i = yStart; i < yEnd; i++)
			{
				double num = angle * (double)((yStart + yEnd) / 2 - i);
				double num2 = num - Math.Floor(num);
				int num3 = (int)Math.Floor(num);
				for (int j = 0; j < this.width; j++)
				{
					if (j + num3 >= 0 && j + num3 + 1 < this.width)
					{
						array[i, j] = (byte)((1.0 - num2) * (double)this.subtitleArray[i, j + num3] + num2 * (double)this.subtitleArray[i, j + num3 + 1]);
					}
				}
			}
			for (int k = 0; k < yStart; k++)
			{
				for (int l = 0; l < this.width; l++)
				{
					array[k, l] = this.subtitleArray[k, l];
				}
			}
			for (int m = yEnd; m < this.height; m++)
			{
				for (int n = 0; n < this.width; n++)
				{
					array[m, n] = this.subtitleArray[m, n];
				}
			}
			this.subtitleArray = array;
		}
		private void AdjustItalicLines()
		{
			this.uncorrectedArray = (byte[,])this.subtitleArray.Clone();
			SubtitleImage.TextLine[] array = this.textLines;
			for (int i = 0; i < array.Length; i++)
			{
				SubtitleImage.TextLine textLine = array[i];
				DateTime now = DateTime.Now;
				textLine.angle = this.FindLineAngle(this.subtitleArray, textLine);
				if (textLine.angle != 0.0)
				{
					this.CorrectItalics(textLine.Start, textLine.End, textLine.angle);
				}
				Debugger.angleTime += (DateTime.Now - now).TotalMilliseconds;
			}
		}
		public LinkedList<SubtitleLetter> FindLetters(byte[,] workArray, bool reverseItalics)
		{
			LinkedList<SubtitleLetter> linkedList = new LinkedList<SubtitleLetter>();
			DateTime now = DateTime.Now;
			Debugger.linesTime += (DateTime.Now - now).TotalMilliseconds;
			this.debugLocations = new SortedList<int, Space>();
			this.debugPoints = new LinkedList<Point>();
			int num = 0;
			SubtitleImage.TextLine[] array = this.textLines;
			for (int i = 0; i < array.Length; i++)
			{
				SubtitleImage.TextLine textLine = array[i];
				num++;
				if (textLine.Num != 0)
				{
					Rectangle rectangle = new Rectangle(1, textLine.Start - 20, 10, 25);
					linkedList.AddLast(new SubtitleLetter(rectangle, "\r\n"));
				}
				now = DateTime.Now;
				SortedList<int, Space> sortedList = new SortedList<int, Space>();
				this.FindSpaces(textLine.Start, textLine.End, false, ref sortedList);
				this.FindSpaces(textLine.Start, textLine.Start + textLine.Height * 2 / 3, true, ref sortedList);
				this.FindSpaces(textLine.Start + (textLine.End - textLine.Start) / 3, textLine.End, true, ref sortedList);
				this.FindSpaces(textLine.Start, (textLine.Start + textLine.End) / 2, true, ref sortedList);
				this.FindSpaces((textLine.Start + textLine.End) / 2, textLine.End, true, ref sortedList);
				this.MergeSpaces(textLine.Start, textLine.End, ref sortedList);
				this.ExtendPartialSpaces(textLine.Start, textLine.End, ref sortedList);
				this.FindDiagonalBreaks(textLine.Start, textLine.End, ref sortedList);
				this.CleanupSpaces(ref sortedList);
				Debugger.spacesTime += (DateTime.Now - now).TotalMilliseconds;
				Space space = null;
				foreach (KeyValuePair<int, Space> current in sortedList)
				{
					if (space == null)
					{
						space = current.Value;
					}
					else
					{
						int right = space.Rect.Right;
						int left = current.Value.Rect.Left;
						Rectangle rectangle;
						if (space.Rect.X != 0 && space.Rect.Width > AppOptions.minimumSpaceCharacterWidth)
						{
							rectangle = new Rectangle(space.Rect.Left + 3, textLine.Start + 4, space.Rect.Width - 6, textLine.Height - 10);
							linkedList.AddLast(new SubtitleLetter(rectangle, textLine.angle, " "));
						}
						int start = textLine.Start;
						int end = textLine.End;
						rectangle = new Rectangle(right, start, left - right, end - start);
						now = DateTime.Now;
						SubtitleLetter subtitleLetter = this.ExtractLetter(rectangle, textLine.angle, space, current.Value);
						Debugger.extractTime += (DateTime.Now - now).TotalMilliseconds;
						if (subtitleLetter != null)
						{
							subtitleLetter.Height = (subtitleLetter.Coords.Y + subtitleLetter.Coords.Bottom) / 2 - (textLine.Start + textLine.End) / 2;
							linkedList.AddLast(subtitleLetter);
						}
						space = current.Value;
					}
				}
			}
			return linkedList;
		}
		private void MergeSpaces(int yStart, int yEnd, ref SortedList<int, Space> rectangles)
		{
			Space space = null;
			int value = -1;
			LinkedList<int> linkedList = new LinkedList<int>();
			LinkedList<Space> linkedList2 = new LinkedList<Space>();
			foreach (KeyValuePair<int, Space> current in rectangles)
			{
				if (space != null && space.Partial && space.Partial == current.Value.Partial && current.Value.Rect.Left - space.Rect.Right < 10 && (space.Rect.Top == current.Value.Rect.Top || space.Rect.Bottom == current.Value.Rect.Bottom))
				{
					Space space2 = new Space(space.Rect.X, space.Rect.Bottom - 1, current.Value.Rect.Right - space.Rect.X, 1, true);
					this.ExtendPartialSpace(yStart, yEnd, space2);
					if (space.Rect.Height - space2.Rect.Height < 5 && current.Value.Rect.Height - space2.Rect.Height < 5)
					{
						linkedList.AddLast(current.Key);
						linkedList.AddLast(value);
						linkedList2.AddLast(space2);
					}
				}
				space = current.Value;
				value = current.Key;
			}
			foreach (int current2 in linkedList)
			{
				rectangles.Remove(current2);
			}
			foreach (Space current3 in linkedList2)
			{
				rectangles.Add(current3.Hash, current3);
			}
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
		private bool LineContainsPixels(byte[] bytes, int w, int line, int limit, int x1, int x2)
		{
			for (int i = (line * w + x1) * 4; i < (line * w + x2) * 4; i += 4)
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
		private bool LineContainsPixels(byte[,] image, int line)
		{
			return this.LineContainsPixels(image, line, 0, image.GetLength(1));
		}
		private bool LineContainsPixels(byte[,] image, int line, int x1, int x2)
		{
			int num = x1;
			while (num < x2 && image[line, num] < 60)
			{
				num++;
			}
			return num < x2;
		}
		private bool ColumnContainsPixels(byte[,] image, int column)
		{
			return this.ColumnContainsPixels(image, column, 0, image.GetLength(0));
		}
		private bool ColumnContainsPixels(byte[,] image, int column, int y1, int y2)
		{
			int num = y1;
			while (num < y2 && image[num, column] < 60)
			{
				num++;
			}
			return num < y2;
		}
		private int ColumnFilledPixels(byte[,] image, int column, int y1, int y2)
		{
			int num = 0;
			for (int i = y1; i < y2; i++)
			{
				if (image[i, column] >= 60)
				{
					num++;
				}
			}
			return num;
		}
		private bool ColumnContainsPixels(byte[,] image, int column, double angle, int y1, int y2)
		{
			if (angle == 0.0)
			{
				return this.ColumnContainsPixels(image, column, y1, y2);
			}
			int length = image.GetLength(1);
			int num = column + (int)((double)(y2 - y1) * angle) / 2;
			int num2;
			int num3;
			if (angle > 0.0)
			{
				num2 = Math.Max(y1 + (int)((double)(num - length + 1) / angle) + 1, y1);
				num3 = Math.Min(y1 + (int)((double)num / angle), y2);
			}
			else
			{
				num2 = Math.Max(y1 + (int)((double)num / angle) + 1, y1);
				num3 = Math.Min(y1 + (int)((double)(num - length + 1) / angle), y2);
			}
			int num4 = num2;
			while (num4 < num3 && image[num4, num - (int)((double)(num4 - y1) * angle)] < 60)
			{
				num4++;
			}
			return num4 < num3;
		}
		private bool ColumnContainsPixels(byte[,] image, int column, double angle, int y1, int y2, SubtitleImage.Anchor anchor)
		{
			if (anchor == SubtitleImage.Anchor.Center)
			{
				return this.ColumnContainsPixels(image, column, angle, y1, y2);
			}
			if (anchor == SubtitleImage.Anchor.Bottom)
			{
				int column2 = column + (int)((double)(y2 - y1) * angle) / 2;
				return this.ColumnContainsPixels(image, column2, angle, y1, y2);
			}
			int column3 = column - (int)((double)(y2 - y1) * angle) / 2;
			return this.ColumnContainsPixels(image, column3, angle, y1, y2);
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
		private byte[,] TrimExtension(byte[,] old, SubtitleImage.Side side)
		{
			int length = old.GetLength(0);
			int length2 = old.GetLength(1);
			byte[,] array;
			if (side == SubtitleImage.Side.Right)
			{
				int num = length2 - 1;
				while (num >= 0 && !this.ColumnContainsPixels(old, num))
				{
					num--;
				}
				if (num == -1)
				{
					return null;
				}
				num++;
				array = new byte[length, num];
				for (int i = 0; i < length; i++)
				{
					for (int j = 0; j < num; j++)
					{
						array[i, j] = old[i, j];
					}
				}
			}
			else
			{
				if (side != SubtitleImage.Side.Left)
				{
					throw new Exception("invalid argument for TrimExtension " + side);
				}
				int num = 0;
				while (num < length2 && !this.ColumnContainsPixels(old, num))
				{
					num++;
				}
				if (num == length2)
				{
					return null;
				}
				num = length2 - num;
				array = new byte[length, num];
				for (int k = 0; k < length; k++)
				{
					for (int l = 0; l < num; l++)
					{
						array[k, l] = old[k, l + length2 - num];
					}
				}
			}
			return array;
		}
		private byte[,] CombineArrays(byte[,] a, byte[,] b)
		{
			if (a.GetLength(0) != b.GetLength(0))
			{
				throw new Exception("Trying to combine two arrays that don't have the same height");
			}
			byte[,] array = new byte[a.GetLength(0), a.GetLength(1) + b.GetLength(1)];
			for (int i = 0; i < a.GetLength(0); i++)
			{
				for (int j = 0; j < a.GetLength(1); j++)
				{
					array[i, j] = a[i, j];
				}
				for (int k = 0; k < b.GetLength(1); k++)
				{
					array[i, k + a.GetLength(1)] = b[i, k];
				}
			}
			return array;
		}
		public SubtitleLetter ExtractLetter(Rectangle rect, double angle, Space lastSpace, Space nextSpace)
		{
			byte[,] array = new byte[rect.Height, rect.Width];
			for (int i = 0; i < rect.Height; i++)
			{
				for (int j = 0; j < rect.Width; j++)
				{
					array[i, j] = this.subtitleArray[rect.Top + i, rect.Left + j];
				}
			}
			if (lastSpace.Type == Space.SpaceType.TopRight)
			{
				for (int k = 0; k < lastSpace.SlopeStart; k++)
				{
					int num = 0;
					while (num < (int)((double)(lastSpace.SlopeStart - k) * lastSpace.Angle) + 1 && num < rect.Width)
					{
						array[k, num] = 0;
						num++;
					}
				}
			}
			if (lastSpace.Type == Space.SpaceType.BottomRight)
			{
				for (int l = lastSpace.SlopeStart; l < rect.Height; l++)
				{
					int num2 = 0;
					while (num2 < (int)((double)(l - lastSpace.SlopeStart) * -lastSpace.Angle) + 1 && num2 < rect.Width)
					{
						array[l, num2] = 0;
						num2++;
					}
				}
			}
			if (nextSpace.Type == Space.SpaceType.TopLeft)
			{
				for (int m = 0; m < nextSpace.SlopeStart; m++)
				{
					int num3 = 0;
					while (num3 < (int)((double)(nextSpace.SlopeStart - m) * -nextSpace.Angle) + 1 && num3 < rect.Width)
					{
						array[m, rect.Width - num3 - 1] = 0;
						num3++;
					}
				}
			}
			if (nextSpace.Type == Space.SpaceType.BottomLeft)
			{
				for (int n = nextSpace.SlopeStart; n < rect.Height; n++)
				{
					int num4 = 0;
					while (num4 < (int)((double)(n - nextSpace.SlopeStart) * nextSpace.Angle) + 1 && num4 < rect.Width)
					{
						array[n, rect.Width - num4 - 1] = 0;
						num4++;
					}
				}
			}
			if (nextSpace.Type == Space.SpaceType.TopRight)
			{
				int num5 = Math.Min(this.width - rect.Right, (int)((double)nextSpace.SlopeStart * nextSpace.Angle));
				byte[,] array2 = new byte[rect.Height, num5];
				for (int num6 = 0; num6 < nextSpace.SlopeStart; num6++)
				{
					int num7 = 0;
					while (num7 < (int)Math.Round((double)(nextSpace.SlopeStart - num6) * nextSpace.Angle) && num7 < num5)
					{
						array2[num6, num7] = this.subtitleArray[rect.Top + num6, rect.Right + num7];
						num7++;
					}
				}
				array2 = this.TrimExtension(array2, SubtitleImage.Side.Right);
				if (array2 != null)
				{
					array = this.CombineArrays(array, array2);
					rect.Width += array2.GetLength(1);
				}
			}
			if (nextSpace.Type == Space.SpaceType.BottomRight)
			{
				int num8 = Math.Min(this.width - rect.Right, (int)((double)(rect.Height - nextSpace.SlopeStart) * -nextSpace.Angle) + 1);
				byte[,] array3 = new byte[rect.Height, num8];
				for (int num9 = nextSpace.SlopeStart; num9 < rect.Height; num9++)
				{
					int num10 = 0;
					while (num10 < (int)Math.Round((double)(nextSpace.SlopeStart - num9) * nextSpace.Angle) && num10 < num8)
					{
						array3[num9, num10] = this.subtitleArray[rect.Top + num9, rect.Right + num10];
						num10++;
					}
				}
				array3 = this.TrimExtension(array3, SubtitleImage.Side.Right);
				if (array3 != null)
				{
					array = this.CombineArrays(array, array3);
					rect.Width += array3.GetLength(1);
				}
			}
			if (lastSpace.Type == Space.SpaceType.TopLeft)
			{
				int num11 = Math.Min(rect.Left, (int)((double)lastSpace.SlopeStart * -lastSpace.Angle) + 1);
				byte[,] array4 = new byte[rect.Height, num11];
				for (int num12 = 0; num12 < nextSpace.SlopeStart; num12++)
				{
					int num13 = 0;
					while (num13 < (int)Math.Round((double)num11 + (double)(lastSpace.SlopeStart - num12) * lastSpace.Angle) && num13 < num11)
					{
						array4[num12, num13] = this.subtitleArray[rect.Top + num12, rect.Left - num11 + num13];
						num13++;
					}
				}
				array4 = this.TrimExtension(array4, SubtitleImage.Side.Left);
				if (array4 != null)
				{
					array = this.CombineArrays(array4, array);
					rect.Width += array4.GetLength(1);
					rect.X -= array4.GetLength(1);
				}
			}
			if (lastSpace.Type == Space.SpaceType.BottomLeft)
			{
				int num14 = Math.Min(rect.Left, (int)((double)(rect.Height - lastSpace.SlopeStart) * lastSpace.Angle) + 1);
				byte[,] array5 = new byte[rect.Height, num14];
				for (int num15 = lastSpace.SlopeStart; num15 < rect.Height; num15++)
				{
					int num16 = 1;
					while (num16 <= (int)Math.Round((double)(num15 - lastSpace.SlopeStart) * lastSpace.Angle) && num16 < num14)
					{
						array5[num15, num14 - num16] = this.subtitleArray[rect.Top + num15, rect.Left - num16];
						num16++;
					}
				}
				array5 = this.TrimExtension(array5, SubtitleImage.Side.Left);
				if (array5 != null)
				{
					array = this.CombineArrays(array5, array);
					rect.Width += array5.GetLength(1);
					rect.X -= array5.GetLength(1);
				}
			}
			int num17 = 0;
			while (num17 < rect.Height && !this.LineContainsPixels(array, num17, 0, rect.Width))
			{
				num17++;
			}
			if (num17 == rect.Height)
			{
				return null;
			}
			int num18 = num17;
			num17 = rect.Height - 1;
			while (!this.LineContainsPixels(array, num17, 0, rect.Width))
			{
				num17--;
			}
			int num19 = num17 + 1;
			byte[,] array6 = new byte[num19 - num18, array.GetLength(1)];
			for (int num20 = 0; num20 < num19 - num18; num20++)
			{
				for (int num21 = 0; num21 < array.GetLength(1); num21++)
				{
					array6[num20, num21] = array[num20 + num18, num21];
				}
			}
			rect.Y += num18;
			rect.Height -= num18 - num19 + rect.Height;
			return new SubtitleLetter(array6, rect, angle);
		}
		public Bitmap GetSubtitlePart(Rectangle rect)
		{
			Bitmap bitmap = new Bitmap(rect.Width + 6, rect.Height + 6);
			Graphics graphics = Graphics.FromImage(bitmap);
			graphics.FillRectangle(new SolidBrush(Color.Black), 0, 0, bitmap.Width, bitmap.Height);
			graphics.DrawImage(this.subtitleBitmap, 3, 3, rect, GraphicsUnit.Pixel);
			graphics.Dispose();
			return bitmap;
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
					array[num3++] = this.subtitleArray[i, j];
					array[num3++] = this.subtitleArray[i, j];
					array[num3++] = this.subtitleArray[i, j];
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
			foreach (SubtitleLetter current2 in linkedList)
			{
				this.letters.Remove(current2);
			}
		}
	}
}
