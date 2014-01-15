using System;
using System.Windows.Forms;

namespace SupRip
{
	public partial class ErrorForm : Form
	{
		private Exception exception;
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
	}
}
