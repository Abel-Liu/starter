using System;
using System.IO;
using System.Diagnostics;
using ESTool;

namespace Starter
{
    public class MyWork
    {
        #region 字段
        /// <summary>
        /// 启动了应用程序的可执行文件的路径，包括可执行文件的名称。
        /// </summary>
        public static string startExePath = System.Windows.Forms.Application.ExecutablePath;
        /// <summary>
        /// 启动目录
        /// </summary>
        public static string StartDir = AppDomain.CurrentDomain.BaseDirectory;
        /// <summary>
        /// 默认设置信息
        /// </summary>
        public static SettingInfo DefaultSettingInfo = new SettingInfo(true, true, "Alt+Q","1",true);
        /// <summary>
        /// 配置文件路径
        /// </summary>
        public static string ConfigPath = Path.Combine(StartDir , "config.xml");
        public static string LogPath = Path.Combine(StartDir, "Log.txt");
       
        #endregion

        /// <summary>
        /// 记录异常信息
        /// </summary>
        /// <param name="e">异常信息</param>
        public static void NoteLog(Exception e)
        {
            new BugReport(e).ShowDialog();
            MainWindow.closeFlag = "shut";
            System.Windows.Application.Current.Shutdown();
        }

    }
}
