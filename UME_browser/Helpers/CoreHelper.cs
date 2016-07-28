using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UME_browser.Helpers
{
    public class CoreHelper
    {
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

        public static void About()
        {
            MessageBox.Show("UME_browser");
        }
    }
}
