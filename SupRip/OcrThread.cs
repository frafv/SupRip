using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace SupRip
{
	internal class OcrThread
	{
		private ManualResetEvent stopEvent;
		private ManualResetEvent finishedEvent;
		private MainForm parentForm;
		private SubtitleFile subfile;
		private int nSubtitles;
		private int startingSubtitle;
		private bool reportUnknownChar;
		public int FoundNum
		{
			get;
			private set;
		}

		public OcrThread(MainForm f, SubtitleFile file, ManualResetEvent se, ManualResetEvent fe, int n)
		{
			this.parentForm = f;
			this.subfile = file;
			this.stopEvent = se;
			this.finishedEvent = fe;
			this.nSubtitles = n;
			this.reportUnknownChar = false;
		}
		public OcrThread(MainForm f, SubtitleFile file, ManualResetEvent se, ManualResetEvent fe, int start, int n)
			: this(f, file, se, fe, n)
		{
			this.startingSubtitle = start;
			this.reportUnknownChar = true;
			this.FoundNum = -1;
		}

		public void Run()
		{
			this.parentForm.Invoke(this.parentForm.updateProgressDelegate, 0);
			int start = this.reportUnknownChar ? this.startingSubtitle : 0;
			var sw = new Stopwatch();
			sw.Start();
			Enumerable.Range(start, this.nSubtitles - start)
				.Where(i => !AppOptions.forcedOnly || this.subfile.IsSubtitleForced(i))
				.TakeWhile(i => !this.stopEvent.WaitOne(0, true))
				//Sequential file reading
				.Select(i => new { data = this.subfile.ReadBitmap(i), n = i }).AsParallel().AsOrdered()
				//Parallel bitmaps building
				.Select(bitmap =>
				{
					var subtitleImage = new SubtitleImage(this.subfile.GetBitmap(bitmap.n, bitmap.data), false);
					return new { subtitleImage, bitmap.n };
				})
				//Sequential image processing (parallel processing inside)
				.ForEach(image =>
				{
					try
					{
						this.parentForm.ImageOCR(image.subtitleImage, this.reportUnknownChar);
					}
					catch (AggregateException ex)
					{
						if (!ex.InnerExceptions.OfType<UnknownCharacterException>().Any())
							throw;
						if (!this.reportUnknownChar)
							throw ex.InnerExceptions.OfType<UnknownCharacterException>().FirstOrDefault();
						this.FoundNum = image.n;
						return false;
					}
					catch (UnknownCharacterException)
					{
						if (!this.reportUnknownChar)
							throw;
						this.FoundNum = image.n;
						return false;
					}
					this.subfile.UpdateSubtitleText(image.n, image.subtitleImage);
					if (sw.ElapsedMilliseconds > 200L)
					{
						sw.Reset();
						this.parentForm.Invoke(this.parentForm.updateProgressDelegate, image.n);
						sw.Start();
					}
					return true;
				});

			this.finishedEvent.Set();
		}
	}
}
