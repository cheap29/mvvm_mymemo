using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace mymemo.ViewModel
{
    public class MainCommand : ICommand
    {
        #region ICommand
        public bool CanExecute(Object parameter) { return true; }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            action();
        }
        #endregion

        private Action action;
        public MainCommand(Action action)
        {
            this.action = action;
        }
    }
}
