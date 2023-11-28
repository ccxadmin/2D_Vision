using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalFunctionLib
{
    /// <summary>
    /// 外部视觉对应接口功能
    /// </summary>
  public  interface IExternalVision
    {
        /// <summary>
        /// 设定端口编号
        /// </summary>
        /// <param name="portNum"></param>
        void SetPort(int portNum);
       
        /// <summary>
        /// 错误清除
        /// </summary>
        void ClearErr();
        /// <summary>
        /// 品种切换
        /// VarietyNumber：品种编号
        /// 返回True:切换成功，Fasle:切换失败
        /// </summary>
        /// <param name="varietyNumber">品种编号</param>
        /// <returns>切换成功标志</returns>
        bool SwitchVariety(int varietyNumber);
        /// <summary>
        /// 设置曝光
        /// camNumber：相机编号
        /// expouseValue：曝光值
        /// </summary>
        /// <param name="camNumber"></param>
        /// <param name="expouseValue"></param>
        /// 返回True:设置成功，Fasle:设置失败
        /// <returns>设置成功标志</returns>
        bool SetExpouse(int camNumber,long expouseValue);
        ///// <summary>
        ///// 设置光源亮度
        ///// </summary>
        ///// <param name="channel">通道编号</param>
        ///// <param name="value">光源亮度值</param>
        ///// <returns></returns>
        //bool SetBrightness(int channel, byte value);
        /// <summary>
        /// Mark点位识别
        /// camNumber：相机编号
        /// extractType：提取对象类型
        /// objectNumber：对象识别编号
        /// instructionType：指令类型
        /// sn：条码
        /// 返回mark点位识别信息
        /// </summary>
        /// <param name="camNumber">相机编号</param>
        /// <param name="extractType">提取对象类型</param>
        /// <param name="objectNumber">对象识别编号</param>
        /// <param name="instructionType">指令类型</param>
        /// <param name="sn">条码</param>
        /// <returns>返回mark点位识别信息</returns>
        StuResultOfMark GetMarkPosi(int camNumber, EumExtractObjectType extractType,
            int objectNumber,int instructionType,string sn);
        /// <summary>
        /// 轨迹提取
        /// camNumber：相机编号
        /// extractType：提取对象类型
        /// objectNumber：对象识别编号
        /// instructionType：指令类型
        /// sn：条码
        /// 返回轨迹提取信息
        /// </summary>
        /// <param name="camNumber">相机编号</param>
        /// <param name="extractType">提取对象类型</param>
        /// <param name="objectNumber">对象识别编号</param>
        /// <param name="instructionType">指令类型</param>
        /// <param name="sn">条码</param>
        /// <returns>返回轨迹提取信息</returns>
        StuResultOfTrajectory GetTrajectoryPosi(int camNumber, EumExtractObjectType extractType,
            int objectNumber, int instructionType, string sn);

     

    }
    /// <summary>
    /// Mark检测结果
    /// </summary>
    public struct StuResultOfMark
    {
        /// <summary>
        /// 运行结果
        /// </summary>
        public bool runResult;
        /// <summary>
        /// mark点位坐标x
        /// </summary>
        public double posiX;
        /// <summary>
        /// mark点位坐标y
        /// </summary>
        public double posiY;
        /// <summary>
        /// 图像在本地的路径
        /// </summary>
        public string imagePath;
        /// <summary>
        /// 图像宽度
        /// </summary>
        public int imageWidth;
        /// <summary>
        /// 图像高度
        /// </summary>
        public int imageHeight;


    }
    /// <summary>
    ///轨迹检测结果
    /// </summary>
    public struct StuResultOfTrajectory
    {
        /// <summary>
        /// 运行结果
        /// </summary>
        public bool runResult;
        /// <summary>
        /// mark点位坐标x
        /// </summary>
        public double posiX;
        /// <summary>
        /// mark点位坐标y
        /// </summary>
        public double posiY;
        /// <summary>
        /// 图像在本地的路径
        /// </summary>
        public string imagePath;
        /// <summary>
        /// 图像宽度
        /// </summary>
        public int imageWidth;
        /// <summary>
        /// 图像高度
        /// </summary>
        public int imageHeight;
        /// <summary>
        /// 坐标点个数
        /// </summary>
        public int pointNum;
        /// <summary>
        /// 轨迹点集合
        /// </summary>
        public List<PointF> points;

    }
    /// <summary>
    /// 提取对象类型
    /// </summary>
    public enum EumExtractObjectType
    {
        /// <summary>
        /// 特征点
        /// </summary>
        Mark = 0,
        /// <summary>
        /// 圆
        /// </summary>
        Circle,
        /// <summary>
        /// 直线
        /// </summary>
        Line

    }
    /// <summary>
    /// 指令集合
    /// </summary>
    public enum EumCmdSet
    {
        /// <summary>
        /// 清除错误
        /// </summary>
        FRST,
        /// <summary>
        /// 品种切换
        /// </summary>
        FSPC,
        /// <summary>
        /// 设置曝光
        /// </summary>
        FSEX,
        /// <summary>
        /// 特征点识别
        /// </summary>
        Mark,
        /// <summary>
        /// 轨迹点识别
        /// </summary>
        Trajectory
    }

}
