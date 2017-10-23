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

            reBarInfo rbInfo = CalcRebarPos();
            API.MoveWindow(rbInfo.hreBar, rbInfo.x, rbInfo.y, rbInfo.width, rbInfo.height, true);
        }

        public override void InsertButton()
        {
            base.InsertButton();

            SetAppPos();
            API.SetParent(startButtonHandle, API.FindWindow("Shell_TrayWnd", null));
        }

        public override void SetAppPos()
        {
            base.SetAppPos();

            AppPos pos = CalcAppPos(mainWindow.Height, mainWindow.Width);

            API.MoveWindow(pos.rebarInfo.hreBar, pos.rebarInfo.x, pos.rebarInfo.y, pos.rebarInfo.width, pos.rebarInfo.height, true);

            API.MoveWindow(startButtonHandle, pos.btn_x, pos.btn_y, pos.btn_size, pos.btn_size, true);

            mainWindow.Top = pos.main_top;
            mainWindow.Left = pos.main_left;
        }

        /// <summary>
        /// 开始按钮大小
        /// </summary>
        int StartSize
        {
            get { return 54; }
        }

        /// <summary>
        /// 计算rebar正常位置
        /// </summary>
        /// <returns></returns>
        public reBarInfo CalcRebarPos()
        {
            System.Drawing.Rectangle screenRect = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
            RECT rcBar, rcTrayBtn, rcStart;
            reBarInfo rbInfo = new reBarInfo();

            try
            {
                IntPtr hTaskbar = API.FindWindow("Shell_TrayWnd", null);
                IntPtr hBar = API.FindWindowEx(hTaskbar, IntPtr.Zero, "ReBarWindow32", null);
                IntPtr hTray = API.FindWindowEx(hTaskbar, IntPtr.Zero, "TrayNotifyWnd", null);
                IntPtr hTrayBtn = API.FindWindowEx(hTray, IntPtr.Zero, "Button", null);
                IntPtr hStart = API.FindWindowEx(hTaskbar, IntPtr.Zero, "Button", null);
                API.GetWindowRect(hStart, out rcStart);
                API.GetWindowRect(hBar, out rcBar);
                API.GetWindowRect(hTrayBtn, out rcTrayBtn);
                rbInfo.hreBar = hBar;
                rbInfo.hTaskBar = hTaskbar;

                AppBarData taskbarInfo = new AppBarData();
                API.SHAppBarMessage(0x00000005, ref taskbarInfo);

                switch (taskbarInfo.uEdge)
                {
                    case 0:
                    case 2:////任务栏在右边
                        rbInfo.x = 0;
                        rbInfo.y = StartSize;
                        rbInfo.height = rcTrayBtn.Top - rbInfo.y;
                        rbInfo.width = rcBar.Right - rcBar.Left;
                        break;
                    case 1:
                    case 3:////任务栏在下边
                        rbInfo.x = StartSize;
                        rbInfo.y = 0;
                        rbInfo.height = rcBar.Bottom - rcBar.Top;
                        rbInfo.width = rcTrayBtn.Left - rbInfo.x;
                        break;
                }
            }
            catch
            { }

            return rbInfo;
        }

        /// <summary>
        /// 根据任务栏位置计算窗体和主按钮位置
        /// </summary>
        /// <param name="mainWindowHeight">主窗体高度</param>
        /// <param name="mainWindowWidth">主窗体宽度</param>
        public AppPos CalcAppPos(double mainWindowHeight, double mainWindowWidth)
        {
            System.Drawing.Rectangle screenRect = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
            RECT rcBar, rcTrayBtn, rcStart;
            AppPos pos = new AppPos();

            try
            {
                #region
                IntPtr hTaskbar = API.FindWindow("Shell_TrayWnd", null);
                IntPtr hBar = API.FindWindowEx(hTaskbar, IntPtr.Zero, "ReBarWindow32", null);
                IntPtr hTray = API.FindWindowEx(hTaskbar, IntPtr.Zero, "TrayNotifyWnd", null);
                IntPtr hTrayBtn = API.FindWindowEx(hTray, IntPtr.Zero, "Button", null);
                IntPtr hStart = API.FindWindowEx(hTaskbar, IntPtr.Zero, "Button", null);
                API.GetWindowRect(hStart, out rcStart);
                API.GetWindowRect(hBar, out rcBar);
                API.GetWindowRect(hTrayBtn, out rcTrayBtn);
                pos.rebarInfo.hreBar = hBar;
                pos.rebarInfo.hTaskBar = hTaskbar;

                AppBarData taskbarInfo = new AppBarData();
                API.SHAppBarMessage(0x00000005, ref taskbarInfo);
                int max_size = 40;

                switch (taskbarInfo.uEdge)
                {
                    #region
                    case 0:////任务栏在左边
                        pos.main_left = taskbarInfo.rc.Right + 3;
                        pos.main_top = taskbarInfo.rc.Top + 30;
                        pos.btn_size = rcBar.Right - rcBar.Left > max_size ? max_size : rcBar.Right - rcBar.Left;
                        pos.btn_x = (rcBar.Right - rcBar.Left - pos.btn_size) / 2;
                        pos.btn_y = StartSize;

                        pos.rebarInfo.x = 0;
                        pos.rebarInfo.y = pos.btn_y + pos.btn_size;
                        pos.rebarInfo.height = rcTrayBtn.Top - pos.rebarInfo.y;
                        pos.rebarInfo.width = rcBar.Right - rcBar.Left;
                        break;
                    case 1:////任务栏在上边
                        pos.main_left = taskbarInfo.rc.Left + 30;
                        pos.main_top = taskbarInfo.rc.Bottom + 3;
                        pos.btn_size = rcBar.Bottom - rcBar.Top > max_size ? max_size : rcBar.Bottom - rcBar.Top;
                        pos.btn_x = StartSize;
                        pos.btn_y = (rcBar.Bottom - rcBar.Top - pos.btn_size) / 2;

                        pos.rebarInfo.x = pos.btn_x + pos.btn_size;
                        pos.rebarInfo.y = 0;
                        pos.rebarInfo.height = rcBar.Bottom - rcBar.Top;
                        pos.rebarInfo.width = rcTrayBtn.Left - pos.rebarInfo.x;
                        break;
                    case 2:////任务栏在右边
                        pos.main_left = screenRect.Right - mainWindowWidth - 3;
                        pos.main_top = taskbarInfo.rc.Top + 30;
                        pos.btn_size = rcBar.Right - rcBar.Left > max_size ? max_size : rcBar.Right - rcBar.Left;
                        pos.btn_x = (rcBar.Right - rcBar.Left - pos.btn_size) / 2;
                        pos.btn_y = StartSize;

                        pos.rebarInfo.x = 0;
                        pos.rebarInfo.y = pos.btn_y + pos.btn_size;
                        pos.rebarInfo.height = rcTrayBtn.Top - pos.rebarInfo.y;
                        pos.rebarInfo.width = rcBar.Right - rcBar.Left;
                        break;
                    case 3:////任务栏在下边
                        pos.main_left = taskbarInfo.rc.Left + 30;
                        pos.main_top = screenRect.Bottom - mainWindowHeight - 3;
                        pos.btn_size = rcBar.Bottom - rcBar.Top > max_size ? max_size : rcBar.Bottom - rcBar.Top;
                        pos.btn_x = StartSize;
                        pos.btn_y = (rcBar.Bottom - rcBar.Top - pos.btn_size) / 2;

                        pos.rebarInfo.x = pos.btn_x + pos.btn_size;
                        pos.rebarInfo.y = 0;
                        pos.rebarInfo.height = rcBar.Bottom - rcBar.Top;
                        pos.rebarInfo.width = rcTrayBtn.Left - pos.rebarInfo.x;
                        break;
                    default:
                        pos = new AppPos()
                        {
                            main_left = (screenRect.Width - mainWindowWidth) / 2,
                            main_top = (screenRect.Height - mainWindowHeight) / 2,
                            btn_size = -1,
                            btn_x = -1,
                            btn_y = -1
                        };
                        break;
                        #endregion
                }
                #endregion
            }
            catch
            {
                pos = new AppPos()
                {
                    main_left = (screenRect.Width - mainWindowWidth) / 2,
                    main_top = (screenRect.Height - mainWindowHeight) / 2,
                    btn_size = -1,
                    btn_x = -1,
                    btn_y = -1
                };
            }

            return pos;
        }
    }
}
