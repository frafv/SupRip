﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Windows7.DesktopIntegration;
using Windows7.DesktopIntegration.WindowsForms;

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
		private string defaultTitle;
		private string version;

		private void ReadOptions()
		{
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

		public MainForm()
		{
			this.InitializeComponent();
#if DEBUG
			this.debugButton.Show();
			this.dbgSpace.Show();
			this.dbgEdges.Show();
			this.debugLabel.Show();
#endif
			this.bitmapSize = new Size(400, 400);
			this.bitmap = new Bitmap(this.bitmapSize.Width, this.bitmapSize.Height, PixelFormat.Format24bppRgb);
			this.fonts = new SubtitleFonts();
			this.redPen = new Pen(new SolidBrush(Color.Red));
			this.yellowPen = new Pen(new SolidBrush(Color.Yellow));
			this.bluePen = new Pen(new SolidBrush(Color.Blue));
			this.greenPen = new Pen(new SolidBrush(Color.Green));
			this.whitePen = new Pen(new SolidBrush(Color.White));
			this.initialized = false;
			this.options = new AppOptions();
			ReadOptions();
			this.initialized = true;
			new System.Windows.Forms.Timer
			{
				Enabled = true,
				Interval = 100
			}.Tick += new EventHandler(this.TimerEvent);
			defaultTitle = this.Text;
			version = Application.ProductVersion;
			if (version.EndsWith(".0")) version = version.Substring(0, version.Length - 2);
			if (version.EndsWith(".0")) version = version.Substring(0, version.Length - 2);
			this.Text = defaultTitle + " " + version;
			this.cbFonts.Items.AddRange(this.fonts.FontList());
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
				this.Text = defaultTitle + " " + version + " - " + fileName.Substring(fileName.LastIndexOf('\\') + 1);
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
			if (!Double.IsNaN(SubtitleImage.italicAngle))
			{
				this.fontName.Text += String.Format(" (Italic = {0:0.0}°)", 90.0 - Math.Atan(SubtitleImage.italicAngle) * 180.0 / Math.PI);
			}
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
				this.italicLetter.Checked = this.activeLetter.Angle != 0.0;
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
			this.SetTaskbarProgressState(Windows7Taskbar.ThumbnailProgressState.Indeterminate);
			this.ImageOCR(si, false);
			si.FixSpaces();
			this.SetTaskbarProgressState(Windows7Taskbar.ThumbnailProgressState.NoProgress);
		}

		internal void ImageOCR(SubtitleImage si, bool reportUnknownCharacter)
		{
			if (si.letters == null)
			{
				return;
			}
			this.fonts.debugStrings.Clear();
			si.letters.Where(current => current.Text == null).AsParallel().WithExecutionMode(ParallelExecutionMode.ForceParallelism).ForAll(current =>
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
			});
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
				OcrThread ocrThread = new OcrThread(this, this.subfile, this.stopEvent, this.finishedEvent, this.currentNum, this.subfile.NumSubtitles);
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
				this.activeLetter.Angle = italicLetter.Checked ? Double.IsNaN(SubtitleImage.italicAngle) ? 1.0 / 6.0 : SubtitleImage.italicAngle : 0.0;
				//this.activeLetter.ApplyAngle();
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
			if (this.currentSubtitle.SubtitleBitmap == null)
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
			Bitmap bitmap = this.currentSubtitle.SubtitleBitmap.Clone();
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
					var coords = current.Coords;
					graphics.DrawLine(pen, current.LeftTop, coords.Top, current.LeftBottom, coords.Bottom - 1.0f);
					graphics.DrawLine(pen, current.RightTop, coords.Top, current.RightBottom, coords.Bottom - 1.0f);
					graphics.DrawLine(pen, current.LeftTop, coords.Top - 1.0f, current.RightTop, coords.Top - 1.0f);
					graphics.DrawLine(pen, current.LeftBottom, coords.Bottom, current.RightBottom, coords.Bottom);
				}
			}
#if DEBUG
			if (this.currentSubtitle.debugPoints != null && this.dbgSpace.Checked)
			{
				foreach (PointF point1 in this.currentSubtitle.debugPoints)
				{
					graphics.FillRectangle(this.bluePen.Brush, point1.X, point1.Y, 1.0f, 1.0f);
				}
			}
			if (this.currentSubtitle.debugLines != null && this.dbgEdges.Checked)
			{
				foreach (PointF[] line in this.currentSubtitle.debugLines)
				{
					graphics.DrawLine(this.yellowPen, line[0], line[1]);
				}
			}
