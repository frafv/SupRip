using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
namespace SupRip
{
	internal class SubtitleFile
	{
		public enum SubtitleType
		{
			Hddvd,
			Bluray
		}

		private SubtitleFile.SubtitleType type;
		private FileStream supFileStream;
		private List<SupData> supDatas;
		public SubtitleFile.SubtitleType Type
		{
			get
			{
				return this.type;
			}
		}
		public int NumSubtitles
		{
			get
			{
				return this.supDatas.Count;
			}
		}
		public SubtitleFile(string fileName)
		{
			this.LoadFromSup(fileName);
		}
		private void LoadFromSup(string supfile)
		{
			FileStream fileStream = new FileStream(supfile, FileMode.Open);
			char c = (char)fileStream.ReadByte();
			fileStream.Position = 0L;
			if (c == 'P')
			{
				this.LoadBluraySup(fileStream);
				return;
			}
			this.LoadHddvdSup(fileStream);
		}
		private void LoadHddvdSup(FileStream fs)
		{
			this.type = SubtitleFile.SubtitleType.Hddvd;
			List<SupData> list = new List<SupData>();
			byte[] array = new byte[4];
			int num = 0;
			while (fs.ReadByte() == 83 && fs.ReadByte() == 80)
			{
				SupData supData = new SupData();
				supData.startTime = fs.LowEndianInt32();
				supData.startTime /= 90L;
				fs.LowEndianInt32();
				long position = fs.Position;
				fs.LowEndianInt16();
				long num2 = position + fs.BigEndianInt32();
				supData.startControlSeq = position + fs.BigEndianInt32();
				long position2 = fs.Position;
				fs.Position = supData.startControlSeq;
				long num3 = 0L;
				while (fs.Position < num2)
				{
					int num4 = fs.BigEndianInt16();
					long num5 = position + fs.BigEndianInt32();
					if (num5 != num3 && num5 > fs.Position)
					{
						num3 = num5;
					}
					else
					{
						num3 = num2;
					}
					while (fs.Position < num3 && fs.Position < num2)
					{
						long arg_D7_0 = fs.Position;
						int num6 = fs.ReadByte();
						int num7 = num6;
						switch (num7)
						{
						case 1:
							break;
						case 2:
							if (num4 <= 0)
							{
								throw new Exception("timeOfControl is 0 when it should contain the duration");
							}
							supData.duration = ((num4 << 10) + 1023) / 90;
							break;
						case 3:
							throw new Exception("unexpected code 3");
						case 4:
							throw new Exception("unexpected code 4");
						case 5:
							throw new Exception("unexpected code 5");
						case 6:
							throw new Exception("unexpected code 6");
						case 7:
							throw new Exception("unexpected code 7");
						default:
							switch (num7)
							{
							case 131:
								for (int i = 0; i < 256; i++)
								{
									int num8 = fs.ReadByte() - 16;
									int num9 = fs.ReadByte() - 128;
									int num10 = fs.ReadByte() - 128;
									supData.hdColorSet[i, 0] = (byte)Math.Min(Math.Max(Math.Round((double)(1.1644f * (float)num8 + 1.596f * (float)num9)), 0.0), 255.0);
									supData.hdColorSet[i, 1] = (byte)Math.Min(Math.Max(Math.Round((double)(1.1644f * (float)num8 - 0.813f * (float)num9 - 0.391f * (float)num10)), 0.0), 255.0);
									supData.hdColorSet[i, 2] = (byte)Math.Min(Math.Max(Math.Round((double)(1.1644f * (float)num8 + 2.018f * (float)num10)), 0.0), 255.0);
								}
								break;
							case 132:
								for (int j = 0; j < 256; j++)
								{
									supData.hdTransparency[j] = (byte)fs.ReadByte();
									if (j == 0)
									{
										supData.hdTransparency[j] = 255;
									}
								}
								break;
							case 133:
								fs.Read(array, 0, 3);
								supData.bitmapPos.X = ((int)array[0] << 4 | (array[1] & 240) >> 4);
								supData.bitmapPos.Width = ((int)(array[1] & 15) << 8 | (int)array[2]) - supData.bitmapPos.X + 1;
								fs.Read(array, 0, 3);
								supData.bitmapPos.Y = ((int)array[0] << 4 | (array[1] & 240) >> 4);
								supData.bitmapPos.Height = ((int)(array[1] & 15) << 8 | (int)array[2]) - supData.bitmapPos.Y + 1;
								if (supData.bitmapPos.Width < 0 || supData.bitmapPos.Width > 1920 || supData.bitmapPos.Height < 0 || supData.bitmapPos.Height > 1080)
								{
									throw new Exception("subtitle bigger than it should be");
								}
								break;
							case 134:
								supData.bitmapStarts[0, 0] = position + fs.BigEndianInt32();
								supData.bitmapStarts[0, 1] = position + fs.BigEndianInt32();
								if (supData.bitmapStarts[0, 0] != position2)
								{
									throw new Exception(string.Concat(new object[]
									{
										"unexpected gap in the subtitle info: end_header = ",
										position2,
										", bitmap1 = ",
										supData.bitmapStarts[0, 0]
									}));
								}
								break;
							case 135:
								throw new Exception("unexpected code 87");
							default:
								if (num7 != 255)
								{
									throw new Exception("unrecognized block code");
								}
								supData.endCount++;
								break;
							}
							break;
						}
					}
				}
				supData.number = num++;
				supData.bitmapLengths[0, 0] = supData.bitmapStarts[0, 1] - supData.bitmapStarts[0, 0];
				supData.bitmapLengths[0, 1] = supData.startControlSeq - supData.bitmapStarts[0, 1];
				list.Add(supData);
				fs.Position = num2;
			}
			if (fs.Position == 1L)
			{
				fs.Close();
				throw new SUPFileFormatException();
			}
			SupData supData2 = null;
			foreach (SupData current in list)
			{
				if (supData2 != null)
				{
					supData2.endTime = current.startTime;
					supData2 = null;
				}
				if (current.duration == 0)
				{
					supData2 = current;
				}
				else
				{
					current.endTime = current.startTime + (long)((ulong)current.duration);
				}
			}
			this.supFileStream = fs;
			this.supDatas = list;
		}
		public void RecalculateTimes()
		{
			foreach (SupData current in this.supDatas)
			{
				current.UpdateSRTText();
			}
		}
		private void LoadBluraySup(FileStream fs)
		{
			bool flag = false;
			this.type = SubtitleFile.SubtitleType.Bluray;
			this.supDatas = new List<SupData>();
			SupData supData = new SupData();
			SupData supData2 = null;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			while (fs.ReadByte() == 80 && fs.ReadByte() == 71)
			{
				uint num4 = fs.BigEndianInt32() / 90u;
				uint arg_45_0 = fs.BigEndianInt32() / 90u;
				int num5 = fs.ReadByte();
				bool flag2 = false;
				int num6 = num5;
				switch (num6)
				{
				case 20:
				{
					int num7 = fs.BigEndianInt16();
					if (num7 == 2)
					{
						fs.BigEndianInt16();
						supData.emptySubtitle = true;
					}
					else
					{
						if (num7 % 5 != 2)
						{
							throw new Exception();
						}
						int num8 = (num7 - 2) / 5;
						fs.BigEndianInt16();
						for (int i = 0; i < num8; i++)
						{
							int num9 = fs.ReadByte();
							int num10 = fs.ReadByte() - 16;
							int num11 = fs.ReadByte() - 128;
							int num12 = fs.ReadByte() - 128;
							supData.hdColorSet[num9, 0] = (byte)Math.Min(Math.Max(Math.Round((double)(1.1644f * (float)num10 + 1.596f * (float)num11)), 0.0), 255.0);
							supData.hdColorSet[num9, 1] = (byte)Math.Min(Math.Max(Math.Round((double)(1.1644f * (float)num10 - 0.813f * (float)num11 - 0.391f * (float)num12)), 0.0), 255.0);
							supData.hdColorSet[num9, 2] = (byte)Math.Min(Math.Max(Math.Round((double)(1.1644f * (float)num10 + 2.018f * (float)num12)), 0.0), 255.0);
							supData.hdTransparency[num9] = (byte)fs.ReadByte();
						}
					}
					break;
				}
				case 21:
				{
					int num7 = fs.BigEndianInt16();
					fs.BigEndianInt16();
					fs.ReadByte();
					int num13 = fs.ReadByte();
					if (num13 == 192 || num13 == 128)
					{
						int num14 = fs.BigEndianInt24();
						if (num13 == 192 && num7 - num14 != 7)
						{
							throw new Exception("unexpected difference in data block lengths = " + (num7 - num14));
						}
						int width = fs.BigEndianInt16();
						int height = fs.BigEndianInt16();
						if (supData.bitmapStarts[num3, 0] != 0L)
						{
							throw new Exception("Unexpected data on multipart subtitle, cont = " + num13);
						}
						supData.bitmapPos.Width = width;
						supData.bitmapPos.Height = height;
						supData.bitmapStarts[num3, 0] = fs.Position;
						supData.bitmapLengths[num3, 0] = (long)(num7 - 11);
						fs.Position += (long)(num7 - 11);
					}
					else
					{
						if (num13 != 64)
						{
							throw new Exception("Unexpected continuation value = " + num13);
						}
						if (supData.bitmapStarts[num3, 1] != 0L)
						{
							throw new Exception("Three part Bluray bitmapdata, didn't think those would exist");
						}
						if (supData.bitmapStarts[num3, 0] == 0L)
						{
							throw new Exception("Unexpected data on second part of multipart subtitle, start = " + supData.bitmapStarts[num3, 0]);
						}
						supData.bitmapStarts[num3, 1] = fs.Position;
						supData.bitmapLengths[num3, 1] = (long)(num7 - 4);
						fs.Position += (long)(num7 - 4);
					}
					if ((num13 & 64) != 0)
					{
						num3 = 1;
					}
					break;
				}
				case 22:
				{
					int num7 = fs.BigEndianInt16();
					supData.startTime = (long)((ulong)num4);
					fs.BigEndianInt16();
					fs.BigEndianInt16();
					fs.ReadByte();
					fs.Position += 5L;
					int num15 = fs.ReadByte();
					if (num15 * 8 + 11 != num7)
					{
						throw new Exception(string.Concat(new object[]
						{
							"Mismatched Info Blocks: Datalength = ",
							num7,
							", numBlocks = ",
							num15
						}));
					}
					long arg_116_0 = supData.startTime / 1000L;
					if (num15 == 1)
					{
						fs.Position += 3L;
						supData.Forced = ((fs.ReadByte() & 64) != 0);
						fs.BigEndianInt16();
						fs.BigEndianInt16();
					}
					else
					{
						if (num15 == 2)
						{
							fs.Position += 3L;
							int num16 = fs.ReadByte();
							supData.Forced = ((num16 & 64) != 0);
							fs.BigEndianInt16();
							fs.BigEndianInt16();
							fs.Position += 3L;
							fs.ReadByte();
							supData.Forced = ((num16 & 64) != 0);
							fs.BigEndianInt16();
							fs.BigEndianInt16();
						}
					}
					break;
				}
				case 23:
				{
					int num7 = fs.BigEndianInt16();
					supData.numWindows = fs.ReadByte();
					if (supData.numWindows != 1 && supData.numWindows != 2)
					{
						throw new SUPFileFormatException("Number of SUP Window descriptions = " + supData.numWindows);
					}
					fs.Position += 1L;
					supData.bitmapPos.X = fs.BigEndianInt16();
					supData.bitmapPos.Y = fs.BigEndianInt16();
					supData.bitmapPos.Width = fs.BigEndianInt16();
					supData.bitmapPos.Height = fs.BigEndianInt16();
					if (supData.numWindows == 2)
					{
						fs.Position += 1L;
						supData.bitmapPos2.X = fs.BigEndianInt16();
						supData.bitmapPos2.Y = fs.BigEndianInt16();
						supData.bitmapPos2.Width = fs.BigEndianInt16();
						supData.bitmapPos2.Height = fs.BigEndianInt16();
						if (flag)
						{
						}
					}
					break;
				}
				default:
				{
					if (num6 != 128)
					{
						throw new Exception("unknown code");
					}
					if (supData.bitmapStarts[0, 0] == 0L)
					{
						supData.emptySubtitle = true;
					}
					int num7 = fs.BigEndianInt16();
					if (num7 != 0)
					{
						throw new Exception("code 80, length != 0");
					}
					flag2 = true;
					break;
				}
				}
				if (flag2)
				{
					if (supData.emptySubtitle)
					{
						if (supData2 != null)
						{
							supData2.endTime = supData.startTime - 1L;
						}
					}
					else
					{
						if (supData2 != null && supData2.endTime == 0L)
						{
							supData2.endTime = supData.startTime - 1L;
						}
						supData.number = num2++;
						this.supDatas.Add(supData);
						supData2 = supData;
					}
					supData = new SupData();
					num3 = 0;
				}
				num++;
			}
			if (fs.Position == 1L)
			{
				fs.Close();
				throw new SUPFileFormatException();
			}
			if (supData.bitmapStarts[0, 0] == 0L && supData.startTime != 0L && supData2 != null)
			{
				supData2.endTime = supData.startTime - 1L;
			}
			this.supFileStream = fs;
			if (AppOptions.combineSubtitles)
			{
				this.CleanupDuplicateSubtitles();
			}
			foreach (SupData current in this.supDatas)
			{
				current.UpdateSRTText();
			}
		}
		private void CleanupDuplicateSubtitles()
		{
			SupData supData = null;
			LinkedList<SupData> linkedList = new LinkedList<SupData>();
			foreach (SupData current in this.supDatas)
			{
				if (supData != null && supData.bitmapLengths[0, 0] == current.bitmapLengths[0, 0] && supData.bitmapLengths[0, 1] == current.bitmapLengths[0, 1] && supData.bitmapPos == current.bitmapPos && Math.Abs(current.startTime - supData.endTime) < 100L && this.CompareBitmaps(current, supData))
				{
					supData.endTime = current.endTime;
					linkedList.AddLast(current);
				}
				else
				{
					supData = current;
				}
			}
			foreach (SupData current2 in linkedList)
			{
				this.supDatas.Remove(current2);
			}
		}
		private void WriteIdxFile(StreamWriter swidx)
		{
			int num = 720;
			int num2 = 576;
			swidx.WriteLine("# VobSub index file, v7 (do not modify this line!)");
			swidx.WriteLine("# Created by SubtitleCreator v2.3rc1");
			swidx.WriteLine("# ");
			swidx.WriteLine("# To repair desyncronization, you can insert gaps this way:");
			swidx.WriteLine("# (it usually happens after vob id changes)");
			swidx.WriteLine("# ");
			swidx.WriteLine("#\t delay: [sign]hh:mm:ss:ms");
			swidx.WriteLine("# ");
			swidx.WriteLine("# Where:");
			swidx.WriteLine("#\t [sign]: +, - (optional)");
			swidx.WriteLine("#\t hh: hours (0 <= hh)");
			swidx.WriteLine("#\t mm/ss: minutes/seconds (0 <= mm/ss <= 59)");
			swidx.WriteLine("#\t ms: milliseconds (0 <= ms <= 999)");
			swidx.WriteLine("# ");
			swidx.WriteLine("#\t Note: You can't position a sub before the previous with a negative value.");
			swidx.WriteLine("# ");
			swidx.WriteLine("# You can also modify timestamps or delete a few subs you don't like.");
			swidx.WriteLine("# Just make sure they stay in increasing order.");
			swidx.WriteLine();
			swidx.WriteLine();
			swidx.WriteLine("# Settings");
			swidx.WriteLine();
			swidx.WriteLine("# Original frame size");
			swidx.WriteLine("size: {0}x{1}", num, num2);
			swidx.WriteLine();
			swidx.WriteLine("# Origin, relative to the upper-left corner, can be overloaded by aligment");
			swidx.WriteLine("org: 0, 0");
			swidx.WriteLine();
			swidx.WriteLine("# Image scaling (hor,ver), origin is at the upper-left corner or at the alignment coord (x, y)");
			swidx.WriteLine("scale: 100%, 100%");
			swidx.WriteLine();
			swidx.WriteLine("# Alpha blending");
			swidx.WriteLine("alpha: 100%");
			swidx.WriteLine();
			swidx.WriteLine("# Smoothing for very blocky images (use OLD for no filtering)");
			swidx.WriteLine("smooth: OFF");
			swidx.WriteLine();
			swidx.WriteLine("# In millisecs");
			swidx.WriteLine("fadein/out: 0, 0");
			swidx.WriteLine();
			swidx.WriteLine("# Force subtitle placement relative to (org.x, org.y)");
			swidx.WriteLine("align: OFF at LEFT TOP");
			swidx.WriteLine();
			swidx.WriteLine("# For correcting non-progressive desync. (in millisecs or hh:mm:ss:ms)");
			swidx.WriteLine("# Note: Not effective in DirectVobSub, use \"delay: ... \" instead.");
			swidx.WriteLine("time offset: 0");
			swidx.WriteLine();
			swidx.WriteLine("# ON: displays only forced subtitles, OFF: shows everything");
			swidx.WriteLine("forced subs: OFF");
			swidx.WriteLine();
			swidx.WriteLine("# The palette of the generated file");
			swidx.Write("palette: ");
			swidx.WriteLine("# Custom colors (transp idxs and the four colors)");
			string str = "English";
			int num3 = 32;
			swidx.WriteLine("# " + str);
			swidx.WriteLine("id: en, index: " + (num3 - 32));
			swidx.WriteLine("# Decomment next line to activate alternative name in DirectVobSub / Windows Media Player 6.x");
			swidx.WriteLine("# alt: " + str);
			swidx.WriteLine("# Vob/Cell ID: 1, 1 (PTS: 0)");
		}
		[Conditional("DEBUG")]
		private void WriteSub()
		{
			var fileStream = new SubtitleImageStream(new FileStream("c:\\temp\\out.sub", FileMode.Create, FileAccess.Write));
			byte[] array = new byte[29];
			byte b = 32;
			long num = 0L;
			long num2 = 0L;
			StreamWriter streamWriter = new StreamWriter("c:\\temp\\test.idx");
			this.WriteIdxFile(streamWriter);
			array[0] = 0;
			array[1] = 0;
			array[2] = 1;
			array[3] = 186;
			array[4] = 68;
			array[5] = 2;
			array[6] = 196;
			array[7] = 130;
			array[8] = 4;
			array[9] = 169;
			array[10] = 1;
			array[11] = 137;
			array[12] = 195;
			array[13] = 248;
			array[14] = 0;
			array[15] = 0;
			array[16] = 1;
			array[17] = 189;
			array[18] = 19;
			array[19] = 221;
			array[20] = 129;
			array[21] = 128;
			array[22] = 5;
			int num3 = 10810;
			byte[] array2 = new byte[]
			{
				(byte)(num3 >> 24),
				(byte)(num3 >> 16 & 255),
				(byte)(num3 >> 8 & 255),
				(byte)(num3 & 255)
			};
			int num4 = (int)Math.Round((double)((int)array2[0] | (int)array2[1] << 8 | (int)array2[2] << 16 | (int)array2[3] << 24) / 90.09);
			int num5 = num4 / 1000;
			num4 -= num5 * 1000;
			int num6 = num5 / 60;
			num5 -= num6 * 60;
			int num7 = num6 / 60;
			num6 -= num7 * 60;
			DateTime dateTime = new DateTime(1970, 2, 28, num7, num6, num5, num4);
			streamWriter.WriteLine("timestamp: {0:HH\\:mm\\:ss\\:fff}, filepos: {1:x9}", dateTime, num2 + num);
			array[23] = (byte)((array2[3] & 192) >> 5 | 33);
			array[24] = (byte)((int)(array2[3] & 63) << 2 | (array2[2] & 192) >> 6);
			array[25] = (byte)((int)(array2[2] & 63) << 2 | (array2[1] & 128) >> 6 | 1);
			array[26] = (byte)((int)(array2[1] & 127) << 1 | (array2[0] & 128) >> 7);
			array[27] = (byte)((int)(array2[0] & 127) << 1 | 1);
			array[28] = b;
			fileStream.Write(array);
			fileStream.WriteByte(96);
			fileStream.WriteByte(58);
			fileStream.WriteByte(32);
			fileStream.WriteByte(12);
			fileStream.WriteByte(240);
			fileStream.WriteByte(12);
			fileStream.WriteByte(241);
			fileStream.WriteByte(3);
			fileStream.WriteByte(252);
			fileStream.WriteByte(36);
			fileStream.WriteByte(26);
			fileStream.WriteByte(32);
			fileStream.WriteByte(26);
			fileStream.WriteByte(6);
			fileStream.WriteByte(78);
			fileStream.WriteByte(9);
			fileStream.WriteByte(1);
			fileStream.WriteByte(32);
			fileStream.WriteByte(24);
			fileStream.WriteByte(65);
			for (int i = 0; i < 564; i++)
			{
				this.WritePixels(fileStream, 999, 0);
			}
			long num8 = 2048L - fileStream.Position % 2048L;
			int num9 = 0;
			while ((long)num9 < num8)
			{
				fileStream.WriteByte(255);
				num9++;
			}
			fileStream.Close();
		}
		private void WritePixels(SubtitleImageStream fs, int num, byte color)
		{
			if (num < 4)
			{
				fs.Write2Bits((byte)num);
				fs.Write2Bits(color);
				return;
			}
			if (num < 16)
			{
				fs.Write2Bits(0);
				fs.Write2Bits((byte)(num >> 2));
				fs.Write2Bits((byte)(num & 3));
				fs.Write2Bits(color);
				return;
			}
			if (num < 64)
			{
				fs.Write2Bits(0);
				fs.Write2Bits(0);
				fs.Write2Bits((byte)(num >> 4));
				fs.Write2Bits((byte)(num >> 2 & 3));
				fs.Write2Bits((byte)(num & 3));
				fs.Write2Bits(color);
				return;
			}
			if (num < 256)
			{
				fs.Write2Bits(0);
				fs.Write2Bits(0);
				fs.Write2Bits(0);
				fs.Write2Bits((byte)(num >> 6));
				fs.Write2Bits((byte)(num >> 4 & 3));
				fs.Write2Bits((byte)(num >> 2 & 3));
				fs.Write2Bits((byte)(num & 3));
				fs.Write2Bits(color);
				return;
			}
			fs.Write2Bits(0);
			fs.Write2Bits(0);
			fs.Write2Bits(0);
			fs.Write2Bits(0);
			fs.Write2Bits(0);
			fs.Write2Bits(0);
			fs.Write2Bits(0);
			fs.Write2Bits(color);
		}
		private void WriteType1(SubtitleImageStream fs)
		{
			fs.Write2Bits(2);
			fs.Write2Bits(0);
			fs.Write2Bits(3);
			fs.Write2Bits(3);
			fs.Write2Bits(3);
			fs.Write2Bits(3);
		}
		private void WriteType2(SubtitleImageStream fs)
		{
			fs.Write2Bits(2);
			fs.Write2Bits(3);
			fs.Write2Bits(3);
			fs.Write2Bits(3);
			fs.Write2Bits(3);
			fs.Write2Bits(3);
		}
		private void WriteType3(SubtitleImageStream fs)
		{
			fs.Write2Bits(2);
			fs.Write2Bits(0);
			fs.Write2Bits(2);
			fs.Write2Bits(0);
			fs.Write2Bits(3);
			fs.Write2Bits(3);
		}

