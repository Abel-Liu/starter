using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Starter
{
    /// <summary>
    /// 
    /// </summary>
    public partial class About : Window
    {
        BitmapSource[] CloseBtStyle;
        public About(Window p)
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

        private void Window_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {

        }

        private void closebutton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void closebutton_MouseEnter(object sender, MouseEventArgs e)
        {
            this.MouseLeftButtonDown -= new MouseButtonEventHandler(Window_MouseLeftButtonDown_1);
            this.closebutton.Source = CloseBtStyle[1];
        }

        private void closebutton_MouseLeave(object sender, MouseEventArgs e)
        {
            this.closebutton.Source = CloseBtStyle[0];
            this.MouseLeftButtonDown += new MouseButtonEventHandler(Window_MouseLeftButtonDown_1);
        }

        private void closebutton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.closebutton.Source = CloseBtStyle[2];
        }

        private void ook_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void link_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.MouseLeftButtonDown -= new MouseButtonEventHandler(Window_MouseLeftButtonDown_1);
            System.Diagnostics.Process.Start( "https://github.com/Abel-Liu/starter" );
            this.MouseLeftButtonDown += new MouseButtonEventHandler(Window_MouseLeftButtonDown_1);
        }
    }
}
