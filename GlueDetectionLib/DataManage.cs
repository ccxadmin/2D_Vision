using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using HalconDotNet;

namespace GlueDetectionLib
{
    [Serializable]
    public class DataManage
    {
        /// <summary>
        /// 图像缓存集合
        /// </summary>
         public Dictionary<string, HObject> imageBufDic = new Dictionary<string, HObject>();
        /// <summary>
        /// 矩阵缓存集合
        /// </summary>
         public Dictionary<string, HTuple> matrixBufDic = new Dictionary<string, HTuple>();
        /// <summary>
        /// 可以被枚举的工具名称
        /// </summary>
         public List<string> enumerableTooDic = new List<string>();
        /// <summary>
        /// 结果缓存集合:区域轮廓
        /// </summary>
         public Dictionary<string, HObject> resultBufDic = new Dictionary<string, HObject>();

        /// <summary>
        /// 结果缓存集合:打印信息
        /// </summary>
         public Dictionary<string, string> resultInfoDic = new Dictionary<string, string>();

        /// <summary>
        /// 结果缓存集合:工具运行结果
        /// </summary>
         public Dictionary<string, bool> resultFlagDic = new Dictionary<string, bool>();

        // <summary>
        /// 缓存集合复位
        /// </summary>
         public  void ResetBuf()
        {
            Dictionary<string, HObject> temData = DeepCopy2<Dictionary<string, HObject>>(imageBufDic);
            foreach (var s in temData)
            {
                if (s.Key.Contains("原始图像"))   //保留缓存图像中的原始图像
                    continue;
                imageBufDic.Remove(s.Key);
            }
            temData.Clear();
            //imageBufDic.Clear();
            matrixBufDic.Clear();
            enumerableTooDic.Clear();
            resultBufDic.Clear();
            resultInfoDic.Clear();
            resultFlagDic.Clear();
        }
        /// <summary>
        /// 深度复制（二进制）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        static public T DeepCopy2<T>(T obj)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, obj);
                memoryStream.Seek(0, SeekOrigin.Begin);
                var t = (T)binaryFormatter.Deserialize(memoryStream);
                return t;
            }
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] Serialize(object obj)
        {
            if (obj == null)
                return null;

            using var memoryStream = new MemoryStream();
            DataContractSerializer ser = new DataContractSerializer(typeof(object));
            ser.WriteObject(memoryStream, obj);
            var data = memoryStream.ToArray();
            return data;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T Deserialize<T>(byte[] data)
        {
            if (data == null)
                return default(T);

            using var memoryStream = new MemoryStream(data);
            XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(memoryStream, new XmlDictionaryReaderQuotas());
            DataContractSerializer ser = new DataContractSerializer(typeof(T));
            var result = (T)ser.ReadObject(reader, true);
            return result;
        }
    }

    /// <summary>
    /// 胶水关键信息
    /// </summary>
    [Serializable]
    public struct StuGlueMainInfo
    {
        public string Name;
        public double Cx;
        public double Cy;
        public double Rc_Width;
        public double Rc_Height;
        public double Area;
        public double CxDown;
        public double CxUp;
        public double CyDown;
        public double CyUp;
        public double AreaDown;
        public double AreaUp;


        public bool compareX()
        {
            return Cx >= CxDown && Cx <= CxUp;
        }
        public bool compareY()
        {
            return Cy >= CyDown && Cy <= CyUp;
        }
        public override string ToString()
        {
            string info = "";
            info+=string.Format("{0}_X_dis:{1:f3}({2:f3}-{3:f3})\n", Name, Cx, CxDown, CxUp);
            info += string.Format("{0}_Y_dis:{1:f3}({2:f3}-{3:f3})\n", Name, Cy, CyDown, CyUp);
            info += string.Format("{0}_Glue_area:{1:f3}({2:f3}-{3:f3})\n", Name, Area, AreaDown, AreaUp);
            info += string.Format("{0}_Glue_length:{1:f3}\n", Name, Rc_Width);
            info += string.Format("{0}_Glue_Width:{1:f3}\n", Name, Rc_Height); 
            return info;

        }
    }


    [Serializable]
    public struct CoorditionDat
    {
        public CoorditionDat(double row, double column)
        {
            Row = row;
            Column = column;
        }
        public double Row;
        public double Column;
    }
    /// <summary>
    /// 胶水复检信息
    /// </summary>
    [Serializable]
    public class GlueInfo
    {
        public List<CoorditionDat> coorditions = new List<CoorditionDat>();
        public string toolName = "";
        public List<double> areaList = new List<double>();
        public List<double> rect_r1 = new List<double>();
        public List<double> rect_c1 = new List<double>();
        public List<double> rect_r2 = new List<double>();
        public List<double> rect_c2 = new List<double>();
    }

    [Serializable]
    public struct StuFlagInfo
    {
        public StuFlagInfo(string _info,bool _flag)
        {
            info = _info;
            flag = _flag;
        }
        public bool flag;
        public string info;
    }

    /// <summary>
    /// 区域合并方式
    /// </summary>
    public enum EumRegionUnionWay
    {
        concat,
        difference
    }

    /// <summary>
    /// 区域生成方式
    /// </summary>
    public enum EumGenRegionWay
    {
        auto,
        manual
    }
    /// <summary>
    /// 轮廓生成方式
    /// </summary>
    public enum EumGenContourWay
    {
        auto,
        manual
    }
    [Serializable]
    public struct StuRegionBuf
    {
        public string regionName;
        public bool isUse;
        public HObject region;
        public string status;
        public EumRegionUnionWay regionUnionWay;
      

    }
    [Serializable]
    public struct StuContourBuf
    {
        public string contourName;
        public bool isUse;
        public HObject xld;
   
    }

   

    public enum EumPolarity
    {
     white,
     black 
    }


    public enum EumDrawRegionType
    {
        any,
        rectangle,
        rarectangle,
        circle
        
    }



}
