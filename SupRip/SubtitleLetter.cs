using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace SupRip
{
	/// <summary>
	/// Text letter - image atom block
	/// </summary>
	internal class SubtitleLetter
	{
		private static class HashManager
		{
			public delegate void HashLine(byte[,] image, int i, double[] line);
			public delegate void HashImage(byte[,] image, double[] line1, double[] line2, byte[] hash, HashLine line);

			private static IDictionary<int, HashLine> lines = new Dictionary<int, HashLine>();
			private static IDictionary<int, HashImage> images = new Dictionary<int, HashImage>();

			private static HashLine BuildHashLine(int width, double offset)
			{
				var imageArg = Expression.Parameter(typeof(byte[,]), "image");
				var iArg = Expression.Parameter(typeof(int), "i");
				var lineArg = Expression.Parameter(typeof(double[]), "line");
				Func<int, Type, Expression> image = (j, type) =>
					Expression.Convert(Expression.ArrayAccess(imageArg, iArg, Expression.Constant(j)), type);
				var lines = new Expression[8];
				double start = 0.0;
				for (int k = 0; k < 8; k++)
				{
					double end = (width * (k + 1)) / 8.0;
					if (k < 7) end += offset;
					double istart = Math.Ceiling(start);
					double iend = Math.Floor(end);
					Expression sum = null;
					if (iend > istart)
					{
						sum = image((int)istart, typeof(int));
						for (int n = (int)istart + 1; n < (int)iend; n++)
							sum = Expression.Add(sum, image(n, typeof(int)));
						sum = Expression.Convert(sum, typeof(double));
					}
					if (start != istart)
					{
						var mul = Expression.Multiply(image((int)istart - 1, typeof(double)),
							Expression.Constant(istart - start));
						sum = sum == null ? mul : Expression.Add(mul, sum);
					}
					if (end != iend)
					{
						var mul = Expression.Multiply(image((int)iend, typeof(double)),
							Expression.Constant(end - iend));
						sum = sum == null ? mul : Expression.Add(sum, mul);
					}
					lines[k] = Expression.Assign(Expression.ArrayAccess(lineArg, Expression.Constant(k)),
						Expression.Divide(sum, Expression.Constant(end - start)));
					start = end;
				}
				var expr = Expression.Block(lines);
				var lambda = Expression.Lambda<HashLine>(expr, imageArg, iArg, lineArg);
				return lambda.Compile();
			}

			/// <summary>
			/// A += B
			/// </summary>
			/// <param name="A">Vector A</param>
			/// <param name="B">Vector B</param>
			private static void Add(double[] A, double[] B)
			{
				A[0] += B[0];
				A[1] += B[1];
				A[2] += B[2];
				A[3] += B[3];
				A[4] += B[4];
				A[5] += B[5];
				A[6] += B[6];
				A[7] += B[7];
			}

			/// <summary>
			/// A += B * C
			/// </summary>
			/// <param name="A">Vector A</param>
			/// <param name="B">Vector B</param>
			/// <param name="C">Constant C</param>
			private static void AddMul(double[] A, double[] B, double C)
			{
				A[0] += B[0] * C;
				A[1] += B[1] * C;
				A[2] += B[2] * C;
				A[3] += B[3] * C;
				A[4] += B[4] * C;
				A[5] += B[5] * C;
				A[6] += B[6] * C;
				A[7] += B[7] * C;
			}

			/// <summary>
			/// A = B * C
			/// </summary>
			/// <param name="A">Vector A</param>
			/// <param name="B">Vector B</param>
			/// <param name="C">Constant C</param>
			private static void Mul(double[] A, double[] B, double C)
			{
				A[0] = B[0] * C;
				A[1] = B[1] * C;
				A[2] = B[2] * C;
				A[3] = B[3] * C;
				A[4] = B[4] * C;
				A[5] = B[5] * C;
				A[6] = B[6] * C;
				A[7] = B[7] * C;
			}

			private static byte HashByte(double[] line, double C)
			{
				byte b = 0;
				if (line[0] > SubtitleImage.pixelLimitAsSet * C) b |= 1;
				if (line[1] > SubtitleImage.pixelLimitAsSet * C) b |= 2;
				if (line[2] > SubtitleImage.pixelLimitAsSet * C) b |= 4;
				if (line[3] > SubtitleImage.pixelLimitAsSet * C) b |= 8;
				if (line[4] > SubtitleImage.pixelLimitAsSet * C) b |= 16;
				if (line[5] > SubtitleImage.pixelLimitAsSet * C) b |= 32;
				if (line[6] > SubtitleImage.pixelLimitAsSet * C) b |= 64;
				if (line[7] > SubtitleImage.pixelLimitAsSet * C) b |= 128;
				return b;
			}

			private static HashImage BuildHashImage(int height, double offset)
			{
				MethodInfo miAdd = new MethodOf<Action<double[], double[]>>(Add);
				MethodInfo miAddMul = new MethodOf<Action<double[], double[], double>>(AddMul);
				MethodInfo miMul = new MethodOf<Action<double[], double[], double>>(Mul);
				MethodInfo miHashByte = new MethodOf<Func<double[], double, byte>>(HashByte);
				var imageArg = Expression.Parameter(typeof(byte[,]), "image");
				var line1Arg = Expression.Parameter(typeof(double[]), "line1");
				var line2Arg = Expression.Parameter(typeof(double[]), "line2");
				var hashArg = Expression.Parameter(typeof(byte[]), "hash");
				var lineArg = Expression.Parameter(typeof(HashLine), "line");
				Func<int, ParameterExpression, Expression> line = (i, arg) => Expression.Invoke(lineArg, imageArg,
					Expression.Constant(i), arg);
				var list = new List<Expression>();
				double start = 0.0;
				for (int k = 0; k < 8; k++)
				{
					double end = (height * (k + 1)) / 8.0;
					if (k < 7) end += offset;
					double istart = Math.Ceiling(start);
					double iend = Math.Floor(end);
					int istart2;
					if (start != istart)
					{
						list.Add(Expression.Call(miMul, line1Arg, line2Arg, Expression.Constant(istart - start)));
						istart2 = (int)istart;
					}
					else
					{
						if (iend > istart)
							list.Add(line((int)istart, line1Arg));
						istart2 = (int)istart + 1;
					}
					for (int n = istart2; n < (int)iend; n++)
					{
						list.Add(line(n, line2Arg));
						list.Add(Expression.Call(miAdd, line1Arg, line2Arg));
					}
					if (end != iend)
					{
						list.Add(line((int)iend, line2Arg));
						list.Add(Expression.Call(miAddMul, line1Arg, line2Arg, Expression.Constant(end - iend)));
					}
					list.Add(Expression.Assign(Expression.ArrayAccess(hashArg, Expression.Constant(k)),
						Expression.Call(miHashByte, line1Arg, Expression.Constant(end - start))));
					start = end;
				}
				var expr = Expression.Block(list.ToArray());
				var lambda = Expression.Lambda<HashImage>(expr, imageArg, line1Arg, line2Arg, hashArg, lineArg);
				return lambda.Compile();
			}

			public static ulong Hash(byte[,] image, double offsetX, double offsetY)
			{
				int width = image.GetLength(1);
				int height = image.GetLength(0);
				if (width < 8 || height < 8)
					return 0ul;
				HashLine line;
				HashImage hash;
				int keyX = (width << 4) + ((int)(offsetX * 2) + 16) % 16;
				if (!lines.TryGetValue(keyX, out line))
				{
					lock (lines)
					{
						if (!lines.TryGetValue(keyX, out line))
						{
							line = BuildHashLine(width, offsetX);
							lines.Add(keyX, line);
						}
					}
				}
				int keyY = (height << 4) + ((int)(offsetY * 2) + 16) % 16;
				if (!images.TryGetValue(keyY, out hash))
				{
					lock(images)
					{
						if (!images.TryGetValue(keyY, out hash))
						{
							hash = BuildHashImage(height, offsetY);
							images.Add(keyY, hash);
						}
					}
				}
				var l1 = new double[8];
				var l2 = new double[8];
				var l = new byte[8];
				hash(image, l1, l2, l, line);
				return BitConverter.ToUInt64(l, 0);
			}

			public static void HashMask(byte[,] image, double deltaX, double deltaY, out ulong hash, out ulong mask)
			{
				ulong? last = null;
				mask = 0ul;
				for (double x = -deltaX / 2.0; x <= deltaX / 2.0; x += 0.5)
					for (double y = -deltaY / 2.0; y <= deltaY / 2.0; y += 0.5)
					{
						ulong h = HashManager.Hash(image, x, y);
						mask |= (last ?? h) ^ h;
						last = h;
					}
				hash = last.Value | mask;
			}

			public static IEnumerable<ulong> Bits(ulong value)
			{
				ulong mask = 1ul;
				for (int k = 0; k < 64; k++)
				{
					if ((value & mask) == mask)
						yield return mask;
					mask <<= 1;
				}
			}

			public static IEnumerable<int> Bits(int value)
			{
				int mask = 1;
				for (int k = 0; k < 32; k++)
				{
					if ((value & mask) == mask)
						yield return mask;
					mask <<= 1;
				}
			}

			public static IEnumerable<byte> Bits(byte value)
			{
				byte mask = 1;
				for (int k = 0; k < 8; k++)
				{
					if ((value & mask) == mask)
						yield return mask;
					mask <<= 1;
				}
			}
		}

		public const int minAngleDiv = 12;
		public const double maxAngle = 0.45; //less than Space.smoothStep

		private double angle;
		private byte[,] regular;
		private byte[,] italic;

		private Lazy<ulong> hash;
		private Lazy<ulong[]> hashmask;
		private Lazy<byte[,]> widenImage;

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

		public double ExactAngle
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

		public float LeftBottom
		{
			get;
			private set;
		}

		public float RightTop
		{
			get;
			private set;
		}

		public float RightBottom
		{
			get;
			private set;
		}

		public ulong Hash { get { return this.hash.Value; } }

		public ulong FontHash { get { return this.hashmask.Value[0]; } }

		public ulong FontMask { get { return this.hashmask.Value[1]; } }

		public SubtitleLetter(RectangleF r, string s = null)
		{
			this.Coords = r;
			this.Text = s;
			this.angle = 0.0;
			this.ExactAngle = Double.NaN;
			if (s == " ")
			{
				SetSpaceEdges();
			}
			else
			{
				this.LeftTop = this.LeftBottom = r.Left - 1;
				this.RightTop = this.RightBottom = r.Right;
			}
			this.hash = new Lazy<ulong>(() => 0ul);
			this.hashmask = new Lazy<ulong[]>(() => new ulong[2]);
		}

		public SubtitleLetter(byte[,] i, string s = null)
		{
			this.regular = i;
			this.widenImage = new Lazy<byte[,]>(() => Widen1(this.regular));
			this.Text = s;
			this.ExactAngle = Double.NaN;
			this.hash = new Lazy<ulong>(() => HashManager.Hash(this.regular, 0.0, 0.0));
			this.hashmask = new Lazy<ulong[]>(() =>
			{
				double deltaX = Math.Min(3.0, Math.Floor(this.ImageWidth / 12.0));
				double deltaY = Math.Min(1.0, Math.Floor(this.ImageHeight / 12.0));
				ulong hash, mask;
				HashManager.HashMask(this.image, deltaX, deltaY, out hash, out mask);
				return new ulong[] { hash, mask };
			});
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
				this.hash = new Lazy<ulong>(() => HashManager.Hash(this.regular, 0.0, 0.0));
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
			this.hash = new Lazy<ulong>(() => HashManager.Hash(this.italic, 0.0, 0.0));
			this.widenImage = new Lazy<byte[,]>(() => { return null; });
		}

		private double BorderTopWidth(byte[,] array, int position)
		{
			int height = array.GetLength(0);
			for (int i = 0; i < height; i++)
			{
				if (array[i, position] > 200)
				{
					if (i == 0) return (double)i;
					return (double)i - (double)array[i - 1, position] / (double)array[i, position];
				}
			}
			return 0.0;
		}

		private double BorderBottomWidth(byte[,] array, int position)
		{
			int height = array.GetLength(0);
			for (int i = height - 1; i >= 0; i--)
			{
				if (array[i, position] > 200)
				{
					if (i == height - 1) return (double)i;
					return (double)i + (double)array[i + 1, position] / (double)array[i, position];
				}
			}
			return 0.0;
		}

		private double BorderLeftWidth(byte[,] array, int position)
		{
			int width = array.GetLength(1);
			for (int i = 0; i < width; i++)
			{
				if (array[position, i] > 200)
				{
					if (i == 0) return (double)i;
					return (double)i - (double)array[position, i - 1] / (double)array[position, i];
				}
			}
			return 0.0;
		}

		private double BorderRightWidth(byte[,] array, int position)
		{
			int width = array.GetLength(1);
			for (int i = width - 1; i >= 0; i--)
			{
				if (array[position, i] > 200)
				{
					if (i == width - 1) return (double)i;
					return (double)i + (double)array[position, i + 1] / (double)array[position, i];
				}
			}
			return 0.0;
		}

		private double FindTranslation(byte[,] a, byte[,] b)
		{
			int width = a.GetLength(0);
			int step = width / 5;
			double delta = 0.0;
			for (int i = 0, y = step; i < 3; i++, y += step)
			{
				delta += this.BorderRightWidth(b, y) - this.BorderRightWidth(a, y);
				delta += this.BorderLeftWidth(b, y) - this.BorderLeftWidth(a, y);
			}
			delta /= 6.0;
			return delta;
		}

		private int ComputeAbsDiff(byte[,] a, byte[,] b)
		{
			int height = Math.Min(a.GetLength(1), b.GetLength(1));
			int width = Math.Min(a.GetLength(0), b.GetLength(0));
			long num3 = 0L;
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					num3 += (long)((a[i, j] - b[i, j]) * (a[i, j] - b[i, j]));
				}
			}
			return (int)(num3 / (long)width / (long)height);
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

		internal static byte[,] Widen1(byte[,] image)
		{
			int imageW = image.GetLength(1);
			int imageH = image.GetLength(0);
			int boldW = imageW;// + 2;
			int boldH = imageH;// + 2;
			var bold = new byte[boldH, boldW];
			ParallelEnumerable.Range(0, boldH).ForAll(i =>
			{
				for (int x = 0; x < imageW; x++)
				{
					byte maxPix = 0;
					byte p;
					if (i > 0 && (p = image[i - 1, x]) > maxPix) maxPix = p;
					if ((p = image[i, x]) > maxPix) maxPix = p;
					if (i < imageH - 1 && (p = image[i + 1, x]) > maxPix) maxPix = p;
					if (maxPix == 0) continue;
					maxPix = (byte)Math.Min(255, maxPix * 2);
					if (x > 0 && (p = bold[i, x - 1]) < maxPix) bold[i, x - 1] = maxPix;
					if ((p = bold[i, x]) < maxPix) bold[i, x] = maxPix;
					if (x < boldW - 1 && (p = bold[i, x + 1]) < maxPix) bold[i, x + 1] = maxPix;
				}
			});
			return bold;
		}

		internal static byte[,] Widen2(byte[,] image, int levelX, int levelY)
		{
			int imageW = image.GetLength(1);
			int imageH = image.GetLength(0);
			int boldW = imageW;// +levelX * 2;
			int boldH = imageH;// +levelY * 2;
			var bold = new byte[boldH, boldW];
			ParallelEnumerable.Range(0, boldH).ForAll(i =>
			{
				for (int x = 0; x < imageW; x++)
				{
					byte maxPix = 0;
					for (int y = Math.Max(0, i - levelY); y <= Math.Min(imageH - 1, i + levelY); y++)
					{
						byte p = image[y, x];
						if (p >= SubtitleImage.pixelLimitAsSet && p > maxPix)
							maxPix = p;
					}
					if (maxPix == 0)
						continue;
					maxPix = (byte)Math.Min(255, maxPix * 2);
					for (int j = Math.Max(0, x - levelX); j <= Math.Min(boldW - 1, x + levelX); j++)
						bold[i, j] = Math.Max(bold[i, j], maxPix);
				}
			});
			return bold;
		}

		private int Difference(byte[,] a, byte[,] b, byte[,] bWiden)
		{
			DateTime now = DateTime.Now;
			byte[,] array = bWiden ?? Widen1(b);
			Debugger.widenTime += (DateTime.Now - now).TotalMilliseconds;
			int height = Math.Min(a.GetLength(1), array.GetLength(1));
			int width = Math.Min(a.GetLength(0), array.GetLength(0));
			int diff = 0;
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					diff += Math.Max(0, a[i, j] - array[i, j]);
				}
			}
			return diff;
		}

		public int Matches(SubtitleLetter o, int tolerance = 5000)
		{
			int delta = (this.image.GetLength(1) >> 4) + 1;
			if (Math.Abs(o.image.GetLength(1) - this.image.GetLength(1)) > delta || Math.Abs(o.image.GetLength(0) - this.image.GetLength(0)) > 1)
			{
				return 999999;
			}
			DateTime now = DateTime.Now;
			double num = this.FindTranslation(this.image, o.image);
			byte[,] array = o.MoveLetter(-num);
			Debugger.translationTime += (DateTime.Now - now).TotalMilliseconds;
			now = DateTime.Now;
			int num2 = this.Difference(array, this.image, this.widenImage.Value);
			if (tolerance <= 1000 && num2 > 0)
			{
				return num2 + 1000;
			}
			num2 += this.Difference(this.image, array, null);
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
			int delta2 = Math.Sign(p) * (int)(Math.Abs(p) + 1.0);
			int delta1 = (delta2 > 0) ? (delta2 - 1) : (delta2 + 1);
			double level = Math.Abs(Math.IEEERemainder(p, 1.0));
			Math.Floor(-1.4);
			for (int i = 0; i < this.ImageHeight; i++)
			{
				for (int j = 0; j < this.ImageWidth; j++)
				{
					int pix1;
					if (j - delta1 >= 0 && j - delta1 < this.ImageWidth)
					{
						pix1 = (int)this.image[i, j - delta1];
					}
					else
					{
						pix1 = 0;
					}
					int pix2;
					if (j - delta2 >= 0 && j - delta2 < this.ImageWidth)
					{
						pix2 = (int)this.image[i, j - delta2];
					}
					else
					{
						pix2 = 0;
					}
					array[i, j] = (byte)((1.0 - level) * (double)pix1 + level * (double)pix2);
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
			var bold = this.image;//SubtitleLetter.Widen2(this.image, 2, 2);
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
