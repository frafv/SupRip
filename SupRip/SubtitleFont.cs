using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
namespace SupRip
{
	internal class SubtitleFont
	{
		public enum FontType
		{
			ProgramFont = 1,
			UserFont
		}
		private LinkedList<SubtitleLetter> letters;
		private SubtitleFont.FontType type;
		private string fontName;
		private string fileName;
		private bool changed;
		public string Name
		{
			get
			{
				return this.fontName;
			}
		}
		public bool Changed
		{
			get
			{
				return this.changed;
			}
		}
		public SubtitleFont(SubtitleFont.FontType t, string fn)
		{
			this.type = t;
			this.fontName = fn;
			this.letters = new LinkedList<SubtitleLetter>();
			if (this.type == SubtitleFont.FontType.ProgramFont)
			{
				this.fileName = Application.StartupPath + "\\" + this.fontName + ".font.txt";
			}
			else
			{
				this.fileName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SupRip\\" + this.fontName + ".font.txt";
			}
			if (File.Exists(this.fileName))
			{
				StreamReader streamReader = new StreamReader(this.fileName);
				string s = streamReader.ReadLine();
				while ((s = streamReader.ReadLine()) != null)
				{
					string[] array = streamReader.ReadLine().Trim().Split(new char[]
					{
						' '
					});
					if (array.Length != 2)
					{
						throw new FontfileFormatException("arraySize is screwed up: " + array);
					}
					int num = int.Parse(array[0]);
					int num2 = int.Parse(array[1]);
					byte[,] array2 = new byte[num2, num];
					for (int i = 0; i < num2; i++)
					{
						string[] array3 = streamReader.ReadLine().Trim().Split(new char[]
						{
							' '
						});
						if (array3.Length != num)
						{
							throw new FontfileFormatException(string.Concat(new object[]
							{
								"arrayLine is ",
								array3.Length,
								" instead of ",
								num
							}));
						}
						for (int j = 0; j < num; j++)
						{
							array2[i, j] = byte.Parse(array3[j]);
						}
					}
					this.letters.AddLast(new SubtitleLetter(array2, s));
					streamReader.ReadLine();
				}
				streamReader.Close();
			}
		}
		public void MoveLetters(SubtitleFont target)
		{
			foreach (SubtitleLetter current in this.letters)
			{
				target.AddLetter(current);
			}
			this.letters.Clear();
			this.changed = true;
		}
		public void AddLetter(SubtitleLetter l)
		{
			this.changed = true;
			this.letters.AddLast(l);
		}
		public string ListDuplicates()
		{
			string text = "";
			string[] array = new string[this.letters.Count];
			int num = 0;
			foreach (SubtitleLetter current in this.letters)
			{
				array[num++] = current.Text;
			}
			for (int i = 0; i < this.letters.Count; i++)
			{
				for (int j = i + 1; j < this.letters.Count; j++)
				{
					if (array[j] == array[i])
					{
						text = text + " " + array[j];
					}
				}
			}
			return text;
		}
		public SortedList<int, SubtitleLetter> FindMatch(SubtitleLetter l, int tolerance)
		{
			SortedList<int, SubtitleLetter> sortedList = new SortedList<int, SubtitleLetter>();
			foreach (SubtitleLetter current in this.letters)
			{
				if (current.BordersMatch(l))
				{
					int num = current.Matches(l);
					if (num == 0)
					{
						sortedList.Add(num, current);
						return sortedList;
					}
					if (AppOptions.IsEasilyConfusedLetter(current.Text))
					{
						num *= 10;
					}
					if (num < tolerance && !sortedList.ContainsKey(num))
					{
						sortedList.Add(num, current);
					}
				}
			}
			return sortedList;
		}
		public void Save()
		{
			if (this.fileName == null)
			{
				throw new Exception("filename is null on saving a font");
			}
			string path = this.fileName.Substring(0, this.fileName.LastIndexOf('\\'));
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			StreamWriter streamWriter = new StreamWriter(new FileStream(this.fileName, FileMode.Create));
			streamWriter.WriteLine("version 2");
			foreach (SubtitleLetter current in this.letters)
			{
				streamWriter.WriteLine(current.Text);
				streamWriter.WriteLine("{0} {1}", current.ImageWidth, current.ImageHeight);
				streamWriter.WriteLine(current.DumpLetter());
			}
			streamWriter.Close();
		}
		public void DeleteLetter(SubtitleLetter l2)
		{
			this.changed = true;
			this.letters.Remove(l2);
		}
		public static void DeleteUserFont(string name)
		{
			string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SupRip\\" + name + ".font.txt";
			File.Delete(path);
		}
	}
}
