using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Basler.Pylon;
using log4net;

namespace myzy.Util
{
    /// <summary>
    /// Interaction logic for PylonEnumerationComboBox.xaml
    /// </summary>
    public partial class PylonEnumerationComboBox
    {
        private static readonly ILog _log = LogManager.GetLogger("util");

        public static readonly DependencyProperty EnumerationNameProperty = DependencyProperty.Register("EnumerationName", typeof(String), typeof(PylonEnumerationComboBox), new PropertyMetadata(default(String)));

        public bool IsError
        {
            get { return (bool)GetValue(IsErrorProperty); }
            set { SetValue(IsErrorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsError.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsErrorProperty =
            DependencyProperty.Register("IsError", typeof(bool), typeof(PylonEnumerationComboBox), new PropertyMetadata(false));

        public bool IsReadable
        {
            get { return (bool)GetValue(IsReadableProperty); }
            set { SetValue(IsReadableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsReadable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsReadableProperty =
            DependencyProperty.Register("IsReadable", typeof(bool), typeof(PylonEnumerationComboBox), new PropertyMetadata(false));

        public bool IsWriteable
        {
            get { return (bool)GetValue(IsWriteableProperty); }
            set { SetValue(IsWriteableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsWriteable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsWriteableProperty =
            DependencyProperty.Register("IsWriteable", typeof(bool), typeof(PylonEnumerationComboBox), new PropertyMetadata(false));

        public int CurEnumIdx
        {
            get { return (int)GetValue(CurEnumIdxProperty); }
            set { SetValue(CurEnumIdxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurEnumIdx.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurEnumIdxProperty =
            DependencyProperty.Register("CurEnumIdx", typeof(int), typeof(PylonEnumerationComboBox), new PropertyMetadata(-1));

        public PylonEnumerationComboBox()
        {
            InitializeComponent();
            IsReadable = false;
            IsWriteable = false;
            IsError = false;

            this.cbEnumerate.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(cb_MouseLeftButtonUpEvent), true);
        }

        private void cb_MouseLeftButtonUpEvent(object sender, MouseButtonEventArgs e)
        {
            if (CurEnumIdx >= 0 && CurEnumIdx < EnumItems.Count)
            {
                ValueChangedCmd.Execute(EnumItems.ElementAt(CurEnumIdx));
            }
        }

        public string EnumerationName
        {
            get { return (string)GetValue(EnumerationNameProperty); }
            set { SetValue(EnumerationNameProperty, value); }
        }

        private string defaultName = "N/A";

        private IEnumParameter parameter = null; // The interface of the enum parameter.

        // Occurs when the parameter state has changed. Updates the control.
        private void ParameterChanged(Object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                // If called from a different thread, we must use the Invoke method to marshal the call to the proper thread.
                Dispatcher.Invoke(new EventHandler<EventArgs>(ParameterChanged), sender, e);
                return;
            }
            try
            {
                // Update the control values.
                UpdateValues();
            }
            catch
            {
                // If errors occurred, disable the control.
                SetErrorState();
            }
        }

        // Sets the parameter displayed by the user control.
        public IEnumParameter Parameter
        {
            set
            {
                // Remove the old parameter.
                if (parameter != null)
                {
                    parameter.ParameterChanged -= ParameterChanged;
                }

                // Set the new parameter.
                parameter = value;
                if (parameter != null)
                {
                    parameter.ParameterChanged += ParameterChanged;
                    EnumerationName = parameter.Advanced.GetPropertyOrDefault(AdvancedParameterAccessKey.DisplayName, parameter.Name);
                    UpdateValues();
                }
                else
                {
                    EnumerationName = defaultName;
                }
            }
        }

        private ObservableCollection<EnumItem> _enumItems;
        public ObservableCollection<EnumItem> EnumItems
        {
            get { return _enumItems ?? (_enumItems = new ObservableCollection<EnumItem>()); }
        }

        private void SetErrorState()
        {
            // If errors occurred, disable the control.
            CurEnumIdx = -1;
            EnumItems.Clear();
            EnumItems.Add(new EnumItem(null));
            IsError = true;
        }


        // Gets the current values from the node and displays them.
        private void UpdateValues()
        {
            try
            {
                if (parameter != null)
                {
                    _log.DebugFormat("Clear Enum Items for {0}.", EnumerationName);
                    // Reset the Combobox.
                    CurEnumIdx = -1;
                    EnumItems.Clear();

                    IsError = false;
                    // Set the items for the combobox and enable the combobox.
                    if (parameter.IsWritable && parameter.IsReadable)
                    {
                        string selected = parameter.GetValue();
                        foreach (string valueName in parameter)
                        {
                            var item = new EnumItem(parameter, valueName);
                            EnumItems.Add(item);
                            if (selected == valueName)
                            {
                                CurEnumIdx = EnumItems.Count - 1;
                            }
                        }
                        IsReadable = true;
                        IsWriteable = true;
                    }
                    // Disable the combobox, e.g. if camera is grabbing.
                    else if (parameter.IsReadable)
                    {
                        var item = new EnumItem(parameter);
                        EnumItems.Add(item);
                        CurEnumIdx = EnumItems.Count - 1;

                        IsWriteable = false;
                        IsReadable = true;
                    }
                    // If the parameter is not readable, disable the combobox without setting any items.
                    else
                    {
                        IsReadable = false;
                        IsWriteable = false;
                    }
                }
            }
            catch
            {
                // If errors occurred, disable the control.
                SetErrorState();
            }
        }

        private ICommand _valueChangedCmd;
        public ICommand ValueChangedCmd
        {
            get
            {
                return _valueChangedCmd ?? (_valueChangedCmd = new RelayCommand(o =>
                {
                    if (parameter != null && IsWriteable && IsWriteable)
                    {
                        try
                        {
                            var val = o as EnumItem;
                            if (val != null)
                            {
                                // Set the value if writable.
                                var ret = parameter.TrySetValue(val.ValueName);
                            }
                        }
                        catch
                        {
                            // Ignore any errors here.
                        }
                    }
                }, o => IsWriteable));
            }
        }
    }
}
