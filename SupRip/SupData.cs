using System;
using System.Drawing;
namespace SupRip
{
	public class SupData
	{
		public long startTime;
		public long endTime;
		public int duration;
		public int endCount;
		private bool forced;
		public bool scanned;
		private string startString;
		private string endString;
		private string srtText;
		private string text;
		public int number;
		private ulong bitmapHash;
		public int numWindows;
		public Bitmap bitmap;
		public bool emptySubtitle;
		public byte[] colorSet;
		public byte[] transparency;
		public long[,] bitmapStarts;
		public long[,] bitmapLengths;
		public byte[] hdTransparency;
		public byte[,] hdColorSet;
		public long startControlSeq;
		public Rectangle bitmapPos;
		public Rectangle bitmapPos2;
		public string Start
		{
			get
			{
				return this.startString;
			}
		}
		public bool Forced
		{
			get
			{
				return this.forced;
			}
			set
			{
				this.forced = value;
			}
		}
		public string End
		{
			get
			{
				return this.endString;
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
		public ulong BitmapHash
		{
			get
			{
				return this.bitmapHash;
			}
		}
		public string SRTText
		{
			get
			{
				return this.srtText;
			}
		}
		public bool Scanned
		{
			get
			{
				return this.scanned;
			}
		}
		public SupData()
		{
			this.colorSet = new byte[4];
			this.transparency = new byte[4];
			this.hdColorSet = new byte[256, 3];
			this.hdTransparency = new byte[256];
			this.bitmapStarts = new long[2, 2];
			this.bitmapLengths = new long[2, 2];
			this.bitmapPos = default(Rectangle);
		}
		private void UpdateTimeStrings()
		{
			long num = this.startTime - (long)AppOptions.ptsOffset;
			long num2 = this.endTime - (long)AppOptions.ptsOffset;
			long num3 = num % 1000L;
			long num4 = num / 1000L % 60L;
			long num5 = num / 60000L % 60L;
			long num6 = num / 3600000L % 60L;
			this.startString = string.Format("{0:00}:{1:00}:{2:00},{3:000}", new object[]
			{
				num6,
				num5,
				num4,
				num3
			});
			num3 = num2 % 1000L;
			num4 = num2 / 1000L % 60L;
			num5 = num2 / 60000L % 60L;
			num6 = num2 / 3600000L % 60L;
			this.endString = string.Format("{0:00}:{1:00}:{2:00},{3:000}", new object[]
			{
				num6,
				num5,
				num4,
				num3
			});
		}
		public void UpdateSRTText()
		{
			this.UpdateTimeStrings();
			if (this.text != null)
			{
				this.srtText = this.text;
				this.scanned = true;
				return;
			}
			this.srtText = "Line " + this.number;
		}
		public void SetBitmapHash(byte[] array)
		{
			this.bitmapHash = 0uL;
			for (int i = 0; i < array.Length; i++)
			{
				this.bitmapHash = (ulong)array[i] + (this.bitmapHash << 6) + (this.bitmapHash << 16) - this.bitmapHash;
			}
		}
	}
}
