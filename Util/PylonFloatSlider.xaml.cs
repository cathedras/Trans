using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Basler.Pylon;
using log4net.Util.TypeConverters;

namespace myzy.Util
{
    /// <summary>
    /// Interaction logic for PylonFloatSlider.xaml
    /// </summary>
    public partial class PylonFloatSlider
    {
        public PylonFloatSlider()
        {
            InitializeComponent();
            this.Slider.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(Slider_MouseLeftButtonUpEvent), true);
        }

        private void Slider_MouseLeftButtonUpEvent(object sender, MouseButtonEventArgs e)
        {
            ValueChangedCmd.Execute(null);
        }

        public int Val
        {
            get { return (int)GetValue(ValProperty); }
            set
            {
                SetValue(ValProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for Val.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValProperty =
            DependencyProperty.Register("Val", typeof(int), typeof(PylonFloatSlider), new PropertyMetadata(50000));

        public double MinVal
        {
            get { return (double)GetValue(MinValProperty); }
            set { SetValue(MinValProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinVal.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinValProperty =
            DependencyProperty.Register("MinVal", typeof(double), typeof(PylonFloatSlider), new PropertyMetadata(0d));

        public double MaxVal
        {
            get { return (double)GetValue(MaxValProperty); }
            set { SetValue(MaxValProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxVal.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxValProperty =
            DependencyProperty.Register("MaxVal", typeof(double), typeof(PylonFloatSlider), new PropertyMetadata(255d));

        public double SmallChange
        {
            get { return (double)GetValue(SmallChangeProperty); }
            set { SetValue(SmallChangeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SmallChange.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SmallChangeProperty =
            DependencyProperty.Register("SmallChange", typeof(double), typeof(PylonFloatSlider), new PropertyMetadata(0d));

        public bool IsWriteable
        {
            get { return (bool)GetValue(IsWriteableProperty); }
            set { SetValue(IsWriteableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsWriteable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsWriteableProperty =
            DependencyProperty.Register("IsWriteable", typeof(bool), typeof(PylonFloatSlider), new PropertyMetadata(false));

        public bool IsUseable
        {
            get { return (bool)GetValue(IsUseableProperty); }
            set { SetValue(IsUseableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsUseable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsUseableProperty =
            DependencyProperty.Register("IsUseable", typeof(bool), typeof(PylonFloatSlider), new PropertyMetadata(false));

        public double TickFrequency
        {
            get { return (double)GetValue(TickFrequencyProperty); }
            set { SetValue(TickFrequencyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TickFrequency.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TickFrequencyProperty =
            DependencyProperty.Register("TickFrequency", typeof(double), typeof(PylonFloatSlider), new PropertyMetadata(0d));

        public string ParamName
        {
            get { return (string)GetValue(ParamNameProperty); }
            set { SetValue(ParamNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParamName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParamNameProperty =
            DependencyProperty.Register("ParamName", typeof(string), typeof(PylonFloatSlider), new PropertyMetadata("N/A"));

        private IFloatParameter parameter = null; // The interface of the integer parameter.
        private string defaultName = "N/A";

        // Sets the parameter displayed by the user control.
        public IFloatParameter Parameter
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

        private ICommand _valueChangedCmd;

        public ICommand ValueChangedCmd
        {
            get
            {
                return _valueChangedCmd ?? (_valueChangedCmd = new RelayCommand(o =>
                {
                    if (parameter != null && IsUseable && IsWriteable)
                    {
                        try
                        {
                            if (parameter.IsWritable)
                            {
                                // Set the value.
                                var ret = parameter.TrySetValuePercentOfRange(SliderToPercentValue(Val));
                                Console.WriteLine("Write Float Param {0} , Val={1}, ret = {2}", ParamName, Val, ret);
                            }
                        }
                        catch
                        {
                            // Ignore any errors here.
                        }
                    }
                }, o => IsWriteable && IsUseable));
            }
        }


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
                // Update accessibility and parameter values.
                UpdateValues();
            }
            catch
            {
                // If errors occurred disable the control.
                Reset();
            }
        }

        // Deactivate the control and deregister the callback.
        private void Reset()
        {
            IsUseable = false;
        }


        // Converts slider range to percent value.
        internal static int PercentToSliderValue(double percent)
        {
            return (int)((100000.0 / 100.0) * percent);
        }


        // Converts percent value to slider range.
        internal static double SliderToPercentValue(int sliderValue)
        {
            return sliderValue / 100000.0 * 100.0;
        }


        // Gets the current values from the node and displays them.
        private void UpdateValues()
        {
            try
            {
                if (parameter != null)
                {
                    if (parameter.IsReadable) // Check if the parameter is readable.
                    {
                        // Get the values.
                        double min = parameter.GetMinimum();
                        double max = parameter.GetMaximum();
                        double val = parameter.GetValue();
                        double percent = parameter.GetValuePercentOfRange();

                        // Update the slider.
                        MinVal = min; //PercentToSliderValue(0);
                        MaxVal = max; //PercentToSliderValue(100);
                        Val = PercentToSliderValue(percent);
                        SmallChange = PercentToSliderValue(0.05);
                        TickFrequency = PercentToSliderValue(10);

                        // Update the access status.
                        IsWriteable = parameter.IsWritable;

                        IsUseable = true;
                    }
                }
            }
            catch
            {
                // If errors occurred, disable the control.
                Reset();
            }
        }
    }

    public class PylonFloatPercentCoverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int s = System.Convert.ToInt32(value);
            return PylonFloatSlider.SliderToPercentValue(s);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = System.Convert.ToDouble(value);
            return PylonFloatSlider.PercentToSliderValue(val);
        }
    }
}
