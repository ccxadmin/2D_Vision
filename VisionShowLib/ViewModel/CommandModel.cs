using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionShowLib.ViewModel
{
    internal class CommandModel : INotifyPropertyChanged
    {
        bool isCanExec = true;
        public event PropertyChangedEventHandler PropertyChanged;
        public MyICommand ButtonCommand { get; private set; }
        public CommandModel()
        {
            ButtonCommand = new MyICommand(MyExecute, MyCanExec);

        }

        private bool MyCanExec(object parameter)
        {
            return isCanExec;
        }

        private void MyExecute(object parameter)
        {

        }
       
    }
}
