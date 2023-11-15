using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VisionShowLib
{

    internal class MyICommand : ICommand
    {
        Action<object> _TargetExecuteMethod;
        Func<object,  bool> _TargetCanExecuteMethod;

        public MyICommand(Action<object> executeMethod)
        {
            _TargetExecuteMethod = executeMethod;
        }

        public MyICommand(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
        {
            _TargetExecuteMethod = executeMethod;
            _TargetCanExecuteMethod = canExecuteMethod;
        }

        public event EventHandler CanExecuteChanged = delegate { };
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        bool ICommand.CanExecute(object parameter)
        {

            if (_TargetCanExecuteMethod != null)
            {
                return _TargetCanExecuteMethod.Invoke(parameter);
            }

            if (_TargetExecuteMethod != null)
            {
                return true;
            }

            return false;
        }  
        void ICommand.Execute(object parameter)
        {
            if (_TargetExecuteMethod != null)
            {
                _TargetExecuteMethod.Invoke(parameter);
            }
        }
    }


}

