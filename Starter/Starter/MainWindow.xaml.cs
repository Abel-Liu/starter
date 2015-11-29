using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Linq;
using System.Collections.Generic;
using ESTool;

namespace Starter
{
    #region 委托
    /// <summary>
    /// 删除委托
    /// </summary>
    /// <param name="page">所在页面</param>
    /// <param name="index">索引</param>
    public delegate void DelItem(int page, int index);

    /// <summary>
    /// 移动项目到前(后)一页
    /// </summary>
    /// <param name="page">所在页面</param>
    /// <param name="index">索引</param>
    /// <param name="direction">移动方向,true为向前,false为向后</param>
    public delegate void MoveItemPage(int page, int index, bool direction);
    /// <summary>
    /// 改变背景
    /// </summary>
    /// <param name="path">背景图像路径</param>
    public delegate void ChangeBackImage(string path, bool real);

    #region 前后移动项目
    /// <summary>
    /// 前后移动项目
    /// </summary>
    /// <param name="page">所在页面</param>
    /// <param name="index">索引</param>
    /// <param name="direction">移动方向,true为向前,false为向后</param>
    //public delegate void MoveItemIndex(int page, int index, bool direction);
    #endregion
    #endregion
    public partial class MainWindow : Window
    {
        #region 私有字段
        /// <summary>
        /// 页面个数
        /// </summary>
        private static int PageCount;
        /// <summary>
        /// 向后翻页
        /// </summary>
        private DispatcherTimer moveleft;
        /// <summary>
        /// 向前翻页
        /// </summary>
        private DispatcherTimer moveright;
        /// <summary>
        /// 被删除之后的面板前移
        /// </summary>
        private DispatcherTimer moveleftWhenDel;
        /// <summary>
        /// 被删除之前的面板后移
        /// </summary>
        private DispatcherTimer moverightWhenDel;
        /// <summary>
        /// 设置窗体
        /// </summary>
        private Setting setForm = null;
        /// <summary>
        /// 当前背景路径
        /// </summary>
        private string currentBack;
        /// <summary>
        /// 是否处于编辑状态
        /// </summary>
        private bool editItem = false;
        /// <summary>
        /// 拖动前鼠标的位置，相对于窗体
        /// </summary>
        private Point mouseLocationBeforeDrag = new Point();
        /// <summary>
        /// 临时创建用于拖动的项目,属于maingrid
        /// </summary>
        private ItemControl tempItem = null;
        /// <summary>
        /// 被拖动的实际项目的索引
        /// </summary>
        private int tempIndex = 0;
        /// <summary>
        /// 被拖动的实际项目
        /// </summary>
        private ItemControl item = null;
        /// <summary>
        /// 临时项目的初始位置，相对于maingrid
        /// </summary>
        private Point ItemoldMargin = new Point();
        #endregion

        #region 公共字段
        /// <summary>
        /// 当前页,从1开始
        /// </summary>
        public static int CurrentPage;
        /// <summary>
        /// 关闭标志,不为空则直接退出程序而不提示
        /// </summary>
        public static string closeFlag = "";
        /// <summary>
        /// 重命名状态的项目索引
        /// </summary>
        public static string RenameIndex;
        /// <summary>
        /// 重命名状态的项目所在页
        /// </summary>
        public static string RenamePage;
        /// <summary>
        /// 是否处于拖动状态
        /// </summary>
        public static bool dragItem = false;
        public KeyboardStart ks;
        #endregion

        /// <summary>
        /// 资源管理器重启消息
        /// </summary>
        uint taskbarCreated = 0;

        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {

            if (msg == API.WM_COPYDATA)
            {
                CopyDataStruct cds = (CopyDataStruct)System.Runtime.InteropServices.Marshal.PtrToStructure(lParam, typeof(CopyDataStruct));
                AddItem(cds.lpData, CurrentPage);
            }
            else if (msg == API.WM_HOTKEY)
            {
                if (wParam.ToInt32() == 100)
                {
                    if (this.IsActive)
                    {
                        this.menugrid.Visibility = Visibility.Hidden;
                        this.Hide();
                    }
                    else
                        this.Show();
                }
                else if (wParam.ToInt32() == 101)
                {
                    if (!moveleft.IsEnabled && !moveright.IsEnabled)
                        moveleft.IsEnabled = true;
                }
                else if (wParam.ToInt32() == 102)
                {
                    if (!moveleft.IsEnabled && !moveright.IsEnabled)
                        moveright.IsEnabled = true;
                }
                else if (wParam.ToInt32() == 105)
                {
                    if (ks.IsActive)
                        ks.Hide();
                    else
                        ks.Show();
                }
                else if (wParam.ToInt32() == 106)
                {
                    StartButton.positionSvc.SetAppPos();
                }

            }
            else if (msg == API.WM_QUERYENDSESSION)
                closeFlag = "systemShutDown";
            else if (msg == taskbarCreated)
            {
                RestartApp();
            }
            //else if (msg == API.WM_DISPLAYCHANGE)
            //{
            //    //AppPos pos;
            //    //API.CalcAppPos(this.Height, this.Width, out pos);
            //    //this.Left = pos.main_left;
            //    //this.Top = pos.main_top;
            //}
            //else
            //{
            //    StreamWriter sw = new StreamWriter("ttt.txt", true);
            //    sw.WriteLine(msg.ToString());
            //    sw.Close();
            //}

            return hwnd;
        }

