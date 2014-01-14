using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace SupRip
{
	public class ErrorForm : Form
	{
		private Exception exception;
		private IContainer components;
		private Button okButton;
		private TextBox errorText;
		public ErrorForm()
		{
			this.InitializeComponent();
		}
		public ErrorForm(Exception e)
		{
			this.InitializeComponent();
			this.exception = e;
		}
		private void ErrorForm_Load(object sender, EventArgs e)
		{
			this.errorText.Text = this.exception.Message;
			TextBox expr_1C = this.errorText;
			expr_1C.Text += "\r\n\r\n";
			TextBox expr_37 = this.errorText;
			expr_37.Text += this.exception.TargetSite;
			TextBox expr_58 = this.errorText;
			expr_58.Text += "\r\n\r\n";
			TextBox expr_73 = this.errorText;
			expr_73.Text += this.exception.StackTrace;
		}
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
			this.okButton = new Button();
			this.errorText = new TextBox();
			base.SuspendLayout();
			this.okButton.DialogResult = DialogResult.Cancel;
			this.okButton.Location = new Point(396, 318);
			this.okButton.Name = "okButton";
			this.okButton.Size = new Size(75, 23);
			this.okButton.TabIndex = 0;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.errorText.Location = new Point(13, 13);
			this.errorText.Multiline = true;
			this.errorText.Name = "errorText";
			this.errorText.ReadOnly = true;
			this.errorText.ScrollBars = ScrollBars.Both;
			this.errorText.Size = new Size(458, 299);
			this.errorText.TabIndex = 1;
			base.AcceptButton = this.okButton;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(483, 353);
			base.ControlBox = false;
			base.Controls.Add(this.errorText);
			base.Controls.Add(this.okButton);
			base.Name = "ErrorForm";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			this.Text = "Critical Error";
			base.Load += new EventHandler(this.ErrorForm_Load);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
