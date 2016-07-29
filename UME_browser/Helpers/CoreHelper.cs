using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UME_browser.UserControls;

namespace UME_browser.Helpers
{
    public class CoreHelper
    {
        public static Grid _foreGrid;

        public static Grid _mainGrid;

        public static void Render(HtmlDocument doc)
        {
            var br = new Border();
            br.Background = System.Windows.Media.Brushes.Gray;
            _mainGrid.Children.Add(br);
        }

        private static Dictionary<string, Action> _keywords = null;

        private static Dictionary<string, Action> Keywords
        {
            get
            {
                if (_keywords == null)
                {
                    _keywords = new Dictionary<string, Action>();
                    _keywords.Add("ume:about", About);
                }

                return _keywords;
            }
        }

        public static void Navigate(string address)
        {
            var pattern1 = "^http://([0-9a-zA-Z]+[.])+[0-9a-zA-Z]+";
            if (Regex.IsMatch(address, pattern1))
            {
                LoadHtml(address);
                return;
            }

            var pattern2 = "^file:///(.+)";
            if (Regex.IsMatch(address, pattern2))
            {
                var m = Regex.Match(address, pattern2);
                LoadExplorer(m.Groups[1].Value);
                return;
            }

            if (!Keywords.ContainsKey(address))
            {
                MessageBox.Show("无效的指令");
                return;
            }

            var act = Keywords[address];
            act.Invoke();
        }

        public static void LoadHtml(string url)
        {
            var html = new WebClient().DownloadString(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            Render(doc);
        }

        public static void LoadExplorer(string url)
        {
            if (System.IO.Directory.Exists(url))
            {
                _mainGrid.Children.Clear();
                _mainGrid.Children.Add(new ExplorerUserControl(url));
                return;
            }

            if (System.IO.File.Exists(url))
            {
                _mainGrid.Children.Clear();
                _mainGrid.Children.Add(new FileContentUserControl(url));
                return;
            }
        }

        public static void About()
        {
            MessageBox.Show("UME_browser");
        }

        public static string CopyFromScreen(int left, int top, int wt, int ht)
        {
            var img = new Bitmap(wt, ht);
            var g = Graphics.FromImage(img);
            var source = new System.Drawing.Point(left, top);
            var destination = new System.Drawing.Point(0, 0);
            var size = new System.Drawing.Size(wt, ht);
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var filename = String.Format("UM{0}.jpg", DateTime.Now.ToString("yyyyMMddHHmmss"));
            var fullname = System.IO.Path.Combine(new string[] { desktop, filename });
            g.CopyFromScreen(source, destination, size);
            img.Save(fullname, ImageFormat.Jpeg);
            return filename;
        }

        public static void ShowMessage(string message)
        {
            var tbk = new TextBlock();
            tbk.Foreground = System.Windows.Media.Brushes.White;
            tbk.Text = message;

            var br = new Border();
            br.Background = System.Windows.Media.Brushes.Black;
            br.Child = tbk;
            br.Padding = new Thickness(15, 5, 15, 5);
            br.HorizontalAlignment = HorizontalAlignment.Center;
            br.VerticalAlignment = VerticalAlignment.Top;

            _foreGrid.Children.Clear();
            _foreGrid.Children.Add(br);

            Thread t = new Thread(new ThreadStart(new Action(() =>
            {
                Thread.Sleep(1000);
                _foreGrid.Dispatcher.Invoke(new Action(() =>
                {
                    _foreGrid.Children.Clear();
                }));
            })));
            t.IsBackground = true;
            t.Start();
        }
    }
}