#endif
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
			UpdateThumbnailClip(bitmap);
			if (this.activeLetter == null)
			{
				this.letterPictureBox.Image = null;
				this.letterOKButton.Enabled = false;
			}
			else
			{
				var bitmap2 = this.activeLetter.GetBitmap();
				this.letterPictureBox.Image = bitmap2;
				this.letterOKButton.Enabled = (this.letterPictureBox.Image != null);
#if DEBUG
				if (this.dbgEdges.Checked && bitmap2.Width >= 8 && bitmap2.Height >= 8)
				{
					Graphics graphics2 = Graphics.FromImage(bitmap2);
					float size = 8.0f;
					for (float x = bitmap2.Width / size; x < bitmap2.Width; x += bitmap2.Width / size)
					{
						graphics2.DrawLine(greenPen, x, 0.0f, x, bitmap2.Height);
					}
					for (float y = bitmap2.Height / size; y < bitmap2.Height; y += bitmap2.Height / size)
					{
						graphics2.DrawLine(greenPen, 0.0f, y, bitmap2.Width, y);
					}
				}
#endif
			}
		}

		private void UpdateThumbnailClip(Image bitmap)
		{
			if (!Windows7Taskbar.Supported) return;
			if (bitmap == null)
			{
				UpdateThumbnailClip();
				return;
			}
			double zoom = Math.Min((double)subtitlePictureBox.Width / (double)bitmap.Width,
				(double)subtitlePictureBox.Height / (double)bitmap.Height);
			var clipsize = zoom < 1.0 ? subtitlePictureBox.Size : new Size((int)(bitmap.Width * zoom), (int)(bitmap.Height * zoom));
			var clippoint = this.PointToClient(subtitlePictureBox.PointToScreen(Point.Empty));
			if (zoom >= 1.0)
			{
				clippoint.X += (int)((subtitlePictureBox.Width - bitmap.Width * zoom) / 2);
				clippoint.Y += (int)((subtitlePictureBox.Height - bitmap.Height * zoom) / 2);
			}
			this.SetThumbnailClip(new Rectangle(clippoint, clipsize));
		}

		private void UpdateThumbnailClip(Control control)
		{
			if (control.Parent is TabPage)
			{
				var mainTab = mainTabControl.TabPages[0];
				this.SetThumbnailClip(new Rectangle(this.PointToClient(mainTab.PointToScreen(control.Location)), control.Size));
			}
			else
			{
				this.SetThumbnailClip(this.RectangleToClient(control.RectangleToScreen(control.ClientRectangle)));
			}
		}

		private void UpdateThumbnailClip()
		{
			this.SetThumbnailClip(this.ClientRectangle);
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
#if DEBUG
					this.debugLabel.Text = String.Format("Exact angle={0:0.##}° Coords={1:0.#}, {2:0.#}pix Size={3:0.#} x {4:0.#}pix",
						!Double.IsNaN(subtitleLetter.ExactAngle) ? (object)(90.0 - Math.Atan(subtitleLetter.ExactAngle) * 180.0 / Math.PI) : "none",
						subtitleLetter.Coords.Left, subtitleLetter.Coords.Top,
						subtitleLetter.Coords.Width, subtitleLetter.Coords.Height);
#endif
					this.activeLetter = subtitleLetter;
					this.letterInputBox.Text = subtitleLetter.Text;
					this.letterInputBox.SelectAll();
					this.letterInputBox.Focus();
					base.AcceptButton = this.letterOKButton;
					this.italicLetter.Checked = this.activeLetter.Angle != 0.0;
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
			OcrThread @object = new OcrThread(this, this.subfile, this.stopEvent, this.finishedEvent, this.subfile.NumSubtitles);
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
			ReadOptions();
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

		private void options_TextChanged(object sender, EventArgs e)
		{
			this.ApplyOptions();
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.SaveSettings();
		}

		private void cbFonts_SelectedIndexChanged(object sender, EventArgs e)
		{
			pictureBoxLetter.Image = null;
			this.listLetters.BeginUpdate();
			try
			{
				this.listLetters.Items.Clear();
				string fontName = (string)cbFonts.SelectedItem;
				if (!String.IsNullOrEmpty(fontName))
					this.listLetters.Items.AddRange(this.fonts.FontLetters(fontName)
						.OrderBy(letter => letter.Text, StringComparer.Ordinal).Select(letter => new ListViewItem(new string[] { letter.Text,
							String.Format("{0}x{1}", letter.ImageWidth, letter.ImageHeight),
							String.Join(" ", letter.Text.Select(c => ((int)c).ToString("X4")))})).ToArray());
			}
			finally
			{
				this.listLetters.EndUpdate();
			}
		}

		private void listLetters_SelectedIndexChanged(object sender, EventArgs e)
		{
			pictureBoxLetter.Image = null;
			var item = listLetters.SelectedItems.Cast<ListViewItem>().FirstOrDefault();
			if (item == null)
				return;
			string letterText = (string)item.Text;
			string fontName = (string)cbFonts.SelectedItem;
			var letter = this.fonts.FontLetters(fontName).FirstOrDefault(l => l.Text == letterText);
			if (letter == null)
				return;
			var bmp = letter.GetBitmap();
			pictureBoxLetter.Image = bmp;
		}

		private void dbgSpace_CheckedChanged(object sender, EventArgs e)
		{
			UpdateBitmaps();
		}

		private void mainTabControl_Selected(object sender, TabControlEventArgs e)
		{
			if (e.TabPage == imagePage)
				UpdateThumbnailClip(subtitlePictureBox.Image);
			else if (e.TabPage == srtPage)
				UpdateThumbnailClip(srtTextBox);
			else
				UpdateThumbnailClip();
		}
	}
}
