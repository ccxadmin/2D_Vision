using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ControlShareResources.Common
{
    [Serializable]
    public class NotifyBase : INotifyPropertyChanged
    {

      
        public event PropertyChangedEventHandler PropertyChanged;
        public void DoNotify([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(name));
        }
    }
}
