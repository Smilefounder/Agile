using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UMEBrowser
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void _navigateToButton_Click(object sender, RoutedEventArgs e)
        {
            var address = _addressTextBox.Text.Trim();
            if (address.Length == 0)
            {
                _addressTextBox.Focus();
                return;
            }

            _webBrowser1.Navigate(address);
        }

        private void _backButton_Click(object sender, RoutedEventArgs e)
        {
            if (_webBrowser1.CanGoBack)
            {
                _webBrowser1.GoBack();
            }
        }

        private void _refreshButton_Click(object sender, RoutedEventArgs e)
        {
            _webBrowser1.Refresh();
        }

        private void _copyFromScreenButton_Click(object sender, RoutedEventArgs e)
        {
            var g1 = Graphics.FromHwnd(new WindowInteropHelper(this).Handle);
            var s1 = new System.Drawing.Size(Convert.ToInt32(this.Width), Convert.ToInt32(this.Height));
            var img1 = new Bitmap(s1.Width, s1.Height, g1);
            var g2 = Graphics.FromImage(img1);
            g2.CopyFromScreen(Convert.ToInt32(this.Left), Convert.ToInt32(this.Top), 0, 0, s1);

            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var filename = DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
            var fullname = System.IO.Path.Combine(new string[] { desktop, filename });
            img1.Save(fullname, System.Drawing.Imaging.ImageFormat.Jpeg);

            MessageBox.Show("已保存截图到桌面");
        }
    }
}
