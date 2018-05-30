using System;
using System.Windows;
using System.Windows.Markup;

namespace myzy.Util
{
    public class LocalNameExtension : MarkupExtension
    {
        private string _qualifier;

        public LocalNameExtension()
        {
        }
        public LocalNameExtension(string qualifier)
        {
            _qualifier = qualifier;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var targetProvider = (IProvideValueTarget)
                serviceProvider.GetService(typeof(IProvideValueTarget));
            var target = (FrameworkElement)targetProvider.TargetObject;

            string name = LocalName.GetBaseName(target);
            if (_qualifier != null) { name += _qualifier; }
            return name;
        }
    }

}