using ControlShareResources.Common;
using FilesRAW.Common;
using CommunicationTools.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using static CommunicationTools.Models.LightSource;

namespace CommunicationTools.ViewModels
{
    public class LightSourceViewModel
    {

        public static LightSourceViewModel This { get; set; }
        public LightSourceModel Model { get; set; }
       
        
        //打开/关闭端口
        public CommandBase OpenButClickCommand { get; set; }

        //端口扫描
        public CommandBase ScanButClickCommand { get; set; }
        //发送信息
        public CommandBase SendButClickCommand { get; set; }

        //光源是否常亮模式选择
        public CommandBase ChxbNormalOnCommand { get; set; }

        //文本信息清除
        public CommandBase ClearTextCommand { get; set; }
        //光源亮度值滑条调整
        public CommandBase LightSliderValueChangedCommand { get; set; }
        //光源1亮度值调整
        public CommandBase LightNumeric1KeyDownCommand { get; set; }    
        //光源2亮度值调整
        public CommandBase LightNumeric2KeyDownCommand { get; set; }
        //光源3亮度值调整
        public CommandBase LightNumeric3KeyDownCommand { get; set; }
        //光源4亮度值调整
        public CommandBase LightNumeric4KeyDownCommand { get; set; }
        public Action LoadAction { get; set; }


        static LightSourceViewModel LSViewModel = null;
        LightSource lightSource = null;
        ObservableCollection<string> portList = new ObservableCollection<string>();//端口集合
       
