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

namespace UME_browser.UserControls
{
    /// <summary>
    /// ExplorerUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class ExplorerUserControl : UserControl
    {
        public ExplorerUserControl()
        {
            InitializeComponent();
            Loaded += ExplorerUserControl_Loaded;
        }

        public ExplorerUserControl(string baseDirectory)
        {
            _baseDirectory = baseDirectory;

            InitializeComponent();
            Loaded += ExplorerUserControl_Loaded;
        }

        private string _baseDirectory;

        private void ExplorerUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_baseDirectory))
            {
                _baseDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }

            DisplaySubDirectory(_baseDirectory);
        }

        private void DisplaySubDirectory(string directory)
        {
            _itemlistStackPanel.Children.Clear();

            var folders = System.IO.Directory.GetDirectories(directory);
            if (folders != null && folders.Length > 0)
            {
                foreach (var f in folders)
                {
                    _itemlistStackPanel.Children.Add(CreateItem(f));
                }
            }

            var files = System.IO.Directory.GetFiles(directory);
            if (files != null && files.Length > 0)
            {
                foreach (var f in files)
                {
                    _itemlistStackPanel.Children.Add(CreateItem(f));
                }
            }
        }

        private Border CreateItem(string directory)
        {
            var tbk = new TextBlock();
            tbk.Text = directory;

            var br = new Border();
            br.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
            br.BorderThickness = new Thickness(0, 1, 0, 1);
            br.Padding = new Thickness(5);
            br.Tag = directory;
            br.Child = tbk;
            br.MouseLeftButtonDown += Br_MouseLeftButtonDown;
            return br;

        }

        private void Br_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var br = sender as Border;
            if (br == null)
            {
                return;
            }

            var path = br.Tag.ToString();
            if (e.ClickCount == 1)
            {
                if (br.Background != null)
                {
                    br.Background = null;
                    return;
                }

                br.Background = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
                return;
            }

            if (System.IO.Directory.Exists(path))
            {
                DisplaySubDirectory(path);
                return;
            }

            if (System.IO.File.Exists(path))
            {
                CoreHelper._mainGrid.Children.Clear();
                CoreHelper._mainGrid.Children.Add(new FileContentUserControl(path));
                return;
            }
        }
    }
}
