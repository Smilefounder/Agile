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
using UME_Music.ViewModels;

namespace UME_Music.UserControls
{
    /// <summary>
    /// PlayRecordUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class PlayRecordUserControl : UserControl
    {
        public PlayRecordUserControl()
        {
            InitializeComponent();
            Loaded += PlayRecordUserControl_Loaded;
        }

        private void PlayRecordUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //显示播放记录
            DisplayPlayRecord();
        }

        /// <summary>
        /// 显示播放记录
        /// </summary>
        private void DisplayPlayRecord()
        {
            //显示播放记录数
            _recordcountTextBlock.Text = string.Format("共{0}次播放记录", CoreHelper.PlayRecords.Count);

            //先清空列表
            if (_itemlistStackPanel.Children.Count > 0)
            {
                _itemlistStackPanel.Children.Clear();
            }

            //添加列表头
            _itemlistStackPanel.Children.Add(CreateDataGridHeader());

            var playRecords = from p in CoreHelper.PlayRecords
                              from m in CoreHelper.Musiclist
                              where m.FilePath == p.FilePath
                              select new PlayRecordVM
                              {
                                  Artists = m.ArtistsDisplay,
                                  FilePath = m.FilePath,
                                  PlayedAt = p.CreatedAt,
                                  Title = m.TitleDisplay
                              };

            //逐个添加列表项
            foreach (var play in playRecords)
            {
                _itemlistStackPanel.Children.Add(CreateDataGridBody(play));
            }
        }

        private Border CreateDataGridBody(PlayRecordVM play)
        {
            var gd = new Grid();

            var col1 = new ColumnDefinition();
            gd.ColumnDefinitions.Add(col1);

            var col2 = new ColumnDefinition();
            gd.ColumnDefinitions.Add(col2);

            var col3 = new ColumnDefinition();
            gd.ColumnDefinitions.Add(col3);

            var tbk1 = new TextBlock();
            tbk1.Text = play.Title;
            gd.Children.Add(tbk1);
            Grid.SetColumn(tbk1, 0);

            var tbk2 = new TextBlock();
            tbk2.Text = play.Artists;
            gd.Children.Add(tbk2);
            Grid.SetColumn(tbk2, 1);

            var tbk3 = new TextBlock();
            tbk3.Text = play.PlayedAt.ToString("yyyy-MM-dd HH:mm:ss");
            gd.Children.Add(tbk3);
            Grid.SetColumn(tbk3, 2);

            var br = new Border();
            br.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
            br.BorderThickness = new Thickness(0, 0, 0, 1);
            br.Padding = new Thickness(5);
            br.Child = gd;
            br.Tag = play.FilePath;
            return br;
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
            tbk3.Text = "播放于";
            gd.Children.Add(tbk3);
            Grid.SetColumn(tbk3, 2);

            var br = new Border();
            br.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
            br.BorderThickness = new Thickness(0, 1, 0, 1);
            br.Padding = new Thickness(5);
            br.Child = gd;
            return br;
        }

        private static PlayRecordUserControl _instance;

        public static PlayRecordUserControl Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PlayRecordUserControl();
                }

                return _instance;
            }
        }

        public static void Reload()
        {
            _instance = new PlayRecordUserControl();
        }
    }
}
