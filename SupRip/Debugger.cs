using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
namespace SupRip
{
	internal class Debugger
	{
		private static StreamWriter sw;
		public static double lettersTime;
		public static double scanTime;
		public static double diffTime;
		public static double angleTime;
		public static double linesTime;
		public static double widenTime;
		public static double absDiffTime;
		public static double spacesTime;
		public static double translationTime;
		public static double extractTime;
		public static int translationFound;
		public static int translationNotFound;
		[Conditional("DEBUG")]
		public static void ResetTimes()
		{
		}
		[Conditional("DEBUG")]
		public static void PrintTimes()
		{
		}
		[Conditional("DEBUG")]
		public static void Print()
		{
		}
		[Conditional("DEBUG")]
		public static void Print(int i)
		{
		}
		[Conditional("DEBUG")]
		public static void OpenFile()
		{
			Debugger.sw = new StreamWriter(new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\debug.txt", FileMode.Create));
		}
		[Conditional("DEBUG")]
		public static void Print(string s)
		{
			StreamWriter arg_05_0 = Debugger.sw;
			Debugger.sw.WriteLine(s);
			Debugger.sw.Flush();
		}
		[Conditional("DEBUG")]
		public static void PrintTimestamped(string s)
		{
			StreamWriter arg_05_0 = Debugger.sw;
			DateTime now = DateTime.Now;
			Debugger.sw.WriteLine("{1:00}:{2:00}:{3:00},{4:000} {0}", new object[]
			{
				s,
				now.Hour,
				now.Minute,
				now.Second,
				now.Millisecond
			});
			Debugger.sw.Flush();
		}
		[Conditional("DEBUG")]
		public static void Draw2DArrayo(byte[,] b)
		{
			StreamWriter arg_05_0 = Debugger.sw;
			for (int i = 0; i < b.GetLength(0); i++)
			{
				for (int j = 0; j < b.GetLength(1); j++)
				{
					if (b[i, j] > 200)
					{
						Debugger.sw.Write("@");
					}
					else
					{
						if (b[i, j] > 10)
						{
							Debugger.sw.Write("#");
						}
						else
						{
							if (b[i, j] > 40)
							{
								Debugger.sw.Write("*");
							}
							else
							{
								if (b[i, j] > 1)
								{
									Debugger.sw.Write(".");
								}
								else
								{
									Debugger.sw.Write(" ");
								}
							}
						}
					}
				}
				Debugger.sw.WriteLine();
			}
			Debugger.sw.Flush();
		}
		[Conditional("DEBUG")]
		public static void Draw2DArray(byte[,] b)
		{
			StreamWriter arg_05_0 = Debugger.sw;
			for (int i = 0; i < b.GetLength(0); i++)
			{
				for (int j = 0; j < b.GetLength(1); j++)
				{
					Debugger.sw.Write(string.Format("{0,2:x}", b[i, j]));
				}
				Debugger.sw.WriteLine();
			}
			Debugger.sw.Flush();
		}
		public static void CleanUp()
		{
			if (Debugger.sw != null)
			{
				Debugger.sw.Close();
				Debugger.sw.Dispose();
			}
		}
		[Conditional("DEBUG")]
		public static void SaveBitmap(Bitmap bitmap)
		{
			bitmap.Save("c:\\temp\\temp.png");
		}
	}
}
