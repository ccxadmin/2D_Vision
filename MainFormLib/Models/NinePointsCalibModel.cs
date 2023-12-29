using ControlShareResources.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainFormLib.Models
{
    public class NinePointsCalibModel : NotifyBase
    {
        private double txbPixelX;
        public double TxbPixelX
        {
            get { return this.txbPixelX; }
            set
            {
                txbPixelX = value;
                DoNotify();

            }
        }

        private double txbPixelY;
        public double TxbPixelY
        {
            get { return this.txbPixelY; }
            set
            {
                txbPixelY = value;
                DoNotify();

            }
        }

        private double txbRobotX;
        public double TxbRobotX
        {
            get { return this.txbRobotX; }
            set
            {
                txbRobotX = value;
                DoNotify();

            }
        }

        private double txbRobotY;
        public double TxbRobotY
        {
            get { return this.txbRobotY; }
            set
            {
                txbRobotY = value;
                DoNotify();

            }
        }


        private double txbRobotR;
        public double TxbRobotR
        {
            get { return this.txbRobotR; }
            set
            {
                txbRobotR = value;
                DoNotify();

            }
        }

        private double txbRotatePixelX;
        public double TxbRotatePixelX
        {
            get { return this.txbRotatePixelX; }
            set
            {
                txbRotatePixelX = value;
                DoNotify();

            }
        }
        private double txbRotatePixelY;
        public double TxbRotatePixelY
        {
            get { return this.txbRotatePixelY; }
            set
            {
                txbRotatePixelY = value;
                DoNotify();

            }
        }

        private double txbRotateCenterX;
        public double TxbRotateCenterX
        {
            get { return this.txbRotateCenterX; }
            set
            {
                txbRotateCenterX = value;
                DoNotify();

            }
        }

        private double txbRotateCenterY;
        public double TxbRotateCenterY
        {
            get { return this.txbRotateCenterY; }
            set
            {
                txbRotateCenterY = value;
                DoNotify();

            }
        }
        private double txbSx;
        public double TxbSx
        {
            get { return this.txbSx; }
            set
            {
                txbSx = value;
                DoNotify();

            }
        }

        private double txbSy;
        public double TxbSy
        {
            get { return this.txbSy; }
            set
            {
                txbSy = value;
                DoNotify();

            }
        }

        private double txbPhi;
        public double TxbPhi
        {
            get { return this.txbPhi; }
            set
            {
                txbPhi = value;
                DoNotify();

            }
        }

        private double txbTheta;
        public double TxbTheta
        {
            get { return this.txbTheta; }
            set
            {
                txbTheta = value;
                DoNotify();

            }
        }

        private double txbTx;
        public double TxbTx
        {
            get { return this.txbTx; }
            set
            {
                txbTx = value;
                DoNotify();

            }
        }

        private double txbTy;
        public double TxbTy
        {
            get { return this.txbTy; }
            set
            {
                txbTy = value;
                DoNotify();

            }
        }

        private ObservableCollection<DgPixelPointData> dgPixelPointDataList =
                         new ObservableCollection<DgPixelPointData>();
        public ObservableCollection<DgPixelPointData> DgPixelPointDataList
        {
            get { return this.dgPixelPointDataList; }
            set
            {
                dgPixelPointDataList = value;
                DoNotify();

            }
        }

        private int dgPixelPointSelectIndex=-1;
        public int DgPixelPointSelectIndex
        {
            get { return this.dgPixelPointSelectIndex; }
            set
            {
                dgPixelPointSelectIndex = value;
                DoNotify();

            }
        }



        private ObservableCollection<DgRobotPointData> dgRobotPointDataList =
                        new ObservableCollection<DgRobotPointData>();
        public ObservableCollection<DgRobotPointData> DgRobotPointDataList
        {
            get { return this.dgRobotPointDataList; }
            set
            {
                dgRobotPointDataList = value;
                DoNotify();

            }
        }

        private int dgRobotPointSelectIndex=-1;
        public int DgRobotPointSelectIndex
        {
            get { return this.dgRobotPointSelectIndex; }
            set
            {
                dgRobotPointSelectIndex = value;
                DoNotify();

            }
        }



        private ObservableCollection<DgRotatePointData> dgRotatePointDataList =
                      new ObservableCollection<DgRotatePointData>();
        public ObservableCollection<DgRotatePointData> DgRotatePointDataList
        {
            get { return this.dgRotatePointDataList; }
            set
            {
                dgRotatePointDataList = value;
                DoNotify();

            }
        }

        private int dgRotatePointSelectIndex=-1;
        public int DgRotatePointSelectIndex
        {
            get { return this.dgRobotPointSelectIndex; }
            set
            {
                dgRobotPointSelectIndex = value;
                DoNotify();

            }
        }
    }


    /// <summary>
    ///九点像素坐标点位信息
    /// </summary>
    [Serializable]
    public class DgPixelPointData
    {

        public DgPixelPointData( int id, double x, double y)
        {
          
            ID = id;
            X = x;
            Y = y;

        }

       
        public int ID { get; set; }
        /// <summary>
        ///像素X坐标
        /// </summary>
        public double X { get; set; } 

        /// <summary>
        /// 像素Y坐标
        /// </summary>
        public double Y { get; set; }

    }
    /// <summary>
    ///九点像素坐标点位信息
    /// </summary>
    [Serializable]
    public class DgRobotPointData
    {

        public DgRobotPointData(int id, double x, double y)
        {

            ID = id;
            X = x;
            Y = y;

        }


        public int ID { get; set; }
        /// <summary>
        ///物理X坐标
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// 物理Y坐标
        /// </summary>
        public double Y { get; set; }

    }

    /// <summary>
    //旋转像素坐标点位信息
    /// </summary>
    [Serializable]
    public class DgRotatePointData
    {

        public DgRotatePointData(int id, double x, double y)
        {

            ID = id;
            X = x;
            Y = y;

        }


        public int ID { get; set; }
        /// <summary>
        ///像素X坐标
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// 像素Y坐标
        /// </summary>
        public double Y { get; set; }

    }

}