        /*----------------------构造函数-----------------*/
        private LightSourceViewModel(LightSource _lightSource)
        {
            This = this;
            Model = new LightSourceModel();

            //点击打开/关闭按钮
            OpenButClickCommand = new CommandBase();
            OpenButClickCommand.DoExecute = new Action<object>((o) => OpenButClick());
            OpenButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
            //点击端口扫描按钮
            ScanButClickCommand = new CommandBase();
            ScanButClickCommand.DoExecute = new Action<object>((o) => ScanButClick());
            ScanButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            SendButClickCommand = new CommandBase();
            SendButClickCommand.DoExecute = new Action<object>((o) => SendButClick());
            SendButClickCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
           
            //富文本信息显示清除按钮
            ClearTextCommand = new CommandBase();
            ClearTextCommand.DoExecute = new Action<object>((o) => ClearTextClick());
            ClearTextCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
            //光源是否常亮模式选择
            ChxbNormalOnCommand = new CommandBase();
            ChxbNormalOnCommand.DoExecute = new Action<object>((o) => ChxbNormalOnCheckChange());
            ChxbNormalOnCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            //光源亮度值滑条调整
            LightSliderValueChangedCommand = new CommandBase();
            LightSliderValueChangedCommand.DoExecute = new Action<object>((o) => LightSliderValueChanged(o));
            LightSliderValueChangedCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
            //光源1亮度值输入
            LightNumeric1KeyDownCommand = new CommandBase();
            LightNumeric1KeyDownCommand.DoExecute = new Action<object>((o) => LightNumeric1KeyDown(o));
            LightNumeric1KeyDownCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            Model.LightNumeric1Command = new Action(() => LightNumeric1Event());
           
            //光源2亮度值输入
            LightNumeric2KeyDownCommand = new CommandBase();
            LightNumeric2KeyDownCommand.DoExecute = new Action<object>((o) => LightNumeric2KeyDown(o));
            LightNumeric2KeyDownCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
            Model.LightNumeric2Command = new Action(() => LightNumeric2Event());
            //光源3亮度值输入
            LightNumeric3KeyDownCommand = new CommandBase();
            LightNumeric3KeyDownCommand.DoExecute = new Action<object>((o) => LightNumeric3KeyDown(o));
            LightNumeric3KeyDownCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
            Model.LightNumeric3Command = new Action(() => LightNumeric3Event());
            //光源4亮度值输入
            LightNumeric4KeyDownCommand = new CommandBase();
            LightNumeric4KeyDownCommand.DoExecute = new Action<object>((o) => LightNumeric4KeyDown(o));
            LightNumeric4KeyDownCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
            Model.LightNumeric4Command = new Action(() => LightNumeric4Event());
            lightSource = _lightSource;
          
            ScanPort();
            Model.PortNames = portList;
            Model.SelectedPort= lightSource.portName;
    
            if (lightSource.isOpen)
            {
                lightSource.getDataHandle += new LightSource.GetDataHandle(OnGetDataHandle);
                Model.OpenButContent = "断开";
                GetControllerStatus();
            }
            else
            {
               
                Model.OpenButContent = "连接";
            }
            SetControlEnable(lightSource.isOpen);
        }
        static public LightSourceViewModel CreateSingleInstance(LightSource _lightSource)
        {
            if (LSViewModel == null)
                LSViewModel = new LightSourceViewModel(_lightSource);
            return LSViewModel;
        }
        /// <summary>
        /// 接收串口数据事件
        /// </summary>
        /// <param name="data"></param>
        void OnGetDataHandle(string data)
        {
            Model.RecieveDat = data; 
        }
        /// <summary>
        /// 扫描计算机端口
        /// </summary>
        void ScanPort()
        {
            string[] ports = SerialPort.GetPortNames();
            portList.Clear();
            foreach(var s in ports)
               portList.Add(s);
        }
        /// <summary>
        /// 获取光源控制器状态
        /// </summary>
        void GetControllerStatus()
        {
            EumLightMode mode = lightSource.ReadLightMode();
            if (mode == EumLightMode.normal_on)
                Model.ChxbNormalOnEnable = true;
            else
                Model.ChxbNormalOnEnable = false;

            byte ch1 = lightSource.GetLightValue(1);
            Model.LightSliderValue1 = ch1;
            Model.LightNumricValue1 = ch1;
            byte ch2 = lightSource.GetLightValue(2);
            Model.LightSliderValue2 = ch2;
            Model.LightNumricValue2 = ch2;
            byte ch3 = lightSource.GetLightValue(3);
            Model.LightSliderValue3 = ch3;
            Model.LightNumricValue3 = ch3;
            byte ch4 = lightSource.GetLightValue(4);
            Model.LightSliderValue4 = ch4;
            Model.LightNumricValue4 = ch4;

        }
        /// <summary>
        /// 控件使能
        /// </summary>
        /// <param name="flag"></param>
        void SetControlEnable(bool flag)
        {
            Model.CobxPortEnable= !flag;
            Model.SendRevEnable = flag;
            Model.LightValueEnable = flag;
            Model.BtnScanEnable = !flag;
        }
        /// <summary>
        /// 光源连接或断开按钮
        /// </summary>
        void OpenButClick()
        {
            if (Model.OpenButContent== "连接")
            {
                string port = Model.SelectedPort;
                if (!port.Contains("COM")) return;
                bool openFlag = lightSource.Open(port);
                if (openFlag)
                {

                    Model.OpenButContent = "断开";
                    Model.RichIfo = "成功打开串口";
                    GeneralUse.WriteValue("光源控制器", "端口", port, "config");
                    GetControllerStatus();
                    lightSource.getDataHandle += new LightSource.GetDataHandle(OnGetDataHandle);
                }
                else
                {

                    Model.RichIfo = "打开串口失败";
                }
                SetControlEnable(openFlag);
            }
            else if (Model.OpenButContent == "断开")
            {
                SetControlEnable(false);
                lightSource.Close();
                Model.OpenButContent = "连接";
                Model.RichIfo = "成功关闭串口";
                lightSource.getDataHandle -= new LightSource.GetDataHandle(OnGetDataHandle);
            }
        }
        /// <summary>
        /// 端口查询按钮
        /// </summary>
        void ScanButClick()
        {
            ScanPort();
            Model.PortNames = portList;
        }
        /// <summary>
        ///数据发送按钮
        /// </summary>
        void SendButClick()
        {
            string cmd = Model.SendDat;
            bool sendFlag = lightSource.SendData(cmd);
            if (!sendFlag)
                Model.RichIfo = string.Format("数据：{0}发送失败！", cmd);
        }     
        /// <summary>
        /// 富文本信息清除
        /// </summary>
        void ClearTextClick()
        {
            Model.ClearRichText = false;
            Model.ClearRichText = true;
        }
        /// <summary>
        /// 光源是否常亮模式切换
        /// </summary>
        void ChxbNormalOnCheckChange()
        {
            if (Model.IsNormalOn)
                lightSource.WriteLightMode(EumLightMode.normal_on);
            else
                lightSource.WriteLightMode(EumLightMode.normal_off);
        }
        /*----------------光源亮度设置及调整------------------*/
        /// <summary>
        /// 光源亮度值设置：slider bar
        /// </summary>
        /// <param name="obj"></param>
        void LightSliderValueChanged(object obj)
        {
            
            string name = ((Slider)obj).Name;
            int chn = -1;
            int currValue = 0;
            switch (name)
            {
                case "LightSlider1":
                    currValue = Model.LightSliderValue1;
                    Model.LightNumricValue1 = currValue;
                    chn = 1;
                    break;
                case "LightSlider2":
                    currValue = Model.LightSliderValue2;
                    Model.LightNumricValue2 = currValue;
                    chn = 2;
                    break;
                case "LightSlider3":
                    currValue = Model.LightSliderValue3;
                    Model.LightNumricValue3 = currValue;
                    chn = 3;
                    break;
                case "LightSlider4":
                    currValue = Model.LightSliderValue4;
                    Model.LightNumricValue4 = currValue;
                    chn = 4;
                    break;
            }
            bool flag = lightSource.WriteLightValue(chn, (byte)currValue);
            if (!flag)
                Model.RichIfo = string.Format("通道{1}光源亮度值：{0}设置失败！", currValue, chn);
        }
        /// <summary>
        /// 光源1亮度值输入
        /// </summary>
        /// <param name="obj"></param>
        void LightNumeric1KeyDown(object obj)
        {
            KeyEventArgs args = (KeyEventArgs)obj;       
            if(args.Key==Key.Enter)
            {
                Model.LightSliderValue1= Model.LightNumricValue1;             
                bool flag = lightSource.WriteLightValue(1, (byte)Model.LightNumricValue1);
                if (!flag)
                    Model.RichIfo = string.Format("通道1光源亮度值：{0}设置失败！", Model.LightNumricValue1);
            }           
        }
        void LightNumeric1Event()
        {
            Model.LightSliderValue1 = Model.LightNumricValue1;
            bool flag = lightSource.WriteLightValue(1, (byte)Model.LightNumricValue1);
            if (!flag)
                Model.RichIfo = string.Format("通道1光源亮度值：{0}设置失败！", Model.LightNumricValue1);

        }
        
