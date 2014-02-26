using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace SupRip
{
	public sealed class SubtitleBitmap : IDisposable
	{
		private Lazy<Bitmap> bitmap;
		private Bitmap bitmap2;
		private Lazy<byte[]> image;
		private byte[] image2;

		public int Width { get; private set; }
		public int Height { get; private set; }

		public PixelFormat PixelFormat
		{
			get
			{
				return this.bitmap2 == null ? PixelFormat.Format32bppArgb : this.bitmap2.PixelFormat;
			}
		}

		public SubtitleBitmap(byte[] image, int width, int height)
		{
			this.image2 = image;
			this.Width = width;
			this.Height = height;
			this.image = new Lazy<byte[]>(() => this.image2);
			this.bitmap = new Lazy<Bitmap>(() =>
			{
				var bitmap = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb);
				var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
				try
				{
					IntPtr scan = bitmapData.Scan0;
					Marshal.Copy(this.image2, 0, scan, this.image2.Length);
				}
				finally
				{
					bitmap.UnlockBits(bitmapData);
				}
				return bitmap;
			});
		}

		private SubtitleBitmap(Bitmap bitmap)
		{
			this.bitmap2 = bitmap;
			this.Width = bitmap.Width;
			this.Height = bitmap.Height;
			this.bitmap = new Lazy<Bitmap>(() => this.bitmap2);
			this.image = new Lazy<byte[]>(() =>
			{
				var image = new byte[this.Width * this.Height * 4];
				var bitmapData = this.bitmap2.LockBits(new Rectangle(0, 0, this.Width, this.Height), ImageLockMode.ReadOnly, this.bitmap2.PixelFormat);
				try
				{
					IntPtr scan = bitmapData.Scan0;
					Marshal.Copy(scan, image, 0, image.Length);
				}
				finally
				{
					this.bitmap2.UnlockBits(bitmapData);
				}
				return image;
			});
		}

		public Bitmap Clone()
		{
			return (Bitmap)this.bitmap.Value.Clone();
		}

		public static implicit operator Bitmap(SubtitleBitmap source)
		{
			return source.bitmap.Value;
		}

		public static explicit operator byte[](SubtitleBitmap source)
		{
			return source.image.Value;
		}

		public static implicit operator SubtitleBitmap(Bitmap source)
		{
			return new SubtitleBitmap(source);
		}

		public void Dispose()
		{
			if (this.bitmap2 != null)
				this.bitmap2.Dispose();
			else if (this.bitmap.IsValueCreated)
				this.bitmap.Value.Dispose();
		}
	}
}
