using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ESTool.Position
{
    public class PositionSvcWin7 : PositionBase
    {

        public PositionSvcWin7(IntPtr _startButtonHandle, System.Windows.Window _mainWindow)
            : base(_startButtonHandle, _mainWindow)
        {

        }

        public override void RemoveButton()
        {
            base.RemoveButton();

            reBarInfo rbInfo = API.CalcRebarPos();
            API.MoveWindow(rbInfo.hreBar, rbInfo.x, rbInfo.y, rbInfo.width, rbInfo.height, true);
        }

        public override void InsertButton()
        {
            base.InsertButton();

            SetAppPos(true);
            API.SetParent(startButtonHandle, API.FindWindow("Shell_TrayWnd", null));
        }

        public override void SetAppPos(bool moveRebar)
        {
            base.SetAppPos(moveRebar);

            AppPos pos;
            reBarInfo rbInfo = API.CalcAppPos(mainWindow.Height, mainWindow.Width, out pos);

            if (moveRebar)
                API.MoveWindow(rbInfo.hreBar, rbInfo.x, rbInfo.y, rbInfo.width, rbInfo.height, true);
            API.MoveWindow(startButtonHandle, pos.btn_x, pos.btn_y, pos.btn_size, pos.btn_size, true);

            mainWindow.Top = pos.main_top;
            mainWindow.Left = pos.main_left;
        }

        public override void InjectDll()
        {
            base.InjectDll();

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

        public override void UnInjectDll()
        {
            base.UnInjectDll();

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
    }
}
