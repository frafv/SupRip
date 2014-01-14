using System;
using System.Windows.Forms;
namespace SupRip
{
	internal static class Program
	{
		[STAThread]
		private static void Main(string[] args)
		{
			Application.SetCompatibleTextRenderingDefault(false);
			if (args.Length >= 1)
			{
				SubtitleFile subtitleFile = new SubtitleFile(args[0]);
				int numSubtitles = 20;
				if (args.Length >= 2)
				{
					numSubtitles = int.Parse(args[1]);
				}
				int spacing = 100;
				if (args.Length >= 3)
				{
					spacing = int.Parse(args[2]);
				}
				try
				{
					subtitleFile.WriteBitmaps(args[0], numSubtitles, spacing);
					return;
				}
				catch (Exception ex)
				{
					Console.WriteLine("exception " + ex.Message);
					return;
				}
			}
			try
			{
				Application.Run(new MainForm());
			}
			catch (Exception e)
			{
				ErrorForm errorForm = new ErrorForm(e);
				errorForm.ShowDialog();
			}
		}
	}
}
