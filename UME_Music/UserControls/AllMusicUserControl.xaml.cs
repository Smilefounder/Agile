using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
            //显示歌曲列表
            DisplayMusicList();

            CoreHelper.PlayerMediaChanged += CoreHelper_PlayerMediaChanged;
        }

        private void CoreHelper_PlayerMediaChanged()
        {
            foreach (var child in _itemlistStackPanel.Children)
            {
                var br = child as Border;
                if (br == null)
                {
                    continue;
                }

                if (br.Tag == null)
                {
                    continue;
                }

                var filepath = br.Tag.ToString();
                if (string.IsNullOrEmpty(filepath))
                {
                    continue;
                }

                if (filepath == CoreHelper.CurrentMusic.FilePath)
                {
                    br.BorderBrush = Brushes.Green;
                }
                else
                {
                    br.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
                }
            }
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
            var chooseFile = new ChooseFileUserControl();
            chooseFile.Width = 525;
            chooseFile.Height = 350;
            chooseFile.Choosed += ChooseFile_Choosed;
            UIHelper.ShowDialog<ChooseFileUserControl>(chooseFile);
        }

        private void ChooseFile_Choosed(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                return;
            }

            try
            {
                CoreHelper.AddMusicFromFile(filepath);
                DisplayMusicList();
            }
            catch (Exception ex)
            {
                UIHelper.ShowMessage(ex.Message);
            }
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
            //找到目录下的文件
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

            //找到目录下的目录
            var folders = System.IO.Directory.GetDirectories(directory);
            if (folders != null && folders.Any())
            {
                foreach (var f in folders)
                {
                    FetchFile(f);
                }
            }
        }

        /// <summary>
        /// 显示歌曲列表
        /// </summary>
        private void DisplayMusicList()
        {
            //显示歌曲总数
            _recordcountTextBlock.Text = string.Format("共{0}首歌曲", CoreHelper.Musiclist.Count);

            //先清空列表
            if (_itemlistStackPanel.Children.Count > 0)
            {
                _itemlistStackPanel.Children.Clear();
            }

            //添加列表头
            _itemlistStackPanel.Children.Add(CreateDataGridHeader());

            //逐个添加列表项
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
            tbk1.Text = music.TitleDisplay;
            gd.Children.Add(tbk1);
            Grid.SetColumn(tbk1, 0);

            var tbk2 = new TextBlock();
            tbk2.Text = music.ArtistsDisplay;
            gd.Children.Add(tbk2);
            Grid.SetColumn(tbk2, 1);

            var tbk3 = new TextBlock();
            tbk3.Text = music.AlbumDisplay;
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

        private Border _choosedBorder = null;

        private void Br_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var br = sender as Border;
            if (br == null)
            {
                return;
            }

            var filepath = br.Tag as string;
            if (e.ClickCount == 1)
            {
                if (br.Background != null)
                {
                    br.Background = null;
                    _choosedBorder = null;
                    return;
                }

                br.Background = new SolidColorBrush(Color.FromArgb(255, 231, 231, 231));
                if (_choosedBorder != null)
                {
                    _choosedBorder.Background = null;
                }

                _choosedBorder = br;
                return;
            }

            if (e.ClickCount == 2)
            {
                CoreHelper.Play(filepath);
                if (_choosedBorder != null)
                {
                    _choosedBorder.Background = null;
                }

                _choosedBorder = null;
                return;
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
            if (_choosedBorder == null)
            {
                UIHelper.ShowMessage("请选择一首歌曲");
                return;
            }

            if (_choosedBorder.Tag == null)
            {
                return;
            }

            var path = _choosedBorder.Tag.ToString();
            if (!string.IsNullOrEmpty(path))
            {
                CoreHelper.RemoveMusic(path);
                DisplayMusicList();
            }
        }

        private void _removeAllTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CoreHelper.Musiclist.Clear();
            CoreHelper.IsMusicListChanged = true;
            DisplayMusicList();
        }
    }
}