		private Bitmap GetHdBitmap(SupData data, byte[] buffer)
		{
			int width = data.bitmapPos.Width;
			int height = data.bitmapPos.Height;
			byte[] array = new byte[width * height * 4];
			var memoryStream = new SubtitleImageStream(buffer);
			bool flag = false;
			int i = 0;
			int num = width * 4;
			for (int j = 0; j < 2; j++)
			{
				long num2;
				int num3;
				if (j == 0)
				{
					memoryStream.Position = 0L;
					num2 = data.bitmapLengths[0, 0];
					num3 = 0;
				}
				else
				{
					memoryStream.Position = data.bitmapLengths[0, 0];
					num2 = data.bitmapLengths[0, 0] + data.bitmapLengths[0, 1];
					num3 = 1;
				}
				while (num3 < height && memoryStream.Position < num2)
				{
					int num4 = memoryStream.Read2Bits();
					int num5;
					if ((num4 & 1) == 0)
					{
						num5 = memoryStream.Read2Bits();
					}
					else
					{
						num5 = memoryStream.Read2Bits();
						num5 = (num5 << 2) + memoryStream.Read2Bits();
						num5 = (num5 << 2) + memoryStream.Read2Bits();
						num5 = (num5 << 2) + memoryStream.Read2Bits();
					}
					if ((num4 & 2) == 0)
					{
						num4 = 1;
					}
					else
					{
						num4 = memoryStream.Read2Bits();
						if ((num4 & 2) == 0)
						{
							num4 = (num4 << 2) + memoryStream.Read2Bits();
							num4 += 2;
						}
						else
						{
							num4 &= 1;
							num4 = (num4 << 2) + memoryStream.Read2Bits();
							num4 = (num4 << 2) + memoryStream.Read2Bits();
							num4 = (num4 << 2) + memoryStream.Read2Bits();
							if (num4 == 0)
							{
								num4 = data.bitmapPos.Width - i;
								flag = true;
							}
							else
							{
								num4 += 9;
							}
						}
					}
					int num6 = i + num4;
					while (i < num6)
					{
						array[num3 * num + i * 4] = data.hdColorSet[num5, 0];
						array[num3 * num + i * 4 + 1] = data.hdColorSet[num5, 1];
						array[num3 * num + i * 4 + 2] = data.hdColorSet[num5, 2];
						array[num3 * num + i * 4 + 3] = (byte)(255 - data.hdTransparency[num5]);
						i++;
					}
					if (flag || i >= width)
					{
						flag = false;
						i = 0;
						num3 += 2;
						memoryStream.Read2Bits(true);
					}
				}
			}
			Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
			IntPtr scan = bitmapData.Scan0;
			Marshal.Copy(array, 0, scan, array.Length);
			bitmap.UnlockBits(bitmapData);
			memoryStream.Close();
			return bitmap;
		}
		private byte[] ReadIntoMemory(FileStream fs, SupData data)
		{
			byte[] array;
			if (this.type == SubtitleFile.SubtitleType.Bluray)
			{
				array = new byte[data.bitmapLengths[0, 0] + data.bitmapLengths[0, 1]];
				fs.Position = data.bitmapStarts[0, 0];
				fs.Read(array, 0, (int)data.bitmapLengths[0, 0]);
				fs.Position = data.bitmapStarts[0, 1];
				fs.Read(array, (int)data.bitmapLengths[0, 0], (int)data.bitmapLengths[0, 1]);
			}
			else
			{
				if (this.type != SubtitleFile.SubtitleType.Hddvd)
				{
					throw new Exception("trying to get an image without a type set");
				}
				array = new byte[data.startControlSeq - data.bitmapStarts[0, 0]];
				fs.Position = data.bitmapStarts[0, 0];
				fs.Read(array, 0, array.Length);
			}
			return array;
		}

