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
        /// 结果缓存集合:需要显示的区域轮廓
        /// </summary>
        //[NonSerialized]
        public Dictionary<string, HObject> resultBufDic = new Dictionary<string, HObject>();

        /// <summary>
        /// 结果缓存集合:测试信息
        /// </summary>
         public Dictionary<string, string> resultInfoDic = new Dictionary<string, string>();

        /// <summary>
        /// 结果缓存集合:测试结果
        /// </summary>
         public Dictionary<string, bool> resultFlagDic = new Dictionary<string, bool>();
        /// <summary>
        ///结果缓存集合:直线结果
        /// </summary>
         public Dictionary<string, StuLineData> LineDataDic = new Dictionary<string, StuLineData>();
        /// <summary>
        ///结果缓存集合: 定位坐标结果 
        /// </summary>
         public Dictionary<string, StuCoordinateData> PositionDataDic = new Dictionary<string, StuCoordinateData>();
        
        /// <summary>
        /// 缓存集合复位
        /// </summary>
        public  void  ResetBuf()
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
            resultBufDic.Clear();
            resultInfoDic.Clear();
            resultFlagDic.Clear();
            LineDataDic.Clear();
            PositionDataDic.Clear();

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
        public StuCoordinateData(double _row, double _column, double _angle)
        {
            row = _row;
            column = _column;
            angle = _angle;
         
        }
        public double row;
        public double column;
        public double angle;
       
      

    } /// <summary>
      /// 定位检测坐标全数据
      /// </summary>
    [Serializable]
    public struct StuTotalData
    {
        public StuTotalData(double _row, double _column, double _angle, double x, double y)
        {
            row = _row;
            column = _column;
            angle = _angle;
            X = x;
            Y = y;
        }
        public double row;
        public double column;
        public double angle;

        public double X;
        public double Y;

    }


    [Serializable]
    public class LineReslutData
    {
        public LineReslutData(double _row1, double _column1, double _row2, double _column2,
          double _angle)
        {
            row1 = _row1.ToString("f3");
            column1 = _column1.ToString("f3");
            row2 = _row2.ToString("f3");
            column2 = _column2.ToString("f3");
            angle = _angle.ToString("f3");
            ID++;
            id = ID.ToString();
        }
        static int ID = 0;
        public string id { get; set; }
        public string row1 { get; set; }
        public string column1 { get; set; }
        public string row2 { get; set; }
        public string column2 { get; set; }
        public string angle { get; set; }

        public void dataToRow(ref DataGridViewRow dgrow)
        {

            dgrow.SetValues(
                    id,
                    row1,
                    column1,
                     row2,
                    column2,
                    angle
                 );
        }

    }
    [Serializable]
    public class FitLineReslutData
    {
        public FitLineReslutData(
          double _angle)
        {
          
            angle = _angle.ToString("f3");
            ID++;
            id = ID.ToString();
        }
        static int ID = 0;
        public string id { get; set; }
     
        public string angle { get; set; }

        public void dataToRow(ref DataGridViewRow dgrow)
        {

            dgrow.SetValues(
                    id,
                  
                    angle
                 );
        }

    }
    [Serializable]
    public class LineCentreReslutData
    {
        public LineCentreReslutData(
          double _centreRow, double _centreCol)
        {

            centreRow = _centreRow.ToString("f3");
            centreCol = _centreCol.ToString("f3");
            ID++;
            id = ID.ToString();
        }
        static int ID = 0;
        public string id { get; set; }

        public string centreRow { get; set; }
        public string centreCol { get; set; }

        public void dataToRow(ref DataGridViewRow dgrow)
        {

            dgrow.SetValues(
                    id,
                    centreRow,
                    centreCol
                 );
        }

    }

    [Serializable]
    public class LineIntersectionData
    {
        public LineIntersectionData(double _row1, double _column1)
        {
            row1 = _row1.ToString("f3");
            column1 = _column1.ToString("f3");
          
            ID++;
            id = ID.ToString();
        }
        static int ID = 0;
        public string id { get; set; }
        public string row1 { get; set; }
        public string column1 { get; set; }
     

        public void dataToRow(ref DataGridViewRow dgrow)
        {

            dgrow.SetValues(
                    id,
                    row1,
                    column1
                 
                 );
        }

    }

    [Serializable]
    public class ParallelLineData
    {
        public ParallelLineData(double _row1, double _column1, double _row2, double _column2,
          double _angle)
        {
            row1 = _row1.ToString("f3");
            column1 = _column1.ToString("f3");
            row2 = _row2.ToString("f3");
            column2 = _column2.ToString("f3");
            angle = _angle.ToString("f3");
            ID++;
            id = ID.ToString();
        }
        static int ID = 0;
        public string id { get; set; }
        public string row1 { get; set; }
        public string column1 { get; set; }
        public string row2 { get; set; }
        public string column2 { get; set; }
        public string angle { get; set; }

        public void dataToRow(ref DataGridViewRow dgrow)
        {

            dgrow.SetValues(
                    id,
                    row1,
                    column1,
                     row2,
                    column2,
                    angle
                 );
        }
    }

    [Serializable]
    public class MatchData
    {
        public MatchData( double _score,  double _row1, double _column1,double _angle)
        {
            score = _score.ToString("f3");        
            row1 = _row1.ToString("f3");
            column1 = _column1.ToString("f3");
            angle = _angle.ToString("f3");
            ID++;
            id = ID.ToString();
        }
        static int ID = 0;
        public string id { get; set; }
        public string score { get; set; }
        public string row1 { get; set; }
        public string column1 { get; set; }
        public string angle { get; set; }
        public void dataToRow(ref DataGridViewRow dgrow)
        {

            dgrow.SetValues(
                    id,
                    score,
                    row1,
                    column1,
                    angle

                 );
        }

    }


    [Serializable]
    public class CircleReslutData
    {
        public CircleReslutData(double _row, double _column, 
          double _radius)
        {
            row = _row.ToString("f3");
            column = _column.ToString("f3");
            radius = _radius.ToString("f3");       
            ID++;
            id = ID.ToString();
        }
        static int ID = 0;
        public string id { get; set; }
        public string row { get; set; }
        public string column { get; set; }     
        public string radius { get; set; }

        public void dataToRow(ref DataGridViewRow dgrow)
        {

            dgrow.SetValues(
                    id,
                    row,
                    column,
                     radius               
                 );
        }

    }
    [Serializable]
    public class ParticleFeaturesData
    {
        public ParticleFeaturesData(bool t_isUse, string t_features,
            string t_minValue, string t_maxValue)
        {
            isUse = t_isUse;
            features = t_features;
            minValue = t_minValue;
            maxValue = t_maxValue;
        }
        public bool isUse { get; set; }
        public  string  features{ get; set; }
        public string minValue { get; set; }
        public string maxValue { get; set; }
        public void dataToRow(ref DataGridViewRow dgrow)
        {

            dgrow.SetValues(
                    isUse,
                    features,
                    minValue,
                    maxValue
                 );
        }

    }

    [Serializable]
    public class BlobFeaturesResultData
    {
        public BlobFeaturesResultData(double _row, double _column,
            double _area)
        {
            row = _row.ToString("f3");
            column = _column.ToString("f3");
            area = _area.ToString("f3");
            ID++;
            id = ID.ToString();
        }
        static int ID = 0;
        public string id { get; set; }
        public string row { get; set; }
        public string column { get; set; }
        public string area { get; set; }

        public void dataToRow(ref DataGridViewRow dgrow)
        {

            dgrow.SetValues(
                    id,
                    row,
                    column,
                    area
                 );
        }

    }

    [Serializable]
    public class ResultShowData
    {
        public ResultShowData(bool t_isUse, string t_toolName, string t_status)
        {
            isUse = t_isUse;
            toolName = t_toolName;
            status = t_status;
        }
        public bool isUse { get; set; }
        public string toolName { get; set; }
        public string status { get; set; }


        public void dataToRow(ref DataGridViewRow dgrow)
        {

            dgrow.SetValues(
                    isUse,
                    toolName,
                    status
                 );

        }
    }

}