        public MainWindow(double left_margin)
        {
            taskbarCreated = API.RegisterWindowMessage("TaskbarCreated");
            #region
            InitializeComponent();

            moveleft = new DispatcherTimer();
            moveleft.Interval = new TimeSpan(1);
            moveleft.Tick += new EventHandler(moveleft_Tick);
            moveright = new DispatcherTimer();
            moveright.Interval = new TimeSpan(1);
            moveright.Tick += new EventHandler(moveright_Tick);
            moveleftWhenDel = new DispatcherTimer();
            moverightWhenDel = new DispatcherTimer();
            moverightWhenDel.Interval = new TimeSpan(1);
            moveleftWhenDel.Interval = new TimeSpan(1);
            moveleftWhenDel.Tick += new EventHandler(moveleftWhenDel_Tick);
            moverightWhenDel.Tick += new EventHandler(moverightWhenDel_Tick);
            moverightWhenDel.IsEnabled = false;
            moveleftWhenDel.IsEnabled = false;
            moveright.IsEnabled = false;
            moveleft.IsEnabled = false;
            #endregion
            btnskin.Source = new BitmapImage(new Uri("pack://application:,,/images/skin1.png"));
            btnmenu.Source = new BitmapImage(new Uri("pack://application:,,/images/menu1.png"));
            btnedit.Source = new BitmapImage(new Uri("pack://application:,,/images/edit1.png"));
            menugrid.Visibility = Visibility.Hidden;
            this.Left = left_margin;

            if (RegWork.CheckFirstRun())
            {
                if (false)
                {
                    MyWork.startExePath.CheckRegRun();
                    MyWork.startExePath.AddRegMenu();
                }
            }
            if (!File.Exists(Path.Combine(MyWork.StartDir, "data.xml")))
                new XElement("Data").Save(Path.Combine(MyWork.StartDir, "data.xml"));

            SettingInfo tempset;
            MyWork.ConfigPath.ReadSetting(out tempset);
            currentBack = tempset.BackImg;
            SetBackImage(currentBack, false);

            UpdateItems();
            Dal.RepairData();
            PageCount = Dal.GetPageCount();
            PageCount = PageCount == 0 ? PageCount + 1 : PageCount;
            CurrentPage = 1;

            SetPanel(PageCount);
            UpdatePageControl(PageCount);
            UpdatePageControlChecked(CurrentPage);
            ks = new KeyboardStart(ReadData());
            this.Title += "i";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            (PresentationSource.FromVisual(this) as HwndSource).AddHook(new HwndSourceHook(WndProc));
        }

