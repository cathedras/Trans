using System;
using System.Windows;

namespace myzy.Util
{
    public class ExCommandParameter
    {
        public DependencyObject OriSender { get; set; }

        public EventArgs EventArgs { get; set; }

        public object Parameter { get; set; }
    }
}