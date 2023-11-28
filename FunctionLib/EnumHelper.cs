using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionLib
{
    public static class EnumHelper
    {
        /// <summary>
        /// 根据枚举的值获取枚举名称
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="status">枚举的值</param>
        /// <returns></returns>
        public static string GetEnumName<T>(this int status)
        {
            return Enum.GetName(typeof(T), status);
        }
        /// <summary>
        /// 获取枚举名称集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string[] GetNamesArr<T>()
        {
            return Enum.GetNames(typeof(T));
        }
        /// <summary>
        /// 将枚举转换成字典集合
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <returns></returns>
        public static Dictionary<string, int> getEnumDic<T>()
        {

            Dictionary<string, int> resultList = new Dictionary<string, int>();
            Type type = typeof(T);
            var strList = GetNamesArr<T>().ToList();
            foreach (string key in strList)
            {
                string val = Enum.Format(type, Enum.Parse(type, key), "d");
                resultList.Add(key, int.Parse(val));
            }
            return resultList;
        }
        /// <summary>
        /// 将枚举转换成字典
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static Dictionary<string, int> GetDic<TEnum>()
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();
            Type t = typeof(TEnum);
            var arr = Enum.GetValues(t);
            foreach (var item in arr)
            {
                dic.Add(item.ToString(), (int)item);
            }

            return dic;
        }

    }

    [Serializable]
    public enum ROIType
    {
        /// <summary>
        /// 直线
        /// </summary>
        Line = 10,
        /// <summary>
        /// 圆
        /// </summary>
        Circle,
        /// <summary>
        /// 圆弧
        /// </summary>
        CircleArc,
        /// <summary>
        /// 矩形
        /// </summary>
        Rectangle1,
        /// <summary>
        /// 带角度矩形
        /// </summary>
        Rectangle2
    }
    /// <summary>
    /// ROI运算
    /// </summary>
    public enum ROIOperation
    {
        /// <summary>
        /// ROI求和模式
        /// </summary>
        Positive = 21,
        /// <summary>
        /// ROI求差模式
        /// </summary>
        Negative,
        /// <summary>
        /// ROI模式为无
        /// </summary>
        None,
    }
    public enum ViewMessage
    {
        /// <summary>Constant describing an update of the model region</summary>
        UpdateROI = 50,

        ChangedROISign,

        /// <summary>Constant describing an update of the model region</summary>
        MovingROI,
        DeletedActROI,
        DelectedAllROIs,

        ActivatedROI,

        MouseMove,
        CreatedROI,
        /// <summary>
        /// Constant describes delegate message to signal new image
        /// </summary>
        UpdateImage,
        /// <summary>
        /// Constant describes delegate message to signal error
        /// when reading an image from file
        /// </summary>
        ErrReadingImage,
        /// <summary> 
        /// Constant describes delegate message to signal error
        /// when defining a graphical context
        /// </summary>
        ErrDefiningGC
    }
    public enum ShowMode
    {
        /// <summary>
        /// 包含ROI显示
        /// </summary>
        IncludeROI = 1,
        /// <summary>
        /// 不包含ROI显示
        /// </summary>
        ExcludeROI
    }
    public enum ResultShow
    {
        原图,
        处理后
    }
    public enum eumSaveType
    {
        OK,
        NG,
        ALL,
        None

    }
    public enum eumImgDelWays
    {
        AutoClear,
        ManulClear

    }
}