        /// <summary>
        /// 光源2亮度值输入
        /// </summary>
        /// <param name="obj"></param>
        void LightNumeric2KeyDown(object obj)
        {
            KeyEventArgs args = (KeyEventArgs)obj;
            if (args.Key == Key.Enter)
            {
                Model.LightSliderValue2 = Model.LightNumricValue2;
                bool flag = lightSource.WriteLightValue(2, (byte)Model.LightNumricValue2);
                if (!flag)
                    Model.RichIfo = string.Format("通道2光源亮度值：{0}设置失败！", Model.LightNumricValue2);
            }
        }
        void LightNumeric2Event()
        {
            Model.LightSliderValue2 = Model.LightNumricValue2;
            bool flag = lightSource.WriteLightValue(2, (byte)Model.LightNumricValue2);
            if (!flag)
                Model.RichIfo = string.Format("通道2光源亮度值：{0}设置失败！", Model.LightNumricValue2);

        }
        /// <summary>
        /// 光源3亮度值输入
        /// </summary>
        /// <param name="obj"></param>
        void LightNumeric3KeyDown(object obj)
        {
            KeyEventArgs args = (KeyEventArgs)obj;
            if (args.Key == Key.Enter)
            {
                Model.LightSliderValue3 = Model.LightNumricValue3;
                bool flag = lightSource.WriteLightValue(3, (byte)Model.LightNumricValue3);
                if (!flag)
                    Model.RichIfo = string.Format("通道3光源亮度值：{0}设置失败！", Model.LightNumricValue3);
            }
        }
        void LightNumeric3Event()
        {
            Model.LightSliderValue3 = Model.LightNumricValue3;
            bool flag = lightSource.WriteLightValue(3, (byte)Model.LightNumricValue3);
            if (!flag)
                Model.RichIfo = string.Format("通道3光源亮度值：{0}设置失败！", Model.LightNumricValue3);

        }
        /// <summary>
        /// 光源4亮度值输入
        /// </summary>
        /// <param name="obj"></param>
        void LightNumeric4KeyDown(object obj)
        {
            KeyEventArgs args = (KeyEventArgs)obj;
            if (args.Key == Key.Enter)
            {
                Model.LightSliderValue4 = Model.LightNumricValue4;
                bool flag = lightSource.WriteLightValue(4, (byte)Model.LightNumricValue4);
                if (!flag)
                    Model.RichIfo = string.Format("通道4光源亮度值：{0}设置失败！", Model.LightNumricValue4);
            }
        }
        void LightNumeric4Event()
        {
            Model.LightSliderValue4 = Model.LightNumricValue4;
            bool flag = lightSource.WriteLightValue(4, (byte)Model.LightNumricValue4);
            if (!flag)
                Model.RichIfo = string.Format("通道4光源亮度值：{0}设置失败！", Model.LightNumricValue4);

        }
    }
 
}
