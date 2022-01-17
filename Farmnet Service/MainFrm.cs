using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Farmnet_Service
{
    public partial class MainFrm : Form
    {
        Process process;
        public MainFrm()
        {
            InitializeComponent();
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            inputJSFile.Text = Properties.Settings.Default.JSFILE;
        }

        private void LogDsp_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private void MainFrm_FormClosed(object sender, FormClosedEventArgs e)
        {
            notify.Visible = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "You want to close Framnet Service?", "Confirm!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                Application.Exit();
            }
        }

        private void MainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show(this, "You want to close Framnet Service?", "Confirm!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                Application.Exit();
            }
            else
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void configToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void MainFrm_Resize(object sender, EventArgs e)
        {
            switch (this.WindowState)
            {
                case FormWindowState.Minimized:
                    this.ShowInTaskbar = false;
                    this.Hide();
                    notify.Visible = true;
                    notify.ShowBalloonTip(2000, "Farmnet Service", "Farmnet Service is running in background", ToolTipIcon.Info);
                    break;
                case FormWindowState.Normal:
                    notify.Visible = false;
                    this.ShowInTaskbar = true;
                    break;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(inputJSFile.Text))
                {
                    if (process == null)
                        process = new Process()
                        {
                            StartInfo = new ProcessStartInfo()
                            {
                                FileName = "node.exe",
                                Arguments = inputJSFile.Text,
                                WindowStyle = ProcessWindowStyle.Hidden,
                                RedirectStandardOutput = true,
                                CreateNoWindow = true,
                                UseShellExecute=false
                            }
                        };
                    process.Exited += Process_Exited;
                    process.OutputDataReceived += Process_OutputDataReceived;
                    process.Start();
                    process.BeginOutputReadLine();
                    btnStop.Enabled = true;
                    btnStart.Enabled = false;
                }
                else
                {
                    MessageBox.Show(this, "JS File not exists!", "Farmnet Service", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Farmnet Service", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (sender != process) return;
            Console.Write(e.Data);
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            process = null;
            process.Dispose();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if ((openFileDialog.ShowDialog()) == DialogResult.OK)
            {
                inputJSFile.Text = openFileDialog.FileName;
                Properties.Settings.Default.JSFILE = inputJSFile.Text;
                Properties.Settings.Default.Save();
            }
        }

        private void btnLog_Click(object sender, EventArgs e)
        {
            //if (logDsp.Visible)
            //{
            //    logDsp.Focus();
            //}
            //else
            //{
            //    logDsp.Show();
            //}
            process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (process != null)
            {
                //process.close();
                process.Kill();
                process.Dispose();
                process = null;
                btnStart.Enabled = true;
                btnStop.Enabled = false;
            }
        }
    }
}
