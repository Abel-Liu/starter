using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace ESTool.Position
{
    public class PositionBase
    {
        public PositionBase(IntPtr _startButtonHandle, System.Windows.Window _mainWindow)
        {
            this.startButtonHandle = _startButtonHandle;
            this.mainWindow = _mainWindow;
        }

        protected readonly string dllname = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ESRemote.dll");

        /// <summary>
        /// 主按钮
        /// </summary>
        protected IntPtr startButtonHandle;
        protected System.Windows.Window mainWindow;

        /// <summary>
        /// explorer process
        /// </summary>
        protected Process explorerProcess;

        public virtual void SetAppPos()
        { }

        public virtual void InsertButton()
        { }

        public virtual void RemoveButton()
        { }

        public virtual void InjectDll()   //CreateRemoteThread only workd with x86 on windows 10
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
                explorerProcess = ps[0];
                int baseaddress = API.VirtualAllocEx(explorerProcess.Handle, IntPtr.Zero, dlllength, MEM_COMMIT, PAGE_READWRITE);
                if (baseaddress == 0)
                    throw new Exception("申请内存空间失败！");

                if (API.WriteProcessMemory(explorerProcess.Handle, baseaddress, dllname, dlllength, temp) == 0)
                    throw new Exception("写内存失败！");

                var hThread = API.CreateRemoteThread(explorerProcess.Handle, IntPtr.Zero, 0, new IntPtr(loadAddr), new IntPtr(baseaddress), 0, 0);
                if (hThread == null)
                {
                    API.VirtualFreeEx(explorerProcess.Handle, new IntPtr(baseaddress), dlllength, MEM_RELEASE);
                    throw new Exception("创建远程线程失败！");
                }
            }
        }

        public virtual void UnInjectDll()
        {
            if (explorerProcess != null)
            {
                MODULEENTRY32 stModuleEntry = new MODULEENTRY32();
                bool bFlag = true;
                IntPtr hFindModule = IntPtr.Zero;

                stModuleEntry.dwSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(stModuleEntry);

                var hModuleSnap = API.CreateToolhelp32Snapshot(0x00000008, explorerProcess.Id);
                bFlag = API.Module32First(hModuleSnap, ref stModuleEntry);
                for (; bFlag;)
                {
                    if (stModuleEntry.szExePath.ToLower() == dllname.ToLower())
                    {
                        hFindModule = stModuleEntry.hModule;
                        break;
                    }
                    bFlag = API.Module32Next(hModuleSnap, ref stModuleEntry);
                }

                var freeAddr = API.GetProcAddress(API.GetModuleHandleA("Kernel32"), "FreeLibrary");
                API.CreateRemoteThread(explorerProcess.Handle, IntPtr.Zero, 0, new IntPtr(freeAddr), hFindModule, 0, 0);
            }
        }

    }
}