        #region 主要事件
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)/////准备拖动项目或结束编辑
        {
            if (editItem)/////处于编辑状态
            {
                var panel = maingrid.Children[CurrentPage - 1] as PanelControl;
                var ing = panel.InputHitTest(e.GetPosition(panel)) as FrameworkElement;
                var ing2 = topGrid.InputHitTest(e.GetPosition(topGrid)) as FrameworkElement;
                //if(ing2!=null)
                if (ing == null)////不是在maingrid点击
                {
                    if (ing2 == null || ing2.GetType().Equals(typeof(System.Windows.Controls.TextBlock)))
                        ChangeEdit();
                }
                else if (ing.Height > 20)////点击的不是移除按钮
                {
                    var border = ing.Parent as FrameworkElement;
                    var grid = border.Parent as FrameworkElement;
                    item = grid.Parent as ItemControl;

                    tempIndex = item.OverallIndex;
                    tempItem = new ItemControl(item.Path, item.DisplayName, item.PageIndex, item.OverallIndex);
                    tempItem.image_del.Visibility = Visibility.Visible;
                    tempItem.HorizontalAlignment = HorizontalAlignment.Left;
                    tempItem.VerticalAlignment = VerticalAlignment.Top;
                    item.image1.Source = null;
                    item.textBox1.Text = "";
                    item.image_del.Visibility = Visibility.Hidden;
                    int left, top;
                    if ((item.OverallIndex + 1) % 5 == 0)
                    {
                        left = 4 * 120;
                        top = (item.OverallIndex + 1) / 5 * 90 - 90;
                    }
                    else
                    {
                        left = ((item.OverallIndex + 1) % 5 - 1) * 120;
                        top = (item.OverallIndex + 1) / 5 * 90;
                    }
                    tempItem.Margin = new Thickness(left, top, 0, 0);
                    maingrid.Children.Add(tempItem);
                    dragItem = true;
                    mouseLocationBeforeDrag = e.GetPosition(this);
                    ItemoldMargin = new Point(tempItem.Margin.Left, tempItem.Margin.Top);
                }
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)  ////拖动
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed && dragItem)
            {
                var p = e.GetPosition(this);
                var offset = new Point(p.X - mouseLocationBeforeDrag.X, p.Y - mouseLocationBeforeDrag.Y);
                tempItem.Margin = new Thickness(ItemoldMargin.X + offset.X, ItemoldMargin.Y + offset.Y, 0, 0);

                int dragToIndex = GetDragOverIndex(e.GetPosition((maingrid.Children[CurrentPage - 1] as PanelControl).wrapPanel1));
                if (dragToIndex != -1 && dragToIndex != tempIndex)////拖动到新位置
                {
                    RegulateItem(dragToIndex);
                }
            }
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)////接受拖动的改变
        {
            base.OnMouseLeftButtonUp(e);
            if (dragItem)
            {
                maingrid.Children.Remove(tempItem);
                item.image1.Source = tempItem.image1.Source;
                item.DisplayName = tempItem.DisplayName;
                item.textBox1.Text = item.DisplayName;
                item.image_del.Visibility = Visibility.Visible;
                int i = 0;
                foreach (ItemControl ic in (maingrid.Children[CurrentPage - 1] as PanelControl).wrapPanel1.Children)
                {
                    ic.OverallIndex = i;
                    ic.Path.ChangeIndex(i);
                    i++;
                }
                dragItem = false;
            }
        }

        private void Window_MouseDown_1(object sender, MouseButtonEventArgs e)////结束重命名,隐藏菜单
        {
            var ing = topGrid.InputHitTest(e.GetPosition(topGrid)) as FrameworkElement;
            if (menugrid.InputHitTest(e.GetPosition(menugrid)) == null)////未点击label（菜单项）
            {
                if (ing != btnmenu && menugrid.Visibility == Visibility.Visible)
                    menugrid.Visibility = Visibility.Hidden;
            }

            if (RenamePage != null && RenameIndex != null)
            {
                var temp = (maingrid.Children[int.Parse(RenamePage) - 1] as PanelControl).wrapPanel1.Children[int.Parse(RenameIndex)] as ItemControl;
                temp.textBox1.IsReadOnly = true;
                temp.textBox1.BorderThickness = new Thickness(0);
                temp.DisplayName = temp.textBox1.Text;
                temp.image1.ToolTip = temp.DisplayName + "\n" + temp.Path;
                temp.textBox1.Select(0, 0);
                temp.Path.ChangeName(temp.DisplayName);
                RenameIndex = null;
                RenamePage = null;
            }
        }
        private void Window_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)//禁用主菜单"删除此页"
        {
            if (PageCount == 1)
                removepanle.IsEnabled = false;
            else
                removepanle.IsEnabled = true;
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!moveleft.IsEnabled && !moveright.IsEnabled)
            {
                if (e.Delta < 0 && CurrentPage != this.maingrid.Children.Count)  //////滚轮向下滚动
                {
                    moveleft.IsEnabled = true;
                }
                if (e.Delta > 0 && CurrentPage != 1)
                {
                    moveright.IsEnabled = true;
                }
            }
        }
        void moverightWhenDel_Tick(object sender, EventArgs e)
        {
            try
            {
                if (((PanelControl)this.maingrid.Children[CurrentPage - 2]).Margin.Left + 6 <= 0)
                {
                    for (int i = 0; i <= CurrentPage - 2; i++)
                    {
                        double tempMargin = ((PanelControl)this.maingrid.Children[i]).Margin.Left;
                        ((PanelControl)this.maingrid.Children[i]).Margin = new Thickness(tempMargin + 6, 0, 0, 0);
                    }
                }
                else
                {
                    moverightWhenDel.IsEnabled = false;
                    CurrentPage -= 1;
                    UpdatePageControl(PageCount);
                    UpdatePageControlChecked(CurrentPage);
                }
            }
            catch (Exception ee)
            {
                MyWork.NoteLog(ee);
            }
        }
        void moveleftWhenDel_Tick(object sender, EventArgs e)
        {
            try
            {
                if (((PanelControl)this.maingrid.Children[0]).Margin.Left - 6 >= 0)
                {
                    for (int i = 0; i < this.maingrid.Children.Count; i++)
                    {
                        double tempMargin = ((PanelControl)this.maingrid.Children[i]).Margin.Left;
                        ((PanelControl)this.maingrid.Children[i]).Margin = new Thickness(tempMargin - 6, 0, 0, 0);
                    }
                }
                else
                {
                    moveleftWhenDel.IsEnabled = false;
                    UpdatePageControl(PageCount);
                    UpdatePageControlChecked(CurrentPage);
                }
            }
            catch (Exception ee)
            {
                MyWork.NoteLog(ee);
            }
        }
        void moveright_Tick(object sender, EventArgs e)//向前翻页
        {
            try
            {
                if (((PanelControl)this.maingrid.Children[CurrentPage - 2]).Margin.Left + 6 <= 0)
                {
                    for (int i = 0; i < this.maingrid.Children.Count; i++)
                    {
                        double tempMargin = ((PanelControl)this.maingrid.Children[i]).Margin.Left;
                        ((PanelControl)this.maingrid.Children[i]).Margin = new Thickness(tempMargin + 6, 0, 0, 0);
                    }
                }
                else
                {
                    moveright.IsEnabled = false;
                    CurrentPage -= 1;
                    UpdatePageControlChecked(CurrentPage);
                }
            }
            catch (Exception ee)
            {
                MyWork.NoteLog(ee);
            }
        }
        void moveleft_Tick(object sender, EventArgs e)//向后翻页
        {
            try
            {
                if (((PanelControl)this.maingrid.Children[CurrentPage]).Margin.Left - 6 >= 0)
                {
                    for (int i = 0; i < this.maingrid.Children.Count; i++)
                    {
                        double tempMargin = ((PanelControl)this.maingrid.Children[i]).Margin.Left;
                        ((PanelControl)this.maingrid.Children[i]).Margin = new Thickness(tempMargin - 6, 0, 0, 0);
                    }
                }
                else
                {
                    moveleft.IsEnabled = false;
                    CurrentPage += 1;
                    UpdatePageControlChecked(CurrentPage);
                }
            }
            catch (Exception ee)
            {
                MyWork.NoteLog(ee);
            }
        }

        private void addfile_Click(object sender, RoutedEventArgs e)//右键菜单--添加文件
        {
            try
            {
                this.Deactivated -= new EventHandler(Window_Deactivated);
                System.Windows.Forms.OpenFileDialog of = new System.Windows.Forms.OpenFileDialog();
                of.Multiselect = true;
                of.DereferenceLinks = false;
                if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    foreach (string str in of.FileNames)
                        AddItem(str, CurrentPage);
                }
                this.Deactivated += new EventHandler(Window_Deactivated);
            }
            catch (Exception ee)
            {
                MyWork.NoteLog(ee);
            }
        }
        private void adddir_Click(object sender, RoutedEventArgs e)//右键菜单--添加文件夹
        {
            try
            {
                this.Deactivated -= new EventHandler(Window_Deactivated);
                System.Windows.Forms.FolderBrowserDialog fb = new System.Windows.Forms.FolderBrowserDialog();
                fb.ShowNewFolderButton = false;
                fb.Description = "选择要添加的目录";
                if (fb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    AddItem(fb.SelectedPath, CurrentPage);
                }
                this.Deactivated += new EventHandler(Window_Deactivated);
            }
            catch (Exception ee) { MyWork.NoteLog(ee); }
        }
        private void addpanle_Click(object sender, RoutedEventArgs e)/// 右键菜单--添加新面板
        {
            this.maingrid.Children.Add(new PanelControl((PageCount - CurrentPage + 1) * this.Width));
            this.pagingGrid.Children.Add(new PageControl(false));
            PageCount += 1;
            UpdatePageControl(PageCount);
        }
        private void removepanle_Click(object sender, RoutedEventArgs e)/// 右键菜单--删除面板
        {
            CurrentPage.Del();
            for (int i = CurrentPage; i < maingrid.Children.Count; i++)
            {
                for (int j = 0; j < ((PanelControl)maingrid.Children[i]).wrapPanel1.Children.Count; j++)
                {
                    ((ItemControl)((PanelControl)maingrid.Children[i]).wrapPanel1.Children[j]).PageIndex -= 1;
                }
            }
            Dal.UpdatePage(CurrentPage + 1);

            this.maingrid.Children.RemoveAt(CurrentPage - 1);
            this.pagingGrid.Children.RemoveAt(CurrentPage - 1);
            PageCount -= 1;
            if (CurrentPage != 1)
            {
                moverightWhenDel.IsEnabled = true;
            }
            if (CurrentPage == 1 && PageCount >= 1)
            {
                moveleftWhenDel.IsEnabled = true;
            }
        }
        private void enterEdit_Click(object sender, RoutedEventArgs e)//右键菜单--编辑
        {
            ChangeEdit();
        }
        private void Window_Drop(object sender, DragEventArgs e)/// 拖动文件到主面板并放下
        {
            string[] fileNames = (String[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string str in fileNames)
                AddItem(str, CurrentPage);
        }

        private void menuItemClick(object sender, MouseButtonEventArgs e)////主菜单
        {
            menugrid.Visibility = Visibility.Hidden;
            switch ((sender as System.Windows.Controls.Label).Content.ToString())
            {
                case "设置":
                    this.Deactivated -= new EventHandler(Window_Deactivated);
                    setForm = new Setting(this);
                    setForm.ShowDialog();
                    this.Deactivated += new EventHandler(Window_Deactivated);
                    break;
                case "导入图标":
                    this.Deactivated -= new EventHandler(Window_Deactivated);
                    //new ImportLnk(this).ShowDialog();
                    this.Deactivated += new EventHandler(Window_Deactivated);
                    break;
                case "帮助":
                    this.Deactivated -= new EventHandler(Window_Deactivated);
                    Help h = new Help(this);
                    h.ShowDialog();
                    this.Deactivated += new EventHandler(Window_Deactivated);
                    break;
                case "关于":
                    this.Deactivated -= new EventHandler(Window_Deactivated);
                    About aboutForm = new About(this);
                    aboutForm.ShowDialog();
                    this.Deactivated += new EventHandler(Window_Deactivated);
                    break;
                case "反馈":

                    break;
                case "退出":
                    closeFlag = "exit";
                    System.Windows.Application.Current.Shutdown();
                    break;
                default: break;
            }

        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (closeFlag == "")
                {
                    this.Deactivated -= new EventHandler(Window_Deactivated);
                    this.WindowState = WindowState.Normal;
                    if (MessageBox.Show("退出EasyStarter吗？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        Application.Current.Shutdown();
                    else
                    {
                        e.Cancel = true;
                        this.Deactivated += new EventHandler(Window_Deactivated);
                    }
                }
            }
            catch (Exception ee)
            {
                MyWork.NoteLog(ee);
            }
        }
        #endregion

        #region 其他事件
        //关机按钮
        private void labelShutDown_MouseEnter_1(object sender, MouseEventArgs e)
        {
            labelShutDown.BorderThickness = new Thickness(1);
        }
        private void labelShutDown_MouseLeave_1(object sender, MouseEventArgs e)
        {
            labelShutDown.BorderThickness = new Thickness(0);
        }
        private void labelShutDown_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            labelShutDown.BorderThickness = new Thickness(2);
        }
        private void labelShutDown_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            labelShutDown.BorderThickness = new Thickness(1);

        }


        //private void LR_MouseEnter(object sender, MouseEventArgs e)
        //{
        //   (sender as System.Windows.Controls.Label).BorderThickness = new Thickness(1);
        //}
        //private void LR_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    (sender as System.Windows.Controls.Label).BorderThickness = new Thickness(0);
        //}
        //private void LR_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    (sender as System.Windows.Controls.Label).BorderThickness = new Thickness(2);
        //}

        //private void LR_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    (sender as System.Windows.Controls.Label).BorderThickness = new Thickness(1);
        //    if (!moveright.IsEnabled && !moveleft.IsEnabled)
        //    {
        //        if (sender == labelNext)
        //            moveleft.IsEnabled = true;
        //        else if (sender == labelPre)
        //            moveright.IsEnabled = true;
        //    }
        //}


        //菜单项
        private void menuItem_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as System.Windows.Controls.Border).Background = Brushes.CadetBlue;
        }
        private void menuItem_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as System.Windows.Controls.Border).Background = null;
        }
        private void menuItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (sender as System.Windows.Controls.Border).Background = Brushes.DimGray;
        }
        private void menuItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (sender as System.Windows.Controls.Border).Background = null;
        }

        //换肤按钮
        private void btnskin_MouseEnter(object sender, MouseEventArgs e)
        {
            btnskin.Source = new BitmapImage(new Uri("pack://application:,,/images/skin2.png"));
        }
        private void btnskin_MouseLeave(object sender, MouseEventArgs e)
        {
            btnskin.Source = new BitmapImage(new Uri("pack://application:,,/images/skin1.png"));
        }
        private void btnskin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnskin.Source = new BitmapImage(new Uri("pack://application:,,/images/skin3.png"));
        }
        private void btnskin_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Skin skin = new Skin(currentBack);
            skin.changeBack += (this.SetBackImage);
            skin.Top = this.Top + 25;
            skin.Left = this.Left + 520;
            this.Deactivated -= new EventHandler(Window_Deactivated);
            skin.Show();
            this.Deactivated += new EventHandler(Window_Deactivated);
        }

        //编辑按钮
        private void btnedit_MouseEnter(object sender, MouseEventArgs e)
        {
            btnedit.Source = new BitmapImage(new Uri("pack://application:,,/images/edit2.png"));
        }
        private void btnedit_MouseLeave(object sender, MouseEventArgs e)
        {
            btnedit.Source = new BitmapImage(new Uri("pack://application:,,/images/edit1.png"));
        }
        private void btnedit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnedit.Source = new BitmapImage(new Uri("pack://application:,,/images/edit3.png"));
        }
        private void btnedit_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            btnedit.Source = new BitmapImage(new Uri("pack://application:,,/images/edit2.png"));
            ChangeEdit();
        }

        //菜单按钮
        private void btnmenu_MouseEnter(object sender, MouseEventArgs e)
        {
            btnmenu.Source = new BitmapImage(new Uri("pack://application:,,/images/menu2.png"));
        }
        private void btnmenu_MouseLeave(object sender, MouseEventArgs e)
        {
            btnmenu.Source = new BitmapImage(new Uri("pack://application:,,/images/menu1.png"));
        }
        private void btnmenu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnmenu.Source = new BitmapImage(new Uri("pack://application:,,/images/menu3.png"));
        }
        private void btnmenu_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            btnmenu.Source = new BitmapImage(new Uri("pack://application:,,/images/menu2.png"));

            menugrid.Visibility = menugrid.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            API.SetForegroundWindow(new System.Windows.Interop.WindowInteropHelper(this).Handle);
        }
        private void Window_Deactivated(object sender, EventArgs e)
        {
            if (RenamePage != null && RenameIndex != null)
            {
                var temp = ((PanelControl)maingrid.Children[int.Parse(RenamePage) - 1]).wrapPanel1.Children[int.Parse(RenameIndex)] as ItemControl;
                temp.textBox1.IsReadOnly = true;
                temp.textBox1.BorderThickness = new Thickness(0);
                temp.textBox1.Text = temp.image1.ToolTip.ToString().Split('\n')[0];
                temp.textBox1.Select(0, 0);
                RenameIndex = null;
                RenamePage = null;

            }
            this.menugrid.Visibility = Visibility.Hidden;
            this.Hide();
        }
        #endregion

        #region 基本方法
        private void RestartApp()
        {
            string file = "LoadStarter.exe";
            string path = Path.Combine(MyWork.StartDir, file);
            if (File.Exists(path))
                System.Diagnostics.Process.Start(path);
            else
            {
                MessageBox.Show("无法启动程序" + file + "，请手动重新启动程序。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            closeFlag = "exit";
            System.Windows.Application.Current.Shutdown();
        }

        /// <summary>
        /// 拖动时调整项目布局
        /// </summary>
        /// <param name="index">拖动到的位置</param>
        private void RegulateItem(int index)
        {
            (maingrid.Children[CurrentPage - 1] as PanelControl).wrapPanel1.Children.RemoveAt(tempIndex);
            (maingrid.Children[CurrentPage - 1] as PanelControl).wrapPanel1.Children.Insert(index, item);
            tempIndex = index;////更新为调整后的索引，用于持续拖动到下一个位置
        }
        /// <summary>
        /// 切换编辑模式
        /// </summary>
        private void ChangeEdit()
        {
            if (editItem)
            {
                editItem = false;
                enterEdit.Header = "进入编辑模式";
                foreach (PanelControl panel in maingrid.Children)
                {
                    foreach (ItemControl ic in panel.wrapPanel1.Children)
                    {
                        ic.image_del.Visibility = Visibility.Hidden;
                    }
                }
            }
            else
            {
                editItem = true;
                enterEdit.Header = "退出编辑模式";
                foreach (PanelControl panel in maingrid.Children)
                {
                    foreach (ItemControl ic in panel.wrapPanel1.Children)
                    {
                        ic.image_del.Visibility = Visibility.Visible;
                    }
                }
            }
        }
        /// <summary>
        /// 获取鼠标拖动到的项目索引
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private int GetDragOverIndex(Point p)
        {
            if (p.X % 120 > 0 && p.X % 120 < 120 && p.Y % 90 > 0 && p.Y % 90 < 90)
            {
                int result = (int)(p.Y / 90) * 5 + (int)(p.X / 120);
                if (result >= (maingrid.Children[CurrentPage - 1] as PanelControl).wrapPanel1.Children.Count)
                    result = (maingrid.Children[CurrentPage - 1] as PanelControl).wrapPanel1.Children.Count - 1;
                return result;
            }
            else return -1;
        }

        /// <summary>
        /// 设置背景图片
        /// </summary>
        /// <param name="url">图片路径</param>
        private void SetBackImage(string url, bool real)
        {
            string temp = "";
            if (url == "1" || url == "2" || url == "3" || url == "4")
                temp = "pack://application:,,/images/deback/back" + url + ".png";
            else if (File.Exists(url))
                temp = url;
            else
            {
                temp = "pack://application:,,/images/deback/back1.png";
                url = "1";
            }
            ImageBrush b = new ImageBrush();
            b.ImageSource = new BitmapImage(new Uri(temp));
            this.mainBorder.Background = b;
            if (real)
            {
                currentBack = url;
                url.SetBackConfig(MyWork.ConfigPath);
            }
        }

        /// <summary>
        /// 创建指定数目的面板
        /// </summary>
        /// <param name="count">要创建的面板个数</param>
        private void SetPanel(int count)
        {
            try
            {
                double marginLeft = this.Width;
                for (int i = 0; i < count; i++)
                {
                    PanelControl panel = new PanelControl(marginLeft * i);
                    PageControl page = new PageControl(false);
                    this.maingrid.Children.Add(panel);
                    this.pagingGrid.Children.Add(page);
                }
            }
            catch (Exception ee)
            {
                MyWork.NoteLog(ee);
            }
        }
        /// <summary>
        /// 更新分页标志选中状态
        /// </summary>
        /// <param name="currentPage">当前显示页面</param>
        private void UpdatePageControlChecked(int currentPage)
        {
            try
            {
                for (int i = 0; i < this.pagingGrid.Children.Count; i++)
                {
                    if (i + 1 == currentPage)
                    {
                        ((PageControl)this.pagingGrid.Children[i]).Checked = true;
                    }
                    else
                    {
                        ((PageControl)this.pagingGrid.Children[i]).Checked = false;
                    }
                }
            }
            catch (Exception ee)
            {
                MyWork.NoteLog(ee);
            }
        }
        /// <summary>
        /// 更新分页标志布局
        /// </summary>
        /// <param name="count">页面个数</param>
        private void UpdatePageControl(int count)
        {
            try
            {
                double firstMargin = (500 - 30 * count + 10) / 2;
                for (int i = 0; i < count; i++)
                {
                    ((PageControl)this.pagingGrid.Children[i]).Margin = new Thickness(firstMargin + 30 * i, 0, 0, 0);
                }
            }
            catch (Exception ee)
            {
                MyWork.NoteLog(ee);
            }
        }

        #region 前后移动项目
        ///// <summary>
        ///// 前后移动项目
        ///// </summary>
        ///// <param name="page">所在页面</param>
        ///// <param name="index">索引</param>
        ///// <param name="direction">移动方向,true为向前,false为向后</param>
        //private void MoveItemOnIndex(int page, int index, bool direction)
        //{
        //    if (direction && index != 0)
        //    {
        //        ItemControl temp = ((maingrid.Children[page - 1] as PanelControl).wrapPanel1.Children[index - 1]) as ItemControl;
        //        (maingrid.Children[page - 1] as PanelControl).wrapPanel1.Children.RemoveAt(index - 1);
        //        (maingrid.Children[page - 1] as PanelControl).wrapPanel1.Children.Insert(index, temp);
        //        temp.OverallIndex += 1;
        //        (((maingrid.Children[page - 1] as PanelControl).wrapPanel1.Children[index - 1]) as ItemControl).OverallIndex -= 1;
        //        Dal.ChangeIndex((((maingrid.Children[page - 1] as PanelControl).wrapPanel1.Children[index - 1]) as ItemControl).ItemPath, index - 1);
        //        Dal.ChangeIndex((((maingrid.Children[page - 1] as PanelControl).wrapPanel1.Children[index]) as ItemControl).ItemPath, index);
        //    }
        //    if (!direction && index != (maingrid.Children[page - 1] as PanelControl).wrapPanel1.Children.Count - 1)
        //    {
        //        ItemControl temp = ((maingrid.Children[page - 1] as PanelControl).wrapPanel1.Children[index]) as ItemControl;
        //        (maingrid.Children[page - 1] as PanelControl).wrapPanel1.Children.RemoveAt(index);
        //        (maingrid.Children[page - 1] as PanelControl).wrapPanel1.Children.Insert(index + 1, temp);
        //        temp.OverallIndex += 1;
        //        (((maingrid.Children[page - 1] as PanelControl).wrapPanel1.Children[index]) as ItemControl).OverallIndex = index;
        //        Dal.ChangeIndex((((maingrid.Children[page - 1] as PanelControl).wrapPanel1.Children[index + 1]) as ItemControl).ItemPath, index + 1);
        //        Dal.ChangeIndex((((maingrid.Children[page - 1] as PanelControl).wrapPanel1.Children[index]) as ItemControl).ItemPath, index);
        //    }
        //}
        #endregion
        /// <summary>
        /// 移动项目到前(后)页
        /// </summary>
        /// <param name="page">所在页面</param>
        /// <param name="index">索引</param>
        /// <param name="direction">移动方向,true为向前,false为向后</param>
        private void MoveItemOnPage(int page, int index, bool direction)
        {
            /////是否为此页最后一个
            bool flag = (maingrid.Children[page - 1] as PanelControl).wrapPanel1.Children.Count - 1 == index ? true : false;

            if (!direction)
            {
                #region
                if (page == maingrid.Children.Count)
                {
                    this.maingrid.Children.Add(new PanelControl((PageCount - CurrentPage + 1) * this.Width));
                    this.pagingGrid.Children.Add(new PageControl(false));
                    PageCount += 1;
                    UpdatePageControl(PageCount);
                    MoveItemOnPage(page, index, direction);
                }
                else
                {
                    ItemControl temp = (maingrid.Children[page - 1] as PanelControl).wrapPanel1.Children[index] as ItemControl;
                    (maingrid.Children[page - 1] as PanelControl).wrapPanel1.Children.RemoveAt(index);
                    if ((maingrid.Children[page] as PanelControl).wrapPanel1.Children.Count == 20)
                    {
                        MoveItemOnPage(page + 1, 19, false);/////后一页的最后一个
                    }
                    (maingrid.Children[page] as PanelControl).wrapPanel1.Children.Add(temp);
                    temp.OverallIndex = (maingrid.Children[page] as PanelControl).wrapPanel1.Children.Count - 1;
                    temp.PageIndex = page + 1;
                    temp.Path.ChangeIndex(temp.PageIndex, temp.OverallIndex);
                }
                #endregion
            }
            else if (CurrentPage != 1)
            {
                #region
                ItemControl temp = (maingrid.Children[page - 1] as PanelControl).wrapPanel1.Children[index] as ItemControl;
                (maingrid.Children[page - 1] as PanelControl).wrapPanel1.Children.RemoveAt(index);
                if ((maingrid.Children[page - 2] as PanelControl).wrapPanel1.Children.Count == 20)
                {
                    MoveItemOnPage(page - 1, 19, false);/////前一页的最后一个
                }
                (maingrid.Children[page - 2] as PanelControl).wrapPanel1.Children.Add(temp);
                temp.OverallIndex = (maingrid.Children[page - 2] as PanelControl).wrapPanel1.Children.Count - 1;
                temp.PageIndex = page - 1;
                temp.Path.ChangeIndex(temp.PageIndex, temp.OverallIndex);
                #endregion
            }
            if (!flag)
            {
                for (int i = index; i < (maingrid.Children[page - 1] as PanelControl).wrapPanel1.Children.Count; i++)
                {
                    ((maingrid.Children[page - 1] as PanelControl).wrapPanel1.Children[i] as ItemControl).OverallIndex -= 1;
                }
                page.UpdateIndex(index);
            }
        }

        /// <summary>
        /// 从面板删除快捷方式
        /// </summary>
        /// <param name="page">所在页面,从1开始</param>
        /// <param name="index">索引</param>
        private void DelItem(int page, int index)
        {
            try
            {
                int tempcount = PageCount;
                PanelControl tempPanel = (PanelControl)this.maingrid.Children[page - 1];
                string path = ((ItemControl)tempPanel.wrapPanel1.Children[index]).Path;
                ((PanelControl)this.maingrid.Children[page - 1]).wrapPanel1.Children.RemoveAt(index);
                path.Del();
                if (((PanelControl)this.maingrid.Children[page - 1]).wrapPanel1.Children.Count == 0)////如果此页已经没有图标
                {
                    #region
                    if (this.maingrid.Children.Count != 1)
                    {
                        this.maingrid.Children.RemoveAt(page - 1);
                        this.pagingGrid.Children.RemoveAt(page - 1);
                        PageCount -= 1;
                        page.UpdatePage();
                        if (page != 1)
                        {
                            moverightWhenDel.IsEnabled = true;
                        }
                        if (page == 1 && PageCount >= 1)
                        {
                            moveleftWhenDel.IsEnabled = true;
                        }
                    }
                    #endregion
                }
                else
                {
                    page.UpdateIndex(index);
                    for (int i = index; i < ((PanelControl)this.maingrid.Children[page - 1]).wrapPanel1.Children.Count; i++)
                    {
                        ((ItemControl)((PanelControl)this.maingrid.Children[page - 1]).wrapPanel1.Children[i]).OverallIndex--;
                    }
                }
            }
            catch (Exception ee)
            {
                MyWork.NoteLog(ee);
            }
        }
        /// <summary>
        /// 添加新快捷方式
        /// </summary>
        /// <param name="path">执行路径</param>
        /// <param name="page">添加到的面板</param>
        private void AddItem(string path, int page, params string[] args)
        {
            if (path.JudgeFileType() != FileType.None)
            {
                if (path.Substring(path.LastIndexOf('.') + 1, 3).ToLower() == "lnk")
                {
                    #region 若文件(夹)存在，使用真实路径递归
                    string truePath = path.getPathByLnk();
                    if (!File.Exists(truePath) && !Directory.Exists(truePath))
                    {
                        #region
                        this.Deactivated -= new EventHandler(Window_Deactivated);
                        MessageBox.Show("文件(夹)不存在或快捷方式信息错误", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Deactivated += new EventHandler(Window_Deactivated);
                        #endregion
                    }
                    else////文件(夹)存在，递归
                    {
                        string name = path.GetNameByPath();
                        //try
                        //{
                        //    File.Delete(path);
                        //}
                        //catch (Exception eee){ throw eee; }
                        AddItem(truePath, page, name);
                    }
                    #endregion
                }
                else  ////不是lnk文件
                {
                    if (!path.CheckExists())
                    {
                        int thiscount = ((PanelControl)this.maingrid.Children[page - 1]).wrapPanel1.Children.Count;
                        if (thiscount == 20)
                        {
                            #region 若此页已满,递归向下一页添加
                            page += 1;
                            if (this.maingrid.Children.Count < page)
                            {
                                this.maingrid.Children.Add(new PanelControl((PageCount - CurrentPage + 1) * this.Width));
                                this.pagingGrid.Children.Add(new PageControl(false));
                                PageCount += 1;
                                UpdatePageControl(PageCount);
                            }
                            if (args.Length == 0)
                                AddItem(path, page);
                            else AddItem(path, page, args[0]);
                            #endregion
                        }
                        else////此页未满
                        {
                            ItemControl ic = args.Length == 0 ? new ItemControl(path, page, thiscount) : new ItemControl(path, args[0], page, thiscount);
                            ic.del += new DelItem(DelItem);

                            ic.movePage += new MoveItemPage(MoveItemOnPage);
                            (this.maingrid.Children[page - 1] as PanelControl).wrapPanel1.Children.Add(ic);
                            Dal.Add(path, ic.DisplayName, ic.PageIndex, ic.OverallIndex, ic.ItemType);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 初始化每个面板的快捷方式信息
        /// </summary>
        private List<ItemInfo> ReadData()
        {
            try
            {
                List<ItemInfo> allItem = new List<ItemInfo>();

                for (int i = 1; i <= PageCount; i++)
                {
                    List<ItemInfo> itemList = i.GetData();
                    allItem.AddRange(itemList);
                    foreach (var info in itemList)
                    {
                        var ic = new ItemControl(info.ItemPath, info.ItemName, i, info.ItemIndex);
                        ((PanelControl)this.maingrid.Children[i - 1]).wrapPanel1.Children.Insert(ic.OverallIndex, ic);
                        ic.del += new DelItem(DelItem);

                        ic.movePage += new MoveItemPage(MoveItemOnPage);
                    }
                }
                return allItem;
            }
            catch (Exception ee)
            {
                MyWork.NoteLog(ee);
                return null;
            }
        }
        /// <summary>
        /// 判断快捷方式所指的文件(目录或驱动器)是否存在
        /// </summary>
        private void UpdateItems()
        {
            try
            {
                foreach (var info in Dal.GetData())
                {
                    if (info.ItemType == 1 && !File.Exists(info.ItemPath))
                    {
                        info.ItemPath.Del();
                        info.ItemPage.UpdateIndex(info.ItemIndex);
                    }
                    else if ((info.ItemType == 2 || info.ItemType == 3) && !Directory.Exists(info.ItemPath))
                    {
                        info.ItemPath.Del();
                        info.ItemPage.UpdateIndex(info.ItemIndex);
                    }
                }
            }
            catch (Exception ee)
            {
                MyWork.NoteLog(ee);
            }
        }
        #endregion


    }
}
