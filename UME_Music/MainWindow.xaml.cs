using System;
using System.Windows;
using System.Windows.Input;
using UME_Music.Helpers;
using UME_Music.Models;
using UME_Music.UserControls;

namespace UME_Music
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

        private void MainWindow_Drop(object sender, DragEventArgs e)
        {
            var paths = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (System.IO.Directory.Exists(paths[0]))
            {
                try
                {
                    CoreHelper.AddMusicFromDirectory(paths[0]);
                }
                catch (Exception ex)
                {
                    LogHelper.WriteAsync(ex.ToString());
                    UIHelper.ShowMessage(ex.Message);
                }

                return;
            }

            if (System.IO.File.Exists(paths[0]))
            {
                try
                {
                    CoreHelper.AddMusicFromFile(paths[0]);
                    CoreHelper.Play(paths[0]);
                }
                catch (Exception ex)
                {
                    LogHelper.WriteAsync(ex.ToString());
                    UIHelper.ShowMessage(ex.Message);
                }

                return;
            }
        }

        private void MainWindow_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CoreHelper.PlayerStateChanged += CoreHelper_PlayerStateChanged;
            CoreHelper.PlayerMediaChanged += CoreHelper_PlayerMediaChanged;

            DisplayPlayMode();
            LoadAllMusicUserControl();

            UIHelper._backgroundBorder = _backgroundBorder;
            UIHelper._mainBorder = _mainBorder;
            UIHelper._maskBorder = _maskBorder;
            UIHelper._dialogBorder = _dialogBorder;
            UIHelper._messageBorder = _messageBorder;
            UIHelper._menuGrid = _menuGrid;
            UIHelper._userControlGrid = _userControlGrid;
        }

        private void CoreHelper_PlayerMediaChanged()
        {
            var fp = CoreHelper.CurrentMusic.FilePath;
            if (string.IsNullOrEmpty(fp))
            {
                fp = "柚萌音乐";
            }

            Title = fp;

            _musicTitleTextBlock.Text = CoreHelper.CurrentMusic.TitleDisplay;
            _musicArtistsTextBlock.Text = CoreHelper.CurrentMusic.ArtistsDisplay;
        }

        private void CoreHelper_PlayerStateChanged()
        {
            if (CoreHelper.PlayerState != (int)PlayerStateEnum.Playing)
            {
                _playPauseTextBlock.Text = "播放";
                return;
            }

           _playPauseTextBlock.Text = "暂停";

            if (CoreHelper.Player.NaturalDuration.HasTimeSpan)
            {
                var duration = CoreHelper.Player.NaturalDuration.TimeSpan;
                var position = CoreHelper.Player.Position;
                var percent = position.TotalSeconds / duration.TotalSeconds;
                Dispatcher.Invoke(new Action(() =>
                {
                    _positionBorder.Width = _durationBorder.Width * percent;
                    _positionTextBlock.Text = StringHelper.GetTimeSpanStr(position);
                    _durationTextBlock.Text = StringHelper.GetTimeSpanStr(duration);
                }));
            }
        }

        private void LoadAllMusicUserControl()
        {
            _userControlGrid.Children.Clear();
            _userControlGrid.Children.Add(AllMusicUserControl.Instance);
        }

        private void _allMusicBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LoadAllMusicUserControl();
        }

        private void _playRecordBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LoadPlayRecordUserControl();
        }

        private void LoadPlayRecordUserControl()
        {
            _userControlGrid.Children.Clear();
            _userControlGrid.Children.Add(PlayRecordUserControl.Instance);
        }

        private void _playThePrevTextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CoreHelper.PlayThePrev();
        }

        private void _playPauseTextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CoreHelper.PlayPause();
        }

        private void _playTheNextTextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CoreHelper.PlayTheNext();
        }

        private void _playModeTextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CoreHelper.SetPlayMode();
            DisplayPlayMode();
        }

        /// <summary>
        /// 显示播放模式
        /// </summary>
        private void DisplayPlayMode()
        {
            var mode = CoreHelper.PlayerConfig.PlayMode.GetValueOrDefault();
            switch (mode)
            {
                case (int)PlayModeEnum.Order:
                    {
                        _playModeTextBlock.Text = "顺序播放";
                    }
                    break;
                case (int)PlayModeEnum.Random:
                    {
                        _playModeTextBlock.Text = "随机播放";
                    }
                    break;
                case (int)PlayModeEnum.Recycle:
                    {
                        _playModeTextBlock.Text = "列表循环";
                    }
                    break;
                case (int)PlayModeEnum.Repeat:
                    {
                        _playModeTextBlock.Text = "单曲循环";
                    }
                    break;
            }
        }
    }
}
