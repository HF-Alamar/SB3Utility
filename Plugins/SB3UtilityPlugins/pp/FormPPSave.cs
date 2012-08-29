using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SB3Utility
{
	public partial class FormPPSave : Form
	{
		BackgroundWorker worker;

		public FormPPSave(BackgroundWorker worker)
		{
			InitializeComponent();

			this.worker = worker;
			worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
			worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);

			this.Shown += new EventHandler(FormPPSave_Shown);
		}

		void FormPPSave_Shown(object sender, EventArgs e)
		{
			worker.RunWorkerAsync();
		}

		void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			progressBar1.Value = e.ProgressPercentage;
		}

		void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Cancelled)
			{
				this.DialogResult = DialogResult.Cancel;
			}
			if (e.Error != null)
			{
				Utility.ReportException(e.Error);
				this.DialogResult = DialogResult.Cancel;
			}

			Close();
		}
	}
}
