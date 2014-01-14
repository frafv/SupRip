using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
namespace SupRip
{
	internal class SubtitleLetter
	{
		private Rectangle coords;
		private double angle;
		private int height;
		private string text;
		private double[] borders;
		private byte[,] image;
		public int ImageWidth
		{
			get
			{
				return this.image.GetLength(1);
			}
		}
		public int ImageHeight
		{
			get
			{
				return this.image.GetLength(0);
			}
		}
		public Rectangle Coords
		{
			get
			{
				return this.coords;
			}
			set
			{
				this.coords = value;
			}
		}
		public double Angle
		{
			get
			{
				return this.angle;
			}
			set
			{
				this.angle = value;
			}
		}
		public string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
			}
		}
		public int Height
		{
			get
			{
				return this.height;
			}
			set
			{
				this.height = value;
			}
		}
		public SubtitleLetter(Rectangle r)
		{
			this.coords = r;
			this.text = null;
		}
		public SubtitleLetter(Rectangle r, string s) : this(r)
		{
			this.text = s;
		}
		public SubtitleLetter(Rectangle r, double a, string s) : this(r, s)
		{
			this.angle = a;
		}
		public SubtitleLetter(byte[,] i)
		{
			this.image = i;
			this.ReduceImage();
		}
		public SubtitleLetter(byte[,] i, string s) : this(i)
		{
			this.text = s;
		}
		public SubtitleLetter(byte[,] i, double a) : this(i)
		{
			this.angle = a;
		}
		public SubtitleLetter(byte[,] i, Rectangle r, double a) : this(i, a)
		{
			this.coords = r;
		}
		private void ReduceImage()
		{
			this.borders = new double[4];
			for (int i = 0; i < 4; i++)
			{
				this.borders[i] = this.AveragedBorderWidth(this.image, i);
			}
		}
		private string DrawDiff(int[,] da)
		{
			int length = da.GetLength(0);
			int length2 = da.GetLength(1);
			StringBuilder stringBuilder = new StringBuilder(1000);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					if (da[i, j] > 100)
					{
						stringBuilder.Append('#');
					}
					else
					{
						if (da[i, j] > 50)
						{
							stringBuilder.Append('+');
						}
						else
						{
							if (da[i, j] > 20)
							{
								stringBuilder.Append(':');
							}
							else
							{
								if (da[i, j] > 10)
								{
									stringBuilder.Append('.');
								}
								else
								{
									stringBuilder.Append(' ');
								}
							}
						}
					}
				}
				stringBuilder.Append("\n");
			}
			return stringBuilder.ToString();
		}
		private double BorderWidth(byte[,] array, int side)
		{
			return this.BorderWidth(array, side, -1);
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
		private double BorderWidth(byte[,] array, int side, int position)
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
		private Point OldFindTranslation(byte[,] a, byte[,] b)
		{
			int length = a.GetLength(1);
			int length2 = a.GetLength(0);
			if (b.GetLength(1) != length || b.GetLength(0) != length2)
			{
				throw new Exception("the two arrays in FindTranslation need to be of equal size");
			}
			int[,] array = new int[5, 5];
			for (int i = -2; i <= 2; i++)
			{
				for (int j = -2; j <= 2; j++)
				{
					for (int k = 2; k < length2 - 2; k++)
					{
						for (int l = 2; l < length - 2; l++)
						{
							int num = Math.Abs((int)(a[k, l] - b[k + i, l + j]));
							array[i + 2, j + 2] += num;
						}
					}
				}
			}
			int num2 = 99999;
			int y = 0;
			int x = 0;
			for (int m = -2; m <= 2; m++)
			{
				for (int n = -2; n <= 2; n++)
				{
					if (array[m + 2, n + 2] < num2)
					{
						num2 = array[m + 2, n + 2];
						y = m;
						x = n;
					}
				}
			}
			if (num2 < 1000)
			{
				Debugger.translationFound++;
			}
			else
			{
				Debugger.translationNotFound++;
			}
			if (num2 < 1000)
			{
				return new Point(x, y);
			}
			return new Point(0, 0);
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
		private int ComputeAbsDiff(byte[,] a, byte[,] b, Point translation)
		{
			int length = a.GetLength(1);
			int length2 = a.GetLength(0);
			int num = Math.Abs(translation.X);
			int num2 = Math.Abs(translation.Y);
			long num3 = 0L;
			for (int i = num2; i < length2 - num2; i++)
			{
				for (int j = num; j < length - num; j++)
				{
					num3 += (long)((a[i, j] - b[i + translation.Y, j + translation.X]) * (a[i, j] - b[i + translation.Y, j + translation.X]));
				}
			}
			return (int)(num3 / (long)length2 / (long)length);
		}
		public int OldMatches(SubtitleLetter o)
		{
			if (Math.Abs(o.image.GetLength(1) - this.image.GetLength(1)) > 1 || Math.Abs(o.image.GetLength(0) - this.image.GetLength(0)) > 1)
			{
				return 999999;
			}
			double num = this.FindTranslation(this.image, o.image);
			byte[,] b = o.MoveLetter(-num);
			DateTime now = DateTime.Now;
			int result = this.ComputeAbsDiff(this.image, b);
			Debugger.absDiffTime += (DateTime.Now - now).TotalMilliseconds;
			return result;
		}
		private byte[,] Widen(byte[,] img)
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
		private int Difference(byte[,] a, byte[,] b)
		{
			DateTime now = DateTime.Now;
			byte[,] array = this.Widen(b);
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
			if (this.text != null)
			{
				stringBuilder.Append("\"" + this.text + "\" ");
			}
			stringBuilder.AppendFormat("({0}/{1}), ({2}/{3})", new object[]
			{
				this.coords.Left,
				this.coords.Top,
				this.coords.Right,
				this.coords.Bottom
			});
			return stringBuilder.ToString();
		}
		public string StringDrawing()
		{
			int length = this.image.GetLength(0);
			int length2 = this.image.GetLength(1);
			StringBuilder stringBuilder = new StringBuilder(1000);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					if (this.image[i, j] > 200)
					{
						stringBuilder.Append('#');
					}
					else
					{
						if (this.image[i, j] > 100)
						{
							stringBuilder.Append('+');
						}
						else
						{
							if (this.image[i, j] > 50)
							{
								stringBuilder.Append('.');
							}
							else
							{
								stringBuilder.Append(' ');
							}
						}
					}
				}
				stringBuilder.Append("\n");
			}
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
			int length = this.image.GetLength(1);
			int length2 = this.image.GetLength(0);
			Bitmap bitmap = new Bitmap(length + 4, length2 + 4, PixelFormat.Format32bppArgb);
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			byte[] array = new byte[bitmap.Width * bitmap.Height * 4];
			for (int i = 0; i < array.Length; i++)
			{
				int num = i / 4 % bitmap.Width;
				int num2 = i / 4 / bitmap.Width;
				if (num < 2 || num >= bitmap.Width - 2 || num2 < 2 || num2 >= bitmap.Height - 2)
				{
					if (i % 4 == 3)
					{
						array[i] = 255;
					}
					else
					{
						array[i] = 0;
					}
				}
				else
				{
					if (i % 4 == 3)
					{
						array[i] = 255;
					}
					else
					{
						array[i] = this.image[num2 - 2, num - 2];
					}
				}
			}
			Marshal.Copy(array, 0, bitmapData.Scan0, array.Length);
			bitmap.UnlockBits(bitmapData);
			return bitmap;
		}
	}
}
