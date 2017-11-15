using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FileTransferProtocols.Interfaces;

namespace CBMTerm2.Forms
{
    public partial class FileTransferDialog : Form, IStatusRecipient
    {
        int TotalBytesInFile = 0;
        bool xferDone = false;
        Action Killxfer;
        public FileTransferDialog()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (xferDone)
            {
                //this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            else
            {
                Killxfer();
                Done(false);
                Application.DoEvents();

            }
        }

        public delegate void UpdateHandler(int bytestransferred);
        public delegate void InitializeHandler(int totalbytesinfile, string filename, bool sending, string caption, Action xferAbort);
        public delegate void AbortHandler();
        public delegate void DoneHandler(bool good);
        public delegate void MicroUpdateHandler(string text);

        public void Update(int bytestransferred)
        {
            if (this.label1.InvokeRequired)
            {
                object[] parametersArray = new object[] { bytestransferred };
                this.Invoke(new UpdateHandler(Update), parametersArray);
            }
            else
            {
                this.label2.Text = bytestransferred.ToString() + " bytes of " + (TotalBytesInFile > 0 ? TotalBytesInFile.ToString() : "unknown");
                progressBar1.Minimum = 0;
                progressBar1.Maximum = TotalBytesInFile;
                progressBar1.Value = bytestransferred;

                Application.DoEvents();
            }
        }

        public void Abort()
        {
            if (this.label1.InvokeRequired)
            {
                this.Invoke(new AbortHandler(Abort), null);
            }
            else
            {
                Done(false);
                Application.DoEvents();

            }

        }

        public void Initialize(int totalbytesinfile, string filename, bool sending, string caption, Action xferAbort)
        {
            if (this.label1.InvokeRequired)
            {
                object[] parametersArray = new object[] { totalbytesinfile,filename,sending,caption,xferAbort };
                this.Invoke(new InitializeHandler(Initialize), parametersArray);
            }
            else
            {
                Killxfer = xferAbort;
                TotalBytesInFile = totalbytesinfile;
                this.Text = caption;
                this.label1.Text = ((sending) ? "Sending" : "Receiving") + ((filename == "") ? " file." : (" " + filename));
                this.label2.Text = "0 bytes of " + (totalbytesinfile > 0 ? totalbytesinfile.ToString() : "unknown");
                this.label3.Text = "";
                this.progressBar1.Visible = (totalbytesinfile > 0);
                this.progressBar1.Maximum = (totalbytesinfile > 0) ? totalbytesinfile : 100000;
                this.Show();
                Application.DoEvents();
            }
        }


        public void MicroUpdate(string text)
        {
            if (this.label3.InvokeRequired)
            {
                object[] parametersArray = new object[] { text };
                this.Invoke(new MicroUpdateHandler(MicroUpdate), parametersArray);
            }
            else
            {

                label3.Text = text;
                Application.DoEvents();

            }
        }

        public void Done(bool good)
        {
            if (this.label2.InvokeRequired)
            {
                object[] parametersArray = new object[] { good };
                this.Invoke(new DoneHandler(Done), parametersArray);
            }
            else
            {
                xferDone = true;
                if (good) progressBar1.Value = progressBar1.Maximum;
                label2.Text = "";
                label3.Text = (good)?"Transfer completed successfully.": "Transfer failed.";
                Application.DoEvents();

            }
        }
    }
}
