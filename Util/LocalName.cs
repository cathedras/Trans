using System.Windows;

namespace myzy.Util
{
    public class LocalName
    {
        public static string GetBaseName(FrameworkElement obj)
        {
            return (string)obj.GetValue(BaseNameProperty);
        }

        public static void SetBaseName(FrameworkElement obj, string value)
        {
            obj.SetValue(BaseNameProperty, value);
        }

        public static readonly DependencyProperty BaseNameProperty =
            DependencyProperty.RegisterAttached("BaseName", typeof(string),
                typeof(LocalName),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.Inherits));
    }
}