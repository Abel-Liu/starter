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
    /// 分页按钮
    /// </summary>
    public partial class PageControl : UserControl
    {
        /// <summary>
        /// 以指定选中状态初始化分页按钮
        /// </summary>
        /// <param name="_checked">是否选中</param>
        public PageControl(bool _checked)
        {
            InitializeComponent();
            this.Checked = _checked;
        }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool Checked
        {
            set { this.inside.Visibility = value ? Visibility.Visible : Visibility.Hidden; }
            get { return this.inside.IsVisible; }
        }
    }
}
