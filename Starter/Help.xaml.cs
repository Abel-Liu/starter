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

namespace Starter
{
    /// <summary>
    /// Help.xaml 的交互逻辑
    /// </summary>
    public partial class Help : Window
    {
        BitmapSource[] CloseBtStyle;
        public Help(Window p)
        {
            InitializeComponent();
            this.Owner = p;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            CloseBtStyle = new BitmapSource[3];
            CloseBtStyle[0] = new BitmapImage(new Uri("pack://application:,,/images/close1.png"));
            CloseBtStyle[1] = new BitmapImage(new Uri("pack://application:,,/images/close2.png"));
            CloseBtStyle[2] = new BitmapImage(new Uri("pack://application:,,/images/close3.png"));
            this.closebutton.Source = CloseBtStyle[0];
            ImageBrush b = new ImageBrush();
            b.ImageSource = new BitmapImage(new Uri("pack://application:,,/images/deback/back1.png"));
            this.border.Background = b;
        }

        private void closebutton_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void closebutton_MouseEnter_1(object sender, MouseEventArgs e)
        {
            this.MouseLeftButtonDown -= new MouseButtonEventHandler(Window_MouseLeftButtonDown_1);
            this.closebutton.Source = CloseBtStyle[1];
        }

        private void closebutton_MouseLeave_1(object sender, MouseEventArgs e)
        {
            this.closebutton.Source = CloseBtStyle[0];
            this.MouseLeftButtonDown += new MouseButtonEventHandler(Window_MouseLeftButtonDown_1);
        }

        private void closebutton_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            this.closebutton.Source = CloseBtStyle[2];
        }

        private void ook_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
