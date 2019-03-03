using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ESTool;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using ESTool.Position;

namespace Starter
{
    public partial class StartButton : Form
    {
        MainWindow mainwindow;
        public static PositionBase positionSvc;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == API.WM_WININICHANGE)
            {
                positionSvc.SetAppPos();
            }

            base.WndProc(ref m);
        }

        public StartButton()
        {
            InitializeComponent();

            mainwindow = new MainWindow(6000);
            mainwindow.Show();

            InitPositionSvc();
        }

        void InitPositionSvc()
        {
            Version version = Environment.OSVersion.Version;
            if (version.Major == 6 && (version.Minor == 0 || version.Minor == 1))
                positionSvc = new PositionSvcWin7(this.Handle, mainwindow);
            else if (version.Major == 6 && version.Minor == 2)
                positionSvc = new PositionSvcWin10(this.Handle, mainwindow);
            else
                positionSvc = new PositionBase(this.Handle, mainwindow);
        }

        private void StartButton_Load(object sender, EventArgs e)
        {
            positionSvc.InsertButton();

            API.RegisterHotKey(new System.Windows.Interop.WindowInteropHelper(mainwindow).Handle, 100, KeyModifiers.Alt, System.Windows.Forms.Keys.Q);
            //API.RegisterHotKey(new System.Windows.Interop.WindowInteropHelper(mainwindow).Handle, 101, KeyModifiers.Ctrl, System.Windows.Forms.Keys.Left);
            //API.RegisterHotKey(new System.Windows.Interop.WindowInteropHelper(mainwindow).Handle, 102, KeyModifiers.Ctrl, System.Windows.Forms.Keys.Right);
            //API.RegisterHotKey(new System.Windows.Interop.WindowInteropHelper(mainwindow).Handle, 105, KeyModifiers.Alt, System.Windows.Forms.Keys.R);
            API.RegisterHotKey(new System.Windows.Interop.WindowInteropHelper(mainwindow).Handle, 106, KeyModifiers.Alt, System.Windows.Forms.Keys.A);

            try
            {
                positionSvc.InjectDll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void StartButton_FormClosing(object sender, FormClosingEventArgs e)
        {
            API.UnregisterHotKey(new System.Windows.Interop.WindowInteropHelper(mainwindow).Handle, 100);
            //API.UnregisterHotKey(new System.Windows.Interop.WindowInteropHelper(mainwindow).Handle, 105);
            API.UnregisterHotKey(new System.Windows.Interop.WindowInteropHelper(mainwindow).Handle, 106);

            positionSvc.UnInjectDll();
            positionSvc.RemoveButton();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mainwindow.Show();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainWindow.closeFlag = "Exit";
            System.Windows.Application.Current.Shutdown();
            System.Windows.Forms.Application.Exit();
        }

        private void button1_DragOver(object sender, DragEventArgs e)
        {
            mainwindow.Show();
        }

        private void 重置位置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            positionSvc.SetAppPos();
        }
    }
}
