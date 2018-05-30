using System.ComponentModel;
using System.Runtime.CompilerServices;
using Basler.Pylon;
using myzy.Util.Annotations;

namespace myzy.Util
{
    public class EnumItem : INotifyPropertyChanged
    {
        private string _valueDisplayName;
        private string _valueName;
        // Provides the display name and the name of an enum value.
        public EnumItem(IEnumParameter parameter)
        {
            ValueName = parameter.GetValue();
            ValueDisplayName = parameter.GetAdvancedValueProperties(ValueName).GetPropertyOrDefault(AdvancedParameterAccessKey.DisplayName, ValueName);
        }

        public EnumItem(IEnumParameter parameter, string valueName)
        {
            ValueName = valueName;
            ValueDisplayName = parameter.GetAdvancedValueProperties(valueName).GetPropertyOrDefault(AdvancedParameterAccessKey.DisplayName, valueName);
        }

        public override string ToString()
        {
            return ValueDisplayName;
        }

        public string ValueName
        {
            get { return _valueName; }
            set
            {
                if (value == _valueName) return;
                _valueName = value;
                OnPropertyChanged();
            }
        }

        public string ValueDisplayName
        {
            get { return _valueDisplayName; }
            set
            {
                if (value == _valueDisplayName) return;
                _valueDisplayName = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
