using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace myzy.AgCustom
{
    [ValueConversion(typeof(int), typeof(int))]
    public class IdxDispCvt : IValueConverter
    {
        public IdxDispCvt()
        {
            BaseIdx = 1;
        }
        public int BaseIdx { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                return (int) value + BaseIdx;
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                return (int) value - BaseIdx;
            }
            return DependencyProperty.UnsetValue;
        }
    }

    public class IdxDispCvtForSfg : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                return $"S{(int)value + 1:D3}";
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}