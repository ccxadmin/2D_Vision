using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HalconDotNet;
using PositionToolsLib.窗体.Pages;

namespace PositionToolsLib
{
    [Serializable]
    public class DataManage
    {
        /// <summary>
        /// 图像缓存集合
        /// </summary>
        //[NonSerialized]
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
        /// 轨迹工具名称
        /// </summary>
        public List<string> trajectoryTooDic = new List<string>();
        /// <summary>
        /// 轨迹工具名称
        /// </summary>
        public List<string> sizeTooDic = new List<string>();
        /// <summary>
        /// 结果缓存集合:需要显示的区域轮廓
        /// </summary>
        //[NonSerialized]
        public Dictionary<string, HObject> resultBufDic = new Dictionary<string, HObject>();

        /// <summary>
        /// 结果缓存集合:测试信息
        /// </summary>
        //[NonSerialized]
        public Dictionary<string, string> resultInfoDic = new Dictionary<string, string>();

        /// <summary>
        /// 结果缓存集合:测试结果
        /// </summary>
        //[NonSerialized]
        public Dictionary<string, bool> resultFlagDic = new Dictionary<string, bool>();
        /// <summary>
        ///结果缓存集合:直线结果
        /// </summary>
         public Dictionary<string, StuLineData> LineDataDic = new Dictionary<string, StuLineData>();
        /// <summary>
        ///结果缓存集合: 定位坐标结果 (像素坐标或物理坐标)（x,y,angle）
        /// </summary>
         public Dictionary<string, StuCoordinateData> PositionDataDic = new Dictionary<string, StuCoordinateData>();
        /// <summary>
        /// 轨迹点集合
        /// </summary>
        public Dictionary<string, List<DgTrajectoryData>> TrajectoryDataDic = new Dictionary<string, List<DgTrajectoryData>>();
        /// <summary>
        /// 尺寸集合
        /// </summary>
        public Dictionary<string,double> SizeDataDic = new Dictionary<string, double>();
        /// <summary>
        ///结果缓存集合: 定位坐标结果 (像素坐标+物理坐标)
        /// </summary>
     //   public Dictionary<string, StuTotalData> PositionAllDataDic = new Dictionary<string, StuTotalData>();

        /// <summary>
        /// 缓存集合复位
        /// </summary>
        public void  ResetBuf()
        {
            Dictionary<string, HObject> temData = DeepCopy2< Dictionary<string, HObject> > (imageBufDic);
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
            trajectoryTooDic.Clear();
            sizeTooDic.Clear();
            resultBufDic.Clear();
            resultInfoDic.Clear();
            resultFlagDic.Clear();
            LineDataDic.Clear();
            PositionDataDic.Clear();
            TrajectoryDataDic.Clear();
            SizeDataDic.Clear();
        }
    
        /// <summary>
        /// 深度复制（反射）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        static public T DeepCopy<T>(T obj)
        {
            if (obj is string || typeof(T).IsValueType)
            {
                return obj;
            }

            var instance = Activator.CreateInstance(obj.GetType());
            FieldInfo[] fieldInfos = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var item in fieldInfos)
            {
                try
                {
                    item.SetValue(instance, DeepCopy(item.GetValue(obj)));
                }
                catch (Exception e) { }
            }

            return (T)instance;
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
        public void dataToRow(ref DataGridViewRow dgrow)
        {

            dgrow.SetValues(
                    isUse,
                    regionName,
                    status
                 );

        }

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
    /// <summary>
    /// blob筛选粒子特性
    /// </summary>
    public enum EumParticleFeatures
    {
      area,
      width,
      height,
      circularity,
      rectangularity
    }
    /// <summary>
    /// Blob粒子特性结果
    /// </summary>
    [Serializable]
    public struct StuBlobFeaturesResult
    {
        /// <summary>
        /// 中心行坐标
        /// </summary>
        public double row;
        /// <summary>
        /// 中心列坐标
        /// </summary>
        public double column;
        /// <summary>
        /// 面积
        /// </summary>
        public double area;
        ///// <summary>
        ///// 宽度
        ///// </summary>
        //public double width;
        ///// <summary>
        ///// 高度
        ///// </summary>
        //public double height;
        ///// <summary>
        ///// 圆度
        ///// </summary>
        //public double circularity;
        ///// <summary>
        ///// 矩形度
        ///// </summary>
        //public double rectangularity;
    }

    public enum EumTransition
    { 
        positive,
        negative,
        all
    }
    public enum EumSelect
    {
        first,
        last,
        max
    }

    public enum EumDirection
    {
        outer,
        inner
    }

    /// <summary>
    /// 直线定义
    /// </summary>
    [Serializable]
    public struct StuLineData
    {
        public StuLineData(double row1,double column1,double row2,double column2)
        {
            spRow = row1;
            spColumn = column1;
            epRow = row2;
            epColumn = column2;
        }


        public double spRow;
        public double spColumn;
        public double epRow;
        public double epColumn; 
    }
    /// <summary>
    /// 定位检测坐标数据
    /// </summary>
    [Serializable]
    public struct StuCoordinateData
    {
        public StuCoordinateData(double _x, double _y, double _angle)
        {
            x = _x;
            y = _y;
            angle = _angle;
         
        }
        public double x;
        public double y;
        public double angle;
       
      

    }

    #region----暂不使用----

    /// <summary>
    /// 定位检测坐标全数据
    /// </summary>
    //[Serializable]
    //public struct StuTotalData
    //{
    //    public StuTotalData(double _px, double _py, double _angle, double _rx, double _ry)
    //    {
    //        px = _px;
    //        py = _py;
    //        angle = _angle;
    //        rx = _rx;
    //        ry = _ry;
    //    }
    //    public double px;
    //    public double py;
    //    public double angle;

    //    public double rx;
    //    public double ry;

    //}

    #endregion

}
