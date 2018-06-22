using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace myzy.Util
{
    public static class ItemSourceItemInfoEnumHelper
    {
        public static ObservableCollection<ItemSourceItemInfo> GenItemSourceItems(Type t)
        {
            if (!t.IsEnum)
            {
                throw new NotSupportedException($"{t} must be enum");
            }
            var objCollection = new ObservableCollection<ItemSourceItemInfo>();
            foreach (var value in Enum.GetValues(t))
            {
                var fieldName = Enum.GetName(t, value);
                if (fieldName != null)
                {
                    var desstr = $"{fieldName.ToUpperInvariant()}";
                    var des = t.GetField(fieldName).GetCustomAttributes(typeof(DescriptionAttribute)).LastOrDefault();

                    if (des != null)
                    {
                        desstr = ((DescriptionAttribute) des).Description;
                    }
                    objCollection.Add(new ItemSourceItemInfo()
                    {
                        Name = fieldName,
                        Des = desstr,
                        Val = value,
                    });
                }
            }
            return objCollection;
        }
    }


    public class ItemSourceItemInfo : INotifyPropertyChanged
    {
        private object _val;
        public object Val
        {
            get { return _val; }
            set
            {
                if (_val != value)
                {
                    _val = value;
                    OnPropertyChanged("Val");
                }
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        private string _des;
        private string _name;

        public string Des
        {
            get { return _des; }
            set
            {
                if (_des != value)
                {
                    _des = value;
                    OnPropertyChanged("Des");
                }
            }
        }

        private void OnPropertyChanged(string propname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propname));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}