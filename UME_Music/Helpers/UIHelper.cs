using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UME_Music.Helpers
{
    public class UIHelper
    {
        public static Border _backgroundBorder;

        public static Border _mainBorder;

        public static Border _maskBorder;

        public static Border _dialogBorder;

        public static Grid _menuGrid;

        public static Grid _userControlGrid;

        public static void ShowDialog<T>(T child)
            where T : UserControl
        {
            _dialogBorder.Child = child;
            _dialogBorder.HorizontalAlignment = HorizontalAlignment.Center;
            _dialogBorder.VerticalAlignment = VerticalAlignment.Top;
            _dialogBorder.Background = Brushes.White;
            _dialogBorder.Visibility = Visibility.Visible;
            _maskBorder.Background = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0));
            _maskBorder.Visibility = Visibility.Visible;
        }

        public static void ResetDialog()
        {
            _dialogBorder.Child = null;
            _dialogBorder.Visibility = Visibility.Collapsed;
            _maskBorder.Background = null;
            _maskBorder.Visibility = Visibility.Collapsed;
        }
    }

    public enum UIDialogButtonEnum
    {
        Yes = 0,

        No = 1,

        Close = 2,

        Ok = 3,

        Confirm = 4,

        Cancel = 5
    }
}
