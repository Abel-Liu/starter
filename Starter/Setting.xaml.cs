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
using System.Windows.Shapes;
using System.IO;
using ESTool;

namespace Starter
{
    public partial class Setting : Window
    {
        #region 私有字段
        /// <summary>
        /// 设置是否更改,更改为true,否则为false
        /// </summary>
        private static bool changeFlag;
        /// <summary>
        /// 新设置
        /// </summary>
        private SettingInfo tempSet;
        #endregion

        public Setting(Window p)
        {
            InitializeComponent();
            this.Owner = p;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            changeFlag = false;

            MyWork.ConfigPath.ReadSetting(out tempSet);
            checkBoxBoot.IsChecked = tempSet.Boot;
            checkBoxrightmenu.IsChecked = tempSet.SysRightMenu;
            checkBoxsend.IsChecked = tempSet.SendBug;
            #region
            this.checkBoxBoot.Checked += new RoutedEventHandler(checkBoxBoot_Checked);
            this.checkBoxBoot.Unchecked += new RoutedEventHandler(checkBoxBoot_Unchecked);
            this.checkBoxrightmenu.Checked += new RoutedEventHandler(checkBoxrightmenu_Checked);
            this.checkBoxrightmenu.Unchecked += new RoutedEventHandler(checkBoxrightmenu_Unchecked);
            this.checkBoxsend.Checked+=new RoutedEventHandler(checkBoxsend_Checked);
            this.checkBoxsend.Unchecked+=new RoutedEventHandler(checkBoxsend_Unchecked);
            #endregion
            //this.textBox1.Text = tempSet.HotKey;
            ImageBrush b = new ImageBrush();
            b.ImageSource = new BitmapImage(new Uri("pack://application:,,/images/deback/back1.png"));
            this.borderback.Background = b;
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 执行更改并保存配置
        /// </summary>
        private void AcceptSetting(SettingInfo newSet)
        {
            if (newSet.Boot)
                MyWork.startExePath.CheckRegRun();
            else
                RegWork.DelRegRun();

            if (newSet.SysRightMenu)
                MyWork.startExePath.AddRegMenu();
            else
                RegWork.DelRegMenu();
            //hotkey

            newSet.SaveToConfig(MyWork.ConfigPath);
        }

        /// <summary>
        /// 关闭设置窗口
        /// </summary>
        /// <returns></returns>
        private void CloseWindow()
        {
            if (changeFlag)
            {
                MessageBoxResult result = MessageBox.Show("是否保存设置？", "提示", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    AcceptSetting(tempSet);
                    this.Close();
                }
                else if (result == MessageBoxResult.No)
                {
                    this.Close();
                }
            }
            else
            {
                this.Close();
            }
        }
       
        private void esc_Click_1(object sender, RoutedEventArgs e)/// 取消按钮
        {
            changeFlag = false;
            this.Close();
        }
       
        private void save_Click_1(object sender, RoutedEventArgs e)/// 保存按钮
        {
            AcceptSetting(tempSet);
            this.Close();
        }
        private void close_Click(object sender, RoutedEventArgs e)////关闭按钮
        {
            CloseWindow();
        }

        #region 其他事件
        private void Window_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void checkBoxBoot_Checked(object sender, RoutedEventArgs e)
        {
            changeFlag = true;
            tempSet.Boot = true;
        }

        private void checkBoxBoot_Unchecked(object sender, RoutedEventArgs e)
        {
            changeFlag = true;
            tempSet.Boot = false;
        }

        private void checkBoxrightmenu_Checked(object sender, RoutedEventArgs e)
        {
            changeFlag = true;
            tempSet.SysRightMenu = true;
        }

        private void checkBoxrightmenu_Unchecked(object sender, RoutedEventArgs e)
        {
            changeFlag = true;
            tempSet.SysRightMenu = false;
        }
        private void checkBoxsend_Checked(object sender, RoutedEventArgs e)
        {
            changeFlag = true;
            tempSet.SendBug = true;
        }

        private void checkBoxsend_Unchecked(object sender, RoutedEventArgs e)
        {
            changeFlag = true;
            tempSet.SendBug = false;
        }
        #endregion
    }
}
