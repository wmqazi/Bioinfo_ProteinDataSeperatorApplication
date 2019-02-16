using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Qazi.GUI.CommonDialogs;
using Qazi.DataPreprocessing;
using Qazi.Common;

namespace ProteinDataSeperatorApplication
{
    public partial class ProteinSeperationConsole : Form
    {
        private DataTable _SourceDataTable;
        private DataTable _ProteinDataTable;
        private ProteinDataSeperator _ProteinDataSeperator;
        public ProteinSeperationConsole()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(_SourceDataTable != null)
            {
                _SourceDataTable.Dispose();
                _SourceDataTable = null;
            }
            if(_ProteinDataTable != null)
            {
                _ProteinDataTable.Dispose();
                _ProteinDataTable = null;
            }
            if (_ProteinDataSeperator != null)
            {
                _ProteinDataSeperator.Dispose();
                _ProteinDataSeperator = null;
            }
            openDlg.ShowDialog(this);
        }

        private void openDlg_FileOk(object sender, CancelEventArgs e)
        {
            //0331-7017771
            //arslan:
                //0321-4244643
            DataSet ds = new DataSet();
            ds.ReadXml(openDlg.FileName);
            DataTableSelectorWnd dts = new DataTableSelectorWnd("Select Source DataTable", ds);
            dts.ShowDialog(this);
            _SourceDataTable = ds.Tables[dts.TableName];
            gridSites.DataSource = _SourceDataTable;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveDlg.ShowDialog(this);
        }

        private void saveDlg_FileOk(object sender, CancelEventArgs e)
        {
            _ProteinDataTable.WriteXml(saveDlg.FileName);
        }

        private void proteinSeperationProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            _ProteinDataSeperator = new ProteinDataSeperator(_SourceDataTable, "PID", "Sequence", "Position");
            _ProteinDataSeperator.SeperationCompleted += new WorkerCompletedEventHandler(_ProteinDataSeperator_SeperationCompleted);
            _ProteinDataSeperator.SeperationProgressUpdate += new WorkerProgressUpdateEventHandler(_ProteinDataSeperator_SeperationProgressUpdate);
            _ProteinDataSeperator.SeperationStarted += new WorkerStartedEventHandler(_ProteinDataSeperator_SeperationStarted);
            lblTotalSourceRecords.Text = _SourceDataTable.Rows.Count.ToString();
            _ProteinDataSeperator.Run();
            _ProteinDataTable = _ProteinDataSeperator.ProteinDataTable;
            gridProteins.DataSource = _ProteinDataTable;
            lblProteinCount.Text = _ProteinDataTable.Rows.Count.ToString();
        }

        void _ProteinDataSeperator_SeperationStarted(object sender)
        {
            lblStatus.Text = "Started";
            Application.DoEvents();
        }


        void _ProteinDataSeperator_SeperationProgressUpdate(object sender, WorkerProgressEventArg e)
        {
            progressbar.Value = (int)e.ProgressPercentage;
            lblStatus.Text = e.UserState;// +": " + e.ProgressPercentage.ToString();
            Application.DoEvents();
        }

        void _ProteinDataSeperator_SeperationCompleted(object sender, WorkerCompletedEventArg e)
        {
            lblStatus.Text = "Done";
            Application.DoEvents();
        }
    }
}