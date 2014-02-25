using System;
using System.IO;

namespace SupRip
{
	internal class SubtitleImageStream
	{
		private int val;
		private byte pos = 4;
		private byte writeVal;
		private int writePos = 6;
		private Stream fs;

		public SubtitleImageStream(byte[] data)
		{
			this.fs = new MemoryStream(data, false);
		}

		public SubtitleImageStream(Stream fs)
		{
			this.fs = fs;
		}

		public int ReadByte()
		{
			return this.fs.ReadByte();
		}

		public void WriteByte(byte value)
		{
			this.fs.WriteByte(value);
		}

		public void Write(byte[] buffer)
		{
			var writer = new BinaryWriter(this.fs);
			writer.Write(buffer);
		}

		public long Position
		{
			get { return this.fs.Position; }
			set { this.fs.Position = value; }
		}

		public void Close()
		{
			this.fs.Close();
		}

		public int Read2Bits()
		{
			if (this.pos >= 3)
			{
				this.pos = 0;
				this.val = fs.ReadByte();
			}
			else
			{
				this.pos += 1;
			}
			int num = (int)(2 * (3 - this.pos));
			return (this.val & 3 << num) >> num;
		}

		public void Write2Bits(byte v)
		{
			this.writeVal += (byte)(v << this.writePos);
			this.writePos -= 2;
			if (this.writePos < 0)
			{
				fs.WriteByte(this.writeVal);
				this.writePos = 6;
				this.writeVal = 0;
			}
		}

		public void Write2Bits(bool flush)
		{
			if (flush)
			{
				fs.WriteByte(this.writeVal);
				this.pos = 6;
				this.writeVal = 0;
			}
		}

		public void Read2Bits(bool flush)
		{
			if (flush)
			{
				this.pos = 4;
			}
		}
	}
}
