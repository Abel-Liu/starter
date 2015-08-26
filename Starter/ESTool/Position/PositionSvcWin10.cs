using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ESTool.Position
{
    public class PositionSvcWin10 : PositionBase
    {

        public PositionSvcWin10(IntPtr _startButtonHandle, System.Windows.Window _mainWindow)
            : base(_startButtonHandle, _mainWindow)
        {

        }

        public override void RemoveButton()
        {
            base.RemoveButton();

            //reBarInfo rbInfo = API.CalcRebarPos();
            //API.MoveWindow(rbInfo.hreBar, rbInfo.x, rbInfo.y, rbInfo.width, rbInfo.height, true);
        }

        public override void InsertButton()
        {
            base.InsertButton();

            SetAppPos(true);
            //API.SetParent(startButtonHandle, API.FindWindow("Shell_TrayWnd", null));
        }

        public override void SetAppPos(bool moveRebar)
        {
            base.SetAppPos(moveRebar);

            //AppPos pos;
            //reBarInfo rbInfo = CalcAppPos(mainWindow.Height, mainWindow.Width, out pos);

            //if ( moveRebar )
            //    API.MoveWindow( rbInfo.hreBar, rbInfo.x, rbInfo.y, rbInfo.width, rbInfo.height, true );

            API.MoveWindow( startButtonHandle, 5000, 5000, 40, 40, true );


            mainWindow.Top = 600;
            mainWindow.Left = 30;
        }

        public override void InjectDll()
        {
            base.InjectDll();

            //const int MEM_COMMIT = 0x00001000;
            //const int MEM_RELEASE = 0x8000;
            //const int PAGE_READWRITE = 0x04;
            //int temp = 0;

            //int dlllength = System.Text.Encoding.Default.GetByteCount(dllname);
            //var loadAddr = API.GetProcAddress(API.GetModuleHandleA("Kernel32"), "LoadLibraryA");

            //var ps = Process.GetProcessesByName("explorer");

            //if (ps != null && ps.Length > 0)
            //{
            //    process = ps[0];
            //    int baseaddress = API.VirtualAllocEx(process.Handle, IntPtr.Zero, dlllength, MEM_COMMIT, PAGE_READWRITE);
            //    if (baseaddress == 0)
            //        throw new Exception("申请内存空间失败！");

            //    if (API.WriteProcessMemory(process.Handle, baseaddress, dllname, dlllength, temp) == 0)
            //        throw new Exception("写内存失败！");

            //    var hThread = API.CreateRemoteThread(process.Handle, IntPtr.Zero, 0, new IntPtr(loadAddr), new IntPtr(baseaddress), 0, 0);
            //    if (hThread == null)
            //    {
            //        API.VirtualFreeEx(process.Handle, new IntPtr(baseaddress), dlllength, MEM_RELEASE);
            //        throw new Exception("创建远程线程失败！");
            //    }
            //}
        }

        public override void UnInjectDll()
        {
            base.UnInjectDll();

            //if (process != null)
            //{
            //    MODULEENTRY32 stModuleEntry = new MODULEENTRY32();
            //    bool bFlag = true;
            //    IntPtr hFindModule = IntPtr.Zero;

            //    stModuleEntry.dwSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(stModuleEntry);

            //    var hModuleSnap = API.CreateToolhelp32Snapshot(0x00000008, process.Id);
            //    bFlag = API.Module32First(hModuleSnap, ref stModuleEntry);
            //    for (; bFlag; )
            //    {
            //        if (stModuleEntry.szExePath.ToLower() == dllname.ToLower())
            //        {
            //            hFindModule = stModuleEntry.hModule;
            //            break;
            //        }
            //        bFlag = API.Module32Next(hModuleSnap, ref stModuleEntry);
            //    }

            //    var freeAddr = API.GetProcAddress(API.GetModuleHandleA("Kernel32"), "FreeLibrary");
            //    API.CreateRemoteThread(process.Handle, IntPtr.Zero, 0, new IntPtr(freeAddr), hFindModule, 0, 0);
            //}
        }


        public static reBarInfo CalcAppPos( double height, double width, out AppPos pos )
        {
            System.Drawing.Rectangle screenRect = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
            RECT rcBar, rcTrayBtn, rcStart;
            reBarInfo rbInfo = new reBarInfo();

            try
            {
                #region
                IntPtr hTaskbar = API.FindWindow( "Shell_TrayWnd", null );
                IntPtr hBar = API.FindWindowEx( hTaskbar, IntPtr.Zero, "ReBarWindow32", null );
                IntPtr hTray = API.FindWindowEx( hTaskbar, IntPtr.Zero, "TrayNotifyWnd", null );
                IntPtr hTrayBtn = API.FindWindowEx( hTray, IntPtr.Zero, "Button", null );
                IntPtr hStart = API.FindWindowEx( hTaskbar, IntPtr.Zero, "Button", null );
                API.GetWindowRect( hStart, out rcStart );
                API.GetWindowRect( hBar, out rcBar );
                API.GetWindowRect( hTrayBtn, out rcTrayBtn );
                rbInfo.hreBar = hBar;
                rbInfo.hTaskBar = hTaskbar;

                AppBarData taskbarInfo = new AppBarData();
                API.SHAppBarMessage( 0x00000005, ref taskbarInfo );
                int max_size = 40;

                switch ( taskbarInfo.uEdge )
                {
                    #region
                   
                    case 3:////任务栏在下边
                        pos.main_left = taskbarInfo.rc.Left + 30;
                        pos.main_top = screenRect.Bottom - height - 3;
                        pos.btn_size = rcBar.Bottom - rcBar.Top > max_size ? max_size : rcBar.Bottom - rcBar.Top;
                        pos.btn_x = 40;
                        pos.btn_y = ( rcBar.Bottom - rcBar.Top - pos.btn_size ) / 2;

                        rbInfo.x = pos.btn_x + pos.btn_size;
                        rbInfo.y = 0;
                        rbInfo.height = rcBar.Bottom - rcBar.Top;
                        rbInfo.width = rcTrayBtn.Left - rbInfo.x;
                        break;
                    default:
                        pos = new AppPos( ( screenRect.Width - width ) / 2, ( screenRect.Height - height ) / 2 );
                        break;
                    #endregion
                }
                #endregion
            }
            catch
            { pos = new AppPos( ( screenRect.Width - width ) / 2, ( screenRect.Height - height ) / 2 ); }

            return rbInfo;
        }
    }
}
