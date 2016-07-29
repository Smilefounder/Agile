using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UME_browser.Helpers;

namespace UME_browser
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            AllowDrop = true;
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            DragEnter += MainWindow_DragEnter;
            Drop += MainWindow_Drop;
        }

        private void MainWindow_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
        }

        private void MainWindow_Drop(object sender, DragEventArgs e)
        {
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files == null || files.Length == 0)
            {
                return;
            }

            _addressTextBox.Text = "file:///" + files[0];
            Navigate();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CoreHelper._foreGrid = _foreGrid;
            CoreHelper._mainGrid = _mainGrid;
        }

        private void _navigateButton_Click(object sender, RoutedEventArgs e)
        {
            Navigate();
        }

        private void _addressTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Navigate();
            }
        }

        private void Navigate()
        {
            var address = _addressTextBox.Text.Trim();
            if (string.IsNullOrEmpty(address))
            {
                _addressTextBox.Focus();
                return;
            }

            CoreHelper.Navigate(address);
        }

        private void _copyScreenButton_Click(object sender, RoutedEventArgs e)
        {
            var filename = CoreHelper.CopyFromScreen(Convert.ToInt32(Left), Convert.ToInt32(Top), Convert.ToInt32(Width), Convert.ToInt32(Height));
            CoreHelper.ShowMessage("截图已保存到桌面：" + filename);
        }
    }
}
