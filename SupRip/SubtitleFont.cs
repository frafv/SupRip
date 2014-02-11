using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
		private List<SubtitleLetter> letters;
		private SubtitleFont.FontType type;
		private string fileName;
		public string Name
		{
			get;
			private set;
		}
		public bool Changed
		{
			get;
			private set;
		}
		internal IEnumerable<SubtitleLetter> AllLetters
		{
			get { return this.letters; }
		}
		public SubtitleFont(SubtitleFont.FontType t, string fn)
		{
			this.type = t;
			this.Name = fn;
			this.letters = new List<SubtitleLetter>();
			if (this.type == SubtitleFont.FontType.ProgramFont)
			{
				this.fileName = Application.StartupPath + "\\" + this.Name + ".font.txt";
			}
			else
			{
				this.fileName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SupRip\\" + this.Name + ".font.txt";
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
					this.letters.Add(new SubtitleLetter(array2, s));
					streamReader.ReadLine();
				}
				streamReader.Close();
			}
		}
		public void MoveLetters(SubtitleFont target)
		{
			target.letters.AddRange(this.letters);
			this.letters.Clear();
			this.Changed = true;
		}
		public void AddLetter(SubtitleLetter l)
		{
			this.Changed = true;
			this.letters.Add(l);
		}
		public string ListDuplicates()
		{
			return String.Join(" ", this.letters.Select(letter => letter.Text).GroupBy(text => text)
				.Where(group => group.Count() > 1).Select(group => group.Key));
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
			this.Changed = true;
			this.letters.Remove(l2);
		}
		public static void DeleteUserFont(string name)
		{
			string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SupRip", name + ".font.txt");
			File.Delete(path);
		}
	}
}
