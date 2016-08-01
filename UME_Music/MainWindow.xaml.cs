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
                    LogHelper.Write(ex.ToString());
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
                    LogHelper.Write(ex.ToString());
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
            LoadAllMusicUserControl();

            CoreHelper.PlayerStateChanged += CoreHelper_PlayerStateChanged;
            CoreHelper.PlayerMediaChanged += CoreHelper_PlayerMediaChanged;

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
        }

        private void CoreHelper_PlayerStateChanged()
        {
            if (!CoreHelper.PlayerState.HasValue)
            {
                return;
            }

            switch (CoreHelper.PlayerState.Value)
            {
                case (int)PlayerStateEnum.Paused:
                    {

                    }
                    break;
                case (int)PlayerStateEnum.Playing:
                    {

                    }
                    break;
                case (int)PlayerStateEnum.Ready:
                    {

                    }
                    break;
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
    }
}
