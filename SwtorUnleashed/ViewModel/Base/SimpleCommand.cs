using System;
using System.Diagnostics;
using System.Windows.Input;

namespace SwtorUnleashed.ViewModel.Base
{
    public class SimpleCommand : ICommand
    {
        #region members

        private readonly Predicate<object> _canExecute;
        private readonly Action<object>    _execute;

        #endregion

        #region .ctor

        public SimpleCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public SimpleCommand(Action<object> execute, Predicate<object> canExecute)
        {
            #region args check

            if (execute == null)
                throw new ArgumentNullException("execute");

            #endregion
            _canExecute = canExecute;
            _execute    = execute;
        }

        #endregion

        #region ICommand

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return (_canExecute == null) || _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        #endregion

        #region methods

        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
                handler(this, new EventArgs());
        }

        #endregion
    }
}
