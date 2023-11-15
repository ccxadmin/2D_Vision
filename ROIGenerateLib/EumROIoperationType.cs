using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ROIGenerateLib
{
    public class EnumOprationclass
    {
      public static  string GetDescription(Enum enumName)
        {
            string _description = string.Empty;
            FieldInfo _fieldInfo = enumName.GetType().GetField(enumName.ToString());
            DescriptionAttribute[] _attributes = _fieldInfo.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), true)
                        as System.ComponentModel.DescriptionAttribute[];

            if (_attributes != null && _attributes.Length > 0)
                _description = _attributes[0].Description;
            else
                _description = enumName.ToString();
            return _description;
        }

    }
    public enum EumROIConstanceSymbolInfo
    {
        [Description("圆")]
        Circle,
        [Description("圆弧")]
        CircularArc,
        [Description("组合")]
        Combine,
        [Description("椭圆")]
        Ellipse,
        [Description("直线")]
        Line,
        [Description("点")]
        Point,
        [Description("多边形")]
        Polygon,
        [Description("矩形")]
        Rectangle1,
        [Description("旋转矩形")]
        Rectangle2
    
    }
    public enum EumROIoperationType
    {
        CreateRect,
        CreateRatRect,
        CreateCircle,
        CreateCircleArc,
        CreateLine,
        CreatePoint,
        CreatePolygn,
        CreateEllipse,
        ROIDelete,
        ROIAddup,
        ROISubtract,
       

    }
    /// <summary>
    /// ROI更新变化种类枚举
    /// </summary>
    public enum EumROIupdate
    {
        /// <summary>
        /// 创建
        /// </summary>
        create,
        /// <summary>
        /// 删除
        /// </summary>
        delete,
        /// <summary>
        /// 尺寸变化
        /// </summary>
        sizeChange,
        /// <summary>
        /// 移动
        /// </summary>
        move,
        /// <summary>
        /// 组合
        /// </summary>
        combine,
        /// <summary>
        /// 无
        /// </summary>
        none

    }

    public enum EumSystemPattern
    {
        RunningModel,
        DesignModel

    }
}
