using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;

namespace InfoLabWPF.MVVM.View
{
    public partial class Lab2 : UserControl
    {
        public Lab2()
        {
            InitializeComponent();
        }
    }

    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrWhiteSpace(value as string) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is Visibility visibility && visibility == Visibility.Visible) ? string.Empty : null;
        }
    }
}