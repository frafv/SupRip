using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
namespace SupRip
{
	internal class SubtitleFonts
	{
		private LinkedList<SubtitleFont> fonts;
		private SubtitleFont defaultFont;
		private SubtitleFont userFont;
		private Hashtable fontStats;
		public LinkedList<PositionedString> debugStrings;
		public string DefaultFontName
		{
			get
			{
				if (this.defaultFont == null)
				{
					return "-";
				}
				return this.defaultFont.Name;
			}
		}
		public int Count
		{
			get
			{
				return this.fonts.Count;
			}
		}
		public SubtitleFonts()
		{
			this.fonts = new LinkedList<SubtitleFont>();
			this.defaultFont = null;
			this.fontStats = new Hashtable();
			this.debugStrings = new LinkedList<PositionedString>();
			DirectoryInfo directoryInfo = new DirectoryInfo(".");
			FileInfo[] files = directoryInfo.GetFiles("*.font.txt");
			FileInfo[] array = files;
			for (int i = 0; i < array.Length; i++)
			{
				FileInfo fileInfo = array[i];
				string text = fileInfo.Name;
				text = text.Substring(0, text.LastIndexOf('.'));
				text = text.Substring(0, text.LastIndexOf('.'));
				this.fonts.AddLast(new SubtitleFont(SubtitleFont.FontType.ProgramFont, text));
			}
			try
			{
				this.userFont = new SubtitleFont(SubtitleFont.FontType.UserFont, "temp");
			}
			catch (FontfileFormatException)
			{
				SubtitleFont.DeleteUserFont("temp");
				this.userFont = new SubtitleFont(SubtitleFont.FontType.UserFont, "temp");
			}
		}
		public string[] FontList()
		{
			string[] array = new string[this.fonts.Count];
			int num = 0;
			foreach (SubtitleFont current in this.fonts)
			{
				array[num++] = current.Name;
			}
			return array;
		}
		public void Save()
		{
			foreach (SubtitleFont current in this.fonts)
			{
				if (current.Changed)
				{
					current.Save();
				}
			}
			if (this.userFont.Changed)
			{
				this.userFont.Save();
			}
		}
		public SubtitleLetter FindMatch(SubtitleLetter l, int tolerance)
		{
			if (this.defaultFont == null)
			{
				foreach (DictionaryEntry dictionaryEntry in this.fontStats)
				{
					if ((int)dictionaryEntry.Value > 10)
					{
						this.SetDefaultFont((string)dictionaryEntry.Key);
					}
				}
			}
			SortedList<int, SubtitleLetter> sortedList = this.userFont.FindMatch(l, tolerance / 10);
			if (sortedList.Count > 0)
			{
				return sortedList.Values[0];
			}
			if (this.defaultFont != null)
			{
				SortedList<int, SubtitleLetter> sortedList2 = this.defaultFont.FindMatch(l, tolerance);
				if (sortedList2.Count > 0)
				{
					this.debugStrings.AddLast(new PositionedString(l.Coords, sortedList2.Keys[0].ToString()));
					return sortedList2.Values[0];
				}
			}
			SortedList<int, SubtitleLetter> sortedList3 = new SortedList<int, SubtitleLetter>();
			foreach (SubtitleFont current in this.fonts)
			{
				if (current != this.defaultFont)
				{
					SortedList<int, SubtitleLetter> sortedList4 = current.FindMatch(l, tolerance);
					foreach (KeyValuePair<int, SubtitleLetter> current2 in sortedList4)
					{
						if (!sortedList3.ContainsKey(current2.Key))
						{
							sortedList3.Add(current2.Key, current2.Value);
						}
					}
					if (sortedList4.Count > 0)
					{
						if (this.fontStats[current.Name] == null)
						{
							this.fontStats[current.Name] = 1;
						}
						else
						{
							this.fontStats[current.Name] = (int)this.fontStats[current.Name] + 1;
						}
					}
				}
			}
			if (sortedList3.Count > 0)
			{
				this.debugStrings.AddLast(new PositionedString(l.Coords, sortedList3.Keys[0].ToString()));
				return sortedList3.Values[0];
			}
			if (sortedList.Count > 0)
			{
				return sortedList.Values[0];
			}
			return null;
		}
		private void SetDefaultFont(string name)
		{
			foreach (SubtitleFont current in this.fonts)
			{
				if (current.Name == name)
				{
					this.defaultFont = current;
					return;
				}
			}
			throw new Exception("Trying to set an unknown font as default");
		}
		public string ListDuplicates()
		{
			return this.userFont.ListDuplicates();
		}
		public void AddLetter(SubtitleLetter l)
		{
			this.userFont.AddLetter(l);
		}
		public void DeleteLetter(SubtitleLetter l2)
		{
			SubtitleLetter l3 = this.FindMatch(l2, AppOptions.similarityTolerance * 100);
			this.userFont.DeleteLetter(l3);
		}
		public void MergeUserFont(string targetFontName)
		{
			SubtitleFont subtitleFont = null;
			foreach (SubtitleFont current in this.fonts)
			{
				if (current.Name == targetFontName)
				{
					subtitleFont = current;
					break;
				}
			}
			if (subtitleFont == null)
			{
				throw new Exception("invalid font name for merge fonts");
			}
			this.userFont.MoveLetters(subtitleFont);
		}
	}
}
