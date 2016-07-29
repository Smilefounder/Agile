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

namespace UME_browser.UserControls
{
    /// <summary>
    /// FileContentUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class FileContentUserControl : UserControl
    {
        private FileContentUserControl()
        {
            InitializeComponent();
            Loaded += FileContentUserControl_Loaded;
        }

        public FileContentUserControl(string filepath)
        {
            _filepath = filepath;
            InitializeComponent();
            Loaded += FileContentUserControl_Loaded;
        }

        private string _filepath;

        private string[] _imgs = new string[] { ".jpg" };

        private void FileContentUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_filepath))
            {
                return;
            }

            var ext = System.IO.Path.GetExtension(_filepath);
            ext = ext.ToLower();

            if (_imgs.Contains(ext))
            {
                var brush = new ImageBrush(new BitmapImage(new Uri(_filepath)));
                var rect = new Rectangle();
                rect.Fill = brush;
                _mainGrid.Children.Clear();
                _mainGrid.Children.Add(rect);
                return;
            }


            var content = System.IO.File.ReadAllText(_filepath, Encoding.Default);
            var tbx = new TextBox();
            tbx.Text = content;
            _mainGrid.Children.Clear();
            _mainGrid.Children.Add(tbx);
        }
    }
}
