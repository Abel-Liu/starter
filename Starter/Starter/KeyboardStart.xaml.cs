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
using ESTool;

namespace Starter
{
    /// <summary>
    /// KeyboardStart.xaml 的交互逻辑
    /// </summary>
    public partial class KeyboardStart : Window
    {
        List<ListBoxItem> allListBoxItem = new List<ListBoxItem>();

        public KeyboardStart(List<ItemInfo> itemList)
        {
            InitializeComponent();
            ImageBrush b = new ImageBrush();
            b.ImageSource = new BitmapImage(new Uri("pack://application:,,/images/deback/back1.png"));
            this.borderback.Background = b;
            
            foreach (var item in itemList)
            {
                ListBoxItem li = new ListBoxItem();
                li.Content = item.ItemName;
                li.Tag = item.ItemPath;
                this.list_item.Items.Add(li);
                allListBoxItem.Add(li);
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            tBox_input.Focus();
            tBox_input.Text = string.Empty;
            this.list_item.SelectedIndex = -1;
        }

        private void Filter(string input)
        { }

        private void tBox_input_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            if (list_item != null && list_item.Items.Count > 0)
            {
                list_item.SelectedIndex = 0;
                
            }
        }

        private void list_item_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems!=null&&e.AddedItems.Count>0)
            tBox_statu.Text = (list_item.SelectedItem as ListBoxItem).Tag.ToString();
        }

        private void tBox_input_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "\r")
            { }
        }
    }
}
