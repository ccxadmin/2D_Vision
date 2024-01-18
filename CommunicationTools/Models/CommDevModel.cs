using ControlShareResources.Common;
using FunctionLib.Cam;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CommunicationTools.Models
{
    public class CommDevModel : NotifyBase
    {
        public CommDevModel()
        {
            foreach (CommDevType s in Enum.GetValues(typeof(CommDevType)))
                CommDevTypeList.Add(s);
        }
        
       
        /// <summary>
        /// 通讯设备源集合
        /// </summary>
        private ObservableCollection<CommDevType> commDevTypeList = new ObservableCollection<CommDevType>();
        public ObservableCollection<CommDevType> CommDevTypeList
        {
            get { return this.commDevTypeList; }
            set
            {
                commDevTypeList = value;
                DoNotify();
            }
        }


        private string commDevSelectName;
        public string CommDevSelectName
        {
            get { return this.commDevSelectName; }
            set
            {
                commDevSelectName = value;
                DoNotify();
            }
        }


        private int commDevSelectIndex;
        public int CommDevSelectIndex
        {
            get { return this.commDevSelectIndex; }
            set
            {
                commDevSelectIndex = value;
                DoNotify();
            }
        }

        private ObservableCollection<DevInfo> devInfoList 
            = new ObservableCollection<DevInfo>();
        public ObservableCollection<DevInfo> DevInfoList
        {
            get { return this.devInfoList; }
            set
            {
                devInfoList = value;
                DoNotify();
            }
        }

        private int devInfoSelectIndex;
        public int DevInfoSelectIndex
        {
            get { return this.devInfoSelectIndex; }
            set
            {
                devInfoSelectIndex = value;
                DoNotify();
            }
        }

        private ObservableCollection<string> clientsList
           = new ObservableCollection<string>();
        public ObservableCollection<string> ClientsList
        {
            get { return this.clientsList; }
            set
            {
                clientsList = value;
                DoNotify();
            }
        }

        private int clientsSelectIndex;
        public int ClientsSelectIndex
        {
            get { return this.clientsSelectIndex; }
            set
            {
                clientsSelectIndex = value;
                DoNotify();
            }
        }
        private string sendText;
        public string SendText
        {
            get { return this.sendText; }
            set
            {
                sendText = value;
                DoNotify();
            }
        }
        private bool isOpen;
        public bool IsOpen
        {
            get { return this.isOpen; }
            set
            {
                isOpen =value;
                DoNotify();
            }
        }
        private string btnContent="监听";
        public string BtnContent
        {
            get { return this.btnContent; }
            set
            {
                btnContent = value;
                DoNotify();
            }
        }

        private string iPtext="192.169.0.1";
        public string IPtext
        {
            get { return this.iPtext; }
            set
            {
                iPtext = value;
                DoNotify();
            }
        }
        private int portValue=10000;
        public int PortValue
        {
            get { return this.portValue; }
            set
            {
                portValue = value;
                DoNotify();
            }
        }
        private Visibility btnRefreshVisib;
        public Visibility BtnRefreshVisib
        {
            get { return this.btnRefreshVisib; }
            set
            {
                btnRefreshVisib = value;
                DoNotify();
            }
        }

        private Visibility lblClientsVisib;
        public Visibility LblClientsVisib
        {
            get { return this.lblClientsVisib; }
            set
            {
                lblClientsVisib = value;
                DoNotify();
            }
        }
        private Visibility cbxCilentsVisib;
        public Visibility CbxCilentsVisib
        {
            get { return this.cbxCilentsVisib; }
            set
            {
                cbxCilentsVisib = value;
                DoNotify();
            }
        }
        private Visibility frameParamsVisib;
        public Visibility FrameParamsVisib
        {
            get { return this.frameParamsVisib; }
            set
            {
                frameParamsVisib = value;
                DoNotify();
            }
        }
        
    }

    [Serializable]
    public class DevInfo
    { 
      public DevInfo(int id,string devDescrip,string devStatus)
        {
            ID = id;
            DevDescrip = devDescrip;
            DevStatus = devStatus;
        }

        public int ID { get; set; }
        public string DevDescrip { get; set; }
        public string DevStatus { get; set; }
    }
    public class BoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b && b) return parameter;
            return Binding.DoNothing;
        }
    }
}
