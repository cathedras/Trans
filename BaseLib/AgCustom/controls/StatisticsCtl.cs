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
using myzy.Util;

namespace myzy.AgCustom
{
    public class StatisticsCtl : Control
    {
        static StatisticsCtl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StatisticsCtl), new FrameworkPropertyMetadata(typeof(StatisticsCtl)));
        }

        public StatisticsManage StatisticsManage
        {
            get { return (StatisticsManage)GetValue(StatisticsManageProperty); }
            set { SetValue(StatisticsManageProperty, value); }
        }

        private ICommand _resetCountCmd;
        public ICommand ResetCountCmd
        {
            get
            {
                return _resetCountCmd ?? (_resetCountCmd = new RelayCommand(o =>
                {
                    if (o is int)
                    {
                        var idx = (int)o;
                        StatisticsManage.ClearStatics(idx);
                    }
                }));
            }
        }

        // Using a DependencyProperty as the backing store for StatisticsManage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatisticsManageProperty =
            DependencyProperty.Register("StatisticsManage", typeof(StatisticsManage), typeof(StatisticsCtl), new PropertyMetadata(new StatisticsManage(2)));

        public void SetItemCount(int itemCount)
        {
            if (itemCount > 0 && itemCount != StatisticsManage.Count)
            {
                StatisticsManage = new StatisticsManage(itemCount);
            }
        }
    }
}
