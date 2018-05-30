using System;
using System.Windows;
using System.Windows.Input;
using Basler.Pylon;
using log4net;

namespace myzy.Util
{
    /// <summary>
    /// Interaction logic for PylonIntSlider.xaml
    /// </summary>
    public partial class PylonIntSlider
    {
        private static readonly ILog _log = LogManager.GetLogger("log");
        #region DependencyProperty.
        public double TickFrequency
        {
            get { return (double)GetValue(TickFrequencyProperty); }
            set { SetValue(TickFrequencyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TickFrequency.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TickFrequencyProperty =
            DependencyProperty.Register("TickFrequency", typeof(double), typeof(PylonIntSlider), new PropertyMetadata(1d));

        public int Val
        {
            get { return (int)GetValue(ValProperty); }
            set { SetValue(ValProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Val.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValProperty =
            DependencyProperty.Register("Val", typeof(int), typeof(PylonIntSlider), new PropertyMetadata(128));

        public int MinVal
        {
            get { return (int)GetValue(MinValProperty); }
            set { SetValue(MinValProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinVal.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinValProperty =
            DependencyProperty.Register("MinVal", typeof(int), typeof(PylonIntSlider), new PropertyMetadata(0));

        public int MaxVal
        {
            get { return (int)GetValue(MaxValProperty); }
            set { SetValue(MaxValProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxVal.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxValProperty =
            DependencyProperty.Register("MaxVal", typeof(int), typeof(PylonIntSlider), new PropertyMetadata(255));

        public double SmallChange
        {
            get { return (double)GetValue(SmallChangeProperty); }
            set { SetValue(SmallChangeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SmallChange.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SmallChangeProperty =
            DependencyProperty.Register("SmallChange", typeof(double), typeof(PylonIntSlider), new PropertyMetadata(0d));

        public bool IsWriteable
        {
            get { return (bool)GetValue(IsWriteableProperty); }
            set { SetValue(IsWriteableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsWriteable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsWriteableProperty =
            DependencyProperty.Register("IsWriteable", typeof(bool), typeof(PylonIntSlider), new PropertyMetadata(false));

        public bool IsUseable
        {
            get { return (bool)GetValue(IsUseableProperty); }
            set { SetValue(IsUseableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsUseable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsUseableProperty =
            DependencyProperty.Register("IsUseable", typeof(bool), typeof(PylonIntSlider), new PropertyMetadata(false));

        public string ParamName
        {
            get { return (string)GetValue(ParamNameProperty); }
            set { SetValue(ParamNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParamName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParamNameProperty =
            DependencyProperty.Register("ParamName", typeof(string), typeof(PylonIntSlider), new PropertyMetadata("N/A"));
        #endregion

        #region Property & Getter-Setter

        private IIntegerParameter parameter = null; // The interface of the integer parameter.
        private string defaultName = "N/A";

        // Sets the default name of the control.
        public string DefaultName
        {
            set
            {
                defaultName = value;
                if (parameter == null)
                {
                    ParamName = defaultName;
                }
            }
            get
            {
                return defaultName;
            }
        }

        // Sets the parameter displayed by the user control.
        public IIntegerParameter Parameter
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
                    ParamName = parameter.Advanced.GetPropertyOrDefault(AdvancedParameterAccessKey.DisplayName, parameter.Name);
                    UpdateValues();
                }
                else
                {
                    ParamName = defaultName;
                    Reset();
                }
            }
        }

        private ICommand _valueChangedCmd;
        public ICommand ValueChangedCmd
        {
            get
            {
                return _valueChangedCmd ?? (_valueChangedCmd = new RelayCommand(o =>
                {
                    if (parameter != null && IsWriteable && IsUseable)
                    {
                        try
                        {
                            // Set the value if writable.
                            var ret = parameter.TrySetValue(Val, IntegerValueCorrection.Nearest);
                            _log.DebugFormat("Set Parameter {0}, {1}, Ret = {2}", ParamName, Val, ret);
                        }
                        catch
                        {
                            // Ignore any errors here.
                        }
                    }
                }, o => IsWriteable && IsUseable));
            }
        }

        #endregion

        #region Ctor.

        public PylonIntSlider()
        {
            InitializeComponent();
            this.Slider.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(Slider_MouseLeftButtonUpEvent), true);
        }

        private void Slider_MouseLeftButtonUpEvent(object sender, MouseButtonEventArgs e)
        {
            ValueChangedCmd.Execute(null);
        }

        #endregion

        #region Func

        // The parameter state changed. Update the control.
        private void ParameterChanged(Object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                // If called from a different thread, we must use the Invoke method to marshal the call to the proper thread.
                Dispatcher.BeginInvoke(new EventHandler<EventArgs>(ParameterChanged), sender, e);
                return;
            }
            try
            {
                UpdateValues();
            }
            catch
            {
                // If errors occurred disable the control.
                Reset();
            }
        }

        // Deactivate the control.
        private void Reset()
        {
            IsUseable = false;
        }


        // Get the current values from the parameter and display them.
        private void UpdateValues()
        {
            try
            {
                if (parameter != null)
                {
                    if (parameter.IsReadable)  // Check if parameter is accessible.
                    {
                        // Get values.
                        int min = checked((int)parameter.GetMinimum());
                        int max = checked((int)parameter.GetMaximum());
                        int val = checked((int)parameter.GetValue());
                        int inc = checked((int)parameter.GetIncrement());

                        // Update the slider.
                        MinVal = min;
                        MaxVal = max;
                        Val = val;
                        SmallChange = inc;
                        TickFrequency = (max - min + 5) / 10d;

                        // Update accessibility.
                        IsWriteable = parameter.IsWritable;
                        IsUseable = true;

                        return;
                    }
                }
            }
            catch
            {
                // If errors occurred disable the control.
            }
            Reset();
        }

        #endregion
    }
}
