using System;
using System.Windows.Forms;
using Windows7.DesktopIntegration.WindowsForms;

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
			this.progressBar.SetTaskbarProgress();
		}
		public void SetProgressBarPosition(int p)
		{
			this.progressBar.Value = p;
			this.numLabel.Text = p + " / " + this.progressBar.Maximum;
			this.progressBar.SetTaskbarProgress();
		}
		private void cancelButton_Click(object sender, EventArgs e)
		{
			this.mainForm.CancelThread();
		}

		private void ProgressForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.progressBar.ResetTaskbarProgress();
		}
	}
}
