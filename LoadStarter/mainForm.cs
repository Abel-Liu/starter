using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace LoadStarter
{
    public partial class mainForm : Form
    {
        public mainForm()
        {
            InitializeComponent();
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            string file = "Starter.exe";
            string path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, file);
            if (File.Exists(path))
            {
                var process = Process.GetProcessesByName("Starter");
                if (process != null && process.Count() > 0)
                {
                    Process p = process[0];
                    p.Kill();
                }
                Process.Start(path);
            }
            else
            {
                MessageBox.Show("无法启动程序" + file + "，请确保文件名正确！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            this.Close();
        }
    }
}
