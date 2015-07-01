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
    public partial class ImportLnk : Window
    {
        public ImportLnk(Window owner)
        {
            InitializeComponent();
            this.Owner = owner;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.main.Background = Brushes.Black;
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
