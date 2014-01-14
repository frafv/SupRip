using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace SupRip
{
	public class ProgressForm : Form
	{
		private IContainer components;
		private ProgressBar progressBar;
		private Button cancelButton;
		private Label numLabel;
		private MainForm mainForm;
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}
		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ProgressForm));
			this.progressBar = new ProgressBar();
			this.cancelButton = new Button();
			this.numLabel = new Label();
			base.SuspendLayout();
			this.progressBar.Location = new Point(12, 12);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new Size(365, 23);
			this.progressBar.TabIndex = 0;
			this.cancelButton.DialogResult = DialogResult.Cancel;
			this.cancelButton.Location = new Point(156, 66);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new Size(75, 23);
			this.cancelButton.TabIndex = 1;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new EventHandler(this.cancelButton_Click);
			this.numLabel.AutoSize = true;
			this.numLabel.Location = new Point(177, 38);
			this.numLabel.Name = "numLabel";
			this.numLabel.Size = new Size(35, 13);
			this.numLabel.TabIndex = 2;
			this.numLabel.Text = "label1";
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.CancelButton = this.cancelButton;
			base.ClientSize = new Size(389, 101);
			base.ControlBox = false;
			base.Controls.Add(this.numLabel);
			base.Controls.Add(this.cancelButton);
			base.Controls.Add(this.progressBar);
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ProgressForm";
			this.Text = "Scanning...";
			base.Load += new EventHandler(this.ProgressForm_Load);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
		public ProgressForm(MainForm m, int numSubtitles)
		{
			this.mainForm = m;
			this.InitializeComponent();
			this.progressBar.Maximum = numSubtitles;
		}
		public void SetProgressBarPosition(int p)
		{
			this.progressBar.Value = p;
			this.numLabel.Text = p.ToString() + " / " + this.progressBar.Maximum;
		}
		private void ProgressForm_Load(object sender, EventArgs e)
		{
		}
		private void cancelButton_Click(object sender, EventArgs e)
		{
			this.mainForm.CancelThread();
		}
	}
}
