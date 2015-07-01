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

namespace Starter
{
    public partial class StartButton : Form
    {
        MainWindow mainwindow;

        /// <summary>
        /// explorer process
        /// </summary>
        Process process;


        protected override void WndProc(ref Message m)
        {
            if (m.Msg == API.WM_WININICHANGE)
            {
                SetAppPos(false);
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// 设置主窗体和主按钮的位置
        /// </summary>
        public void SetAppPos(bool moveRebar = true)
        {
            AppPos pos;
            reBarInfo rbInfo = API.CalcAppPos(mainwindow.Height, mainwindow.Width, out pos);

            if (moveRebar)
                API.MoveWindow(rbInfo.hreBar, rbInfo.x, rbInfo.y, rbInfo.width, rbInfo.height, true);
            API.MoveWindow(this.Handle, pos.btn_x, pos.btn_y, pos.btn_size, pos.btn_size, true);

            mainwindow.Top = pos.main_top;
            mainwindow.Left = pos.main_left;
        }

        public StartButton()
        {
            InitializeComponent();
        }

        readonly string dllname = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ESRemote.dll");

        private void InjectDll()
        {
            const int MEM_COMMIT = 0x00001000;
            const int MEM_RELEASE = 0x8000;
            const int PAGE_READWRITE = 0x04;
            int temp = 0;

            int dlllength = System.Text.Encoding.Default.GetByteCount(dllname);
            var loadAddr = API.GetProcAddress(API.GetModuleHandleA("Kernel32"), "LoadLibraryA");

            var ps = Process.GetProcessesByName("explorer");

            if (ps != null && ps.Length > 0)
            {
                process = ps[0];
                int baseaddress = API.VirtualAllocEx(process.Handle, IntPtr.Zero, dlllength, MEM_COMMIT, PAGE_READWRITE);
                if (baseaddress == 0)
                    throw new Exception("申请内存空间失败！");

                if (API.WriteProcessMemory(process.Handle, baseaddress, dllname, dlllength, temp) == 0)
                    throw new Exception("写内存失败！");

                var hThread = API.CreateRemoteThread(process.Handle, IntPtr.Zero, 0, new IntPtr(loadAddr), new IntPtr(baseaddress), 0, 0);
                if (hThread == null)
                {
                    API.VirtualFreeEx(process.Handle, new IntPtr(baseaddress), dlllength, MEM_RELEASE);
                    throw new Exception("创建远程线程失败！");
                }
            }
        }

        private void UnInject()
        {
            if (process != null)
            {
                MODULEENTRY32 stModuleEntry = new MODULEENTRY32();
                bool bFlag = true;
                IntPtr hFindModule = IntPtr.Zero;

                stModuleEntry.dwSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(stModuleEntry);

                var hModuleSnap = API.CreateToolhelp32Snapshot(0x00000008, process.Id);
                bFlag = API.Module32First(hModuleSnap, ref stModuleEntry);
                for (; bFlag; )
                {
                    if (stModuleEntry.szExePath.ToLower() == dllname.ToLower())
                    {
                        hFindModule = stModuleEntry.hModule;
                        break;
                    }
                    bFlag = API.Module32Next(hModuleSnap, ref stModuleEntry);
                }

                var freeAddr = API.GetProcAddress(API.GetModuleHandleA("Kernel32"), "FreeLibrary");
                API.CreateRemoteThread(process.Handle, IntPtr.Zero, 0, new IntPtr(freeAddr), hFindModule, 0, 0);
            }
        }

        private void RevertTaskBar()
        {
            reBarInfo rbInfo = API.CalcRebarPos();
            API.MoveWindow(rbInfo.hreBar, rbInfo.x, rbInfo.y, rbInfo.width, rbInfo.height, true);
        }

        private void StartButton_Load(object sender, EventArgs e)
        {
            mainwindow = new MainWindow(6000);
            mainwindow.Show();

            SetAppPos();
            API.SetParent(this.Handle, API.FindWindow("Shell_TrayWnd", null));
            API.RegisterHotKey(new System.Windows.Interop.WindowInteropHelper(mainwindow).Handle, 100, KeyModifiers.Alt, System.Windows.Forms.Keys.Q);
            //API.RegisterHotKey(new System.Windows.Interop.WindowInteropHelper(mainwindow).Handle, 101, KeyModifiers.Ctrl, System.Windows.Forms.Keys.Left);
            //API.RegisterHotKey(new System.Windows.Interop.WindowInteropHelper(mainwindow).Handle, 102, KeyModifiers.Ctrl, System.Windows.Forms.Keys.Right);
            API.RegisterHotKey(new System.Windows.Interop.WindowInteropHelper(mainwindow).Handle, 105, KeyModifiers.Alt, System.Windows.Forms.Keys.R);

            try
            {
                InjectDll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void StartButton_FormClosing(object sender, FormClosingEventArgs e)
        {
            API.UnregisterHotKey(new System.Windows.Interop.WindowInteropHelper(mainwindow).Handle, 100);
            API.UnregisterHotKey(new System.Windows.Interop.WindowInteropHelper(mainwindow).Handle, 105);
            //API.MoveWindow(hMin, 0, 0, rcMin.Right - rcMin.Left, rcMin.Bottom - rcMin.Top, true);
            UnInject();
            RevertTaskBar();
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
            SetAppPos();
        }
    }
}
