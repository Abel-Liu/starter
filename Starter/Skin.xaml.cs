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

namespace Starter
{
    public partial class Skin : Window
    {
        public ChangeBackImage changeBack;
        private string oldBack;

        public Skin(string spath)
        {
            InitializeComponent();
            oldBack = spath;
            skinpanel.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,/images/deback/back1.png")));
            image1.Source = new BitmapImage(new Uri("pack://application:,,/images/deback/back1.png"));
            image2.Source = new BitmapImage(new Uri("pack://application:,,/images/deback/back2_pre.png"));
            image3.Source = new BitmapImage(new Uri("pack://application:,,/images/deback/back3_pre.png"));
            image4.Source = new BitmapImage(new Uri("pack://application:,,/images/deback/back4_pre.png"));
            addskin.Source = new BitmapImage(new Uri("pack://application:,,/images/add1.png"));
        }

        private void image1_MouseLeave(object sender, MouseEventArgs e)
        {
            b1.BorderThickness = new Thickness(0);
            changeBack(oldBack, false);
        }
        private void image2_MouseLeave(object sender, MouseEventArgs e)
        {
            b2.BorderThickness = new Thickness(0);
            changeBack(oldBack, false);
        }
        private void image3_MouseLeave(object sender, MouseEventArgs e)
        {
            b3.BorderThickness = new Thickness(0);
            changeBack(oldBack, false);
        }
        private void image4_MouseLeave(object sender, MouseEventArgs e)
        {
            b4.BorderThickness = new Thickness(0);
            changeBack(oldBack, false);
        }
        private void addskin_MouseEnter(object sender, MouseEventArgs e)
        {
            addskin.Source = new BitmapImage(new Uri("pack://application:,,/images/add2.png"));
        }
        private void addskin_MouseLeave(object sender, MouseEventArgs e)
        {
            addskin.Source = new BitmapImage(new Uri("pack://application:,,/images/add1.png"));
        }
        private void addskin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            addskin.Source = new BitmapImage(new Uri("pack://application:,,/images/add3.png"));
        }
        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Close();
        }
        private void image1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            b1.BorderThickness = new Thickness(2);
        }
        private void image2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            b2.BorderThickness = new Thickness(2);
        }
        private void image3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            b3.BorderThickness = new Thickness(2);
        }
        private void image4_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            b4.BorderThickness = new Thickness(2);
        }

        private void image1_MouseEnter(object sender, MouseEventArgs e)
        {
            changeBack("1", false);
        }
        private void image2_MouseEnter(object sender, MouseEventArgs e)
        {
            changeBack("2", false);
        }
        private void image3_MouseEnter(object sender, MouseEventArgs e)
        {
            changeBack("3", false);
        }
        private void image4_MouseEnter(object sender, MouseEventArgs e)
        {
            changeBack("4", false);
        }
        private void image4_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Deactivated -= new EventHandler(Window_Deactivated);
            image4.MouseLeave -= new MouseEventHandler(image4_MouseLeave);
            changeBack("4", true);
            this.Close();
        }
        private void image3_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Deactivated -= new EventHandler(Window_Deactivated);
            image3.MouseLeave -= new MouseEventHandler(image3_MouseLeave);
            changeBack("3", true);
            this.Close();
        }
        private void image2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Deactivated -= new EventHandler(Window_Deactivated);
            image2.MouseLeave -= new MouseEventHandler(image2_MouseLeave);
            changeBack("2", true);
            this.Close();
        }
        private void image1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Deactivated -= new EventHandler(Window_Deactivated);
            image1.MouseLeave -= new MouseEventHandler(image1_MouseLeave);
            changeBack("1", true);
            this.Close();
        }
        private void addskin_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Deactivated -= new EventHandler(Window_Deactivated);
            System.Windows.Forms.OpenFileDialog of = new System.Windows.Forms.OpenFileDialog();
            of.Filter = "图像文件|*.gif;*.jpg;*.jpeg;*.png;*.bmp";
            if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string tempname = MyWork.StartDir + "image\\UsBack\\" + of.FileName.Substring(of.FileName.LastIndexOf('\\') + 1);
                if (!Directory.Exists(MyWork.StartDir + "image\\UsBack"))
                    Directory.CreateDirectory(MyWork.StartDir + "image\\UsBack");
                File.Copy(of.FileName, tempname, true);
                changeBack(tempname, true);
                this.Close();
            }
            else this.Close();
        }
    }
}
