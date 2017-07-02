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

namespace Starter
{
    /// <summary>
    /// 面板类
    /// </summary>
    public partial class PanelControl : UserControl
    {
        /// <summary>
        /// 初始化面板并指定位置
        /// </summary>
        /// <param name="leftMargin">左边距</param>
        public PanelControl(double leftMargin)
        {
            InitializeComponent();
            this.Margin = new Thickness(leftMargin, 0, 0, 0);
        }
    }
}
