using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
namespace SupRip
{
	public class MainForm : Form
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
		private IContainer components;
		private Button loadButton;
		private Button autoOCRButton;
		private TabPage srtPage;
		private GroupBox groupBox3;
		private CheckBox convertDoubleApostrophes;
		private GroupBox groupBox2;
		private Label finishedLabel;
		private Label containingErrorsLabel;
		private Label unscannedLabel;
		private Label label4;
		private Label label3;
		private Label label2;
		private TextBox srtTextBox;
		private Button saveButton;
		private TabPage imagePage;
		private CheckBox autoProgress;
		private PictureBox letterPictureBox;
		private GroupBox groupBox1;
		private TextBox minimumSpaceCharacterWidthTextBox;
		private Label label1;
		private PictureBox subtitlePictureBox;
		private Button previousButton;
		private Button nextButton;
		private Label totalPages;
		private Button ocrButton;
		private Button letterOKButton;
		private TextBox letterInputBox;
		private TabControl mainTabControl;
		private TextBox charSplitTolerance;
		private Label label5;
		private CheckBox stripFormatting;
		private TextBox pageNum;
		private TextBox similarityTolerance;
		private Label label6;
		private CheckBox replaceHighCommas;
		private Button startCharacterMap;
		private RichTextBox subtitleTextBox2;
		private Button resetDefaults;
		private TextBox ptsOffset;
		private Label label7;
		private Label label8;
		private CheckBox forcedOnly;
		private Label subtitleType;
		private Label label9;
		private CheckBox combineSubtitles;
		private Button debugButton;
		private Label label10;
		private Label fontName;
		private Label label11;
		private TextBox contrast;
		private CheckBox italicLetter;
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
		protected override void Dispose(bool disposing)
		{
			this.SaveSettings();
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}
		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(MainForm));
			this.loadButton = new Button();
			this.autoOCRButton = new Button();
			this.srtPage = new TabPage();
			this.groupBox3 = new GroupBox();
			this.combineSubtitles = new CheckBox();
			this.forcedOnly = new CheckBox();
			this.label8 = new Label();
			this.ptsOffset = new TextBox();
			this.label7 = new Label();
			this.replaceHighCommas = new CheckBox();
			this.stripFormatting = new CheckBox();
			this.convertDoubleApostrophes = new CheckBox();
			this.groupBox2 = new GroupBox();
			this.finishedLabel = new Label();
			this.containingErrorsLabel = new Label();
			this.unscannedLabel = new Label();
			this.label4 = new Label();
			this.label3 = new Label();
			this.label2 = new Label();
			this.srtTextBox = new TextBox();
			this.saveButton = new Button();
			this.imagePage = new TabPage();
			this.subtitleTextBox2 = new RichTextBox();
			this.startCharacterMap = new Button();
			this.pageNum = new TextBox();
			this.autoProgress = new CheckBox();
			this.letterPictureBox = new PictureBox();
			this.groupBox1 = new GroupBox();
			this.contrast = new TextBox();
			this.label11 = new Label();
			this.resetDefaults = new Button();
			this.similarityTolerance = new TextBox();
			this.charSplitTolerance = new TextBox();
			this.label6 = new Label();
			this.minimumSpaceCharacterWidthTextBox = new TextBox();
			this.label5 = new Label();
			this.label1 = new Label();
			this.subtitlePictureBox = new PictureBox();
			this.previousButton = new Button();
			this.nextButton = new Button();
			this.totalPages = new Label();
			this.ocrButton = new Button();
			this.letterOKButton = new Button();
			this.letterInputBox = new TextBox();
			this.mainTabControl = new TabControl();
			this.subtitleType = new Label();
			this.label9 = new Label();
			this.debugButton = new Button();
			this.label10 = new Label();
			this.fontName = new Label();
			this.italicLetter = new CheckBox();
			this.srtPage.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.imagePage.SuspendLayout();
			((ISupportInitialize)this.letterPictureBox).BeginInit();
			this.groupBox1.SuspendLayout();
			((ISupportInitialize)this.subtitlePictureBox).BeginInit();
			this.mainTabControl.SuspendLayout();
			base.SuspendLayout();
			this.loadButton.Location = new Point(12, 12);
			this.loadButton.Name = "loadButton";
			this.loadButton.Size = new Size(75, 23);
			this.loadButton.TabIndex = 0;
			this.loadButton.Text = "&Open...";
			this.loadButton.UseVisualStyleBackColor = true;
			this.loadButton.Click += new EventHandler(this.loadButton_Click);
			this.autoOCRButton.Location = new Point(93, 12);
			this.autoOCRButton.Name = "autoOCRButton";
			this.autoOCRButton.Size = new Size(75, 23);
			this.autoOCRButton.TabIndex = 1;
			this.autoOCRButton.Text = "&Auto-OCR";
			this.autoOCRButton.UseVisualStyleBackColor = true;
			this.autoOCRButton.Click += new EventHandler(this.autoOCRButton_Click);
			this.srtPage.Controls.Add(this.groupBox3);
			this.srtPage.Controls.Add(this.groupBox2);
			this.srtPage.Controls.Add(this.srtTextBox);
			this.srtPage.Controls.Add(this.saveButton);
			this.srtPage.Location = new Point(4, 22);
			this.srtPage.Name = "srtPage";
			this.srtPage.Padding = new Padding(3);
			this.srtPage.Size = new Size(873, 406);
			this.srtPage.TabIndex = 1;
			this.srtPage.Text = "SRT";
			this.srtPage.UseVisualStyleBackColor = true;
			this.groupBox3.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.groupBox3.Controls.Add(this.combineSubtitles);
			this.groupBox3.Controls.Add(this.forcedOnly);
			this.groupBox3.Controls.Add(this.label8);
			this.groupBox3.Controls.Add(this.ptsOffset);
			this.groupBox3.Controls.Add(this.label7);
			this.groupBox3.Controls.Add(this.replaceHighCommas);
			this.groupBox3.Controls.Add(this.stripFormatting);
			this.groupBox3.Controls.Add(this.convertDoubleApostrophes);
			this.groupBox3.Location = new Point(614, 160);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new Size(245, 160);
			this.groupBox3.TabIndex = 3;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Options";
			this.combineSubtitles.AutoSize = true;
			this.combineSubtitles.Location = new Point(10, 113);
			this.combineSubtitles.Name = "combineSubtitles";
			this.combineSubtitles.Size = new Size(150, 17);
			this.combineSubtitles.TabIndex = 6;
			this.combineSubtitles.Text = "Combine identical subtitles";
			this.combineSubtitles.UseVisualStyleBackColor = true;
			this.combineSubtitles.CheckedChanged += new EventHandler(this.combineSubtitles_CheckedChanged);
			this.forcedOnly.AutoSize = true;
			this.forcedOnly.Location = new Point(10, 91);
			this.forcedOnly.Name = "forcedOnly";
			this.forcedOnly.Size = new Size(121, 17);
			this.forcedOnly.TabIndex = 6;
			this.forcedOnly.Text = "Only forced subtitles";
			this.forcedOnly.UseVisualStyleBackColor = true;
			this.forcedOnly.CheckedChanged += new EventHandler(this.forcedOnly_CheckedChanged);
			this.label8.AutoSize = true;
			this.label8.Location = new Point(164, 134);
			this.label8.Name = "label8";
			this.label8.Size = new Size(20, 13);
			this.label8.TabIndex = 5;
			this.label8.Text = "ms";
			this.ptsOffset.Location = new Point(88, 132);
			this.ptsOffset.Name = "ptsOffset";
			this.ptsOffset.Size = new Size(73, 20);
			this.ptsOffset.TabIndex = 4;
			this.ptsOffset.TextChanged += new EventHandler(this.ptsOffset_TextChanged);
			this.label7.AutoSize = true;
			this.label7.Location = new Point(6, 135);
			this.label7.Name = "label7";
			this.label7.Size = new Size(61, 13);
			this.label7.TabIndex = 3;
			this.label7.Text = "Time Offset";
			this.replaceHighCommas.AutoSize = true;
			this.replaceHighCommas.Location = new Point(10, 67);
			this.replaceHighCommas.Name = "replaceHighCommas";
			this.replaceHighCommas.Size = new Size(122, 17);
			this.replaceHighCommas.TabIndex = 2;
			this.replaceHighCommas.Text = "Replace high , with '";
			this.replaceHighCommas.UseVisualStyleBackColor = true;
			this.replaceHighCommas.CheckedChanged += new EventHandler(this.replaceHighCommas_CheckedChanged);
			this.stripFormatting.AutoSize = true;
			this.stripFormatting.Location = new Point(10, 44);
			this.stripFormatting.Name = "stripFormatting";
			this.stripFormatting.Size = new Size(189, 17);
			this.stripFormatting.TabIndex = 1;
			this.stripFormatting.Text = "Strip HTML style formatting <i></i>";
			this.stripFormatting.UseVisualStyleBackColor = true;
			this.stripFormatting.CheckedChanged += new EventHandler(this.stripFormatting_CheckedChanged);
			this.convertDoubleApostrophes.AutoSize = true;
			this.convertDoubleApostrophes.Location = new Point(10, 20);
			this.convertDoubleApostrophes.Name = "convertDoubleApostrophes";
			this.convertDoubleApostrophes.Size = new Size(123, 17);
			this.convertDoubleApostrophes.TabIndex = 0;
			this.convertDoubleApostrophes.Text = "Convert double ' to \"";
			this.convertDoubleApostrophes.UseVisualStyleBackColor = true;
			this.convertDoubleApostrophes.CheckedChanged += new EventHandler(this.convertDoubleApostrophes_CheckedChanged);
			this.groupBox2.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.groupBox2.Controls.Add(this.finishedLabel);
			this.groupBox2.Controls.Add(this.containingErrorsLabel);
			this.groupBox2.Controls.Add(this.unscannedLabel);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Location = new Point(614, 45);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new Size(245, 98);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Statistics";
			this.finishedLabel.AutoSize = true;
			this.finishedLabel.Location = new Point(124, 69);
			this.finishedLabel.Name = "finishedLabel";
			this.finishedLabel.Size = new Size(24, 13);
			this.finishedLabel.TabIndex = 5;
			this.finishedLabel.Text = "n/a";
			this.containingErrorsLabel.AutoSize = true;
			this.containingErrorsLabel.Location = new Point(124, 44);
			this.containingErrorsLabel.Name = "containingErrorsLabel";
			this.containingErrorsLabel.Size = new Size(24, 13);
			this.containingErrorsLabel.TabIndex = 3;
			this.containingErrorsLabel.Text = "n/a";
			this.unscannedLabel.AutoSize = true;
			this.unscannedLabel.Location = new Point(124, 19);
			this.unscannedLabel.Name = "unscannedLabel";
			this.unscannedLabel.Size = new Size(24, 13);
			this.unscannedLabel.TabIndex = 1;
			this.unscannedLabel.Text = "n/a";
			this.label4.AutoSize = true;
			this.label4.Location = new Point(7, 69);
			this.label4.Name = "label4";
			this.label4.Size = new Size(82, 13);
			this.label4.TabIndex = 4;
			this.label4.Text = "Correctly OCR'd";
			this.label3.AutoSize = true;
			this.label3.Location = new Point(7, 44);
			this.label3.Name = "label3";
			this.label3.Size = new Size(87, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "Containing Errors";
			this.label2.AutoSize = true;
			this.label2.Location = new Point(7, 20);
			this.label2.Name = "label2";
			this.label2.Size = new Size(70, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Unprocessed";
			this.srtTextBox.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left);
			this.srtTextBox.Location = new Point(7, 7);
			this.srtTextBox.Multiline = true;
			this.srtTextBox.Name = "srtTextBox";
			this.srtTextBox.ScrollBars = ScrollBars.Vertical;
			this.srtTextBox.Size = new Size(595, 369);
			this.srtTextBox.TabIndex = 0;
			this.saveButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.saveButton.Location = new Point(608, 5);
			this.saveButton.Name = "saveButton";
			this.saveButton.Size = new Size(75, 23);
			this.saveButton.TabIndex = 1;
			this.saveButton.Text = "Save...";
			this.saveButton.UseVisualStyleBackColor = true;
			this.saveButton.Click += new EventHandler(this.saveButton_Click);
			this.imagePage.Controls.Add(this.italicLetter);
			this.imagePage.Controls.Add(this.subtitleTextBox2);
			this.imagePage.Controls.Add(this.startCharacterMap);
			this.imagePage.Controls.Add(this.pageNum);
			this.imagePage.Controls.Add(this.autoProgress);
			this.imagePage.Controls.Add(this.letterPictureBox);
			this.imagePage.Controls.Add(this.groupBox1);
			this.imagePage.Controls.Add(this.subtitlePictureBox);
			this.imagePage.Controls.Add(this.previousButton);
			this.imagePage.Controls.Add(this.nextButton);
			this.imagePage.Controls.Add(this.totalPages);
			this.imagePage.Controls.Add(this.ocrButton);
			this.imagePage.Controls.Add(this.letterOKButton);
			this.imagePage.Controls.Add(this.letterInputBox);
			this.imagePage.Location = new Point(4, 22);
			this.imagePage.Name = "imagePage";
			this.imagePage.Padding = new Padding(3);
			this.imagePage.Size = new Size(873, 406);
			this.imagePage.TabIndex = 0;
			this.imagePage.Text = "Image";
			this.imagePage.UseVisualStyleBackColor = true;
			this.imagePage.Paint += new PaintEventHandler(this.imagePage_Paint);
			this.subtitleTextBox2.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.subtitleTextBox2.Font = new Font("Arial", 14.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.subtitleTextBox2.Location = new Point(404, 248);
			this.subtitleTextBox2.Name = "subtitleTextBox2";
			this.subtitleTextBox2.ReadOnly = true;
			this.subtitleTextBox2.Size = new Size(463, 96);
			this.subtitleTextBox2.TabIndex = 8;
			this.subtitleTextBox2.Text = "";
			this.startCharacterMap.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.startCharacterMap.Location = new Point(777, 377);
			this.startCharacterMap.Name = "startCharacterMap";
			this.startCharacterMap.Size = new Size(90, 23);
			this.startCharacterMap.TabIndex = 10;
			this.startCharacterMap.Text = "Character &Map";
			this.startCharacterMap.UseVisualStyleBackColor = true;
			this.startCharacterMap.Click += new EventHandler(this.startCharacterMap_Click);
			this.pageNum.Location = new Point(390, 8);
			this.pageNum.Name = "pageNum";
			this.pageNum.Size = new Size(41, 20);
			this.pageNum.TabIndex = 2;
			this.pageNum.TextAlign = HorizontalAlignment.Right;
			this.pageNum.TextChanged += new EventHandler(this.pageNum_TextChanged);
			this.autoProgress.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.autoProgress.AutoSize = true;
			this.autoProgress.Checked = true;
			this.autoProgress.CheckState = CheckState.Checked;
			this.autoProgress.Location = new Point(89, 226);
			this.autoProgress.Name = "autoProgress";
			this.autoProgress.Size = new Size(215, 17);
			this.autoProgress.TabIndex = 5;
			this.autoProgress.Text = "Automatically continue with next Subtitle";
			this.autoProgress.UseVisualStyleBackColor = true;
			this.letterPictureBox.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.letterPictureBox.Location = new Point(293, 301);
			this.letterPictureBox.Name = "letterPictureBox";
			this.letterPictureBox.Size = new Size(101, 95);
			this.letterPictureBox.TabIndex = 9;
			this.letterPictureBox.TabStop = false;
			this.groupBox1.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.groupBox1.Controls.Add(this.contrast);
			this.groupBox1.Controls.Add(this.label11);
			this.groupBox1.Controls.Add(this.resetDefaults);
			this.groupBox1.Controls.Add(this.similarityTolerance);
			this.groupBox1.Controls.Add(this.charSplitTolerance);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.minimumSpaceCharacterWidthTextBox);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new Point(6, 251);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new Size(230, 149);
			this.groupBox1.TabIndex = 9;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Options";
			this.contrast.Location = new Point(160, 93);
			this.contrast.Name = "contrast";
			this.contrast.Size = new Size(60, 20);
			this.contrast.TabIndex = 9;
			this.contrast.TextChanged += new EventHandler(this.options_TextChanged);
			this.label11.AutoSize = true;
			this.label11.Location = new Point(7, 96);
			this.label11.Name = "label11";
			this.label11.Size = new Size(46, 13);
			this.label11.TabIndex = 8;
			this.label11.Text = "Contrast";
			this.resetDefaults.Location = new Point(160, 120);
			this.resetDefaults.Name = "resetDefaults";
			this.resetDefaults.Size = new Size(59, 23);
			this.resetDefaults.TabIndex = 7;
			this.resetDefaults.Text = "Defaults";
			this.resetDefaults.UseVisualStyleBackColor = true;
			this.resetDefaults.Click += new EventHandler(this.resetDefaults_Click);
			this.similarityTolerance.Location = new Point(160, 67);
			this.similarityTolerance.Name = "similarityTolerance";
			this.similarityTolerance.Size = new Size(60, 20);
			this.similarityTolerance.TabIndex = 5;
			this.similarityTolerance.TextChanged += new EventHandler(this.options_TextChanged);
			this.charSplitTolerance.Location = new Point(160, 41);
			this.charSplitTolerance.Name = "charSplitTolerance";
			this.charSplitTolerance.Size = new Size(60, 20);
			this.charSplitTolerance.TabIndex = 3;
			this.charSplitTolerance.TextChanged += new EventHandler(this.options_TextChanged);
			this.label6.AutoSize = true;
			this.label6.Location = new Point(7, 70);
			this.label6.Name = "label6";
			this.label6.Size = new Size(147, 13);
			this.label6.TabIndex = 4;
			this.label6.Text = "Character Similarity Tolerance";
			this.minimumSpaceCharacterWidthTextBox.Location = new Point(160, 17);
			this.minimumSpaceCharacterWidthTextBox.Name = "minimumSpaceCharacterWidthTextBox";
			this.minimumSpaceCharacterWidthTextBox.Size = new Size(60, 20);
			this.minimumSpaceCharacterWidthTextBox.TabIndex = 1;
			this.minimumSpaceCharacterWidthTextBox.TextChanged += new EventHandler(this.options_TextChanged);
			this.label5.AutoSize = true;
			this.label5.Location = new Point(7, 44);
			this.label5.Name = "label5";
			this.label5.Size = new Size(127, 13);
			this.label5.TabIndex = 2;
			this.label5.Text = "Character Split Tolerance";
			this.label1.AutoSize = true;
			this.label1.Location = new Point(7, 20);
			this.label1.Name = "label1";
			this.label1.Size = new Size(69, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Space Width";
			this.subtitlePictureBox.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.subtitlePictureBox.Location = new Point(8, 42);
			this.subtitlePictureBox.Name = "subtitlePictureBox";
			this.subtitlePictureBox.Size = new Size(859, 174);
			this.subtitlePictureBox.TabIndex = 8;
			this.subtitlePictureBox.TabStop = false;
			this.subtitlePictureBox.MouseClick += new MouseEventHandler(this.subtitlePictureBox_MouseClick);
			this.previousButton.Location = new Point(6, 6);
			this.previousButton.Name = "previousButton";
			this.previousButton.Size = new Size(24, 23);
			this.previousButton.TabIndex = 0;
			this.previousButton.Text = "<";
			this.previousButton.UseVisualStyleBackColor = true;
			this.previousButton.Click += new EventHandler(this.previousButton_Click);
			this.nextButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.nextButton.Location = new Point(843, 6);
			this.nextButton.Name = "nextButton";
			this.nextButton.Size = new Size(24, 23);
			this.nextButton.TabIndex = 1;
			this.nextButton.Text = ">";
			this.nextButton.UseVisualStyleBackColor = true;
			this.nextButton.Click += new EventHandler(this.nextButton_Click);
			this.totalPages.AutoSize = true;
			this.totalPages.Location = new Point(432, 11);
			this.totalPages.Name = "totalPages";
			this.totalPages.Size = new Size(76, 13);
			this.totalPages.TabIndex = 3;
			this.totalPages.Text = "No file opened";
			this.ocrButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.ocrButton.Location = new Point(8, 222);
			this.ocrButton.Name = "ocrButton";
			this.ocrButton.Size = new Size(75, 23);
			this.ocrButton.TabIndex = 4;
			this.ocrButton.Text = "O&CR";
			this.ocrButton.UseVisualStyleBackColor = true;
			this.ocrButton.Click += new EventHandler(this.ocrButton_Click);
			this.letterOKButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.letterOKButton.Location = new Point(510, 364);
			this.letterOKButton.Name = "letterOKButton";
			this.letterOKButton.Size = new Size(75, 23);
			this.letterOKButton.TabIndex = 7;
			this.letterOKButton.Text = "OK";
			this.letterOKButton.UseVisualStyleBackColor = true;
			this.letterOKButton.Click += new EventHandler(this.letterOKButton_Click);
			this.letterInputBox.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.letterInputBox.Font = new Font("Arial", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.letterInputBox.Location = new Point(404, 366);
			this.letterInputBox.Name = "letterInputBox";
			this.letterInputBox.Size = new Size(100, 20);
			this.letterInputBox.TabIndex = 6;
			this.mainTabControl.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.mainTabControl.Controls.Add(this.imagePage);
			this.mainTabControl.Controls.Add(this.srtPage);
			this.mainTabControl.Location = new Point(12, 41);
			this.mainTabControl.Name = "mainTabControl";
			this.mainTabControl.SelectedIndex = 0;
			this.mainTabControl.Size = new Size(881, 432);
			this.mainTabControl.TabIndex = 2;
			this.mainTabControl.Click += new EventHandler(this.mainTabControl_Click);
			this.subtitleType.AutoSize = true;
			this.subtitleType.Location = new Point(412, 21);
			this.subtitleType.Name = "subtitleType";
			this.subtitleType.Size = new Size(10, 13);
			this.subtitleType.TabIndex = 3;
			this.subtitleType.Text = "-";
			this.label9.AutoSize = true;
			this.label9.Location = new Point(372, 21);
			this.label9.Name = "label9";
			this.label9.Size = new Size(34, 13);
			this.label9.TabIndex = 4;
			this.label9.Text = "Type:";
			this.debugButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.debugButton.Location = new Point(813, 12);
			this.debugButton.Name = "debugButton";
			this.debugButton.Size = new Size(75, 23);
			this.debugButton.TabIndex = 5;
			this.debugButton.Text = "Debug";
			this.debugButton.UseVisualStyleBackColor = true;
			this.debugButton.Click += new EventHandler(this.debugButton_Click);
			this.label10.AutoSize = true;
			this.label10.Location = new Point(472, 21);
			this.label10.Name = "label10";
			this.label10.Size = new Size(31, 13);
			this.label10.TabIndex = 4;
			this.label10.Text = "Font:";
			this.fontName.AutoSize = true;
			this.fontName.Location = new Point(509, 21);
			this.fontName.Name = "fontName";
			this.fontName.Size = new Size(10, 13);
			this.fontName.TabIndex = 6;
			this.fontName.Text = "-";
			this.italicLetter.AutoSize = true;
			this.italicLetter.Location = new Point(592, 368);
			this.italicLetter.Name = "italicLetter";
			this.italicLetter.Size = new Size(48, 17);
			this.italicLetter.TabIndex = 11;
			this.italicLetter.Text = "&Italic";
			this.italicLetter.UseVisualStyleBackColor = true;
			this.italicLetter.CheckedChanged += new EventHandler(this.italicLetter_CheckedChanged);
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(900, 478);
			base.Controls.Add(this.fontName);
			base.Controls.Add(this.debugButton);
			base.Controls.Add(this.label10);
			base.Controls.Add(this.label9);
			base.Controls.Add(this.subtitleType);
			base.Controls.Add(this.mainTabControl);
			base.Controls.Add(this.autoOCRButton);
			base.Controls.Add(this.loadButton);
			base.HelpButton = true;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			this.MinimumSize = new Size(916, 490);
			base.Name = "MainForm";
			this.Text = "SupRip";
			base.Load += new EventHandler(this.MainForm_Load);
			this.srtPage.ResumeLayout(false);
			this.srtPage.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.imagePage.ResumeLayout(false);
			this.imagePage.PerformLayout();
			((ISupportInitialize)this.letterPictureBox).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((ISupportInitialize)this.subtitlePictureBox).EndInit();
			this.mainTabControl.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
