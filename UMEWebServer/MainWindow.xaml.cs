using Agile.Helpers;
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
using UMEWebServer.Helpers;
using UMEWebServer.Models;

namespace UMEWebServer
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
            DisplayWebsiteList();
        }

        private void DisplayWebsiteList()
        {
            _websiteListStackPanel.Children.Clear();
            _websiteListStackPanel.Children.Add(CreateTableHeader());

            var list = CoreHelper.GetWebsiteList();
            foreach (var item in list)
            {
                _websiteListStackPanel.Children.Add(CreateListItem(item));
            }
        }

        private Border CreateTableHeader()
        {
            var gd = new Grid();
            var col = new ColumnDefinition();
            gd.ColumnDefinitions.Add(col);

            var tbk = new TextBlock();
            tbk.Text = "站点";
            gd.Children.Add(tbk);
            Grid.SetColumn(tbk, 0);

            col = new ColumnDefinition();
            gd.ColumnDefinitions.Add(col);

            tbk = new TextBlock();
            tbk.Text = "端口";
            gd.Children.Add(tbk);
            Grid.SetColumn(tbk, 1);

            col = new ColumnDefinition();
            gd.ColumnDefinitions.Add(col);

            tbk = new TextBlock();
            tbk.Text = "操作";
            gd.Children.Add(tbk);
            Grid.SetColumn(tbk, 2);

            var br = new Border();
            br.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 231, 231, 231));
            br.BorderThickness = new Thickness(0, 0, 0, 1);
            br.Child = gd;
            return br;
        }

        private Border CreateListItem(Website model)
        {
            var gd = new Grid();
            var col = new ColumnDefinition();
            gd.ColumnDefinitions.Add(col);

            var tbk = new TextBlock();
            tbk.Text = model.Name;
            gd.Children.Add(tbk);
            Grid.SetColumn(tbk, 0);

            col = new ColumnDefinition();
            gd.ColumnDefinitions.Add(col);

            tbk = new TextBlock();
            tbk.Text = model.Port.ToString();
            gd.Children.Add(tbk);
            Grid.SetColumn(tbk, 1);

            col = new ColumnDefinition();
            gd.ColumnDefinitions.Add(col);

            tbk = new TextBlock();
            tbk.Text = "启动";
            tbk.MouseLeftButtonUp += tbk_MouseLeftButtonUp;
            gd.Children.Add(tbk);
            Grid.SetColumn(tbk, 2);

            var br = new Border();
            br.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 231, 231, 231));
            br.BorderThickness = new Thickness(0, 0, 0, 1);
            br.Child = gd;
            br.Tag = model;
            return br;
        }

        private void tbk_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var tbk = sender as TextBlock;
            if (tbk == null)
            {
                return;
            }

            var gd = tbk.Parent as Grid;
            if (gd == null)
            {
                return;
            }

            var br = gd.Parent as Border;
            if (br == null)
            {
                return;
            }

            var website = br.Tag as Website;
            if (website == null)
            {
                return;
            }

            var listener = new SocketHelper(website.IPAddress, website.Port);
            listener.DataReceived += listener_DataReceived;
        }

        private void listener_DataReceived(byte[] buffer, int length)
        {
            var request = Encoding.UTF8.GetString(buffer, 0, length);
            LogHelper.Write(request);
        }

        private void _saveOrUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            int port;
            if (int.TryParse(_websitePortTextBox.Text, out port) == false)
            {
                MessageBox.Show("端口号必须填数字");
                return;
            }

            CoreHelper.NewWebsite(new Website
            {
                Directory = _websiteDirectoryTextBox.Text,
                Domain = _websiteDomainTextBox.Text,
                IPAddress = _websiteIPAddressTextBox.Text,
                Port = port,
                Name = _websiteNameTextBox.Text
            });

            _mainGrid.IsEnabled = true;
            _backgroundMaskBorder.Visibility = Visibility.Collapsed;
            _saveOrUpdateWebsiteBorder.Visibility = Visibility.Collapsed;

            DisplayWebsiteList();
        }

        private void _createNewWebsiteButton_Click(object sender, RoutedEventArgs e)
        {
            _mainGrid.IsEnabled = false;
            _backgroundMaskBorder.Visibility = Visibility.Visible;
            _saveOrUpdateWebsiteBorder.Visibility = Visibility.Visible;
        }
    }
}
