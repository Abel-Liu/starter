using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;
using ESTool;

namespace Starter
{
    public partial class ItemControl : UserControl
    {

        #region 公共字段
        /// <summary>
        /// 快捷方式所指向文件的路径
        /// </summary>
        public string Path
        { get; set; }

        /// <summary>
        /// 快捷方式的名称
        /// </summary>
        public string DisplayName
        {
            get { return this.textBox1.Text; }
            set { this.textBox1.Text = value; }
        }

        public object TooTip
        {
            get { return this.image1.ToolTip; }
            set { this.image1.ToolTip = value; }
        }

        /// <summary>
        /// 快捷方式在面板上的索引,从0开始
        /// </summary>
        public int OverallIndex
        { get; set; }

        /// <summary>
        /// 所在页面,从1开始
        /// </summary>
        public int PageIndex
        { get; set; }

        /// <summary>
        /// 快捷方式的类型,1为文件,2为文件夹,3为驱动器
        /// </summary>
        public int ItemType
        { get; set; }

        public ImageSource Icon
        {
            get { return this.image1.Source; }
            set { this.image1.Source = value; }
        }

        /// <summary>
        /// 删除项目
        /// </summary>
        public DelItem del;

        /// <summary>
        /// 移动项目到前(后)页
        /// </summary>
        public MoveItemPage movePage;
        #endregion

        /// <summary>
        /// 初始化自定义控件
        /// </summary>
        /// <param name="path">文件(夹)路径</param>
        /// <param name="name">要显示的名称</param>
        /// <param name="page">所在页面</param>
        /// <param name="index">索引</param>
        public ItemControl(string path, string name, int page, int index)
        {
            //item.Licon.ToBitmap().Save(AppDomain.CurrentDomain.BaseDirectory+"Icons\\"+index.ToString()+".ico",System.Drawing.Imaging.ImageFormat.Icon);
            InitializeComponent();

            Path = path;
            OverallIndex = index;
            PageIndex = page;
            DisplayName = name;
            TooTip = DisplayName + "\n" + Path;
            ItemType = (int)path.JudgeFileType();
            var ip = Abel.WindowsShell.WindowsHelper.GetIconPtr(path, Abel.WindowsShell.IconType.SHIL_EXTRALARGE);
            Icon = ConvertIcon(ip);

            this.image_del.Source = new BitmapImage(new Uri("pack://application:,,/images/del1.png"));
            this.image_del.Visibility = Visibility.Hidden;
            this.image_del.ToolTip = "删除";
        }

        /// <summary>
        /// 初始化自定义控件
        /// </summary>
        /// <param name="path">文件(夹)路径</param>
        /// <param name="page">所在页面</param>
        /// <param name="index">索引</param>
        public ItemControl(string path, int page, int index)
            : this(path, path.GetNameByPath(), page, index)
        { }

        /// <summary>
        /// 将获取到的图标信息转换为ImageSource
        /// </summary>
        /// <param name="pIcon"></param>
        /// <returns></returns>
        private ImageSource ConvertIcon(IntPtr pIcon)
        {
            System.IO.MemoryStream tempstream = new System.IO.MemoryStream();
            System.Drawing.Icon.FromHandle(pIcon).ToBitmap().Save(tempstream, System.Drawing.Imaging.ImageFormat.Png);
            ImageSourceConverter converter = new ImageSourceConverter();
            return (ImageSource)converter.ConvertFrom(tempstream);
        }

        /// <summary>
        /// 执行重命名
        /// </summary>
        private void AcceptNewName()
        {
            this.textBox1.IsReadOnly = true;
            this.textBox1.BorderThickness = new Thickness(0);

            this.image1.ToolTip = this.textBox1.Text + "\n" + Path;
            this.textBox1.Select(0, 0);
            Path.ChangeName(DisplayName);
            MainWindow.RenameIndex = null;
            MainWindow.RenamePage = null;
        }

        private void image1_MouseEnter(object sender, MouseEventArgs e)
        {
            this.border1.BorderThickness = new Thickness(2);
        }
        private void image1_MouseLeave(object sender, MouseEventArgs e)
        {
            this.border1.BorderThickness = new Thickness(0);
        }
        private void image1_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ItemType == 1 && !System.IO.File.Exists(Path))
            {
                del(PageIndex, OverallIndex);
            }
            if ((ItemType == 2 || ItemType == 3) && !System.IO.Directory.Exists(Path))
            {
                del(PageIndex, OverallIndex);
            }
        }
        private void image1_PreviewMouseRightButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            OpenAdmin.IsEnabled = (Path.Substring(Path.Length - 3, 3).ToLower() == "exe");//菜单-管理员运行
            prepage.IsEnabled = (MainWindow.CurrentPage != 1);////菜单-移动到前页
        }
        private void image1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.border1.BorderThickness = new Thickness(3);
        }
        private void image1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.border1.BorderThickness = new Thickness(2);
            if (!MainWindow.dragItem)
            {
                if (ItemType == 1)
                    Process.Start(Path);
                if (ItemType == 2 || ItemType == 3)
                    Process.Start(@"Explorer", "/root," + Path);
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                AcceptNewName();
            }
        }
        private void textBox1_LostFocus(object sender, RoutedEventArgs e)
        {
            //AcceptNewName();
        }

        private void ReName_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.RenameIndex = OverallIndex.ToString();
            MainWindow.RenamePage = PageIndex.ToString();
            this.textBox1.IsReadOnly = false;
            this.textBox1.BorderThickness = new Thickness(1);
            this.textBox1.Focus();
            this.textBox1.SelectAll();
        }
        private void Del_Click(object sender, RoutedEventArgs e)
        {
            del(PageIndex, OverallIndex);
        }
        private void OpenDir_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(@"Explorer", "/select," + Path);
        }
        private void OpenAdmin_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = Path;
            startInfo.Verb = "runas";
            startInfo.UseShellExecute = true;
            Process.Start(startInfo);
        }
        private void SendDesktop_Click(object sender, RoutedEventArgs e)
        {
            string lnkpath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), DisplayName + ".lnk");
            lnkpath.CreatShortCut(Path);

        }

        private void nextpage_Click(object sender, RoutedEventArgs e)
        {
            movePage(PageIndex, OverallIndex, false);
        }
        private void prepage_Click(object sender, RoutedEventArgs e)
        {
            movePage(PageIndex, OverallIndex, true);
        }

        private void image_del_MouseEnter(object sender, MouseEventArgs e)
        {
            image_del.Source = new BitmapImage(new Uri("pack://application:,,/images/del2.png"));
        }
        private void image_del_MouseLeave(object sender, MouseEventArgs e)
        {
            image_del.Source = new BitmapImage(new Uri("pack://application:,,/images/del1.png"));
        }
        private void image_del_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            image_del.Source = new BitmapImage(new Uri("pack://application:,,/images/del3.png"));
        }
        private void image_del_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            del(PageIndex, OverallIndex);
        }
    }
}
