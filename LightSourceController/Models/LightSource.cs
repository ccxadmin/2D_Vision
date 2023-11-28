using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;



namespace LightSourceController.Models
{
    public class LightSource
    {
        public LightSource(string _portName)
        {
            if (sp == null) sp = new SerialPort(_portName, 19200, Parity.None, 8, StopBits.One);
            sp.DataReceived += new SerialDataReceivedEventHandler(OnSerialDataReceivedEventHandler);
            portName = _portName;
        }
     
        ~LightSource()
        {
            if (sp.IsOpen)
                sp.Close();
            isOpen = false;
            sp.DataReceived -= new SerialDataReceivedEventHandler(OnSerialDataReceivedEventHandler);
            sp.Dispose();
        }
        SerialPort sp = null;//串口
        public delegate void GetDataHandle(string dat);
        public GetDataHandle getDataHandle = null;
        public string portName = "COM1";

        public bool isOpen=false;
        /// <summary>
        /// 打开端口
        /// </summary>
        /// <returns></returns>
        public bool Open(string _portName)
        {
            //*string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits
            if (sp == null)
            {
                sp = new SerialPort(portName, 19200, Parity.None, 8, StopBits.One);
                sp.DataReceived += new SerialDataReceivedEventHandler(OnSerialDataReceivedEventHandler);
                portName = _portName;  
            }
            if (sp.IsOpen)
            {
                portName = sp.PortName;
                isOpen = true;
                return true;
            }
            try
            {
                sp.PortName = _portName;
                sp.Open();
               
            }
           catch(Exception er)
            {
                isOpen = false;
                return false;
            }
            portName = sp.PortName;
            isOpen = sp.IsOpen;
            return sp.IsOpen;
        }
        /// <summary>
        /// 关闭端口
        /// </summary>
        public void Close()
        {
            if (sp == null) return;
            if(!sp.IsOpen) return;
            sp.DataReceived -= new SerialDataReceivedEventHandler(OnSerialDataReceivedEventHandler);
            sp.Close();
            isOpen = false;
        }

        /// <summary>
        /// 手动发送数据
        /// </summary>
        /// <param name="cmd">数据</param>
        /// <returns></returns>
        public bool SendData(string cmd)
        {
            if (sp == null) return false;
            if (!sp.IsOpen) return false;
            sp.ReadTimeout = 1000;
            sp.WriteLine(cmd);
            try
            {
                string dat = sp.ReadExisting();
                Console.WriteLine(dat);

            }
            catch (Exception er)
            {
                Console.WriteLine(er.Message);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 获取亮度值
        /// </summary>
        /// <returns></returns>
        public byte GetLightValue(int chn)
        {
            if (chn < 1 || chn > 4) return 0;
            string channel = Enum.GetName(typeof(EnumChn), (EnumChn)chn);
            string cmd = string.Concat("S", channel, "#");
            if (sp == null) return 0;
            if (!sp.IsOpen) return 0;
            sp.ReadTimeout = 1000;
            sp.WriteLine(cmd);
            try
            {
                int tick = Environment.TickCount;
                string dat = "";
                while (Environment.TickCount - tick < 100)
                {
                    dat = sp.ReadExisting();
                    if (dat != "")
                        break;
                }
                Console.WriteLine(dat);
                if (dat.Length == 5)
                {
                    string temDat = dat.Substring(1, 4);
                    byte value = byte.Parse(temDat);
                    return value;
                }
            }
            catch (Exception er)
            {
                Console.WriteLine(er.Message);
            }
            return 0;
        }
        /// <summary>
        ///  设置亮度值
        /// </summary>
        /// <param name="chn">通道</param>
        /// <param name="lightValue">亮度值</param>
        /// <returns></returns>
        public bool WriteLightValue(int chn,byte lightValue)
        {
            if (chn < 1 || chn > 4) return false;
            string channel = Enum.GetName(typeof(EnumChn), (EnumChn)chn);
            string cmd = string.Concat("S", channel, lightValue.ToString("0000"), "#");
            if (sp == null) return false;
            if (!sp.IsOpen) return false;
            sp.ReadTimeout = 1000;
            sp.WriteLine(cmd);
            try
            {
                int tick = Environment.TickCount;
                string dat = "";
                while (Environment.TickCount - tick < 100)
                {
                    dat = sp.ReadExisting();
                    if (dat != "")
                        break;
                }        
                Console.WriteLine(dat);
                if (dat.ToUpper().Equals("A") ||
                    dat.ToUpper().Equals("B") ||
                    dat.ToUpper().Equals("C") ||
                    dat.ToUpper().Equals("D"))
                    return true;
            }
            catch (Exception er)
            {
                Console.WriteLine(er.Message);
            }
            return false;
        }
        /// <summary>
        /// 读取光源模式
        /// </summary>
        /// <returns></returns>
        public EumLightMode ReadLightMode()
        {
            string cmd = string.Concat("T", "#");
            if (sp == null) return EumLightMode.unknown; 
            if (!sp.IsOpen) return EumLightMode.unknown;
            sp.ReadTimeout = 1000;
            sp.WriteLine(cmd);
            try
            {
                int tick = Environment.TickCount;
                string dat = "";
                while (Environment.TickCount - tick < 100)
                {
                    dat = sp.ReadExisting();
                    if (dat != "")
                        break;
                }             
                Console.WriteLine(dat);
                if (dat.ToUpper().Equals("H")) return EumLightMode.normal_on;
                else if (dat.ToUpper().Equals("L")) return EumLightMode.normal_off;
            }
            catch (Exception er)
            {
                Console.WriteLine(er.Message);
            }
            return EumLightMode.unknown;
        }
        /// <summary>
        /// 设置光源模式
        /// </summary>
        /// <param name="mode"></param>
        public void WriteLightMode(EumLightMode mode)
        {
            string cmd = string.Concat("T", mode == EumLightMode.normal_on ? "H" : "L", "#");
            if (sp == null) return;
            if (!sp.IsOpen) return;
            sp.ReadTimeout = 1000;
            sp.WriteLine(cmd);
            try
            {
                int tick = Environment.TickCount;
                string dat = "";
                while (Environment.TickCount - tick < 100)
                {
                    dat = sp.ReadExisting();
                    if (dat != "")
                        break;
                }
                if (dat.ToUpper().Equals("H") ||
                    dat.ToUpper().Equals("L"))
                    Console.WriteLine("OK");
            }
            catch (Exception er)
            {
                Console.WriteLine(er.Message);
            }
        }
        /// <summary>
        /// 串口接收数据事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSerialDataReceivedEventHandler(object sender, SerialDataReceivedEventArgs e)
        {
            string dat = sp.ReadExisting();
            getDataHandle?.Invoke(dat);
        }
    }
}
