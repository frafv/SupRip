using System;
using System.Windows.Forms;

namespace SupRip
{
	public partial class ProgressForm : Form
	{
		private MainForm mainForm;
		public ProgressForm(MainForm m, int numSubtitles)
		{
			this.mainForm = m;
			this.InitializeComponent();
			this.progressBar.Maximum = numSubtitles;
		}
		public void SetProgressBarPosition(int p)
		{
			this.progressBar.Value = p;
			this.numLabel.Text = p + " / " + this.progressBar.Maximum;
		}
		private void cancelButton_Click(object sender, EventArgs e)
		{
			this.mainForm.CancelThread();
		}
	}
}
