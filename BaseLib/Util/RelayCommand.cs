using System;
using System.Diagnostics;
using System.Windows.Input;

namespace myzy.Util
{
    //ICommand实现辅助类
    public class RelayCommand : ICommand
    {
        #region ICommand realization

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            if (_execute != null)
            {
                _execute(parameter);
            }
        }

        #endregion

        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;

        public RelayCommand()
            : this(null, null)
        {

        }

        public RelayCommand(Action<object> execute)
            : this(execute, null)
        { }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("RelayCommand argument can not be null!");

            _execute = execute;
            _canExecute = canExecute;
        }
    }
}
