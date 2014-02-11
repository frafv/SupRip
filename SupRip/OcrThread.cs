using System;
using System.Threading;
namespace SupRip
{
	internal class OcrThread
	{
		private ManualResetEvent stopEvent;
		private ManualResetEvent finishedEvent;
		private MainForm parentForm;
		private int nSubtitles;
		private int startingSubtitle;
		private bool reportUnknownChar;
		public int FoundNum
		{
			get;
			private set;
		}
		public OcrThread(MainForm f, ManualResetEvent se, ManualResetEvent fe, int n)
		{
			this.parentForm = f;
			this.stopEvent = se;
			this.finishedEvent = fe;
			this.nSubtitles = n;
			this.reportUnknownChar = false;
		}
		public OcrThread(MainForm f, ManualResetEvent se, ManualResetEvent fe, int start, int n)
		{
			this.parentForm = f;
			this.stopEvent = se;
			this.finishedEvent = fe;
			this.nSubtitles = n;
			this.startingSubtitle = start;
			this.reportUnknownChar = true;
			this.FoundNum = -1;
		}
		public void Run()
		{
			if (this.reportUnknownChar)
			{
				for (int i = this.startingSubtitle; i < this.nSubtitles; i++)
				{
					if (!AppOptions.forcedOnly || this.parentForm.IsSubtitleForced(i))
					{
						if (this.stopEvent.WaitOne(0, true))
						{
							break;
						}
						try
						{
							this.parentForm.ImageOCR(i, true);
							this.parentForm.Invoke(this.parentForm.updateProgressDelegate, new object[]
							{
								i
							});
						}
						catch (UnknownCharacterException)
						{
							this.FoundNum = i;
							break;
						}
					}
				}
			}
			else
			{
				for (int j = 0; j < this.nSubtitles; j++)
				{
					if (!AppOptions.forcedOnly || this.parentForm.IsSubtitleForced(j))
					{
						if (this.stopEvent.WaitOne(0, true))
						{
							break;
						}
						this.parentForm.ImageOCR(j);
						this.parentForm.Invoke(this.parentForm.updateProgressDelegate, new object[]
						{
							j
						});
					}
				}
			}
			this.finishedEvent.Set();
		}
	}
}
