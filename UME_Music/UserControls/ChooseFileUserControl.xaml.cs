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
    /// ChooseFileUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class ChooseFileUserControl : UserControl
    {
        public delegate void ChoosedEventHandler(string filepath);

        public event ChoosedEventHandler Choosed;

        public ChooseFileUserControl() : this("")
        {
        }

        public ChooseFileUserControl(string baseDirectory)
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
        }

        private void DisplaySubDirectory(string directory)
        {
            _addressTextBox.Text = directory;
            _choosedFileTextBlock.Text = "";
            _choosedBorder = null;
            _itemlistStackPanel.Children.Clear();

            var folders = System.IO.Directory.GetDirectories(directory);
            if (folders != null && folders.Count() > 0)
            {
                foreach (var f in folders)
                {
                    _itemlistStackPanel.Children.Add(CreateItem(f));
                }
            }

            var files = System.IO.Directory.GetFiles(directory);
            if (files != null && files.Count() > 0)
            {
                foreach (var f in files)
                {
                    _itemlistStackPanel.Children.Add(CreateItem(f));
                }
            }
        }

        private Border CreateItem(string path)
        {
            var tbk = new TextBlock();
            tbk.Text = path;

            var br = new Border();
            br.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
            br.BorderThickness = new Thickness(0, 1, 0, 1);
            br.Padding = new Thickness(5);
            br.Tag = path;
            br.Child = tbk;
            br.MouseLeftButtonDown += Br_MouseLeftButtonDown;
            return br;

        }

        private Border _choosedBorder = null;

        private void Br_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var br = sender as Border;
            if (br == null)
            {
                return;
            }

            var path = br.Tag.ToString();
            if (e.ClickCount == 1 && System.IO.File.Exists(path))
            {
                if (br.Background != null)
                {
                    _choosedBorder = null;
                    _choosedFileTextBlock.Text = "";
                    br.Background = null;
                    return;
                }

                _choosedFileTextBlock.Text = path;
                br.Background = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
                if (_choosedBorder != null)
                {
                    _choosedBorder.Background = null;
                }

                _choosedBorder = br;
                return;
            }

            if (System.IO.Directory.Exists(path))
            {
                DisplaySubDirectory(path);
            }
        }

        private void _yesButton_Click(object sender, RoutedEventArgs e)
        {
            var choosedFile = _choosedFileTextBlock.Text;
            if (Choosed != null && !string.IsNullOrEmpty(choosedFile))
            {
                Choosed.Invoke(choosedFile);
            }

            UIHelper.ResetDialog();
        }

        private void _closeButton_Click(object sender, RoutedEventArgs e)
        {
            UIHelper.ResetDialog();
        }

        private void _addressTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }

            var path = _addressTextBox.Text.Trim();
            if (System.IO.Directory.Exists(path))
            {
                DisplaySubDirectory(path);
            }
        }
    }
}
