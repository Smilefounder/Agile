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
using System.Windows.Threading;

namespace KeyUpDisplay
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Topmost = true;
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
            Background = null;
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            KeyUp += Window_KeyUp;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Left = 0;
            this.Top = 0;
            this.Height = SystemParameters.WorkArea.Height;
            this.Width = SystemParameters.WorkArea.Width;

            var timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromSeconds(0.1);
            timer1.Tick += timer1_Tick;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_lastKeyUpAt.HasValue == false)
            {
                return;
            }

            var dt = DateTime.Now;
            var ts = dt - _lastKeyUpAt.Value;
            if (ts.TotalSeconds < 1)
            {
                return;
            }

            this.Dispatcher.Invoke(new Action(() =>
            {
                _wrapPanel1.Children.Clear();
            }));
        }

        private DateTime? _lastKeyUpAt = null;

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            var dt = DateTime.Now;
            var btn = new Button();
            btn.Content = GetKeyStr(e.Key);
            _wrapPanel1.Children.Add(btn);
            _lastKeyUpAt = dt;
        }

        private string GetKeyStr(Key k)
        {
            switch (k)
            {
                default:
                    return k.ToString();
                case Key.D0:
                    return "0";
                case Key.D1:
                    return "1";
                case Key.D2:
                    return "2";
                case Key.D3:
                    return "3";
                case Key.D4:
                    return "4";
                case Key.D5:
                    return "5";
                case Key.D6:
                    return "6";
                case Key.D7:
                    return "7";
                case Key.D8:
                    return "8";
                case Key.D9:
                    return "9";
                case Key.Capital:
                    return "Caps Lock";
                case Key.Escape:
                    return "Esc";
            }
        }
    }
}
