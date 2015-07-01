using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ESTool
{
    /// <summary>
    /// 声明API方法和消息常量
    /// </summary>
    public class API
    {
        #region 消息常量
        /// <summary>
        /// 进程间传递消息
        /// </summary>
        public const int WM_COPYDATA = 0x004A;
        /// <summary>
        /// 热键
        /// </summary>
        public const int WM_HOTKEY = 0x0312;
        /// <summary>
        /// 计算机关闭
        /// </summary>
        public const int WM_QUERYENDSESSION = 0x0011;
        /// <summary>
        /// 任务栏位置改变
        /// </summary>
        public const int WM_WININICHANGE = 0x001A;
        /// <summary>
        /// 分辨率改变
        /// </summary>
        public const int WM_DISPLAYCHANGE = 0x7E;
        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int WM_SYSKEYDOWN = 0x104;
        public const int WM_SYSKEYUP = 0x105;
        #endregion

        /// <summary>
        /// keyboard hook constant
        /// </summary>
        public const int WH_KEYBOARD_LL = 13;

        #region  自定义方法
        /// <summary>
        /// 处理附加的消息
        /// </summary>
        /// <param name="args">消息</param>
        public static void DealMessage(string[] args)
        {
            if (args.Length != 0)
            {
                while (FindWindow(null, "EasyStarterByAbelGuaizi") == IntPtr.Zero)
                {
                    System.Threading.Thread.Sleep(200);
                }
                CopyDataStruct cds;
                cds.dwData = IntPtr.Zero;
                cds.lpData = args[0];
                cds.cbData = System.Text.Encoding.Default.GetBytes(args[0]).Length + 1;
                int fromWindowHandler = 0;
                SendMessage(FindWindow(null, "EasyStarterByAbelGuaizi"), WM_COPYDATA, fromWindowHandler, ref cds);
            }
        }
        /// <summary>
        /// 获取开始按钮大小
        /// </summary>
        /// <param name="taskBarLocation">任务栏位置</param>
        /// <param name="rcStart"></param>
        /// <returns></returns>
        private static int GetStartSpace(int taskBarLocation, RECT rcStart)
        {
            int result;
            Version version = Environment.OSVersion.Version;
            if (version.Major == 5 && version.Minor == 1)////XP
            {
                if (taskBarLocation == 0 || taskBarLocation == 2)
                    result = rcStart.Bottom - rcStart.Top;
                else result = rcStart.Right - rcStart.Left;
            }
            else if (version.Major == 6 && (version.Minor == 0 || version.Minor == 1))////Vista&Win7
                result = 54;
            else if (version.Major == 6 && version.Minor == 2)////Win8
                result = 11;
            else result = 55;
            return result;
        }

        /// <summary>
        /// 根据任务栏位置计算窗体和主按钮位置,返回计算好的reBar信息
        /// </summary>
        /// <param name="height">主窗体高度</param>
        /// <param name="width">主窗体宽度</param>
        /// <param name="pos">包含主界面和任务栏按钮信息的结构体</param>
        /// <returns></returns>
        public static reBarInfo CalcAppPos(double height, double width, out AppPos pos)
        {
            System.Drawing.Rectangle screenRect = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
            RECT rcBar, rcTrayBtn, rcStart;
            reBarInfo rbInfo = new reBarInfo();

            try
            {
                #region
                IntPtr hTaskbar = FindWindow("Shell_TrayWnd", null);
                IntPtr hBar = FindWindowEx(hTaskbar, IntPtr.Zero, "ReBarWindow32", null);
                IntPtr hTray = FindWindowEx(hTaskbar, IntPtr.Zero, "TrayNotifyWnd", null);
                IntPtr hTrayBtn = FindWindowEx(hTray, IntPtr.Zero, "Button", null);
                IntPtr hStart = FindWindowEx(hTaskbar, IntPtr.Zero, "Button", null);
                GetWindowRect(hStart, out rcStart);
                GetWindowRect(hBar, out rcBar);
                GetWindowRect(hTrayBtn, out rcTrayBtn);
                rbInfo.hreBar = hBar;
                rbInfo.hTaskBar = hTaskbar;

                AppBarData taskbarInfo = new AppBarData();
                SHAppBarMessage(0x00000005, ref taskbarInfo);
                int max_size = 40;

                switch (taskbarInfo.uEdge)
                {
                    #region
                    case 0:////任务栏在左边
                        pos.main_left = taskbarInfo.rc.Right + 3;
                        pos.main_top = taskbarInfo.rc.Top + 30;
                        pos.btn_size = rcBar.Right - rcBar.Left > max_size ? max_size : rcBar.Right - rcBar.Left;
                        pos.btn_x = (rcBar.Right - rcBar.Left - pos.btn_size) / 2;
                        pos.btn_y = GetStartSpace(taskbarInfo.uEdge, rcStart);

                        rbInfo.x = 0;
                        rbInfo.y = pos.btn_y + pos.btn_size;
                        rbInfo.height = rcTrayBtn.Top - rbInfo.y;
                        rbInfo.width = rcBar.Right - rcBar.Left;
                        break;
                    case 1:////任务栏在上边
                        pos.main_left = taskbarInfo.rc.Left + 30;
                        pos.main_top = taskbarInfo.rc.Bottom + 3;
                        pos.btn_size = rcBar.Bottom - rcBar.Top > max_size ? max_size : rcBar.Bottom - rcBar.Top;
                        pos.btn_x = GetStartSpace(taskbarInfo.uEdge, rcStart);
                        pos.btn_y = (rcBar.Bottom - rcBar.Top - pos.btn_size) / 2;

                        rbInfo.x = pos.btn_x + pos.btn_size;
                        rbInfo.y = 0;
                        rbInfo.height = rcBar.Bottom - rcBar.Top;
                        rbInfo.width = rcTrayBtn.Left - rbInfo.x;
                        break;
                    case 2:////任务栏在右边
                        pos.main_left = screenRect.Right - width - 3;
                        pos.main_top = taskbarInfo.rc.Top + 30;
                        pos.btn_size = rcBar.Right - rcBar.Left > max_size ? max_size : rcBar.Right - rcBar.Left;
                        pos.btn_x = (rcBar.Right - rcBar.Left - pos.btn_size) / 2;
                        pos.btn_y = GetStartSpace(taskbarInfo.uEdge, rcStart);

                        rbInfo.x = 0;
                        rbInfo.y = pos.btn_y + pos.btn_size;
                        rbInfo.height = rcTrayBtn.Top - rbInfo.y;
                        rbInfo.width = rcBar.Right - rcBar.Left;
                        break;
                    case 3:////任务栏在下边
                        pos.main_left = taskbarInfo.rc.Left + 30;
                        pos.main_top = screenRect.Bottom - height - 3;
                        pos.btn_size = rcBar.Bottom - rcBar.Top > max_size ? max_size : rcBar.Bottom - rcBar.Top;
                        pos.btn_x = GetStartSpace(taskbarInfo.uEdge, rcStart);
                        pos.btn_y = (rcBar.Bottom - rcBar.Top - pos.btn_size) / 2;

                        rbInfo.x = pos.btn_x + pos.btn_size;
                        rbInfo.y = 0;
                        rbInfo.height = rcBar.Bottom - rcBar.Top;
                        rbInfo.width = rcTrayBtn.Left - rbInfo.x;
                        break;
                    default:
                        pos = new AppPos((screenRect.Width - width) / 2, (screenRect.Height - height) / 2);
                        break;
                    #endregion
                }
                #endregion
            }
            catch
            { pos = new AppPos((screenRect.Width - width) / 2, (screenRect.Height - height) / 2); }

            return rbInfo;
        }

        /// <summary>
        /// 计算rebar正常位置
        /// </summary>
        /// <returns></returns>
        public static reBarInfo CalcRebarPos()
        {
            System.Drawing.Rectangle screenRect = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
            RECT rcBar, rcTrayBtn, rcStart;
            reBarInfo rbInfo = new reBarInfo();

            try
            {
                #region
                IntPtr hTaskbar = FindWindow("Shell_TrayWnd", null);
                IntPtr hBar = FindWindowEx(hTaskbar, IntPtr.Zero, "ReBarWindow32", null);
                IntPtr hTray = FindWindowEx(hTaskbar, IntPtr.Zero, "TrayNotifyWnd", null);
                IntPtr hTrayBtn = FindWindowEx(hTray, IntPtr.Zero, "Button", null);
                IntPtr hStart = FindWindowEx(hTaskbar, IntPtr.Zero, "Button", null);
                GetWindowRect(hStart, out rcStart);
                GetWindowRect(hBar, out rcBar);
                GetWindowRect(hTrayBtn, out rcTrayBtn);
                rbInfo.hreBar = hBar;
                rbInfo.hTaskBar = hTaskbar;

                AppBarData taskbarInfo = new AppBarData();
                SHAppBarMessage(0x00000005, ref taskbarInfo);

                switch (taskbarInfo.uEdge)
                {
                    case 0:
                    case 2:////任务栏在右边
                        rbInfo.x = 0;
                        rbInfo.y = GetStartSpace(taskbarInfo.uEdge, rcStart);
                        rbInfo.height = rcTrayBtn.Top - rbInfo.y;
                        rbInfo.width = rcBar.Right - rcBar.Left;
                        break;
                    case 1:
                    case 3:////任务栏在下边
                        rbInfo.x = GetStartSpace(taskbarInfo.uEdge, rcStart);
                        rbInfo.y = 0;
                        rbInfo.height = rcBar.Bottom - rcBar.Top;
                        rbInfo.width = rcTrayBtn.Left - rbInfo.x;
                        break;
                }
                #endregion
            }
            catch
            { }

            return rbInfo;
        }
        #endregion

        [DllImport("user32.dll ", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        [DllImport("user32.dll ", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);
        [DllImport("user32 ")]
        public static extern int ToAscii(int uVirtKey, int uScanCode, byte[] lpbKeyState, byte[] lpwTransKey, int fuState);
        [DllImport("user32 ")]
        public static extern int GetKeyboardState(byte[] pbKeyState);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("User32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hwnd);

        /// <summary>
        /// 判断任务栏位置
        /// </summary>
        /// <param name="dwMessage">操作类型</param>
        /// <param name="pData">结构体引用</param>
        /// <returns></returns>
        [DllImport("SHELL32", CallingConvention = CallingConvention.StdCall)]
        public static extern uint SHAppBarMessage(int dwMessage, ref AppBarData pData);
        /// <summary>
        /// 向指定窗口发送消息
        /// </summary>
        /// <param name="hWnd">接收消息的窗口句柄</param>
        /// <param name="Msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam">结构体</param>
        /// <returns></returns>
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, ref  CopyDataStruct lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        /// <summary>
        /// 注册热键
        /// </summary>
        /// <param name="hWnd">要定义热键的窗口的句柄</param>
        /// <param name="id">定义热键ID（不能与其它ID重复）</param>
        /// <param name="fsModifiers">标识热键是否在按Alt、Ctrl、Shift、Windows等键时才会生效</param>
        /// <param name="vk">定义热键的内容</param>
        /// <returns>如果函数执行成功，返回值不为0。如果函数执行失败，返回值为0。要得到扩展错误信息，调用GetLastError。</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, KeyModifiers fsModifiers, System.Windows.Forms.Keys vk);
        /// <summary>
        /// 取消热键
        /// </summary>
        /// <param name="hWnd">要取消热键的窗口的句柄</param>
        /// <param name="id">要取消热键的ID</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        public static extern uint RegisterWindowMessage(string lpString);

        [DllImport("kernel32.dll")]
        public static extern int GetProcAddress(IntPtr hwnd, string lpname);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandleA(string name);

        [DllImport("kernel32.dll")]
        public static extern int VirtualAllocEx(IntPtr hwnd, IntPtr lpaddress, int size, int type, int tect);

        [DllImport("kernel32.dll")]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint dwFreeType);

        [DllImport("kernel32.dll")]
        public static extern int WriteProcessMemory(IntPtr hwnd, int baseaddress, string buffer, int nsize, int filewriten);

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess,
           IntPtr lpThreadAttributes,
           uint dwStackSize,
           IntPtr lpStartAddress, // raw Pointer into remote process
           IntPtr lpParameter,
           uint dwCreationFlags,
           uint lpThreadId);

        [DllImport("kernel32.dll")]
       public static extern IntPtr CreateToolhelp32Snapshot(int dwFlags, int th32ProcessID);

        [DllImport("kernel32.dll")]
        public static extern bool Module32First(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

        [DllImport("kernel32.dll")]
        public static extern bool Module32Next(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

    }
}
