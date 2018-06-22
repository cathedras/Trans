using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
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
using System.Windows.Threading;
using myzy.Util;
using Xceed.Wpf.Toolkit.Core.Utilities;

namespace myzy.AgCustom
{
    public class LogItemViewEx : Control
    {
        static LogItemViewEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LogItemViewEx), new FrameworkPropertyMetadata(typeof(LogItemViewEx)));
        }

        public ObservableCollection<LogItem> MsgLogItems
        {
            get { return (ObservableCollection<LogItem>)GetValue(MsgLogItemsProperty); }
            set { SetValue(MsgLogItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MsgLogItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MsgLogItemsProperty =
            DependencyProperty.Register("MsgLogItems", typeof(ObservableCollection<LogItem>), typeof(LogItemViewEx), new PropertyMetadata(new ObservableCollection<LogItem>(), PropertyChangedCallback));

        public bool AutoScrollItems
        {
            get { return (bool)GetValue(AutoScrollItemsProperty); }
            set { SetValue(AutoScrollItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AutoScrollItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoScrollItemsProperty =
            DependencyProperty.Register("AutoScrollItems", typeof(bool), typeof(LogItemViewEx), new PropertyMetadata(true));
        
        public int AutoScrollLevel
        {
            get { return (int)GetValue(AutoScrollLevelProperty); }
            set { SetValue(AutoScrollLevelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AutoScrollLevel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoScrollLevelProperty =
            DependencyProperty.Register("AutoScrollLevel", typeof(int), typeof(LogItemViewEx), new PropertyMetadata(1));
        
        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LogItemViewEx logItemViewEx)
            {
                logItemViewEx.MsgLogItems.CollectionChanged -= LogItemChangedEventHandler(logItemViewEx);
                logItemViewEx.MsgLogItems.CollectionChanged += LogItemChangedEventHandler(logItemViewEx);
            }
        }

        private static NotifyCollectionChangedEventHandler LogItemChangedEventHandler(LogItemViewEx logItemViewEx)
        {
            return (sender, args) =>
            {
                if (VisualTreeHelperEx.FindDescendantByName(logItemViewEx, @"logListView") is ListView lstViewItem)
                {
                    if (logItemViewEx.AutoScrollItems && logItemViewEx._isLogListAutoScroll)
                    {
                        var data = (ObservableCollection<LogItem>) logItemViewEx.GetValue(MsgLogItemsProperty);
                        var lastestOne = data.LastOrDefault();

                        if (lastestOne != null && lastestOne.Level >= logItemViewEx.AutoScrollLevel)
                        {
                            lstViewItem.Dispatcher.BeginInvoke(DispatcherPriority.Input, (Action) delegate
                            {
                                lstViewItem.SelectedIndex = lstViewItem.Items.Count - 1;
                                lstViewItem.ScrollIntoView(lstViewItem.SelectedItem);
                                //var item = lstViewItem.ItemContainerGenerator.ContainerFromItem(lstViewItem.SelectedItem) as ListViewItem;
                                //item?.Focus();
                            });
                        }
                    }
                }
            };
        }

        private bool _isLogListAutoScroll = true;

        private ICommand _mouseEnterCommand;
        public ICommand MouseEnterCommand
        {
            get => _mouseEnterCommand ?? (_mouseEnterCommand = new RelayCommand(delegate (object obj) { _isLogListAutoScroll = false; }, pre =>
            {
                return true;
            }));
        }


        private ICommand _mouseLeaveCommand;
        public ICommand MouseLeaveCommand
        {
            get => _mouseLeaveCommand ?? (_mouseLeaveCommand = new RelayCommand(delegate (object obj) { _isLogListAutoScroll = true; }, pre =>
            {
                return true;
            }));
        }

    }
}
