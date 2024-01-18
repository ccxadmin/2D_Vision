using ControlShareResources.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlueDetectionLib.窗体.Models
{
    public class TcpSendToolModel : NotifyBase
    {
        private string titleName;
        public string TitleName
        {
            get { return this.titleName; }
            set
            {
                titleName = value;
                DoNotify();
            }
        }

        private ObservableCollection<string> devList = new ObservableCollection<string>();
        public ObservableCollection<string> DevList
        {
            get { return this.devList; }
            set
            {
                devList = value;
                DoNotify();
            }
        }
        private int devSelectIndex = -1;
        public int DevSelectIndex
        {
            get { return this.devSelectIndex; }
            set
            {
                devSelectIndex = value;
                DoNotify();
            }
        }
        private string devSelectName;
        public string DevSelectName
        {
            get { return this.devSelectName; }
            set
            {
                devSelectName = value;
                DoNotify();
            }
        }
        private string txbHead;
        public string TxbHead
        {
            get { return this.txbHead; }
            set
            {
                txbHead = value;
                DoNotify();
            }
        }

        private string txbSpilt;
        public string TxbSpilt
        {
            get { return this.txbSpilt; }
            set
            {
                txbSpilt = value;
                DoNotify();
            }
        }

        private string txbTail;
        public string TxbTail
        {
            get { return this.txbTail; }
            set
            {
                txbTail = value;
                DoNotify();
            }
        }

    }
}
