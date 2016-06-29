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
using UME_Music.Helpers;

namespace UME_Music.UserControls
{
    /// <summary>
    /// ChooseDirectoryUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class ChooseDirectoryUserControl : UserControl
    {
        public delegate void ChoosedEventHandler(string directory);

        public event ChoosedEventHandler Choosed;

        public ChooseDirectoryUserControl() : this("")
        {
        }

        public ChooseDirectoryUserControl(string baseDirectory)
        {
            _baseDirectory = baseDirectory;

            InitializeComponent();
            Loaded += ChooseDirectoryUserControl_Loaded;
        }

        private string _baseDirectory;

        private void ChooseDirectoryUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_baseDirectory))
            {
                _baseDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            }

            DisplaySubDirectory(_baseDirectory);

            _choosedDirectoryTextBlock.Text = _baseDirectory;
        }

        private void DisplaySubDirectory(string directory)
        {
            _addressTextBox.Text = directory;
            _itemlistStackPanel.Children.Clear();

            var folders = System.IO.Directory.GetDirectories(directory);
            if (folders == null || folders.Count() == 0)
            {
                return;
            }

            foreach (var folder in folders)
            {
                _itemlistStackPanel.Children.Add(CreateItem(folder));
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

            var directory = br.Tag.ToString();
            if (e.ClickCount == 1)
            {
                if (br.Background != null)
                {
                    _choosedDirectoryTextBlock.Text = "";
                    br.Background = null;
                    return;
                }

                _choosedDirectoryTextBlock.Text = directory;
                br.Background = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
                return;
            }

            DisplaySubDirectory(directory);
        }

        private void _yesButton_Click(object sender, RoutedEventArgs e)
        {
            var choosedFolder = _addressTextBox.Text;
            if (!string.IsNullOrEmpty(_choosedDirectoryTextBlock.Text))
            {
                choosedFolder = _choosedDirectoryTextBlock.Text;
            }

            if (Choosed != null)
            {
                Choosed.Invoke(choosedFolder);
            }

            UIHelper.ResetDialog();
        }

        private void _closeButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
