using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TspUtil;

namespace TspUtil
{
    public static class ArgbItemHelper
    {
        public static List<ArgbItem> PraseArgbText(string str)
        {
            var lst = new List<ArgbItem>();

            //构建验证正则表达式
            var regex = new Regex(@"([r|g|b|a])(\d*)[;|,|\s]??", RegexOptions.IgnoreCase);
            var matchs = regex.Matches(str);
            foreach (Match match in matchs)
            {
                var item = new ArgbItem()
                {
                    FullDes = match.Groups[0].Value,
                    ArgbValue = (Argb) Enum.Parse(typeof(Argb), match.Groups[1].Value, true),
                    Idx = Convert.ToInt32(match.Groups[2].Value),
                };
                lst.Add(item);
            }

            return lst;
        }
    }

    public class ArgbItem
    {
        public string FullDes { get; set; }

        public Argb ArgbValue { get; set; }

        public int Idx { get; set; }

        public override string ToString()
        {
            return $"{FullDes}";
        }
    }

    public enum Argb
    {
        B,
        G,
        R,
        A,
    }

}
