using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TspUtil
{
    public class NetCommandParam : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string name = string.Empty;
            switch ((string)parameter)
            {
                case "NetCmd":
                    name = values[0] + "" + values[1];
                    break;
                default:
                    name = "";
                    break;
            }
            return name;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class CmdParam
    {
        public string CommandText { get; set; }
        public string ParameterText { get; set; }
    }
}
