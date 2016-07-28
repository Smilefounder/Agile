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
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
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
    }
}
