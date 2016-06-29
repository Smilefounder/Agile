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

namespace UME_Music.UserControls
{
    /// <summary>
    /// RecentPlayUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class RecentPlayUserControl : UserControl
    {
        private RecentPlayUserControl()
        {
            InitializeComponent();
        }

        private static RecentPlayUserControl _instance;

        public static RecentPlayUserControl Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RecentPlayUserControl();
                }

                return _instance;
            }
        }

        public static void Reload()
        {
            _instance = new RecentPlayUserControl();
        }
    }
}
