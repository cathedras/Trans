using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace myzy.AgCustom
{
    public class OrdinalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var oridinal = 0;
            var lvi = value as ListViewItem;
            if (lvi != null)
            {
                var lv = ItemsControl.ItemsControlFromItemContainer(lvi) as ListView;
                if (lv != null)
                    oridinal = lv.ItemContainerGenerator.IndexFromContainer(lvi) + 1;
            }
            return oridinal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}