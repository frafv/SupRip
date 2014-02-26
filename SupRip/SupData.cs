using System;
using System.Drawing;
namespace SupRip
{
	public class SupData
	{
		public long StartTime { get; set; }
		public long EndTime { get; set; }
		public int Duration { get; set; }
		public int EndCount { get; set; }
		public int Number { get; set; }
		public int NumWindows { get; set; }
		public bool EmptySubtitle { get; set; }
		public byte[] ColorSet { get; set; }
		public byte[] Transparency { get; set; }
		public long[,] BitmapStarts { get; set; }
		public long[,] BitmapLengths { get; set; }
		public byte[] HDTransparency { get; set; }
		public byte[,] HDColorSet { get; set; }
		public long StartControlSeq { get; set; }
		public Rectangle BitmapPos { get; set; }
		public Rectangle BitmapPos2 { get; set; }
		public string Start { get; private set; }
		public bool Forced { get; set; }
		public string End { get; private set; }
		public string Text { get; set; }
		public ulong BitmapHash { get; private set; }
		public string SRTText { get; private set; }
		public bool Scanned { get; private set; }
		public SupData()
		{
			this.ColorSet = new byte[4];
			this.Transparency = new byte[4];
			this.HDColorSet = new byte[256, 3];
			this.HDTransparency = new byte[256];
			this.BitmapStarts = new long[2, 2];
			this.BitmapLengths = new long[2, 2];
			this.BitmapPos = default(Rectangle);
		}
		private void UpdateTimeStrings()
		{
			long num = this.StartTime - (long)AppOptions.ptsOffset;
			long num2 = this.EndTime - (long)AppOptions.ptsOffset;
			long num3 = num % 1000L;
			long num4 = num / 1000L % 60L;
			long num5 = num / 60000L % 60L;
			long num6 = num / 3600000L % 60L;
			this.Start = string.Format("{0:00}:{1:00}:{2:00},{3:000}", num6, num5, num4, num3);
			num3 = num2 % 1000L;
			num4 = num2 / 1000L % 60L;
			num5 = num2 / 60000L % 60L;
			num6 = num2 / 3600000L % 60L;
			this.End = string.Format("{0:00}:{1:00}:{2:00},{3:000}", num6, num5, num4, num3);
		}
		public void UpdateSRTText()
		{
			this.UpdateTimeStrings();
			if (this.Text != null)
			{
				this.SRTText = this.Text;
				this.Scanned = true;
				return;
			}
			this.SRTText = "Line " + this.Number;
		}
		public void SetBitmapHash(byte[] array)
		{
			this.BitmapHash = 0uL;
			for (int i = 0; i < array.Length; i++)
			{
				this.BitmapHash = (ulong)array[i] + (this.BitmapHash << 6) + (this.BitmapHash << 16) - this.BitmapHash;
			}
		}
	}
}
