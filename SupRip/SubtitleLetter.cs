using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SupRip
{
	/// <summary>
	/// Text letter - image atom block
	/// </summary>
	internal class SubtitleLetter
	{
		public const int minAngleDiv = 12;
		public const double maxAngle = 0.45; //less than Space.smoothStep

		private double angle;
		private double[] borders;
		private byte[,] regular;
		private byte[,] italic;

		private byte[,] image
		{
			get { return this.italic ?? this.regular; }
		}

		public int ImageWidth
		{
			get { return this.image.GetLength(1); }
		}

		public int ImageHeight
		{
			get { return this.image.GetLength(0); }
		}

		public RectangleF Coords
		{
			get;
			private set;
		}

		public float[] LeftSpace
		{
			get;
			private set;
		}

		public float[] RightSpace
		{
			get;
			private set;
		}

		public double Angle
		{
			get { return this.angle; }
			set {
				if (this.angle == value) return;
				this.angle = value;
				float height = this.Coords.Height;
				if (height < 1.0f) return;
				if (value == 0.0)
				{
					this.LeftTop = this.LeftBottom = this.Coords.Left - 1.0f;
					this.RightTop = this.RightBottom = this.Coords.Right;
					return;
				}
				float delta = -(float)(height * value);
				if (this.LeftSpace == null)
					SetSimpleLeftAngle(delta);
				if (this.RightSpace == null)
					SetSimpleRightAngle(delta);
				if (this.LeftSpace != null || this.RightSpace != null)
					SetAngle(delta);
				//ApplyItalic(delta, italic);
			}
		}

		public double? ExactAngle
		{
			get;
			private set;
		}

		public string Text
		{
			get;
			set;
		}

		public int Height
		{
			get;
			set;
		}

		public float LeftTop
		{
			get;
			private set;
		}
		public PointF LeftTopPoint
		{
			get { return new PointF(this.LeftTop, this.Coords.Top); }
		}

		public float LeftBottom
		{
			get;
			private set;
		}
		public PointF LeftBottomPoint
		{
			get { return new PointF(this.LeftBottom, this.Coords.Bottom - 1.0f); }
		}

		public float RightTop
		{
			get;
			private set;
		}
		public PointF RightTopPoint
		{
			get { return new PointF(this.RightTop, this.Coords.Top); }
		}

		public float RightBottom
		{
			get;
			private set;
		}
		public PointF RightBottomPoint
		{
			get { return new PointF(this.RightBottom, this.Coords.Bottom - 1.0f); }
		}

		public SubtitleLetter(RectangleF r, string s = null)
		{
			this.Coords = r;
			this.Text = s;
			this.angle = 0.0;
			if (s == " ")
			{
				SetSpaceEdges();
			}
			else
			{
				this.LeftTop = this.LeftBottom = r.Left - 1;
				this.RightTop = this.RightBottom = r.Right;
			}
		}

		public SubtitleLetter(byte[,] i, string s = null)
		{
			this.regular = i;
			this.ReduceImage();
			this.Text = s;
		}

		public SubtitleLetter(byte[,] i, RectangleF r, double a) : this(i)
		{
			this.Coords = r;
			this.Angle = a;
		}

		public SubtitleLetter(byte[,] i, RectangleF r, float[] leftSpace, float[] rightSpace)
			: this(i)
		{
			this.Coords = r;
			this.LeftSpace = leftSpace;
			this.RightSpace = rightSpace;
			this.LeftTop = this.LeftBottom = r.Left - 1;
			this.RightTop = this.RightBottom = r.Right;
			this.angle = 0.0;
			float x1 = 0.0f, x2 = 0.0f, x3 = 0.0f, x4 = 0.0f;
			bool exact1 = false, exact2 = false;
			if (leftSpace != null && r.Height >= SubtitleImage.minLineSpace / 2)
				CalcAngle(leftSpace, true, out x1, out x2, out exact1);
			if (rightSpace != null && r.Height >= SubtitleImage.minLineSpace / 2)
				CalcAngle(rightSpace, false, out x3, out x4, out exact2);
			float d1 = x1 - x2; float d2 = x3 - x4;
			double angle = maxAngle;
			if (exact1 && exact2)
			{
				if (d1 * d2 >= 0 && Math.Abs(d1 - d2) <= r.Height / minAngleDiv)
				{
					angle = (d1 + d2) / (r.Height * 2.0f);
				}
			}
			else if (exact1)
			{
				angle = d1 / r.Height;
			}
			else if (exact2)
			{
				angle = d2 / r.Height;
			}
			if (Math.Abs(angle) < maxAngle)
			{
				this.ExactAngle = angle;
				//this.Angle = this.ExactAngle.Value;
			}
		}

		internal static void CalcAngle(float[] space, bool leftSide, out float X1, out float X2, out bool exact)
		{
			float side = leftSide ? 1.0f : -1.0f;
			int height = space.Length;
			//Min volume should be on the middle
			double middle = (double)(height - 1) / 2.0;
			int j1 = 0, j2 = 0, j3 = 0;
			double angle;
			while ((double)j3 <= middle) //only first half
			{
				j1 = j2;
				j2 = j3;
				j3 = height - 1;
				double maxAngle = SubtitleLetter.maxAngle * side;
				//Find max angle from current point
				for (int j = j2 + 1; j < height; j++)
				{
					double a = (space[j] - space[j2]) / (double)(j - j2);
					if (a * side <= maxAngle * side)
					{
						maxAngle = a;
						j3 = j;
					}
				}
			}
			if ((double)j2 == middle) //Use 3 lines
			{
				double a1 = (space[j2] - space[j1]) / (double)(j2 - j1);
				double a2 = (space[j3] - space[j2]) / (double)(j3 - j2);
				angle = a1 * a2 <= 0.0 ? 0.0 : (a1 + a2) / 2.0;
				X1 = space[j2] - (float)(angle * j2);
				X2 = space[j2] + (float)(angle * (height - 1 - j2));
			}
			else //Use 2 lines
			{
				angle = (space[j3] - space[j2]) / (double)(j3 - j2);
				X1 = space[j3] - (float)(angle * j3);
				X2 = space[j2] + (float)(angle * (height - 1 - j2));
			}
			float x1 = X1;
			//Count near points
			int near = space.Select((s, j) => (s - x1 - (float)(angle * j)) * side).Count(d => d < 1.0f);
			exact = near > height * 2 / 3;
		}

		private void SetSimpleLeftAngle(float delta)
		{
			if (delta >= 0.0f)
			{
				this.LeftTop = this.Coords.Left - delta - 1.0f;
				this.LeftBottom = this.Coords.Left - 1.0f;
			}
			else
			{
				this.LeftTop = this.Coords.Left - 1.0f;
				this.LeftBottom = this.Coords.Left + delta - 1.0f;
			}
		}

		private void SetSimpleRightAngle(float delta)
		{
			if (delta >= 0.0f)
			{
				this.RightTop = this.Coords.Right;
				this.RightBottom = this.Coords.Right + delta;
			}
			else
			{
				this.RightTop = this.Coords.Right - delta;
				this.RightBottom = this.Coords.Right;
			}
		}

		private void SetSpaceEdges()
		{
			float d = Math.Min(3.0f, AppOptions.minimumSpaceCharacterWidth / 3.0f);
			float left = this.Coords.Left, right = this.Coords.Right;
			if (right - left > d * 3.0f)
			{
				left += d;
				right -= d;
			}
			var delta = (float)(this.angle * this.Coords.Height / 2.0);
			this.LeftTop = left + delta;
			this.LeftBottom = left - delta;
			this.RightTop = right + delta;
			this.RightBottom = right - delta;
		}

		private void SetAngle(float delta)
		{
			if (this.LeftSpace != null)
			{
				float x = this.LeftSpace[0];
				float offset = this.LeftSpace.Select((space, line) => x + delta * line / this.LeftSpace.Length - space).Max();
				if (offset > 0.0f) x -= offset;
				this.LeftTop = x - 0.5f;
				this.LeftBottom = x + delta - 0.5f;
			}
			if (this.RightSpace != null)
			{
				float x = this.RightSpace[0];
				float offset = this.RightSpace.Select((space, line) => space - x - delta * line / this.LeftSpace.Length).Max();
				if (offset > 0.0f) x += offset;
				this.RightTop = x + 0.5f;
				this.RightBottom = x + delta + 0.5f;
			}
		}

		public void ApplyAngle()
		{
			float height = this.Coords.Height;
			if (height < 1.0f) return;
			if (this.Text == " ")
			{
				SetSpaceEdges();
			}
			else
			{
				float delta = -(float)(height * this.angle);
				ApplyItalic(delta);
			}
		}

		private void ApplyItalic(float delta)
		{
			if (delta == 0.0f)
			{
				this.italic = null;
				ReduceImage();
				return;
			}
			int height = this.regular.GetLength(0);
			int width = this.regular.GetLength(1);
			int x1 = (int)Math.Ceiling(this.LeftTop), x2 = (int)Math.Floor(this.RightTop);
			this.italic = new byte[height, x2 - x1 + 1];
			double angle = (double)delta / (double)height;
			for (int i = 0; i < height; i++)
			{
				float d = (float)(angle * i);
				int italic = (int)Math.Round(d);
				float d2 = d - italic;
				int left = this.LeftSpace == null ? x1 : (int)Math.Ceiling(this.LeftSpace[i]);
				int right = this.RightSpace == null ? x2 : (int)Math.Floor(this.RightSpace[i]);
				for (int x = left; x <= right; x++)
				{
					int j = x - (int)Math.Ceiling(this.Coords.Left);
					byte b = (byte)((1.0 - Math.Abs(d2)) * (double)this.regular[i, j]);
					if (d2 < 0.0 && j > 0) b += (byte)(-d2 * (double)this.regular[i, j - 1]);
					if (d2 > 0.0 && j < width - 1) b += (byte)(d2 * (double)this.regular[i, j + 1]);
					int j2 = x - x1 - italic;
					if (j2 >= 0 && j2 <= x2 - x1)
						this.italic[i, j2] = b;
				}
			}
			ReduceImage();
		}

		private void ReduceImage()
		{
			this.borders = new double[4];
			for (int i = 0; i < 4; i++)
			{
				this.borders[i] = this.AveragedBorderWidth(this.image, i);
			}
		}

		private double AveragedBorderWidth(byte[,] array, int side)
		{
			int length = array.GetLength(0);
			int length2 = array.GetLength(1);
			int num = length / 2;
			int num2 = length2 / 2;
			int num3;
			if (side == 0 || side == 2)
			{
				num3 = num2;
			}
			else
			{
				num3 = num;
			}
			double num4 = this.BorderWidth(array, side, num3 - 1);
			double num5 = this.BorderWidth(array, side, num3);
			double num6 = this.BorderWidth(array, side, num3 + 1);
			double num7 = (num4 + num5 + num6) / 3.0;
			if (Math.Abs(num4 - num7) > 3.0 || Math.Abs(num5 - num7) > 3.0 || Math.Abs(num6 - num7) > 3.0)
			{
				return -1.0;
			}
			return num7;
		}
		private double BorderWidth(byte[,] array, int side, int position = -1)
		{
			double result = 0.0;
			int length = array.GetLength(0);
			int length2 = array.GetLength(1);
			int num = length / 2;
			int num2 = length2 / 2;
			if (position != -1)
			{
				num = position;
				num2 = position;
			}
			switch (side)
			{
			case 0:
			{
				int i = 0;
				while (i < length)
				{
					if (array[i, num2] > 200)
					{
						if (i == 0)
						{
							result = (double)i;
							break;
						}
						result = (double)i - (double)array[i - 1, num2] / (double)array[i, num2];
						break;
					}
					else
					{
						i++;
					}
				}
				break;
			}
			case 1:
			{
				int i = length2 - 1;
				while (i >= 0)
				{
					if (array[num, i] > 200)
					{
						if (i == length2 - 1)
						{
							result = (double)i;
							break;
						}
						result = (double)i + (double)array[num, i + 1] / (double)array[num, i];
						break;
					}
					else
					{
						i--;
					}
				}
				break;
			}
			case 2:
			{
				int i = length - 1;
				while (i >= 0)
				{
					if (array[i, num2] > 200)
					{
						if (i == length - 1)
						{
							result = (double)i;
							break;
						}
						result = (double)i + (double)array[i + 1, num2] / (double)array[i, num2];
						break;
					}
					else
					{
						i--;
					}
				}
				break;
			}
			case 3:
			{
				int i = 0;
				while (i < length2)
				{
					if (array[num, i] > 200)
					{
						if (i == 0)
						{
							result = (double)i;
							break;
						}
						result = (double)i - (double)array[num, i - 1] / (double)array[num, i];
						break;
					}
					else
					{
						i++;
					}
				}
				break;
			}
			}
			return result;
		}
		private double FindTranslation(byte[,] a, byte[,] b)
		{
			a.GetLength(1);
			int length = a.GetLength(0);
			double[,] array = new double[4, 3];
			int num = length / 5;
			int num2 = num;
			for (int i = 0; i < 3; i++)
			{
				array[1, i] = this.BorderWidth(a, 1, num2);
				array[3, i] = this.BorderWidth(a, 3, num2);
				num2 += num;
			}
			double[,] array2 = new double[4, 3];
			num = length / 5;
			num2 = num;
			for (int j = 0; j < 3; j++)
			{
				array2[1, j] = this.BorderWidth(b, 1, num2);
				array2[3, j] = this.BorderWidth(b, 3, num2);
				num2 += num;
			}
			double num3 = 0.0;
			for (int k = 0; k < 3; k++)
			{
				num3 += array2[1, k] - array[1, k];
			}
			for (int l = 0; l < 3; l++)
			{
				num3 += array2[3, l] - array[3, l];
			}
			num3 /= 6.0;
			double num4 = 0.0;
			for (int m = 0; m < 3; m++)
			{
				num4 += Math.Pow(array2[1, m] - array[1, m] - num3, 2.0);
			}
			for (int n = 0; n < 3; n++)
			{
				num4 += Math.Pow(array2[3, n] - array[3, n] - num3, 2.0);
			}
			return num3;
		}

		private int ComputeAbsDiff(byte[,] a, byte[,] b)
		{
			int num = Math.Min(a.GetLength(1), b.GetLength(1));
			int num2 = Math.Min(a.GetLength(0), b.GetLength(0));
			long num3 = 0L;
			for (int i = 0; i < num2; i++)
			{
				for (int j = 0; j < num; j++)
				{
					num3 += (long)((a[i, j] - b[i, j]) * (a[i, j] - b[i, j]));
				}
			}
			return (int)(num3 / (long)num2 / (long)num);
		}

		public bool BordersMatch(SubtitleLetter other)
		{
			for (int i = 0; i < 4; i++)
			{
				if (this.borders[i] != -1.0 && other.borders[i] != -1.0 && Math.Abs(this.borders[i] - other.borders[i]) > 4.0)
				{
					return false;
				}
			}
			return true;
		}

		internal static byte[,] Widen(byte[,] img)
		{
			int length = img.GetLength(1);
			int length2 = img.GetLength(0);
			byte[,] array = new byte[length2, length];
			for (int i = 0; i < length2; i++)
			{
				for (int j = 0; j < length; j++)
				{
					byte b = img[i, j];
					if (i > 0 && img[i - 1, j] > b)
					{
						b = img[i - 1, j];
					}
					if (i < length2 - 1 && img[i + 1, j] > b)
					{
						b = img[i + 1, j];
					}
					if (j > 0 && img[i, j - 1] > b)
					{
						b = img[i, j - 1];
					}
					if (j < length - 1 && img[i, j + 1] > b)
					{
						b = img[i, j + 1];
					}
					array[i, j] = (byte)Math.Min(255, (int)(b * 2));
				}
			}
			return array;
		}

		internal static byte[,] Widen2(byte[,] image, int levelX, int levelY)
		{
			int imageW = image.GetLength(1);
			int imageH = image.GetLength(0);
			int boldW = imageW + levelX * 2;
			int boldH = imageH + levelY * 2;
			var bold = new byte[boldH, boldW];
			ParallelEnumerable.Range(0, boldH).ForAll(i =>
			{
				for (int x = 0; x < imageW; x++)
				{
					byte maxPix = 0;
					for (int y = Math.Max(0, i - levelY * 2); y <= Math.Min(imageH - 1, i); y++)
					{
						byte p = image[y, x];
						if (p >= SubtitleImage.pixelLimitAsSet && p > maxPix)
							maxPix = p;
					}
					if (maxPix == 0)
						continue;
					for (int j = Math.Max(0, x); j <= Math.Min(boldW - 1, x + levelX * 2); j++)
						bold[i, j] = Math.Max(bold[i, j], maxPix);
				}
			});
			return bold;
		}

		private int Difference(byte[,] a, byte[,] b)
		{
			DateTime now = DateTime.Now;
			byte[,] array = Widen(b);
			Debugger.widenTime += (DateTime.Now - now).TotalMilliseconds;
			int num = Math.Min(a.GetLength(1), array.GetLength(1));
			int num2 = Math.Min(a.GetLength(0), array.GetLength(0));
			int num3 = 0;
			for (int i = 0; i < num2; i++)
			{
				for (int j = 0; j < num; j++)
				{
					num3 += (int)((byte)Math.Max(0, (int)(a[i, j] - array[i, j])));
				}
			}
			return num3;
		}

		public int Matches(SubtitleLetter o)
		{
			if (Math.Abs(o.image.GetLength(1) - this.image.GetLength(1)) > 1 || Math.Abs(o.image.GetLength(0) - this.image.GetLength(0)) > 1)
			{
				return 999999;
			}
			DateTime now = DateTime.Now;
			double num = this.FindTranslation(this.image, o.image);
			byte[,] array = o.MoveLetter(-num);
			Debugger.translationTime += (DateTime.Now - now).TotalMilliseconds;
			now = DateTime.Now;
			int num2 = this.Difference(this.image, array) + this.Difference(array, this.image);
			Debugger.diffTime += (DateTime.Now - now).TotalMilliseconds;
			if (num2 > 0)
			{
				return num2 + 1000;
			}
			now = DateTime.Now;
			int num3 = this.ComputeAbsDiff(this.image, array);
			Debugger.absDiffTime += (DateTime.Now - now).TotalMilliseconds;
			return Math.Min(1000, num3 / 10);
		}

		private byte[,] MoveLetter(double p)
		{
			if (p == 0.0)
			{
				return this.image;
			}
			byte[,] array = new byte[this.ImageHeight, this.ImageWidth];
			int num = Math.Sign(p) * (int)(Math.Abs(p) + 1.0);
			int num2 = (num > 0) ? (num - 1) : (num + 1);
			double num3 = Math.Abs(Math.IEEERemainder(p, 1.0));
			Math.Floor(-1.4);
			for (int i = 0; i < this.ImageHeight; i++)
			{
				for (int j = 0; j < this.ImageWidth; j++)
				{
					int num4;
					if (j - num2 >= 0 && j - num2 < this.ImageWidth)
					{
						num4 = (int)this.image[i, j - num2];
					}
					else
					{
						num4 = 0;
					}
					int num5;
					if (j - num >= 0 && j - num < this.ImageWidth)
					{
						num5 = (int)this.image[i, j - num2];
					}
					else
					{
						num5 = 0;
					}
					array[i, j] = (byte)((1.0 - num3) * (double)num4 + num3 * (double)num5);
				}
			}
			return array;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(50);
			if (this.Text != null)
			{
				stringBuilder.Append("\"" + this.Text + "\" ");
			}
			stringBuilder.AppendFormat("({0}, {1}) ({2} x {3})", new object[]
			{
				this.Coords.Left,
				this.Coords.Top,
				this.Coords.Right,
				this.Coords.Bottom
			});
			return stringBuilder.ToString();
		}

		public string DumpLetter()
		{
			int length = this.image.GetLength(0);
			int length2 = this.image.GetLength(1);
			StringBuilder stringBuilder = new StringBuilder(1000);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					stringBuilder.AppendFormat("{0:000} ", this.image[i, j]);
				}
				stringBuilder.Append("\n");
			}
			return stringBuilder.ToString();
		}

		public Bitmap GetBitmap()
		{
			if (this.image == null)
			{
				return null;
			}
			var bold = this.image;//SubtitleImage.MakeBold(this.image, 3, 3);
			int width = bold.GetLength(1);
			int height = bold.GetLength(0);
			Bitmap bitmap = new Bitmap(width + 4, height + 4, PixelFormat.Format32bppArgb);
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			byte[] array = new byte[bitmap.Width * bitmap.Height * 4];
			int num = 0;
			for (int i = -2; i < height + 2; i++)
			{
				for (int j = -2; j < width + 2; j++)
				{
					byte level;
					if (i < 0 || i >= height || j < 0 || j >= width)
					{
						level = 0;
					}
					else
					{
						level = bold[i, j];
					}
					array[num++] = level;
					array[num++] = level;
					array[num++] = level;
					array[num++] = 255;
				}
			}

			Marshal.Copy(array, 0, bitmapData.Scan0, array.Length);
			bitmap.UnlockBits(bitmapData);
			return bitmap;
		}

		public static SubtitleLetter Extract(byte[,] workArray, Rectangle rect, float[] leftSpace, float[] rightSpace, int lineOffset)
		{
			byte[,] array = new byte[rect.Height, rect.Width];
			for (int i = rect.Top; i < rect.Bottom; i++)
			{
				for (int j = rect.Left; j < rect.Right; j++)
				{
					int x = j - rect.X;
					int y = i - rect.Y;
					if ((leftSpace == null || (float)j > leftSpace[y]) && (rightSpace == null || (float)j < rightSpace[y]))
						array[y, x] = workArray[i, j];
				}
			}
			return new SubtitleLetter(array, rect, leftSpace, rightSpace)
			{
				Height = lineOffset
			};
		}
	}
}