		internal byte[] ReadBitmap(int n)
		{
			return this.ReadIntoMemory(this.supFileStream, this.supDatas[n]);
		}

		internal Bitmap GetBitmap(int n, byte[] data)
		{
			return GetBitmap(this.supDatas[n], data);
		}

		public Bitmap GetBitmap(SupData data)
		{
			return GetBitmap(data, this.ReadIntoMemory(this.supFileStream, data));
		}

		private Bitmap GetBitmap(SupData data, byte[] buffer)
		{
			if (this.type == SubtitleFile.SubtitleType.Bluray)
			{
				return this.GetBlurayBitmap(data, buffer);
			}
			if (this.type == SubtitleFile.SubtitleType.Hddvd)
			{
				return this.GetHdBitmap(data, buffer);
			}
			throw new Exception("trying to get an image without a type set");
		}

		private bool CompareBitmaps(SupData a, SupData b)
		{
			if (a.BitmapHash == 0uL)
			{
				a.SetBitmapHash(this.ReadIntoMemory(this.supFileStream, a));
			}
			if (b.BitmapHash == 0uL)
			{
				b.SetBitmapHash(this.ReadIntoMemory(this.supFileStream, b));
			}
			return a.BitmapHash == b.BitmapHash;
		}
		private Bitmap GetBlurayBitmap(SupData data, byte[] buffer)
		{
			int width = data.bitmapPos.Width;
			int height = data.bitmapPos.Height;
			byte[] array = new byte[width * height * 4];
			bool flag = false;
			int i = 0;
			int j = 0;
			int num = width * 4;
			var memoryStream = new SubtitleImageStream(buffer);
			int num2 = 0;
			while (j < height)
			{
				byte b = (byte)memoryStream.ReadByte();
				int num3;
				if (b != 0)
				{
					num3 = (int)b;
					num2 = 1;
				}
				else
				{
					int num4 = memoryStream.Read2Bits();
					if ((num4 & 2) == 0)
					{
						num3 = 0;
						if ((num4 & 1) == 0)
						{
							int num5 = 0;
							num5 = (num5 << 2) + memoryStream.Read2Bits();
							num5 = (num5 << 2) + memoryStream.Read2Bits();
							num5 = (num5 << 2) + memoryStream.Read2Bits();
							if (num5 == 0)
							{
								flag = true;
							}
							else
							{
								num2 = num5;
							}
						}
						else
						{
							int num5 = 0;
							num5 = (num5 << 2) + memoryStream.Read2Bits();
							num5 = (num5 << 2) + memoryStream.Read2Bits();
							num5 = (num5 << 2) + memoryStream.Read2Bits();
							num5 = (num5 << 8) + memoryStream.ReadByte();
							num2 = num5;
						}
					}
					else
					{
						if ((num4 & 1) == 0)
						{
							int num5 = 0;
							num5 = (num5 << 2) + memoryStream.Read2Bits();
							num5 = (num5 << 2) + memoryStream.Read2Bits();
							num5 = (num5 << 2) + memoryStream.Read2Bits();
							num2 = num5;
						}
						else
						{
							int num5 = 0;
							num5 = (num5 << 2) + memoryStream.Read2Bits();
							num5 = (num5 << 2) + memoryStream.Read2Bits();
							num5 = (num5 << 2) + memoryStream.Read2Bits();
							num5 = (num5 << 8) + memoryStream.ReadByte();
							num2 = num5;
						}
						num3 = memoryStream.ReadByte();
					}
				}
				if (!flag)
				{
					int num6 = i + num2;
					if (num6 > width)
					{
						throw new Exception("line too long");
					}
					while (i < num6)
					{
						array[j * num + i * 4] = data.hdColorSet[num3, 0];
						array[j * num + i * 4 + 1] = data.hdColorSet[num3, 1];
						array[j * num + i * 4 + 2] = data.hdColorSet[num3, 2];
						array[j * num + i * 4 + 3] = data.hdTransparency[num3];
						i++;
					}
				}
				else
				{
					if (i < width)
					{
						throw new Exception("unfinished line");
					}
					flag = false;
					i = 0;
					j++;
				}
			}
			Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
			IntPtr scan = bitmapData.Scan0;
			Marshal.Copy(array, 0, scan, array.Length);
			bitmap.UnlockBits(bitmapData);
			return bitmap;
		}
		public SubtitleImage GetSubtitleImage(int n)
		{
			return new SubtitleImage(this.GetBitmap(this.supDatas[n]));
		}
		public void UpdateSubtitleText(int n, SubtitleImage si)
		{
			this.supDatas[n].Text = si.GetText();
			this.supDatas[n].UpdateSRTText();
		}
		public string GetSRTText()
		{
			StringBuilder stringBuilder = new StringBuilder(10000);
			string text = null;
			string str = null;
			string str2 = null;
			int num = 1;
			foreach (SupData current in this.supDatas)
			{
				string text2 = current.SRTText;
				if (AppOptions.convertDoubleApostrophes)
				{
					text2 = text2.Replace("''", "\"");
				}
				if (AppOptions.stripFormatting)
				{
					text2 = text2.Replace("<i>", "").Replace("</i>", "");
				}
				if (!AppOptions.forcedOnly || current.Forced)
				{
					if (AppOptions.combineSubtitles)
					{
						if (text != null)
						{
							if (text2.CompareTo(text) == 0)
							{
								str2 = current.End;
							}
							else
							{
								stringBuilder.Append(num + "\r\n");
								stringBuilder.Append(str + " --> " + str2 + "\r\n");
								stringBuilder.Append(text);
								stringBuilder.Append("\r\n\r\n");
								num++;
								str = current.Start;
								str2 = current.End;
								text = text2;
							}
						}
						else
						{
							str = current.Start;
							str2 = current.End;
							text = text2;
						}
					}
					else
					{
						stringBuilder.Append(num + "\r\n");
						stringBuilder.Append(current.Start + " --> " + current.End + "\r\n");
						stringBuilder.Append(text2);
						stringBuilder.Append("\r\n\r\n");
						num++;
					}
				}
			}
			if (AppOptions.combineSubtitles && text != null)
			{
				stringBuilder.Append(num + "\r\n");
				stringBuilder.Append(str + " --> " + str2 + "\r\n");
				stringBuilder.Append(text);
				stringBuilder.Append("\r\n\r\n");
			}
			return stringBuilder.ToString();
		}
		public SRTInfo GetSRTInfo()
		{
			SRTInfo sRTInfo = new SRTInfo();
			foreach (SupData current in this.supDatas)
			{
				if (!current.Scanned)
				{
					sRTInfo.unscanned++;
				}
				else
				{
					if (current.SRTText.Contains("¤"))
					{
						sRTInfo.containingErrors++;
					}
					else
					{
						sRTInfo.finished++;
					}
				}
			}
			return sRTInfo;
		}
		public void WriteBitmaps(string supName, int numSubtitles, int spacing)
		{
			Bitmap[] array = new Bitmap[numSubtitles];
			int i = 0;
			int num = 0;
			while (i < this.supDatas.Count)
			{
				int num2 = 0;
				int num3 = 0;
				numSubtitles = Math.Min(numSubtitles, this.supDatas.Count - i);
				for (int j = i; j < i + numSubtitles; j++)
				{
					SubtitleImage subtitleImage = new SubtitleImage(this.GetBitmap(this.supDatas[j]));
					array[j - i] = subtitleImage.GetBitmap();
					if (array[j - i].Width > num2)
					{
						num2 = array[j - i].Width;
					}
					num3 += array[j - i].Height + spacing;
				}
				Bitmap bitmap = new Bitmap(num2, num3, PixelFormat.Format32bppArgb);
				Graphics graphics = Graphics.FromImage(bitmap);
				graphics.FillRectangle(new SolidBrush(Color.Black), 0, 0, bitmap.Width, bitmap.Height);
				int num4 = 0;
				for (int k = 0; k < numSubtitles; k++)
				{
					graphics.DrawImage(array[k], 0, num4);
					num4 += array[k].Height + spacing;
				}
				int length = supName.LastIndexOf('.');
				string filename = string.Concat(new object[]
				{
					supName.Substring(0, length),
					".",
					num,
					".png"
				});
				bitmap.Save(filename);
				num++;
				i += numSubtitles;
			}
		}
		public bool IsSubtitleForced(int n)
		{
			return this.supDatas[n].Forced;
		}
		[Conditional("DEBUG")]
		public void SaveXml(string fileName)
		{
			StreamWriter streamWriter = new StreamWriter(fileName);
			foreach (SupData current in this.supDatas)
			{
				streamWriter.WriteLine("\t<subpicture>");
				streamWriter.WriteLine("\t\t<start>" + current.startTime.ToString() + "</start>");
				streamWriter.WriteLine("\t\t<end>" + current.endTime.ToString() + "</end>");
				streamWriter.WriteLine("\t\t<forced>" + (current.Forced ? "true" : "false") + "</forced>");
				streamWriter.WriteLine("\t</subpicture>");
			}
			streamWriter.Close();
		}
		public void Close()
		{
			this.supFileStream.Close();
		}
	}
}
