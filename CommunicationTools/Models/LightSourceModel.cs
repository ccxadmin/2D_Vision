using ControlShareResources.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CommunicationTools.Models
{
    public class LightSourceModel : NotifyBase
    {

        /// <summary>
        /// 打开/关闭端口按钮
        /// </summary>
        private string _openButContent = "打开";
        public string OpenButContent
        {
            get { return this._openButContent; }
            set
            {
                _openButContent = value;
                DoNotify();
            }
        }
        /// <summary>
        /// 端口名称集合
        /// </summary>
        private ObservableCollection<string> portNames = new ObservableCollection<string>();
        public ObservableCollection<string> PortNames
        {
            get { return this.portNames; }
            set
            {
                portNames = value;
                DoNotify();
            }
        }
        /// <summary>
        /// 选择端口
        /// </summary>
        private string selectedPort;
        public string SelectedPort
        {
            get { return selectedPort; }
            set
            {
                selectedPort = value;
                DoNotify();              
            }
        }
        /// <summary>
        /// 富文本信息
        /// </summary>
        private string richIfo;
        public string RichIfo
        {
            get { return richIfo; }
            set
            {
                richIfo = value;
                DoNotify();
            }
        }
        /// <summary>
        /// 发送串口信息
        /// </summary>
        private string sendDat;
        public string SendDat
        {
            get { return sendDat; }
            set
            {
                sendDat = value;
                DoNotify();
            }
        }
        /// <summary>
        /// 接收串口信息
        /// </summary>
        private string recieveDat;
        public string RecieveDat
        {
            get { return recieveDat; }
            set
            {
                recieveDat = value;
                DoNotify();
            }
        }
        /// <summary>
        /// 常亮使能
        /// </summary>
        private bool chxbNormalOnEnable;
        public bool ChxbNormalOnEnable
        {
            get { return chxbNormalOnEnable; }
            set
            {
                chxbNormalOnEnable = value;
                DoNotify();
            }
        }

        /// <summary>
        /// 是否常亮
        /// </summary>
        private bool isNormalOn;
        public bool IsNormalOn
        {
            get { return isNormalOn; }
            set
            {
                isNormalOn = value;
                DoNotify();
            }
        }

        /// <summary>
        /// 端口选择使能
        /// </summary>
        private bool cobxPortEnable;
        public bool CobxPortEnable
        {
            get { return cobxPortEnable; }
            set
            {
                cobxPortEnable = value;
                DoNotify();
            }
        }
        /// <summary>
        /// 搜索按钮使能
        /// </summary>
        private bool btnScanEnable;
        public bool BtnScanEnable
        {
            get { return btnScanEnable; }
            set
            {
                btnScanEnable = value;
                DoNotify();
            }
        }
        /// <summary>
        /// 发送接收区使能
        /// </summary>
        private bool sendRevEnable;
        public bool SendRevEnable
        {
            get { return sendRevEnable; }
            set
            {
                sendRevEnable = value;
                DoNotify();
            }
        }
        /// <summary>
        /// 文本信息清除
        /// </summary>
        private bool clearRichText;
        public bool ClearRichText
        {
            get { return clearRichText; }
            set
            {
                clearRichText = value;
                DoNotify();
            }
        }
        /// <summary>
        /// 光源亮度数据区使能
        /// </summary>
        private bool lightValueEnable;
        public bool LightValueEnable
        {
            get { return sendRevEnable; }
            set
            {
                sendRevEnable = value;
                DoNotify();
            }
        }
        /// <summary>
        /// 光源1亮度值
        /// </summary>
        private int lightSliderValue1;
        public int LightSliderValue1
        {
            get { return lightSliderValue1; }
            set
            {
                lightSliderValue1 = value;
                DoNotify();
            }
        }
        /// <summary>
        /// 光源1亮度值
        /// </summary>
        private int lightNumricValue1;
        public int LightNumricValue1
        {
            get { return lightNumricValue1; }
            set
            {
                lightNumricValue1 = value;
                DoNotify();
            }
        }

        private Action lightNumeric1Command;
        public Action LightNumeric1Command
        {
            get { return lightNumeric1Command; }
            set
            {
                lightNumeric1Command = value;
                DoNotify();
            }
        }

        /// <summary>
        /// 光源2亮度值
        /// </summary>
        private int lightSliderValue2;
        public int LightSliderValue2
        {
            get { return lightSliderValue2; }
            set
            {
                lightSliderValue2 = value;
                DoNotify();
            }
        }
     
        /// <summary>
        /// 光源2亮度值
        /// </summary>
        private int lightNumricValue2;
        public int LightNumricValue2
        {
            get { return lightNumricValue2; }
            set
            {
                lightNumricValue2 = value;
                DoNotify();
            }
        }
        private Action lightNumeric2Command;
        public Action LightNumeric2Command
        {
            get { return lightNumeric2Command; }
            set
            {
                lightNumeric2Command = value;
                DoNotify();
            }
        }
        /// <summary>
        /// 光源3亮度值
        /// </summary>
        private int lightSliderValue3;
        public int LightSliderValue3
        {
            get { return lightSliderValue3; }
            set
            {
                lightSliderValue3 = value;
                DoNotify();
            }
        }
        /// <summary>
        /// 光源3亮度值
        /// </summary>
        private int lightNumricValue3;
        public int LightNumricValue3
        {
            get { return lightNumricValue3; }
            set
            {
                lightNumricValue3 = value;
                DoNotify();
            }
        }
        private Action lightNumeric3Command;
        public Action LightNumeric3Command
        {
            get { return lightNumeric3Command; }
            set
            {
                lightNumeric3Command = value;
                DoNotify();
            }
        }
        /// <summary>
        /// 光源4亮度值
        /// </summary>
        private int lightSliderValue4;
        public int LightSliderValue4
        {
            get { return lightSliderValue4; }
            set
            {
                lightSliderValue4 = value;
                DoNotify();
            }
        }
        /// <summary>
        /// 光源4亮度值
        /// </summary>
        private int lightNumricValue4;
        public int LightNumricValue4
        {
            get { return lightNumricValue4; }
            set
            {
                lightNumricValue4 = value;
                DoNotify();
            }
        }
        private Action lightNumeric4Command;
        public Action LightNumeric4Command
        {
            get { return lightNumeric4Command; }
            set
            {
                lightNumeric4Command = value;
                DoNotify();
            }
        }

    }
}
