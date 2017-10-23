using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Runtime.InteropServices;

namespace ESTool
{
    /// <summary>
    /// 比较两个页面的索引是否相等
    /// </summary>
    public class ComparerPageIndexInXml : IEqualityComparer<XElement>
    {
        public bool Equals(XElement a, XElement b)
        {
            return a.Value == b.Value ? true : false;
        }
        public int GetHashCode(XElement e)
        {
            return int.Parse(e.Value);
        }
    }

    /// <summary>
    /// 包含进程模块的信息
    /// </summary>
    public struct MODULEENTRY32
    {
        public const int MAX_PATH = 255;
        public uint dwSize;
        public uint th32ModuleID;
        public uint th32ProcessID;
        public uint GlblcntUsage;
        public uint ProccntUsage;
        public IntPtr modBaseAddr;
        public uint modBaseSize;
        public IntPtr hModule;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH + 1)]
        public string szModule;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH + 5)]
        public string szExePath;
    }


    /// <summary>
    /// 键盘钩子的封送结构类型 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class KeyboardHookStruct
    {
        /// <summary>
        /// 表示一个在1到254间的虚似键盘码 
        /// </summary>
        public int vkCode;
        public int scanCode;   //表示硬件扫描码 
        public int flags;
        public int time;
        public int dwExtraInfo;
    }

    /// <summary>
    /// 包含主界面和任务栏按钮的位置信息
    /// </summary>
    public struct AppPos
    {
        public reBarInfo rebarInfo;

        /// <summary>
        /// 主窗体左边距
        /// </summary>
        public double main_left;
        /// <summary>
        /// 主窗体上边距
        /// </summary>
        public double main_top;
        /// <summary>
        /// 主按钮X坐标
        /// </summary>
        public int btn_x;
        /// <summary>
        /// 主按钮Y坐标
        /// </summary>
        public int btn_y;
        /// <summary>
        /// 主按钮大小
        /// </summary>
        public int btn_size;
    }

    /// <summary>
    /// 包含reBar的句柄、位置和大小
    /// </summary>
    public struct reBarInfo
    {
        public IntPtr hreBar;
        public IntPtr hTaskBar;
        public int x;
        public int y;
        public int width;
        public int height;
    }

    /// <summary>
    /// 包含任务栏的信息
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AppBarData
    {
        public int cbSize;
        public IntPtr hWnd;
        public int uCallbackMessage;
        /// <summary>
        /// 任务栏位置左0,上1,右2,下3
        /// </summary>
        public int uEdge;
        /// <summary>
        /// 任务栏的坐标
        /// </summary>
        public RECT rc;
        public IntPtr lParam;
    }

    /// <summary>
    /// 包含消息的结构体
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CopyDataStruct
    {
        public IntPtr dwData;
        public int cbData;
        /// <summary>
        /// 包含要添加的文件路径
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string lpData;
    }

    [Flags()]
    public enum KeyModifiers
    {
        None = 0,
        Alt = 1,
        Ctrl = 2,
        Shift = 4,
        WindowsKey = 8
    }

    /// <summary>
    /// 包含设置信息的结构
    /// </summary>
    public struct SettingInfo
    {
        /// <summary>
        /// 使用默认配置初始化
        /// </summary>
        /// <param name="defaultPath"></param>
        /// <param name="defaultBoot"></param>
        public SettingInfo(bool defaultBoot, bool defaultSysRightMenu, string defaultHotKey, string defaultBack, bool defaultSendbug)
        {
            Boot = defaultBoot;
            SysRightMenu = defaultSysRightMenu;
            HotKey = defaultHotKey;
            BackImg = defaultBack;
            SendBug = defaultSendbug;
        }
        /// <summary>
        /// 开机启动
        /// </summary>
        public bool Boot;
        /// <summary>
        /// 是否添加到系统右键菜单
        /// </summary>
        public bool SysRightMenu;
        /// <summary>
        /// 热键
        /// </summary>
        public string HotKey;
        /// <summary>
        /// 背景
        /// </summary>
        public string BackImg;
        /// <summary>
        /// 自动发送错误报告
        /// </summary>
        public bool SendBug;
    }

    /// <summary>
    /// 文件类型
    /// </summary>
    public enum FileType
    {
        /// <summary>
        /// 无法确定类型
        /// </summary>
        None = 0,
        /// <summary>
        /// 文件
        /// </summary>
        File = 1,
        /// <summary>
        /// 文件夹
        /// </summary>
        Directory = 2,
        /// <summary>
        /// 驱动器
        /// </summary>
        Driver = 3,
    }
    /// <summary>
    /// 包含项目信息的结构体
    /// </summary>
    public struct ItemInfo
    {
        /// <summary>
        /// 初始化项目信息
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="name">显示名称</param>
        /// <param name="page">所在页面</param>
        /// <param name="index">页面内的索引</param>
        /// <param name="type">文件类型</param>
        public ItemInfo(string path, string name, int page, int index, int type)
        {
            ItemPath = path;
            ItemName = name;
            ItemPage = page;
            ItemIndex = index;
            ItemType = type;
        }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string ItemPath;
        /// <summary>
        /// 显示名称
        /// </summary>
        public string ItemName;
        /// <summary>
        /// 所在页面
        /// </summary>
        public int ItemPage;
        /// <summary>
        /// 页面内的索引
        /// </summary>
        public int ItemIndex;
        /// <summary>
        /// 文件类型
        /// </summary>
        public int ItemType;
    }

    /// <summary>
    /// 矩形结构
    /// </summary>
    public struct RECT
    {
        public RECT(System.Drawing.Rectangle rectangle)
        {
            Left = rectangle.Left;
            Top = rectangle.Top;
            Right = rectangle.Right;
            Bottom = rectangle.Bottom;
        }
        public RECT(System.Drawing.Point location, System.Drawing.Size size)
        {
            Left = location.X;
            Top = location.Y;
            Right = location.X + size.Width;
            Bottom = location.Y + size.Height;
        }
        public Int32 Left;
        public Int32 Top;
        public Int32 Right;
        public Int32 Bottom;
    }
}
