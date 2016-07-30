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
            InitializeComponent();
            Loaded += MainWindow_Loaded;
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
