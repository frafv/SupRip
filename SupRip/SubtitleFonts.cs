using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SupRip
{
	internal class SubtitleFonts
	{
		private List<SubtitleFont> fonts;
		private SubtitleFont defaultFont;
		private SubtitleFont userFont;
		private IDictionary<string, int> fontStats;
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
			this.fonts = new List<SubtitleFont>();
			this.defaultFont = null;
			this.fontStats = new Dictionary<string, int>();
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
				this.fonts.Add(new SubtitleFont(SubtitleFont.FontType.ProgramFont, text));
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
			return this.fonts.Select(font => font.Name).ToArray();
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
				foreach (var dictionaryEntry in this.fontStats)
				{
					if (dictionaryEntry.Value > 10)
					{
						this.SetDefaultFont(dictionaryEntry.Key);
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
						int count;
						if (!this.fontStats.TryGetValue(current.Name, out count))
							count = 0;
						this.fontStats[current.Name] = count + 1;
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
			var defaultFont = this.fonts.FirstOrDefault(font => font.Name == name);
			if (defaultFont == null)
				throw new Exception("Trying to set an unknown font as default");
			this.defaultFont = defaultFont;
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
			SubtitleFont subtitleFont = this.fonts.FirstOrDefault(font => font.Name == targetFontName);
			if (subtitleFont == null)
				throw new Exception("invalid font name for merge fonts");
			this.userFont.MoveLetters(subtitleFont);
		}
		public IEnumerable<SubtitleLetter> FontLetters(string name)
		{
			var subtitleFont = this.fonts.FirstOrDefault(font => font.Name == name);
			if (subtitleFont == null)
				throw new Exception("Trying to set an unknown font as default");
			return subtitleFont.AllLetters;
		}
	}
}
