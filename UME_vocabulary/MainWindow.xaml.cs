using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using UME_vocabulary.Helpers;

namespace UME_vocabulary
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
            ShowInTaskbar = false;
            Topmost = true;
            Height = 30;
            Width = SystemParameters.FullPrimaryScreenWidth;
            Top = 0;
            Left = 0;

            //开始显示单词
            DisplayNextVocabulary();

            //从配置读取显示间隔
            var ts = 2.0;
            var configfile = System.IO.Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory,"config.txt"});
            if (System.IO.File.Exists(configfile))
            {
                var lines = System.IO.File.ReadAllLines(configfile);
                if (lines != null && lines.Any())
                {
                    var tsstr = lines[0];
                    tsstr = tsstr.Replace("ts=","").Trim();

                    var ts2 = default(double);
                    if (double.TryParse(tsstr, out ts2))
                    {
                        ts = ts2;
                    }
                }
            }

            //启动定时器定时显示单词
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(ts);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            DisplayNextVocabulary();
        }

        private void DisplayNextVocabulary()
        {
            var model = CoreHelper.NextOne();
            if (model == null)
            {
                return;
            }

            this.DataContext = model;
            _vocabularyTextBlock.Text = model.ToString();
        }

        private void _rememberedTextBlock_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var model = this.DataContext as string;
            if (model == null)
            {
                return;
            }

            CoreHelper.RememberOne(model);
            DisplayNextVocabulary();
        }

        private void _exitTextBlock_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Close();
        }
    }
}
