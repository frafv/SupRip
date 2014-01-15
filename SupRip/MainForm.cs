using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SupRip
{
	public partial class MainForm : Form
	{
		private Size bitmapSize;
		private Bitmap bitmap;
		private SubtitleImage currentSubtitle;
		private int currentNum;
		private int oldNum;
		private SubtitleLetter activeLetter;
		private SubtitleFonts fonts;
		private Rectangle subtitleImageRectangle;
		private SubtitleFile subfile;
		private bool initialized;
		private AppOptions options;
		private double bitmapScale;
		private Pen redPen;
		private Pen bluePen;
		private Pen yellowPen;
		private Pen greenPen;
		private Pen whitePen;
		private ProgressForm pf;
		private ManualResetEvent stopEvent;
		private ManualResetEvent finishedEvent;
		public DelegateUpdateProgress updateProgressDelegate;
		public string version = "1.16";
		private bool ignoreItalicChanges;
		public MainForm()
		{
			this.InitializeComponent();
			this.debugButton.Hide();
			this.bitmapSize = new Size(400, 400);
			this.bitmap = new Bitmap(this.bitmapSize.Width, this.bitmapSize.Height, PixelFormat.Format24bppRgb);
			this.fonts = new SubtitleFonts();
			this.redPen = new Pen(new SolidBrush(Color.Red));
			this.yellowPen = new Pen(new SolidBrush(Color.Yellow));
			this.bluePen = new Pen(new SolidBrush(Color.Blue));
			this.greenPen = new Pen(new SolidBrush(Color.Green));
			this.whitePen = new Pen(new SolidBrush(Color.White));
			this.subtitlePictureBox.SizeMode = PictureBoxSizeMode.Zoom;
			this.letterPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
			ToolTip toolTip = new ToolTip();
			toolTip.AutoPopDelay = 20000;
			toolTip.SetToolTip(this.nextButton, "Moves to the next subtitle image\n\nCtrl+N");
			toolTip.SetToolTip(this.previousButton, "Moves to the previous subtitle image\n\nCtrl+P");
			toolTip.SetToolTip(this.ocrButton, "Tries to scan the current image, and will prompt the user to identify any unknown character.\n\nCtrl+O");
			toolTip.SetToolTip(this.minimumSpaceCharacterWidthTextBox, "Configures how big an empty space between two letters has to be to be counted as a space character.\nIf spaces are inserted where there shouldn't be any, increase this number.\nIf too many spaces are not detected, lower this number.");
			toolTip.SetToolTip(this.charSplitTolerance, "Configures how eagerly the OCR function splits characters.\nIf too many characters (especially 'k') get split in the middle, increase this number.\nIf too many double characters get erroneously detected as a single one, lower this number.");
			toolTip.SetToolTip(this.similarityTolerance, "Configures how similar two letters have to be so they are seen as equal.\nIf you have to manually enter too many letters, increase this number.\nIf there are some accidentially misidentified letters, lower this number.");
			toolTip.SetToolTip(this.contrast, "Sets a contrast correction on the image. Helpful for some subtitles that have large gray zones, but slows down OCR if it is set to any other value than zero.");
			toolTip.SetToolTip(this.autoProgress, "Automatically continues with the next subtitle if all characters in this one can be scanned. OCR will stop as soon as an unknown character is encountered.");
			toolTip.SetToolTip(this.autoOCRButton, "Automatically scans all subtitles. Unknown characters will simply be skipped.");
			toolTip.SetToolTip(this.loadButton, "Load a new subtitle file.");
			toolTip.SetToolTip(this.saveButton, "Save the scanned SRT file as you can see it on the left to a file.");
			toolTip.SetToolTip(this.convertDoubleApostrophes, "Automatically replaces double-apostrophes with a single quote sign.");
			toolTip.SetToolTip(this.replaceHighCommas, "Automatically replaces comma signs that are pretty high up in their line with apostrophes.");
			toolTip.SetToolTip(this.forcedOnly, "Only output forced subtitles.");
			toolTip.SetToolTip(this.combineSubtitles, "Combines two subsequent subtitles with completely identical text so they only use one line in the SRT.");
			toolTip.SetToolTip(this.ptsOffset, "The delay that should be applied to timestamps. For most subtitles it will be zero.");
			this.nextButton.Enabled = false;
			this.previousButton.Enabled = false;
			this.autoOCRButton.Enabled = false;
			this.ocrButton.Enabled = false;
			this.letterOKButton.Enabled = false;
			this.saveButton.Enabled = false;
			this.initialized = false;
			this.options = new AppOptions();
			this.minimumSpaceCharacterWidthTextBox.Text = AppOptions.minimumSpaceCharacterWidth.ToString();
			this.charSplitTolerance.Text = AppOptions.charSplitTolerance.ToString();
			this.similarityTolerance.Text = AppOptions.similarityTolerance.ToString();
			this.contrast.Text = AppOptions.contrast.ToString();
			this.convertDoubleApostrophes.Checked = AppOptions.convertDoubleApostrophes;
			this.stripFormatting.Checked = AppOptions.stripFormatting;
			this.replaceHighCommas.Checked = AppOptions.replaceHighCommas;
			this.forcedOnly.Checked = AppOptions.forcedOnly;
			this.combineSubtitles.Checked = AppOptions.combineSubtitles;
			this.initialized = true;
			new System.Windows.Forms.Timer
			{
				Enabled = true,
				Interval = 100
			}.Tick += new EventHandler(this.TimerEvent);
			this.Text = "SupRip " + this.version;
		}
		private void SaveSettings()
		{
			try
			{
				Debugger.CleanUp();
				this.fonts.Save();
				this.options.SaveOptions();
			}
			catch (Exception)
			{
			}
		}
		private void LoadSubtitleFile(string fileName)
		{
			try
			{
				if (this.subfile != null)
				{
					this.subfile.Close();
				}
				this.subfile = new SubtitleFile(fileName);
			}
			catch (SUPFileFormatException)
			{
				MessageBox.Show("Couldn't open the file\n" + fileName + ".\nMaybe it's not an HD .sup file?\nStandard resolution DVD subtitles aren't supported.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			catch (IOException ex)
			{
				MessageBox.Show("Couldn't open the file\n" + fileName + "\nbecause of \n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			catch (Exception e)
			{
				ErrorForm errorForm = new ErrorForm(e);
				errorForm.ShowDialog();
			}
			if (this.subfile != null)
			{
				this.nextButton.Enabled = true;
				this.previousButton.Enabled = true;
				this.autoOCRButton.Enabled = true;
				this.ocrButton.Enabled = true;
				this.letterOKButton.Enabled = true;
				this.saveButton.Enabled = true;
				if (this.subfile.Type == SubtitleFile.SubtitleType.Hddvd)
				{
					this.subtitleType.Text = "HD DVD";
				}
				else
				{
					if (this.subfile.Type == SubtitleFile.SubtitleType.Bluray)
					{
						this.subtitleType.Text = "Bluray";
					}
				}
				this.currentNum = 0;
				this.currentSubtitle = this.LoadSubtitleImage(this.currentNum);
				this.UpdateTextBox();
				this.totalPages.Text = "/ " + this.subfile.NumSubtitles;
				this.UpdateBitmaps();
				this.Text = "SupRip " + this.version + " - " + fileName.Substring(fileName.LastIndexOf('\\') + 1);
			}
		}
		private void MainForm_Load(object sender, EventArgs e)
		{
		}
		private SubtitleImage LoadSubtitleImage(int number)
		{
			SubtitleImage result;
			try
			{
				this.pageNum.Text = (this.currentNum + 1).ToString();
				SubtitleImage subtitleImage = this.subfile.GetSubtitleImage(number);
				this.activeLetter = null;
				this.ImageOCR(subtitleImage);
				result = subtitleImage;
			}
			catch (Exception ex)
			{
				this.fonts.Save();
				throw new Exception(ex.Message + "\n\n" + ex.StackTrace);
			}
			return result;
		}
		private void MoveToImage(int num)
		{
			this.fontName.Text = this.fonts.DefaultFontName;
			if (this.currentSubtitle != null && num != this.currentNum)
			{
				this.subfile.UpdateSubtitleText(this.currentNum, this.currentSubtitle);
				this.currentNum = num;
				if (this.currentNum < 0)
				{
					this.currentNum = 0;
				}
				if (this.currentNum >= this.subfile.NumSubtitles)
				{
					this.currentNum = this.subfile.NumSubtitles - 1;
				}
				this.currentSubtitle = this.LoadSubtitleImage(this.currentNum);
				this.UpdateTextBox();
				this.UpdateBitmaps();
			}
		}
		private void pageNum_TextChanged(object sender, EventArgs e)
		{
			try
			{
				this.MoveToImage(int.Parse(this.pageNum.Text) - 1);
			}
			catch (FormatException)
			{
				this.pageNum.Text = this.currentNum.ToString();
			}
		}
		private void ActivateNextUnknownLetter()
		{
			this.activeLetter = null;
			foreach (SubtitleLetter current in this.currentSubtitle.letters)
			{
				if (current.Text == null)
				{
					SubtitleLetter subtitleLetter = this.fonts.FindMatch(current, AppOptions.similarityTolerance * 100);
					if (subtitleLetter == null)
					{
						this.activeLetter = current;
						break;
					}
					current.Text = subtitleLetter.Text;
				}
			}
			if (this.activeLetter != null)
			{
				this.letterInputBox.Focus();
				base.AcceptButton = this.letterOKButton;
				this.UpdateBitmaps();
				return;
			}
			this.UpdateBitmaps();
		}
		private void AssignLetterText(SubtitleLetter l, string text)
		{
			this.activeLetter.Text = text;
			SubtitleLetter subtitleLetter = this.fonts.FindMatch(l, AppOptions.similarityTolerance * 100);
			if (subtitleLetter != null)
			{
				this.fonts.DeleteLetter(subtitleLetter);
			}
			if (this.letterInputBox.Text != "")
			{
				this.activeLetter.Text = text;
				this.fonts.AddLetter(this.activeLetter);
			}
		}
		private void ImageOCR(SubtitleImage si)
		{
			this.ImageOCR(si, false);
			si.FixSpaces();
		}
		private void ImageOCR(SubtitleImage si, bool reportUnknownCharacter)
		{
			if (si.letters == null)
			{
				return;
			}
			this.fonts.debugStrings.Clear();
			foreach (SubtitleLetter current in si.letters)
			{
				if (current.Text == null)
				{
					DateTime now = DateTime.Now;
					SubtitleLetter subtitleLetter = this.fonts.FindMatch(current, AppOptions.similarityTolerance * 100);
					Debugger.scanTime += (DateTime.Now - now).TotalMilliseconds;
					if (subtitleLetter != null)
					{
						if (AppOptions.replaceHighCommas && current.Height < -10 && subtitleLetter.Text.Equals(","))
						{
							current.Text = "'";
						}
						else
						{
							if (AppOptions.replaceHighCommas && current.Height > 10 && subtitleLetter.Text.Equals("'"))
							{
								current.Text = ",";
							}
							else
							{
								current.Text = subtitleLetter.Text;
							}
						}
					}
					else
					{
						if (reportUnknownCharacter)
						{
							throw new UnknownCharacterException();
						}
					}
				}
			}
		}
		public void ImageOCR(int n)
		{
			this.ImageOCR(n, false);
		}
		public void ImageOCR(int n, bool reportUnknownCharacter)
		{
			SubtitleImage subtitleImage = this.subfile.GetSubtitleImage(n);
			this.ImageOCR(subtitleImage, reportUnknownCharacter);
			this.subfile.UpdateSubtitleText(n, subtitleImage);
		}
		private void EnterHTMLText(RichTextBox r, string t)
		{
			int num = 0;
			Font selectionFont = new Font(r.Font, FontStyle.Regular);
			Font selectionFont2 = new Font(r.Font, FontStyle.Italic);
			string text;
			while (true)
			{
				int num2 = t.IndexOf("<i>", num);
				if (num2 == -1)
				{
					break;
				}
				text = t.Substring(num, num2 - num);
				r.SelectionFont = selectionFont;
				r.AppendText(text);
				num = num2 + 3;
				num2 = t.IndexOf("</i>", num);
				text = t.Substring(num, num2 - num);
				r.SelectionFont = selectionFont2;
				r.AppendText(text);
				num = num2 + 4;
			}
			text = t.Substring(num, t.Length - num);
			r.SelectionFont = selectionFont;
			r.AppendText(text);
		}
		private void UpdateTextBox()
		{
			if (this.currentSubtitle != null)
			{
				string text = this.currentSubtitle.GetText();
				this.subtitleTextBox2.Clear();
				this.EnterHTMLText(this.subtitleTextBox2, text);
				return;
			}
			this.subtitleTextBox2.Text = "";
		}
		private void previousButton_Click(object sender, EventArgs e)
		{
			this.MoveToImage(this.currentNum - 1);
		}
		private void nextButton_Click(object sender, EventArgs e)
		{
			this.MoveToImage(this.currentNum + 1);
		}
		private void TimerEvent(object sender, EventArgs e)
		{
			if (this.finishedEvent != null && this.finishedEvent.WaitOne(0, true))
			{
				this.pf.Dispose();
				this.finishedEvent.Reset();
			}
		}
		private void StartImageOCR()
		{
			this.ImageOCR(this.currentSubtitle);
			this.UpdateTextBox();
			this.subfile.UpdateSubtitleText(this.currentNum, this.currentSubtitle);
			this.ActivateNextUnknownLetter();
			if (this.activeLetter == null && this.autoProgress.Checked)
			{
				this.oldNum = this.currentNum;
				this.stopEvent = new ManualResetEvent(false);
				this.finishedEvent = new ManualResetEvent(false);
				this.stopEvent.Reset();
				this.finishedEvent.Reset();
				this.updateProgressDelegate = new DelegateUpdateProgress(this.UpdateProgress);
				OcrThread ocrThread = new OcrThread(this, this.stopEvent, this.finishedEvent, this.currentNum, this.subfile.NumSubtitles);
				Thread thread = new Thread(new ThreadStart(ocrThread.Run));
				thread.Start();
				using (this.pf = new ProgressForm(this, this.subfile.NumSubtitles))
				{
					this.pf.ShowDialog();
				}
				this.finishedEvent.WaitOne(0, true);
				if (ocrThread.FoundNum != -1)
				{
					this.currentNum = ocrThread.FoundNum;
				}
				else
				{
					this.currentNum = this.subfile.NumSubtitles - 1;
				}
				this.currentSubtitle = this.LoadSubtitleImage(this.currentNum);
				this.ImageOCR(this.currentSubtitle);
				this.UpdateTextBox();
				this.ActivateNextUnknownLetter();
			}
		}
		private void ocrButton_Click(object sender, EventArgs e)
		{
			this.StartImageOCR();
		}
		private void letterOKButton_Click(object sender, EventArgs e)
		{
			if (this.letterInputBox.Text == "")
			{
				this.AssignLetterText(this.activeLetter, null);
			}
			else
			{
				this.AssignLetterText(this.activeLetter, this.letterInputBox.Text);
			}
			this.letterInputBox.Text = "";
			this.ImageOCR(this.currentSubtitle);
			this.UpdateTextBox();
			this.subfile.UpdateSubtitleText(this.currentNum, this.currentSubtitle);
			this.ActivateNextUnknownLetter();
			if (this.activeLetter == null)
			{
				this.ocrButton.Focus();
			}
		}
		protected override bool ProcessCmdKey(ref Message msg, Keys keydata)
		{
			if (keydata != Keys.Escape)
			{
				switch (keydata)
				{
					case (Keys)131150:
						this.MoveToImage(this.currentNum + 1);
						break;
					case (Keys)131151:
						this.StartImageOCR();
						break;
					case (Keys)131152:
						this.MoveToImage(this.currentNum - 1);
						break;
					default:
						return base.ProcessCmdKey(ref msg, keydata);
				}
			}
			else
			{
				Application.Exit();
			}
			return true;
		}
		private void UpdateBitmaps()
		{
			if (this.currentSubtitle == null)
			{
				return;
			}
			if (this.currentSubtitle.subtitleBitmap == null)
			{
				try
				{
					this.subtitlePictureBox.Image = new Bitmap("empty.png");
				}
				catch (ArgumentException)
				{
					this.subtitlePictureBox.Image = null;
				}
				return;
			}
			Bitmap bitmap = (Bitmap)this.currentSubtitle.subtitleBitmap.Clone();
			Graphics graphics = Graphics.FromImage(bitmap);
			if (this.currentSubtitle.letters != null)
			{
				foreach (SubtitleLetter current in this.currentSubtitle.letters)
				{
					Pen pen;
					if (this.activeLetter != null && current == this.activeLetter)
					{
						pen = this.yellowPen;
					}
					else
					{
						if (current.Text != null)
						{
							pen = this.greenPen;
						}
						else
						{
							pen = this.redPen;
						}
					}
					Rectangle coords = current.Coords;
					if (current.Angle != 0.0)
					{
						int num = (int)(current.Angle * (double)coords.Height / 2.0);
						int num2 = (int)((double)current.Height * current.Angle);
						graphics.DrawPolygon(pen, new Point[]
						{
							new Point(coords.Left + num - num2, coords.Top),
							new Point(coords.Right + num - num2, coords.Top),
							new Point(coords.Right - num - num2, coords.Bottom),
							new Point(coords.Left - num - num2, coords.Bottom)
						});
					}
					else
					{
						graphics.DrawRectangle(pen, coords);
					}
				}
			}
			if (this.currentSubtitle.debugLocations != null)
			{
				foreach (KeyValuePair<int, Space> current2 in this.currentSubtitle.debugLocations)
				{
					if (current2.Value.Rect.Width == 0)
					{
						graphics.DrawLine(this.bluePen, current2.Value.Rect.X, current2.Value.Rect.Y, current2.Value.Rect.Right, current2.Value.Rect.Bottom);
					}
					else
					{
						if (current2.Value.Partial)
						{
							graphics.DrawRectangle(this.yellowPen, current2.Value.Rect);
						}
						else
						{
							graphics.DrawRectangle(this.bluePen, current2.Value.Rect);
						}
					}
				}
			}
			if (this.currentSubtitle.debugPoints != null)
			{
				foreach (Point current3 in this.currentSubtitle.debugPoints)
				{
					Point pt = new Point(current3.X + 1, current3.Y);
					graphics.DrawLine(this.redPen, current3, pt);
				}
			}
			double num3 = (double)this.subtitlePictureBox.Width / (double)bitmap.Width;
			double num4 = (double)this.subtitlePictureBox.Height / (double)bitmap.Height;
			this.bitmapScale = Math.Min(num3, num4);
			this.subtitleImageRectangle = new Rectangle(this.subtitlePictureBox.Left, this.subtitlePictureBox.Top, this.subtitlePictureBox.Width, this.subtitlePictureBox.Height);
			if (num3 > num4)
			{
				double num5 = (double)bitmap.Width * num4;
				this.subtitleImageRectangle = new Rectangle((this.subtitlePictureBox.Width - (int)num5) / 2, 0, this.subtitlePictureBox.Width - (this.subtitlePictureBox.Width - (int)num5), this.subtitlePictureBox.Height);
			}
			else
			{
				double num6 = (double)bitmap.Height * num3;
				this.subtitleImageRectangle = new Rectangle(0, (this.subtitlePictureBox.Height - (int)num6) / 2, this.subtitlePictureBox.Width, this.subtitlePictureBox.Height - (this.subtitlePictureBox.Height - (int)num6));
			}
			this.subtitlePictureBox.Image = bitmap;
			if (this.activeLetter != null)
			{
				this.letterPictureBox.Image = this.activeLetter.GetBitmap();
				this.letterOKButton.Enabled = (this.letterPictureBox.Image != null);
				return;
			}
			this.letterPictureBox.Image = null;
			this.letterOKButton.Enabled = false;
		}
		private void imagePage_Paint(object sender, PaintEventArgs e)
		{
			this.UpdateBitmaps();
		}
		private void ApplyOptions()
		{
			if (!this.initialized)
			{
				return;
			}
			try
			{
				if (int.Parse(this.minimumSpaceCharacterWidthTextBox.Text) < 1 || int.Parse(this.minimumSpaceCharacterWidthTextBox.Text) > 20)
				{
					throw new FormatException();
				}
				AppOptions.minimumSpaceCharacterWidth = int.Parse(this.minimumSpaceCharacterWidthTextBox.Text);
			}
			catch (FormatException)
			{
				this.minimumSpaceCharacterWidthTextBox.Text = AppOptions.minimumSpaceCharacterWidth.ToString();
			}
			try
			{
				if (int.Parse(this.charSplitTolerance.Text) < 1 || int.Parse(this.charSplitTolerance.Text) > 20)
				{
					throw new FormatException();
				}
				AppOptions.charSplitTolerance = int.Parse(this.charSplitTolerance.Text);
			}
			catch (FormatException)
			{
				this.charSplitTolerance.Text = AppOptions.charSplitTolerance.ToString();
			}
			try
			{
				if (int.Parse(this.similarityTolerance.Text) < 1 || int.Parse(this.similarityTolerance.Text) > 200)
				{
					throw new FormatException();
				}
				AppOptions.similarityTolerance = int.Parse(this.similarityTolerance.Text);
			}
			catch (FormatException)
			{
				this.similarityTolerance.Text = AppOptions.similarityTolerance.ToString();
			}
			try
			{
				if (int.Parse(this.contrast.Text) < 0 || int.Parse(this.contrast.Text) > 10)
				{
					throw new FormatException();
				}
				AppOptions.contrast = int.Parse(this.contrast.Text);
			}
			catch (FormatException)
			{
				this.contrast.Text = AppOptions.contrast.ToString();
			}
			if (this.subfile != null)
			{
				this.currentSubtitle = this.LoadSubtitleImage(this.currentNum);
				this.UpdateBitmaps();
				this.UpdateTextBox();
			}
		}
		private void optionsApplyButton_Click(object sender, EventArgs e)
		{
			this.ApplyOptions();
		}
		private void mainTabControl_Click(object sender, EventArgs e)
		{
			this.UpdateSRTPage();
		}
		private void subtitlePictureBox_MouseClick(object sender, MouseEventArgs e)
		{
			if (this.subtitleImageRectangle.Contains(e.Location))
			{
				Point location = e.Location;
				location.Offset(-this.subtitleImageRectangle.Left, -this.subtitleImageRectangle.Top);
				location = new Point((int)((double)location.X / this.bitmapScale), (int)((double)location.Y / this.bitmapScale));
				SubtitleLetter subtitleLetter = null;
				foreach (SubtitleLetter current in this.currentSubtitle.letters)
				{
					if (current.Coords.Contains(location))
					{
						subtitleLetter = current;
						break;
					}
				}
				if (subtitleLetter != null)
				{
					this.activeLetter = subtitleLetter;
					this.letterInputBox.Text = subtitleLetter.Text;
					this.letterInputBox.SelectAll();
					this.letterInputBox.Focus();
					base.AcceptButton = this.letterOKButton;
					this.ignoreItalicChanges = true;
					if (this.activeLetter.Angle != 0.0)
					{
						this.italicLetter.Checked = true;
					}
					else
					{
						this.italicLetter.Checked = false;
					}
					this.ignoreItalicChanges = false;
					this.UpdateBitmaps();
					return;
				}
				this.activeLetter = null;
				this.letterInputBox.Text = "";
				this.letterOKButton.Enabled = false;
				this.UpdateBitmaps();
			}
		}
		private void UpdateProgress(int x)
		{
			if (this.pf != null)
			{
				this.pf.SetProgressBarPosition(x);
			}
		}
		public void CancelThread()
		{
			this.stopEvent.Set();
		}
		private void autoOCRButton_Click(object sender, EventArgs e)
		{
			this.oldNum = this.currentNum;
			this.stopEvent = new ManualResetEvent(false);
			this.finishedEvent = new ManualResetEvent(false);
			this.stopEvent.Reset();
			this.finishedEvent.Reset();
			this.updateProgressDelegate = new DelegateUpdateProgress(this.UpdateProgress);
			OcrThread @object = new OcrThread(this, this.stopEvent, this.finishedEvent, this.subfile.NumSubtitles);
			Thread thread = new Thread(new ThreadStart(@object.Run));
			thread.Start();
			using (this.pf = new ProgressForm(this, this.subfile.NumSubtitles))
			{
				this.pf.ShowDialog();
			}
			if (!this.finishedEvent.WaitOne(0, true))
			{
				this.CancelThread();
			}
			this.currentNum = this.oldNum;
			this.currentSubtitle = this.LoadSubtitleImage(this.currentNum);
			this.UpdateSRTPage();
			this.mainTabControl.SelectedIndex = 1;
		}
		private void UpdateSRTPage()
		{
			if (this.subfile != null)
			{
				this.subfile.RecalculateTimes();
				this.srtTextBox.Text = this.subfile.GetSRTText();
				SRTInfo sRTInfo = this.subfile.GetSRTInfo();
				this.unscannedLabel.Text = sRTInfo.unscanned.ToString();
				this.containingErrorsLabel.Text = sRTInfo.containingErrors.ToString();
				this.finishedLabel.Text = sRTInfo.finished.ToString();
			}
		}
		private void saveButton_Click(object sender, EventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.AddExtension = true;
			saveFileDialog.DefaultExt = "srt";
			saveFileDialog.Filter = "SRT subtitles (*.srt)|*.srt|All files (*.*)|*.*";
			saveFileDialog.FilterIndex = 0;
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				StreamWriter streamWriter = new StreamWriter(saveFileDialog.OpenFile(), Encoding.Unicode);
				streamWriter.Write(this.subfile.GetSRTText());
				streamWriter.Close();
			}
		}
		private void loadButton_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.CheckFileExists = true;
			openFileDialog.Filter = "Subpicture files (*.sup)|*.sup|Scenarist Subtitles (*.scn-sst)|*.scn-sst|All files (*.*)|*.*";
			openFileDialog.FilterIndex = 0;
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				try
				{
					this.LoadSubtitleFile(openFileDialog.FileName);
					this.mainTabControl.SelectedIndex = 0;
				}
				catch (SSTFileFormatException ex)
				{
					MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
			}
		}
		private void convertDoubleApostrophes_CheckedChanged(object sender, EventArgs e)
		{
			AppOptions.convertDoubleApostrophes = this.convertDoubleApostrophes.Checked;
			this.UpdateSRTPage();
		}
		private void stripFormatting_CheckedChanged(object sender, EventArgs e)
		{
			AppOptions.stripFormatting = this.stripFormatting.Checked;
			this.UpdateSRTPage();
		}
		private void replaceHighCommas_CheckedChanged(object sender, EventArgs e)
		{
			AppOptions.replaceHighCommas = this.replaceHighCommas.Checked;
			if (this.subfile != null)
			{
				this.currentSubtitle = this.LoadSubtitleImage(this.currentNum);
				this.UpdateBitmaps();
			}
		}
		private void forcedOnly_CheckedChanged(object sender, EventArgs e)
		{
			AppOptions.forcedOnly = this.forcedOnly.Checked;
			this.UpdateSRTPage();
		}
		private void startCharacterMap_Click(object sender, EventArgs e)
		{
			new Process
			{
				StartInfo =
				{
					FileName = Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\charmap.exe"
				}
			}.Start();
		}
		private void resetDefaults_Click(object sender, EventArgs e)
		{
			this.options.ResetToDefaults();
			this.minimumSpaceCharacterWidthTextBox.Text = AppOptions.minimumSpaceCharacterWidth.ToString();
			this.charSplitTolerance.Text = AppOptions.charSplitTolerance.ToString();
			this.similarityTolerance.Text = AppOptions.similarityTolerance.ToString();
			this.contrast.Text = AppOptions.contrast.ToString();
			this.convertDoubleApostrophes.Checked = AppOptions.convertDoubleApostrophes;
			this.stripFormatting.Checked = AppOptions.stripFormatting;
			this.replaceHighCommas.Checked = AppOptions.replaceHighCommas;
			this.forcedOnly.Checked = AppOptions.forcedOnly;
			this.combineSubtitles.Checked = AppOptions.combineSubtitles;
		}
		private void ptsOffset_TextChanged(object sender, EventArgs e)
		{
			try
			{
				AppOptions.ptsOffset = int.Parse(this.ptsOffset.Text);
			}
			catch (FormatException)
			{
				if (AppOptions.ptsOffset.ToString().Length == 1)
				{
					this.ptsOffset.Text = "";
				}
				else
				{
					this.ptsOffset.Text = string.Format("{0}", AppOptions.ptsOffset);
				}
			}
			this.UpdateSRTPage();
		}
		private void combineSubtitles_CheckedChanged(object sender, EventArgs e)
		{
			AppOptions.combineSubtitles = this.combineSubtitles.Checked;
			this.UpdateSRTPage();
		}
		private void debugButton_Click(object sender, EventArgs e)
		{
			string[] array = this.fonts.FontList();
			EventHandler value = new EventHandler(this.debugMenu_Click);
			MenuItem[] array2 = new MenuItem[array.Length + 1];
			int num = 0;
			string[] array3 = array;
			for (int i = 0; i < array3.Length; i++)
			{
				string text = array3[i];
				array2[num] = new MenuItem(text);
				array2[num].Click += value;
				num++;
			}
			array2[num] = new MenuItem("Duplicates");
			array2[num].Click += value;
			ContextMenu contextMenu = new ContextMenu(array2);
			contextMenu.Show(this.debugButton, new Point(0, 0));
		}
		private void debugMenu_Click(object sender, EventArgs e)
		{
			MenuItem menuItem = (MenuItem)sender;
			if (menuItem.Index < this.fonts.Count)
			{
				this.fonts.MergeUserFont(((MenuItem)sender).Text);
			}
		}
		public bool IsSubtitleForced(int n)
		{
			return this.subfile.IsSubtitleForced(n);
		}
		private void options_TextChanged(object sender, EventArgs e)
		{
			this.ApplyOptions();
		}
		private void italicLetter_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.ignoreItalicChanges)
			{
				this.activeLetter.Angle = 0.0;
			}
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.SaveSettings();
		}
	}
}
