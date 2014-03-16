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
			this.components = new System.ComponentModel.Container();
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
			this.debugLabel = new System.Windows.Forms.Label();
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
			this.fontPage = new System.Windows.Forms.TabPage();
			this.pictureBoxLetter = new System.Windows.Forms.PictureBox();
			this.listLetters = new System.Windows.Forms.ListView();
			this.columnLetter = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnUnicode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.cbFonts = new System.Windows.Forms.ComboBox();
			this.subtitleType = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.debugButton = new System.Windows.Forms.Button();
			this.label10 = new System.Windows.Forms.Label();
			this.fontName = new System.Windows.Forms.Label();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.dbgSpace = new System.Windows.Forms.CheckBox();
			this.dbgEdges = new System.Windows.Forms.CheckBox();
			this.srtPage.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.imagePage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.letterPictureBox)).BeginInit();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.subtitlePictureBox)).BeginInit();
			this.mainTabControl.SuspendLayout();
			this.fontPage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxLetter)).BeginInit();
			this.SuspendLayout();
			// 
			// loadButton
			// 
			resources.ApplyResources(this.loadButton, "loadButton");
			this.loadButton.Name = "loadButton";
			this.toolTip.SetToolTip(this.loadButton, resources.GetString("loadButton.ToolTip"));
			this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
			// 
			// autoOCRButton
			// 
			resources.ApplyResources(this.autoOCRButton, "autoOCRButton");
			this.autoOCRButton.Name = "autoOCRButton";
			this.toolTip.SetToolTip(this.autoOCRButton, resources.GetString("autoOCRButton.ToolTip"));
			this.autoOCRButton.Click += new System.EventHandler(this.autoOCRButton_Click);
			// 
			// srtPage
			// 
			this.srtPage.Controls.Add(this.groupBox3);
			this.srtPage.Controls.Add(this.groupBox2);
			this.srtPage.Controls.Add(this.srtTextBox);
			this.srtPage.Controls.Add(this.saveButton);
			resources.ApplyResources(this.srtPage, "srtPage");
			this.srtPage.Name = "srtPage";
			// 
			// groupBox3
			// 
			resources.ApplyResources(this.groupBox3, "groupBox3");
			this.groupBox3.Controls.Add(this.combineSubtitles);
			this.groupBox3.Controls.Add(this.forcedOnly);
			this.groupBox3.Controls.Add(this.label8);
			this.groupBox3.Controls.Add(this.ptsOffset);
			this.groupBox3.Controls.Add(this.label7);
			this.groupBox3.Controls.Add(this.replaceHighCommas);
			this.groupBox3.Controls.Add(this.stripFormatting);
			this.groupBox3.Controls.Add(this.convertDoubleApostrophes);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.TabStop = false;
			// 
			// combineSubtitles
			// 
			resources.ApplyResources(this.combineSubtitles, "combineSubtitles");
			this.combineSubtitles.Name = "combineSubtitles";
			this.toolTip.SetToolTip(this.combineSubtitles, resources.GetString("combineSubtitles.ToolTip"));
			this.combineSubtitles.CheckedChanged += new System.EventHandler(this.combineSubtitles_CheckedChanged);
			// 
			// forcedOnly
			// 
			resources.ApplyResources(this.forcedOnly, "forcedOnly");
			this.forcedOnly.Name = "forcedOnly";
			this.toolTip.SetToolTip(this.forcedOnly, resources.GetString("forcedOnly.ToolTip"));
			this.forcedOnly.CheckedChanged += new System.EventHandler(this.forcedOnly_CheckedChanged);
			// 
			// label8
			// 
			resources.ApplyResources(this.label8, "label8");
			this.label8.Name = "label8";
			// 
			// ptsOffset
			// 
			resources.ApplyResources(this.ptsOffset, "ptsOffset");
			this.ptsOffset.Name = "ptsOffset";
			this.toolTip.SetToolTip(this.ptsOffset, resources.GetString("ptsOffset.ToolTip"));
			this.ptsOffset.TextChanged += new System.EventHandler(this.ptsOffset_TextChanged);
			// 
			// label7
			// 
			resources.ApplyResources(this.label7, "label7");
			this.label7.Name = "label7";
			// 
			// replaceHighCommas
			// 
			resources.ApplyResources(this.replaceHighCommas, "replaceHighCommas");
			this.replaceHighCommas.Name = "replaceHighCommas";
			this.toolTip.SetToolTip(this.replaceHighCommas, resources.GetString("replaceHighCommas.ToolTip"));
			this.replaceHighCommas.CheckedChanged += new System.EventHandler(this.replaceHighCommas_CheckedChanged);
			// 
			// stripFormatting
			// 
			resources.ApplyResources(this.stripFormatting, "stripFormatting");
			this.stripFormatting.Name = "stripFormatting";
			this.stripFormatting.CheckedChanged += new System.EventHandler(this.stripFormatting_CheckedChanged);
			// 
			// convertDoubleApostrophes
			// 
			resources.ApplyResources(this.convertDoubleApostrophes, "convertDoubleApostrophes");
			this.convertDoubleApostrophes.Name = "convertDoubleApostrophes";
			this.toolTip.SetToolTip(this.convertDoubleApostrophes, resources.GetString("convertDoubleApostrophes.ToolTip"));
			this.convertDoubleApostrophes.CheckedChanged += new System.EventHandler(this.convertDoubleApostrophes_CheckedChanged);
			// 
			// groupBox2
			// 
			resources.ApplyResources(this.groupBox2, "groupBox2");
			this.groupBox2.Controls.Add(this.finishedLabel);
			this.groupBox2.Controls.Add(this.containingErrorsLabel);
			this.groupBox2.Controls.Add(this.unscannedLabel);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.TabStop = false;
			// 
			// finishedLabel
			// 
			resources.ApplyResources(this.finishedLabel, "finishedLabel");
			this.finishedLabel.Name = "finishedLabel";
			// 
			// containingErrorsLabel
			// 
			resources.ApplyResources(this.containingErrorsLabel, "containingErrorsLabel");
			this.containingErrorsLabel.Name = "containingErrorsLabel";
			// 
			// unscannedLabel
			// 
			resources.ApplyResources(this.unscannedLabel, "unscannedLabel");
			this.unscannedLabel.Name = "unscannedLabel";
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// srtTextBox
			// 
			resources.ApplyResources(this.srtTextBox, "srtTextBox");
			this.srtTextBox.Name = "srtTextBox";
			// 
			// saveButton
			// 
			resources.ApplyResources(this.saveButton, "saveButton");
			this.saveButton.Name = "saveButton";
			this.toolTip.SetToolTip(this.saveButton, resources.GetString("saveButton.ToolTip"));
			this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
			// 
			// imagePage
			// 
			this.imagePage.Controls.Add(this.debugLabel);
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
			resources.ApplyResources(this.imagePage, "imagePage");
			this.imagePage.Name = "imagePage";
			this.imagePage.Paint += new System.Windows.Forms.PaintEventHandler(this.imagePage_Paint);
			// 
			// debugLabel
			// 
			resources.ApplyResources(this.debugLabel, "debugLabel");
			this.debugLabel.Name = "debugLabel";
			// 
			// italicLetter
			// 
			resources.ApplyResources(this.italicLetter, "italicLetter");
			this.italicLetter.Name = "italicLetter";
			// 
			// subtitleTextBox2
			// 
			resources.ApplyResources(this.subtitleTextBox2, "subtitleTextBox2");
			this.subtitleTextBox2.Name = "subtitleTextBox2";
			this.subtitleTextBox2.ReadOnly = true;
			// 
			// startCharacterMap
			// 
			resources.ApplyResources(this.startCharacterMap, "startCharacterMap");
			this.startCharacterMap.Name = "startCharacterMap";
			this.startCharacterMap.Click += new System.EventHandler(this.startCharacterMap_Click);
			// 
			// pageNum
			// 
			resources.ApplyResources(this.pageNum, "pageNum");
			this.pageNum.Name = "pageNum";
			this.pageNum.TextChanged += new System.EventHandler(this.pageNum_TextChanged);
			// 
			// autoProgress
			// 
			resources.ApplyResources(this.autoProgress, "autoProgress");
			this.autoProgress.Checked = true;
			this.autoProgress.CheckState = System.Windows.Forms.CheckState.Checked;
			this.autoProgress.Name = "autoProgress";
			this.toolTip.SetToolTip(this.autoProgress, resources.GetString("autoProgress.ToolTip"));
			// 
			// letterPictureBox
			// 
			resources.ApplyResources(this.letterPictureBox, "letterPictureBox");
			this.letterPictureBox.Name = "letterPictureBox";
			this.letterPictureBox.TabStop = false;
			// 
			// groupBox1
			// 
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Controls.Add(this.contrast);
			this.groupBox1.Controls.Add(this.label11);
			this.groupBox1.Controls.Add(this.resetDefaults);
			this.groupBox1.Controls.Add(this.similarityTolerance);
			this.groupBox1.Controls.Add(this.charSplitTolerance);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.minimumSpaceCharacterWidthTextBox);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// contrast
			// 
			resources.ApplyResources(this.contrast, "contrast");
			this.contrast.Name = "contrast";
			this.toolTip.SetToolTip(this.contrast, resources.GetString("contrast.ToolTip"));
			this.contrast.TextChanged += new System.EventHandler(this.options_TextChanged);
			// 
			// label11
			// 
			resources.ApplyResources(this.label11, "label11");
			this.label11.Name = "label11";
			// 
			// resetDefaults
			// 
			resources.ApplyResources(this.resetDefaults, "resetDefaults");
			this.resetDefaults.Name = "resetDefaults";
			this.resetDefaults.Click += new System.EventHandler(this.resetDefaults_Click);
			// 
			// similarityTolerance
			// 
			resources.ApplyResources(this.similarityTolerance, "similarityTolerance");
			this.similarityTolerance.Name = "similarityTolerance";
			this.toolTip.SetToolTip(this.similarityTolerance, resources.GetString("similarityTolerance.ToolTip"));
			this.similarityTolerance.TextChanged += new System.EventHandler(this.options_TextChanged);
			// 
			// charSplitTolerance
			// 
			resources.ApplyResources(this.charSplitTolerance, "charSplitTolerance");
			this.charSplitTolerance.Name = "charSplitTolerance";
			this.toolTip.SetToolTip(this.charSplitTolerance, resources.GetString("charSplitTolerance.ToolTip"));
			this.charSplitTolerance.TextChanged += new System.EventHandler(this.options_TextChanged);
			// 
			// label6
			// 
			resources.ApplyResources(this.label6, "label6");
			this.label6.Name = "label6";
			// 
			// minimumSpaceCharacterWidthTextBox
			// 
			resources.ApplyResources(this.minimumSpaceCharacterWidthTextBox, "minimumSpaceCharacterWidthTextBox");
			this.minimumSpaceCharacterWidthTextBox.Name = "minimumSpaceCharacterWidthTextBox";
			this.toolTip.SetToolTip(this.minimumSpaceCharacterWidthTextBox, resources.GetString("minimumSpaceCharacterWidthTextBox.ToolTip"));
			this.minimumSpaceCharacterWidthTextBox.TextChanged += new System.EventHandler(this.options_TextChanged);
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.Name = "label5";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// subtitlePictureBox
			// 
			resources.ApplyResources(this.subtitlePictureBox, "subtitlePictureBox");
			this.subtitlePictureBox.Name = "subtitlePictureBox";
			this.subtitlePictureBox.TabStop = false;
			this.subtitlePictureBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.subtitlePictureBox_MouseClick);
			// 
			// previousButton
			// 
			resources.ApplyResources(this.previousButton, "previousButton");
			this.previousButton.Name = "previousButton";
			this.toolTip.SetToolTip(this.previousButton, resources.GetString("previousButton.ToolTip"));
			this.previousButton.Click += new System.EventHandler(this.previousButton_Click);
			// 
			// nextButton
			// 
			resources.ApplyResources(this.nextButton, "nextButton");
			this.nextButton.Name = "nextButton";
			this.toolTip.SetToolTip(this.nextButton, resources.GetString("nextButton.ToolTip"));
			this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
			// 
			// totalPages
			// 
			resources.ApplyResources(this.totalPages, "totalPages");
			this.totalPages.Name = "totalPages";
			// 
			// ocrButton
			// 
			resources.ApplyResources(this.ocrButton, "ocrButton");
			this.ocrButton.Name = "ocrButton";
			this.toolTip.SetToolTip(this.ocrButton, resources.GetString("ocrButton.ToolTip"));
			this.ocrButton.Click += new System.EventHandler(this.ocrButton_Click);
			// 
			// letterOKButton
			// 
			resources.ApplyResources(this.letterOKButton, "letterOKButton");
			this.letterOKButton.Name = "letterOKButton";
			this.letterOKButton.Click += new System.EventHandler(this.letterOKButton_Click);
			// 
			// letterInputBox
			// 
			resources.ApplyResources(this.letterInputBox, "letterInputBox");
			this.letterInputBox.Name = "letterInputBox";
			// 
			// mainTabControl
			// 
			resources.ApplyResources(this.mainTabControl, "mainTabControl");
			this.mainTabControl.Controls.Add(this.imagePage);
			this.mainTabControl.Controls.Add(this.srtPage);
			this.mainTabControl.Controls.Add(this.fontPage);
			this.mainTabControl.Name = "mainTabControl";
			this.mainTabControl.SelectedIndex = 0;
			this.mainTabControl.Selected += new System.Windows.Forms.TabControlEventHandler(this.mainTabControl_Selected);
			this.mainTabControl.Click += new System.EventHandler(this.mainTabControl_Click);
			// 
			// fontPage
			// 
			this.fontPage.Controls.Add(this.pictureBoxLetter);
			this.fontPage.Controls.Add(this.listLetters);
			this.fontPage.Controls.Add(this.cbFonts);
			resources.ApplyResources(this.fontPage, "fontPage");
			this.fontPage.Name = "fontPage";
			// 
			// pictureBoxLetter
			// 
			resources.ApplyResources(this.pictureBoxLetter, "pictureBoxLetter");
			this.pictureBoxLetter.Name = "pictureBoxLetter";
			this.pictureBoxLetter.TabStop = false;
			// 
			// listLetters
			// 
			resources.ApplyResources(this.listLetters, "listLetters");
			this.listLetters.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnLetter,
            this.columnSize,
            this.columnUnicode});
			this.listLetters.FullRowSelect = true;
			this.listLetters.Name = "listLetters";
			this.listLetters.UseCompatibleStateImageBehavior = false;
			this.listLetters.View = System.Windows.Forms.View.Details;
			this.listLetters.SelectedIndexChanged += new System.EventHandler(this.listLetters_SelectedIndexChanged);
			// 
			// columnLetter
			// 
			resources.ApplyResources(this.columnLetter, "columnLetter");
			// 
			// columnSize
			// 
			resources.ApplyResources(this.columnSize, "columnSize");
			// 
			// columnUnicode
			// 
			resources.ApplyResources(this.columnUnicode, "columnUnicode");
			// 
			// cbFonts
			// 
			this.cbFonts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbFonts.FormattingEnabled = true;
			resources.ApplyResources(this.cbFonts, "cbFonts");
			this.cbFonts.Name = "cbFonts";
			this.cbFonts.SelectedIndexChanged += new System.EventHandler(this.cbFonts_SelectedIndexChanged);
			// 
			// subtitleType
			// 
			resources.ApplyResources(this.subtitleType, "subtitleType");
			this.subtitleType.Name = "subtitleType";
			// 
			// label9
			// 
			resources.ApplyResources(this.label9, "label9");
			this.label9.Name = "label9";
			// 
			// debugButton
			// 
			resources.ApplyResources(this.debugButton, "debugButton");
			this.debugButton.Name = "debugButton";
			this.debugButton.Click += new System.EventHandler(this.debugButton_Click);
			// 
			// label10
			// 
			resources.ApplyResources(this.label10, "label10");
			this.label10.Name = "label10";
			// 
			// fontName
			// 
			resources.ApplyResources(this.fontName, "fontName");
			this.fontName.Name = "fontName";
			// 
			// toolTip
			// 
			this.toolTip.AutoPopDelay = 20000;
			this.toolTip.InitialDelay = 500;
			this.toolTip.ReshowDelay = 100;
			// 
			// dbgSpace
			// 
			resources.ApplyResources(this.dbgSpace, "dbgSpace");
			this.dbgSpace.Name = "dbgSpace";
			this.dbgSpace.UseVisualStyleBackColor = true;
			this.dbgSpace.CheckedChanged += new System.EventHandler(this.dbgSpace_CheckedChanged);
			// 
			// dbgEdges
			// 
			resources.ApplyResources(this.dbgEdges, "dbgEdges");
			this.dbgEdges.Name = "dbgEdges";
			this.dbgEdges.UseVisualStyleBackColor = true;
			this.dbgEdges.CheckedChanged += new System.EventHandler(this.dbgSpace_CheckedChanged);
			// 
			// MainForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.dbgEdges);
			this.Controls.Add(this.dbgSpace);
			this.Controls.Add(this.fontName);
			this.Controls.Add(this.debugButton);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.subtitleType);
			this.Controls.Add(this.mainTabControl);
			this.Controls.Add(this.autoOCRButton);
			this.Controls.Add(this.loadButton);
			this.HelpButton = true;
			this.Name = "MainForm";
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
			this.fontPage.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxLetter)).EndInit();
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
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.TabPage fontPage;
		private System.Windows.Forms.ComboBox cbFonts;
		private System.Windows.Forms.PictureBox pictureBoxLetter;
		private System.Windows.Forms.ListView listLetters;
		private System.Windows.Forms.ColumnHeader columnLetter;
		private System.Windows.Forms.ColumnHeader columnSize;
		private System.Windows.Forms.ColumnHeader columnUnicode;
		private System.Windows.Forms.CheckBox dbgSpace;
		private System.Windows.Forms.CheckBox dbgEdges;
		private System.Windows.Forms.Label debugLabel;
	}
}