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
using UME_Music.Models;

namespace UME_Music.UserControls
{
    /// <summary>
    /// AllMusicUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class AllMusicUserControl : UserControl
    {
        private AllMusicUserControl()
        {
            InitializeComponent();
            Loaded += AllMusicUserControl_Loaded;
        }

        private void AllMusicUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayMusicList();
        }

        private static AllMusicUserControl _instance;

        public static AllMusicUserControl Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AllMusicUserControl();
                }

                return _instance;
            }
        }

        public static void Reload()
        {
            _instance = new AllMusicUserControl();
        }

        private void _addFromFileTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void _addFromDirectoryTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var chooseDirectory = new ChooseDirectoryUserControl();
            chooseDirectory.Width = 525;
            chooseDirectory.Height = 350;
            chooseDirectory.Choosed += ChooseDirectory_Choosed;
            UIHelper.ShowDialog<ChooseDirectoryUserControl>(chooseDirectory);
        }

        private void ChooseDirectory_Choosed(string directory)
        {
            if (string.IsNullOrEmpty(directory))
            {
                return;
            }

            FetchFile(directory);

            DisplayMusicList();
        }

        private void FetchFile(string directory)
        {
            var files = System.IO.Directory.GetFiles(directory, "*.*", System.IO.SearchOption.TopDirectoryOnly);
            if (files != null && files.Any())
            {
                foreach (var f in files)
                {
                    try
                    {
                        CoreHelper.AddMusicFromFile(f);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Write(f + ": " + ex.Message);
                    }
                }
            }

            var folders = System.IO.Directory.GetDirectories(directory);
            if (folders != null && folders.Any())
            {
                foreach (var f in folders)
                {
                    FetchFile(f);
                }
            }
        }

        private void DisplayMusicList()
        {
            if (_itemlistStackPanel.Children.Count > 0)
            {
                _itemlistStackPanel.Children.Clear();
            }

            _itemlistStackPanel.Children.Add(CreateDataGridHeader());

            foreach (var music in CoreHelper.Musiclist)
            {
                _itemlistStackPanel.Children.Add(CreateDataGridBody(music));
            }
        }

        private Border CreateDataGridBody(T_music music)
        {
            var gd = new Grid();

            var col1 = new ColumnDefinition();
            gd.ColumnDefinitions.Add(col1);

            var col2 = new ColumnDefinition();
            gd.ColumnDefinitions.Add(col2);

            var col3 = new ColumnDefinition();
            gd.ColumnDefinitions.Add(col3);

            var tbk1 = new TextBlock();
            tbk1.Text = music.Title;
            gd.Children.Add(tbk1);
            Grid.SetColumn(tbk1, 0);

            var tbk2 = new TextBlock();
            tbk2.Text = music.Artists;
            gd.Children.Add(tbk2);
            Grid.SetColumn(tbk2, 1);

            var tbk3 = new TextBlock();
            tbk3.Text = music.Album;
            gd.Children.Add(tbk3);
            Grid.SetColumn(tbk3, 2);

            var br = new Border();
            br.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
            br.BorderThickness = new Thickness(0, 0, 0, 1);
            br.Padding = new Thickness(5);
            br.Child = gd;
            br.Tag = music.FilePath;
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

            var filepath = br.Tag as string;
            if (e.ClickCount == 2)
            {
                CoreHelper.Play(filepath);
            }
        }

        private Border CreateDataGridHeader()
        {
            var gd = new Grid();

            var col1 = new ColumnDefinition();
            gd.ColumnDefinitions.Add(col1);

            var col2 = new ColumnDefinition();
            gd.ColumnDefinitions.Add(col2);

            var col3 = new ColumnDefinition();
            gd.ColumnDefinitions.Add(col3);

            var tbk1 = new TextBlock();
            tbk1.Text = "歌曲";
            gd.Children.Add(tbk1);
            Grid.SetColumn(tbk1, 0);

            var tbk2 = new TextBlock();
            tbk2.Text = "歌手";
            gd.Children.Add(tbk2);
            Grid.SetColumn(tbk2, 1);

            var tbk3 = new TextBlock();
            tbk3.Text = "专辑";
            gd.Children.Add(tbk3);
            Grid.SetColumn(tbk3, 2);

            var br = new Border();
            br.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
            br.BorderThickness = new Thickness(0, 1, 0, 1);
            br.Padding = new Thickness(5);
            br.Child = gd;
            return br;
        }

        private void _removeSelectedTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void _removeAllTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
