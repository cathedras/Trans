using System;
using System.Windows.Markup;

namespace myzy.Util
{
    public class UniqueNameExtension : MarkupExtension
    {
        private string _name;
        public UniqueNameExtension()
        {
            _name = Guid.NewGuid().ToString("N");
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _name;
        }
    }
}