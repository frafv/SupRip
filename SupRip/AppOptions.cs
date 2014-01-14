using Microsoft.Win32;
using System;
using System.Collections;
using System.IO;
namespace SupRip
{
	internal class AppOptions
	{
		public static int minimumSpaceCharacterWidth;
		public static int charSplitTolerance;
		public static int similarityTolerance;
		public static int contrast;
		public static bool convertDoubleApostrophes;
		public static bool stripFormatting;
		public static bool replaceHighCommas;
		public static bool forcedOnly;
		public static bool combineSubtitles;
		private static Hashtable easilyConfused = null;
		private static Hashtable narrow = null;
		public static int ptsOffset;
		public AppOptions()
		{
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\SupRip\\");
			try
			{
				AppOptions.minimumSpaceCharacterWidth = (int)registryKey.GetValue("minimumSpaceCharacterWidth");
			}
			catch (NullReferenceException)
			{
				AppOptions.minimumSpaceCharacterWidth = 12;
			}
			try
			{
				AppOptions.charSplitTolerance = (int)registryKey.GetValue("charSplitTolerance");
			}
			catch (NullReferenceException)
			{
				AppOptions.charSplitTolerance = 2;
			}
			try
			{
				AppOptions.similarityTolerance = (int)registryKey.GetValue("similarityTolerance");
			}
			catch (NullReferenceException)
			{
				AppOptions.similarityTolerance = 5;
			}
			try
			{
				AppOptions.contrast = (int)registryKey.GetValue("contrast");
			}
			catch (NullReferenceException)
			{
				AppOptions.contrast = 0;
			}
			try
			{
				AppOptions.convertDoubleApostrophes = ((string)registryKey.GetValue("convertDoubleApostrophes")).Equals("True");
			}
			catch (NullReferenceException)
			{
				AppOptions.convertDoubleApostrophes = true;
			}
			try
			{
				AppOptions.stripFormatting = ((string)registryKey.GetValue("stripFormatting")).Equals("True");
			}
			catch (NullReferenceException)
			{
				AppOptions.stripFormatting = true;
			}
			try
			{
				AppOptions.replaceHighCommas = ((string)registryKey.GetValue("replaceHighCommas")).Equals("True");
			}
			catch (NullReferenceException)
			{
				AppOptions.replaceHighCommas = true;
			}
			try
			{
				AppOptions.forcedOnly = ((string)registryKey.GetValue("forcedOnly")).Equals("True");
			}
			catch (NullReferenceException)
			{
				AppOptions.forcedOnly = false;
			}
			try
			{
				AppOptions.combineSubtitles = ((string)registryKey.GetValue("combineSubtitles")).Equals("True");
			}
			catch (NullReferenceException)
			{
				AppOptions.combineSubtitles = true;
			}
			try
			{
				StreamReader streamReader = new StreamReader("easilyconfused.txt");
				AppOptions.easilyConfused = new Hashtable();
				string key;
				while ((key = streamReader.ReadLine()) != null)
				{
					AppOptions.easilyConfused.Add(key, true);
				}
			}
			catch (FileNotFoundException)
			{
				AppOptions.easilyConfused = null;
			}
			try
			{
				StreamReader streamReader = new StreamReader("narrow.txt");
				AppOptions.narrow = new Hashtable();
				string key;
				while ((key = streamReader.ReadLine()) != null)
				{
					AppOptions.narrow.Add(key, true);
				}
			}
			catch (FileNotFoundException)
			{
				AppOptions.narrow = null;
			}
		}
		public void SaveOptions()
		{
			RegistryKey registryKey = Registry.CurrentUser.CreateSubKey("Software\\SupRip");
			registryKey.SetValue("minimumSpaceCharacterWidth", AppOptions.minimumSpaceCharacterWidth);
			registryKey.SetValue("charSplitTolerance", AppOptions.charSplitTolerance);
			registryKey.SetValue("similarityTolerance", AppOptions.similarityTolerance);
			registryKey.SetValue("contrast", AppOptions.contrast);
			registryKey.SetValue("convertDoubleApostrophes", AppOptions.convertDoubleApostrophes);
			registryKey.SetValue("stripFormatting", AppOptions.stripFormatting);
			registryKey.SetValue("replaceHighCommas", AppOptions.replaceHighCommas);
			registryKey.SetValue("forcedOnly", AppOptions.forcedOnly);
			registryKey.SetValue("combineSubtitles", AppOptions.combineSubtitles);
		}
		public void ResetToDefaults()
		{
			AppOptions.minimumSpaceCharacterWidth = 12;
			AppOptions.charSplitTolerance = 2;
			AppOptions.similarityTolerance = 5;
			AppOptions.contrast = 0;
			AppOptions.convertDoubleApostrophes = true;
			AppOptions.stripFormatting = true;
			AppOptions.replaceHighCommas = true;
			AppOptions.forcedOnly = false;
			AppOptions.combineSubtitles = true;
		}
		public static bool IsEasilyConfusedLetter(string c)
		{
			return c != null && AppOptions.easilyConfused != null && AppOptions.easilyConfused[c] != null;
		}
		public static bool IsNarrowCharacter(string c)
		{
			return c != null && AppOptions.narrow != null && AppOptions.narrow[c] != null;
		}
	}
}
