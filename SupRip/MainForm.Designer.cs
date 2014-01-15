namespace SupRip
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.loadButton = new System.Windows.Forms.Button();
			this.autoOCRButton = new System.Windows.Forms.Button();
			this.srtPage = new System.Windows.Forms.TabPage();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.combineSubtitles = new System.Windows.Forms.CheckBox();
			this.forcedOnly = new System.Windows.Forms.CheckBox();
			this.label8 = new System.Windows.Forms.Label();
			this.ptsOffset = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.replaceHighCommas = new System.Windows.Forms.CheckBox();
			this.stripFormatting = new System.Windows.Forms.CheckBox();
			this.convertDoubleApostrophes = new System.Windows.Forms.CheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.finishedLabel = new System.Windows.Forms.Label();
			this.containingErrorsLabel = new System.Windows.Forms.Label();
			this.unscannedLabel = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.srtTextBox = new System.Windows.Forms.TextBox();
			this.saveButton = new System.Windows.Forms.Button();
			this.imagePage = new System.Windows.Forms.TabPage();
			this.italicLetter = new System.Windows.Forms.CheckBox();
			this.subtitleTextBox2 = new System.Windows.Forms.RichTextBox();
			this.startCharacterMap = new System.Windows.Forms.Button();
			this.pageNum = new System.Windows.Forms.TextBox();
			this.autoProgress = new System.Windows.Forms.CheckBox();
			this.letterPictureBox = new System.Windows.Forms.PictureBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.contrast = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.resetDefaults = new System.Windows.Forms.Button();
			this.similarityTolerance = new System.Windows.Forms.TextBox();
			this.charSplitTolerance = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.minimumSpaceCharacterWidthTextBox = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.subtitlePictureBox = new System.Windows.Forms.PictureBox();
			this.previousButton = new System.Windows.Forms.Button();
			this.nextButton = new System.Windows.Forms.Button();
			this.totalPages = new System.Windows.Forms.Label();
			this.ocrButton = new System.Windows.Forms.Button();
			this.letterOKButton = new System.Windows.Forms.Button();
			this.letterInputBox = new System.Windows.Forms.TextBox();
			this.mainTabControl = new System.Windows.Forms.TabControl();
			this.subtitleType = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.debugButton = new System.Windows.Forms.Button();
			this.label10 = new System.Windows.Forms.Label();
			this.fontName = new System.Windows.Forms.Label();
			this.srtPage.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.imagePage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.letterPictureBox)).BeginInit();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.subtitlePictureBox)).BeginInit();
			this.mainTabControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// loadButton
			// 
			this.loadButton.Location = new System.Drawing.Point(12, 12);
			this.loadButton.Name = "loadButton";
			this.loadButton.Size = new System.Drawing.Size(75, 23);
			this.loadButton.TabIndex = 0;
			this.loadButton.Text = "&Open...";
			this.loadButton.UseVisualStyleBackColor = true;
			this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
			// 
			// autoOCRButton
			// 
			this.autoOCRButton.Location = new System.Drawing.Point(93, 12);
			this.autoOCRButton.Name = "autoOCRButton";
			this.autoOCRButton.Size = new System.Drawing.Size(75, 23);
			this.autoOCRButton.TabIndex = 1;
			this.autoOCRButton.Text = "&Auto-OCR";
			this.autoOCRButton.UseVisualStyleBackColor = true;
			this.autoOCRButton.Click += new System.EventHandler(this.autoOCRButton_Click);
			// 
			// srtPage
			// 
			this.srtPage.Controls.Add(this.groupBox3);
			this.srtPage.Controls.Add(this.groupBox2);
			this.srtPage.Controls.Add(this.srtTextBox);
			this.srtPage.Controls.Add(this.saveButton);
			this.srtPage.Location = new System.Drawing.Point(4, 22);
			this.srtPage.Name = "srtPage";
			this.srtPage.Padding = new System.Windows.Forms.Padding(3);
			this.srtPage.Size = new System.Drawing.Size(873, 406);
			this.srtPage.TabIndex = 1;
			this.srtPage.Text = "SRT";
			this.srtPage.UseVisualStyleBackColor = true;
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this.combineSubtitles);
			this.groupBox3.Controls.Add(this.forcedOnly);
			this.groupBox3.Controls.Add(this.label8);
			this.groupBox3.Controls.Add(this.ptsOffset);
			this.groupBox3.Controls.Add(this.label7);
			this.groupBox3.Controls.Add(this.replaceHighCommas);
			this.groupBox3.Controls.Add(this.stripFormatting);
			this.groupBox3.Controls.Add(this.convertDoubleApostrophes);
			this.groupBox3.Location = new System.Drawing.Point(614, 160);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(245, 160);
			this.groupBox3.TabIndex = 3;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Options";
			// 
			// combineSubtitles
			// 
			this.combineSubtitles.AutoSize = true;
			this.combineSubtitles.Location = new System.Drawing.Point(10, 113);
			this.combineSubtitles.Name = "combineSubtitles";
			this.combineSubtitles.Size = new System.Drawing.Size(150, 17);
			this.combineSubtitles.TabIndex = 6;
			this.combineSubtitles.Text = "Combine identical subtitles";
			this.combineSubtitles.UseVisualStyleBackColor = true;
			this.combineSubtitles.CheckedChanged += new System.EventHandler(this.combineSubtitles_CheckedChanged);
			// 
			// forcedOnly
			// 
			this.forcedOnly.AutoSize = true;
			this.forcedOnly.Location = new System.Drawing.Point(10, 91);
			this.forcedOnly.Name = "forcedOnly";
			this.forcedOnly.Size = new System.Drawing.Size(121, 17);
			this.forcedOnly.TabIndex = 6;
			this.forcedOnly.Text = "Only forced subtitles";
			this.forcedOnly.UseVisualStyleBackColor = true;
			this.forcedOnly.CheckedChanged += new System.EventHandler(this.forcedOnly_CheckedChanged);
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(164, 134);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(20, 13);
			this.label8.TabIndex = 5;
			this.label8.Text = "ms";
			// 
			// ptsOffset
			// 
			this.ptsOffset.Location = new System.Drawing.Point(88, 132);
			this.ptsOffset.Name = "ptsOffset";
			this.ptsOffset.Size = new System.Drawing.Size(73, 20);
			this.ptsOffset.TabIndex = 4;
			this.ptsOffset.TextChanged += new System.EventHandler(this.ptsOffset_TextChanged);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(6, 135);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(61, 13);
			this.label7.TabIndex = 3;
			this.label7.Text = "Time Offset";
			// 
			// replaceHighCommas
			// 
			this.replaceHighCommas.AutoSize = true;
			this.replaceHighCommas.Location = new System.Drawing.Point(10, 67);
			this.replaceHighCommas.Name = "replaceHighCommas";
			this.replaceHighCommas.Size = new System.Drawing.Size(122, 17);
			this.replaceHighCommas.TabIndex = 2;
			this.replaceHighCommas.Text = "Replace high , with \'";
			this.replaceHighCommas.UseVisualStyleBackColor = true;
			this.replaceHighCommas.CheckedChanged += new System.EventHandler(this.replaceHighCommas_CheckedChanged);
			// 
			// stripFormatting
			// 
			this.stripFormatting.AutoSize = true;
			this.stripFormatting.Location = new System.Drawing.Point(10, 44);
			this.stripFormatting.Name = "stripFormatting";
			this.stripFormatting.Size = new System.Drawing.Size(189, 17);
			this.stripFormatting.TabIndex = 1;
			this.stripFormatting.Text = "Strip HTML style formatting <i></i>";
			this.stripFormatting.UseVisualStyleBackColor = true;
			this.stripFormatting.CheckedChanged += new System.EventHandler(this.stripFormatting_CheckedChanged);
			// 
			// convertDoubleApostrophes
			// 
			this.convertDoubleApostrophes.AutoSize = true;
			this.convertDoubleApostrophes.Location = new System.Drawing.Point(10, 20);
			this.convertDoubleApostrophes.Name = "convertDoubleApostrophes";
			this.convertDoubleApostrophes.Size = new System.Drawing.Size(123, 17);
			this.convertDoubleApostrophes.TabIndex = 0;
			this.convertDoubleApostrophes.Text = "Convert double \' to \"";
			this.convertDoubleApostrophes.UseVisualStyleBackColor = true;
			this.convertDoubleApostrophes.CheckedChanged += new System.EventHandler(this.convertDoubleApostrophes_CheckedChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.finishedLabel);
			this.groupBox2.Controls.Add(this.containingErrorsLabel);
			this.groupBox2.Controls.Add(this.unscannedLabel);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Location = new System.Drawing.Point(614, 45);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(245, 98);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Statistics";
			// 
			// finishedLabel
			// 
			this.finishedLabel.AutoSize = true;
			this.finishedLabel.Location = new System.Drawing.Point(124, 69);
			this.finishedLabel.Name = "finishedLabel";
			this.finishedLabel.Size = new System.Drawing.Size(24, 13);
			this.finishedLabel.TabIndex = 5;
			this.finishedLabel.Text = "n/a";
			// 
			// containingErrorsLabel
			// 
			this.containingErrorsLabel.AutoSize = true;
			this.containingErrorsLabel.Location = new System.Drawing.Point(124, 44);
			this.containingErrorsLabel.Name = "containingErrorsLabel";
			this.containingErrorsLabel.Size = new System.Drawing.Size(24, 13);
			this.containingErrorsLabel.TabIndex = 3;
			this.containingErrorsLabel.Text = "n/a";
			// 
			// unscannedLabel
			// 
			this.unscannedLabel.AutoSize = true;
			this.unscannedLabel.Location = new System.Drawing.Point(124, 19);
			this.unscannedLabel.Name = "unscannedLabel";
			this.unscannedLabel.Size = new System.Drawing.Size(24, 13);
			this.unscannedLabel.TabIndex = 1;
			this.unscannedLabel.Text = "n/a";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(7, 69);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(82, 13);
			this.label4.TabIndex = 4;
			this.label4.Text = "Correctly OCR\'d";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(7, 44);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(87, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "Containing Errors";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(7, 20);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(70, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Unprocessed";
			// 
			// srtTextBox
			// 
			this.srtTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.srtTextBox.Location = new System.Drawing.Point(7, 7);
			this.srtTextBox.Multiline = true;
			this.srtTextBox.Name = "srtTextBox";
			this.srtTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.srtTextBox.Size = new System.Drawing.Size(595, 369);
			this.srtTextBox.TabIndex = 0;
			// 
			// saveButton
			// 
			this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.saveButton.Location = new System.Drawing.Point(608, 5);
			this.saveButton.Name = "saveButton";
			this.saveButton.Size = new System.Drawing.Size(75, 23);
			this.saveButton.TabIndex = 1;
			this.saveButton.Text = "Save...";
			this.saveButton.UseVisualStyleBackColor = true;
			this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
			// 
			// imagePage
			// 
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
			this.imagePage.Location = new System.Drawing.Point(4, 22);
			this.imagePage.Name = "imagePage";
			this.imagePage.Padding = new System.Windows.Forms.Padding(3);
			this.imagePage.Size = new System.Drawing.Size(873, 406);
			this.imagePage.TabIndex = 0;
			this.imagePage.Text = "Image";
			this.imagePage.UseVisualStyleBackColor = true;
			this.imagePage.Paint += new System.Windows.Forms.PaintEventHandler(this.imagePage_Paint);
			// 
			// italicLetter
			// 
			this.italicLetter.AutoSize = true;
			this.italicLetter.Location = new System.Drawing.Point(592, 368);
			this.italicLetter.Name = "italicLetter";
			this.italicLetter.Size = new System.Drawing.Size(48, 17);
			this.italicLetter.TabIndex = 11;
			this.italicLetter.Text = "&Italic";
			this.italicLetter.UseVisualStyleBackColor = true;
			this.italicLetter.CheckedChanged += new System.EventHandler(this.italicLetter_CheckedChanged);
			// 
			// subtitleTextBox2
			// 
			this.subtitleTextBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.subtitleTextBox2.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.subtitleTextBox2.Location = new System.Drawing.Point(404, 248);
			this.subtitleTextBox2.Name = "subtitleTextBox2";
			this.subtitleTextBox2.ReadOnly = true;
			this.subtitleTextBox2.Size = new System.Drawing.Size(463, 96);
			this.subtitleTextBox2.TabIndex = 8;
			this.subtitleTextBox2.Text = "";
			// 
			// startCharacterMap
			// 
			this.startCharacterMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.startCharacterMap.Location = new System.Drawing.Point(777, 377);
			this.startCharacterMap.Name = "startCharacterMap";
			this.startCharacterMap.Size = new System.Drawing.Size(90, 23);
			this.startCharacterMap.TabIndex = 10;
			this.startCharacterMap.Text = "Character &Map";
			this.startCharacterMap.UseVisualStyleBackColor = true;
			this.startCharacterMap.Click += new System.EventHandler(this.startCharacterMap_Click);
			// 
			// pageNum
			// 
			this.pageNum.Location = new System.Drawing.Point(390, 8);
			this.pageNum.Name = "pageNum";
			this.pageNum.Size = new System.Drawing.Size(41, 20);
			this.pageNum.TabIndex = 2;
			this.pageNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.pageNum.TextChanged += new System.EventHandler(this.pageNum_TextChanged);
			// 
			// autoProgress
			// 
			this.autoProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.autoProgress.AutoSize = true;
			this.autoProgress.Checked = true;
			this.autoProgress.CheckState = System.Windows.Forms.CheckState.Checked;
			this.autoProgress.Location = new System.Drawing.Point(89, 226);
			this.autoProgress.Name = "autoProgress";
			this.autoProgress.Size = new System.Drawing.Size(215, 17);
			this.autoProgress.TabIndex = 5;
			this.autoProgress.Text = "Automatically continue with next Subtitle";
			this.autoProgress.UseVisualStyleBackColor = true;
			// 
			// letterPictureBox
			// 
			this.letterPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.letterPictureBox.Location = new System.Drawing.Point(293, 301);
			this.letterPictureBox.Name = "letterPictureBox";
			this.letterPictureBox.Size = new System.Drawing.Size(101, 95);
			this.letterPictureBox.TabIndex = 9;
			this.letterPictureBox.TabStop = false;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox1.Controls.Add(this.contrast);
			this.groupBox1.Controls.Add(this.label11);
			this.groupBox1.Controls.Add(this.resetDefaults);
			this.groupBox1.Controls.Add(this.similarityTolerance);
			this.groupBox1.Controls.Add(this.charSplitTolerance);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.minimumSpaceCharacterWidthTextBox);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(6, 251);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(230, 149);
			this.groupBox1.TabIndex = 9;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Options";
			// 
			// contrast
			// 
			this.contrast.Location = new System.Drawing.Point(160, 93);
			this.contrast.Name = "contrast";
			this.contrast.Size = new System.Drawing.Size(60, 20);
			this.contrast.TabIndex = 9;
			this.contrast.TextChanged += new System.EventHandler(this.options_TextChanged);
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(7, 96);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(46, 13);
			this.label11.TabIndex = 8;
			this.label11.Text = "Contrast";
			// 
			// resetDefaults
			// 
			this.resetDefaults.Location = new System.Drawing.Point(160, 120);
			this.resetDefaults.Name = "resetDefaults";
			this.resetDefaults.Size = new System.Drawing.Size(59, 23);
			this.resetDefaults.TabIndex = 7;
			this.resetDefaults.Text = "Defaults";
			this.resetDefaults.UseVisualStyleBackColor = true;
			this.resetDefaults.Click += new System.EventHandler(this.resetDefaults_Click);
			// 
			// similarityTolerance
			// 
			this.similarityTolerance.Location = new System.Drawing.Point(160, 67);
			this.similarityTolerance.Name = "similarityTolerance";
			this.similarityTolerance.Size = new System.Drawing.Size(60, 20);
			this.similarityTolerance.TabIndex = 5;
			this.similarityTolerance.TextChanged += new System.EventHandler(this.options_TextChanged);
			// 
			// charSplitTolerance
			// 
			this.charSplitTolerance.Location = new System.Drawing.Point(160, 41);
			this.charSplitTolerance.Name = "charSplitTolerance";
			this.charSplitTolerance.Size = new System.Drawing.Size(60, 20);
			this.charSplitTolerance.TabIndex = 3;
			this.charSplitTolerance.TextChanged += new System.EventHandler(this.options_TextChanged);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(7, 70);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(147, 13);
			this.label6.TabIndex = 4;
			this.label6.Text = "Character Similarity Tolerance";
			// 
			// minimumSpaceCharacterWidthTextBox
			// 
			this.minimumSpaceCharacterWidthTextBox.Location = new System.Drawing.Point(160, 17);
			this.minimumSpaceCharacterWidthTextBox.Name = "minimumSpaceCharacterWidthTextBox";
			this.minimumSpaceCharacterWidthTextBox.Size = new System.Drawing.Size(60, 20);
			this.minimumSpaceCharacterWidthTextBox.TabIndex = 1;
			this.minimumSpaceCharacterWidthTextBox.TextChanged += new System.EventHandler(this.options_TextChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(7, 44);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(127, 13);
			this.label5.TabIndex = 2;
			this.label5.Text = "Character Split Tolerance";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(7, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(69, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Space Width";
			// 
			// subtitlePictureBox
			// 
			this.subtitlePictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.subtitlePictureBox.Location = new System.Drawing.Point(8, 42);
			this.subtitlePictureBox.Name = "subtitlePictureBox";
			this.subtitlePictureBox.Size = new System.Drawing.Size(859, 174);
			this.subtitlePictureBox.TabIndex = 8;
			this.subtitlePictureBox.TabStop = false;
			this.subtitlePictureBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.subtitlePictureBox_MouseClick);
			// 
			// previousButton
			// 
			this.previousButton.Location = new System.Drawing.Point(6, 6);
			this.previousButton.Name = "previousButton";
			this.previousButton.Size = new System.Drawing.Size(24, 23);
			this.previousButton.TabIndex = 0;
			this.previousButton.Text = "<";
			this.previousButton.UseVisualStyleBackColor = true;
			this.previousButton.Click += new System.EventHandler(this.previousButton_Click);
			// 
			// nextButton
			// 
			this.nextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.nextButton.Location = new System.Drawing.Point(843, 6);
			this.nextButton.Name = "nextButton";
			this.nextButton.Size = new System.Drawing.Size(24, 23);
			this.nextButton.TabIndex = 1;
			this.nextButton.Text = ">";
			this.nextButton.UseVisualStyleBackColor = true;
			this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
			// 
			// totalPages
			// 
			this.totalPages.AutoSize = true;
			this.totalPages.Location = new System.Drawing.Point(432, 11);
			this.totalPages.Name = "totalPages";
			this.totalPages.Size = new System.Drawing.Size(76, 13);
			this.totalPages.TabIndex = 3;
			this.totalPages.Text = "No file opened";
			// 
			// ocrButton
			// 
			this.ocrButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.ocrButton.Location = new System.Drawing.Point(8, 222);
			this.ocrButton.Name = "ocrButton";
			this.ocrButton.Size = new System.Drawing.Size(75, 23);
			this.ocrButton.TabIndex = 4;
			this.ocrButton.Text = "O&CR";
			this.ocrButton.UseVisualStyleBackColor = true;
			this.ocrButton.Click += new System.EventHandler(this.ocrButton_Click);
			// 
			// letterOKButton
			// 
			this.letterOKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.letterOKButton.Location = new System.Drawing.Point(510, 364);
			this.letterOKButton.Name = "letterOKButton";
			this.letterOKButton.Size = new System.Drawing.Size(75, 23);
			this.letterOKButton.TabIndex = 7;
			this.letterOKButton.Text = "OK";
			this.letterOKButton.UseVisualStyleBackColor = true;
			this.letterOKButton.Click += new System.EventHandler(this.letterOKButton_Click);
			// 
			// letterInputBox
			// 
			this.letterInputBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.letterInputBox.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.letterInputBox.Location = new System.Drawing.Point(404, 366);
			this.letterInputBox.Name = "letterInputBox";
			this.letterInputBox.Size = new System.Drawing.Size(100, 20);
			this.letterInputBox.TabIndex = 6;
			// 
			// mainTabControl
			// 
			this.mainTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mainTabControl.Controls.Add(this.imagePage);
			this.mainTabControl.Controls.Add(this.srtPage);
			this.mainTabControl.Location = new System.Drawing.Point(12, 41);
			this.mainTabControl.Name = "mainTabControl";
			this.mainTabControl.SelectedIndex = 0;
			this.mainTabControl.Size = new System.Drawing.Size(881, 432);
			this.mainTabControl.TabIndex = 2;
			this.mainTabControl.Click += new System.EventHandler(this.mainTabControl_Click);
			// 
			// subtitleType
			// 
			this.subtitleType.AutoSize = true;
			this.subtitleType.Location = new System.Drawing.Point(412, 21);
			this.subtitleType.Name = "subtitleType";
			this.subtitleType.Size = new System.Drawing.Size(10, 13);
			this.subtitleType.TabIndex = 3;
			this.subtitleType.Text = "-";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(372, 21);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(34, 13);
			this.label9.TabIndex = 4;
			this.label9.Text = "Type:";
			// 
			// debugButton
			// 
			this.debugButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.debugButton.Location = new System.Drawing.Point(813, 12);
			this.debugButton.Name = "debugButton";
			this.debugButton.Size = new System.Drawing.Size(75, 23);
			this.debugButton.TabIndex = 5;
			this.debugButton.Text = "Debug";
			this.debugButton.UseVisualStyleBackColor = true;
			this.debugButton.Click += new System.EventHandler(this.debugButton_Click);
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(472, 21);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(31, 13);
			this.label10.TabIndex = 4;
			this.label10.Text = "Font:";
			// 
			// fontName
			// 
			this.fontName.AutoSize = true;
			this.fontName.Location = new System.Drawing.Point(509, 21);
			this.fontName.Name = "fontName";
			this.fontName.Size = new System.Drawing.Size(10, 13);
			this.fontName.TabIndex = 6;
			this.fontName.Text = "-";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(900, 478);
			this.Controls.Add(this.fontName);
			this.Controls.Add(this.debugButton);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.subtitleType);
			this.Controls.Add(this.mainTabControl);
			this.Controls.Add(this.autoOCRButton);
			this.Controls.Add(this.loadButton);
			this.HelpButton = true;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(916, 490);
			this.Name = "MainForm";
			this.Text = "SupRip";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.srtPage.ResumeLayout(false);
			this.srtPage.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.imagePage.ResumeLayout(false);
			this.imagePage.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.letterPictureBox)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.subtitlePictureBox)).EndInit();
			this.mainTabControl.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button loadButton;
		private System.Windows.Forms.Button autoOCRButton;
		private System.Windows.Forms.TabControl mainTabControl;
		private System.Windows.Forms.TabPage srtPage;
		private System.Windows.Forms.TabPage imagePage;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.CheckBox combineSubtitles;
		private System.Windows.Forms.CheckBox forcedOnly;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox ptsOffset;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.CheckBox replaceHighCommas;
		private System.Windows.Forms.CheckBox stripFormatting;
		private System.Windows.Forms.CheckBox convertDoubleApostrophes;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label finishedLabel;
		private System.Windows.Forms.Label containingErrorsLabel;
		private System.Windows.Forms.Label unscannedLabel;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox srtTextBox;
		private System.Windows.Forms.Button saveButton;
		private System.Windows.Forms.RichTextBox subtitleTextBox2;
		private System.Windows.Forms.Button startCharacterMap;
		private System.Windows.Forms.TextBox pageNum;
		private System.Windows.Forms.CheckBox autoProgress;
		private System.Windows.Forms.PictureBox letterPictureBox;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox contrast;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Button resetDefaults;
		private System.Windows.Forms.TextBox similarityTolerance;
		private System.Windows.Forms.TextBox charSplitTolerance;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox minimumSpaceCharacterWidthTextBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox subtitlePictureBox;
		private System.Windows.Forms.Button previousButton;
		private System.Windows.Forms.Button nextButton;
		private System.Windows.Forms.Label totalPages;
		private System.Windows.Forms.Button ocrButton;
		private System.Windows.Forms.Button letterOKButton;
		private System.Windows.Forms.TextBox letterInputBox;
		private System.Windows.Forms.Label subtitleType;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Button debugButton;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label fontName;
		private System.Windows.Forms.CheckBox italicLetter;
	}
}