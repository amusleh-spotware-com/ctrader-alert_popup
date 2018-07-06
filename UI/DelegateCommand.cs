using System;
using System.Windows.Input;

namespace cAlgo.API.Alert.UI
{
    public class DelegateCommand : ICommand
    {
        #region Fields

        private Action _executeMethod;

        private Action<object> _executeMethodWithParameter;

        private Func<bool> _canExecuteMethod;

        private Func<object, bool> _canExecuteMethodWithParameter;

        private bool _isParameterLess;

        #endregion Fields

        #region Constructors

        public DelegateCommand(Action executeMethod)
        {
            _executeMethod = executeMethod;

            _isParameterLess = true;
        }

        public DelegateCommand(Action<object> executeMethod)
        {
            _executeMethodWithParameter = executeMethod;
        }

        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
        {
            _executeMethod = executeMethod;

            _canExecuteMethod = canExecuteMethod;

            _isParameterLess = true;
        }

        public DelegateCommand(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
        {
            _executeMethodWithParameter = executeMethod;

            _canExecuteMethodWithParameter = canExecuteMethod;
        }

        #endregion Constructors

        #region Events

        public event EventHandler CanExecuteChanged;

        #endregion Events

        #region Methods

        public bool CanExecute(object parameter)
        {
            if (_isParameterLess)
            {
                return _canExecuteMethod != null ? _canExecuteMethod() : true;
            }
            else
            {
                return _canExecuteMethodWithParameter != null ? _canExecuteMethodWithParameter(parameter) : true;
            }
        }

        public void Execute(object parameter)
        {
            if (_isParameterLess)
            {
                _executeMethod();
            }
            else
            {
                _executeMethodWithParameter(parameter);
            }
        }

        #endregion Methods
    }
}