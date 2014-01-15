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
			this.errorText.Text = this.exception.Message +
				Environment.NewLine + Environment.NewLine +
				this.exception.TargetSite +
				Environment.NewLine + Environment.NewLine +
				this.exception.StackTrace;
		}
	}
}
