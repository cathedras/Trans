using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace myzy.Util
{
    public class ExInvokeCommandAction : TriggerAction<DependencyObject>
    {
        private string _commandName;
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(ExInvokeCommandAction), null);
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(ExInvokeCommandAction), null);
        public string CommandName
        {
            get
            {
                base.ReadPreamble();
                return this._commandName;
            }
            set
            {
                if (this.CommandName != value)
                {
                    base.WritePreamble();
                    this._commandName = value;
                    base.WritePostscript();
                }
            }
        }
        public ICommand Command
        {
            get
            {
                return (ICommand)base.GetValue(ExInvokeCommandAction.CommandProperty);
            }
            set
            {
                base.SetValue(CommandProperty, value);
            }
        }
        public object CommandParameter
        {
            get
            {
                return base.GetValue(CommandParameterProperty);
            }
            set
            {
                base.SetValue(CommandParameterProperty, value);
            }
        }
        protected override void Invoke(object parameter)
        {
            if (base.AssociatedObject != null)
            {
                ICommand command = this.ResolveCommand();
                /*
                 EventArgs
                 */
                ExCommandParameter exParameter = new ExCommandParameter
                {
                    OriSender = base.AssociatedObject,
                    EventArgs = parameter as EventArgs,
                    Parameter = this.CommandParameter
                };
                if (command != null && command.CanExecute(exParameter))
                {
                    command.Execute(exParameter);
                }
            }
        }
        private ICommand ResolveCommand()
        {
            ICommand result = null;
            if (this.Command != null)
            {
                result = this.Command;
            }
            else
            {
                if (base.AssociatedObject != null)
                {
                    Type type = base.AssociatedObject.GetType();
                    PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    PropertyInfo[] array = properties;
                    foreach (PropertyInfo propertyInfo in array)
                    {
                        if (typeof(ICommand).IsAssignableFrom(propertyInfo.PropertyType) && string.Equals(propertyInfo.Name, this.CommandName, StringComparison.Ordinal))
                        {
                            result = (ICommand)propertyInfo.GetValue(base.AssociatedObject, null);
                        }
                    }
                }
            }
            return result;
        }
    }
}