using ControlShareResources.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace MainFormLib.Models
{
    public class CalibAssistantModel : NotifyBase
    {
        private int dgDataSelectIndex;
        public  int DgDataSelectIndex
        {
            get { return this.dgDataSelectIndex; }
            set
            {
                dgDataSelectIndex = value;
                DoNotify();

            }
        }

        private int txbF=12;
        public int TxbF
        {
            get { return this.txbF; }
            set
            {
                txbF = value;
                DoNotify();

            }
        }

        private double txbSx=1.85;
        public double TxbSx
        {
            get { return this.txbSx; }
            set
            {
                txbSx = value;
                DoNotify();

            }
        }

        private double txbSy=1.85;
        public double TxbSy
        {
            get { return this.txbSy; }
            set
            {
                txbSy = value;
                DoNotify();

            }
        }

        private int txbCx=2012;
        public int TxbCx
        {
            get { return this.txbCx; }
            set
            {
                txbCx = value;
                DoNotify();

            }
        }

        private int txbCy=1518;
        public int TxbCy
        {
            get { return this.txbCy; }
            set
            {
                txbCy = value;
                DoNotify();

            }
        }

        private int txbWidth=4024;
        public int TxbWidth
        {
            get { return this.txbWidth; }
            set
            {
                txbWidth = value;
                DoNotify();

            }
        }
        private int txbHeight=3036;
        public int TxbHeight
        {
            get { return this.txbHeight; }
            set
            {
                txbHeight = value;
                DoNotify();

            }
        }

        private int txbBoardXNum=7;
        public int TxbBoardXNum
        {
            get { return this.txbBoardXNum; }
            set
            {
                txbBoardXNum = value;
                DoNotify();

            }
        }

        private int txbBoardYNum=7;
        public int TxbBoardYNum
        {
            get { return this.txbBoardYNum; }
            set
            {
                txbBoardYNum = value;
                DoNotify();

            }
        }

        private double txbBoardMarkDis=30;
        public double TxbBoardMarkDis
        {
            get { return this.txbBoardMarkDis; }
            set
            {
                txbBoardMarkDis = value;
                DoNotify();

            }
        }

        private double txbBoardMarkDisRotia=0.5;
        public double TxbBoardMarkDisRotia
        {
            get { return this.txbBoardMarkDisRotia; }
            set
            {
                txbBoardMarkDisRotia = value;
                DoNotify();

            }
        }

        private string txbBoardFilePath;
        public string TxbBoardFilePath
        {
            get { return this.txbBoardFilePath; }
            set
            {
                txbBoardFilePath = value;
                DoNotify();

            }
        }

        private double txbCalibRMS;
        public double TxbCalibRMS
        {
            get { return this.txbCalibRMS; }
            set
            {
                txbCalibRMS = value;
                DoNotify();

            }
        }


        private bool btnReadyCalibEnable = true;
        public bool BtnReadyCalibEnable
        {
            get { return this.btnReadyCalibEnable; }
            set
            {
                btnReadyCalibEnable = value;
                DoNotify();

            }
        }
        private bool btnManualCalibEnable;
        public bool BtnManualCalibEnable
        {
            get { return this.btnManualCalibEnable; }
            set
            {
                btnManualCalibEnable = value;
                DoNotify();

            }
        }


        private ObservableCollection<ImageCorrectInfo> dgImageCorrectInfoList =
                          new ObservableCollection<ImageCorrectInfo>();
        public ObservableCollection<ImageCorrectInfo> DgImageCorrectInfoList
        {
            get { return this.dgImageCorrectInfoList; }
            set
            {
                dgImageCorrectInfoList = value;
                DoNotify();

            }
        }
    }

    /// <summary>
    ///校正图像的信息
    /// </summary>
    [Serializable]
    public class ImageCorrectInfo
    {
       
        public ImageCorrectInfo( bool _isUse,int _id, string _name, string _imageInfo = "未标定")
        {
            IsUse = _isUse;
            ID = _id;
            Name = _name;
            ImageInfo = _imageInfo;

        }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsUse { get; set; }

        public int ID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } = "image1";

        /// <summary>
        /// 图像信息
        /// </summary>
        public string ImageInfo { get; set; }

    }
    public class ColorValueConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //var flag = System.Convert.ToBoolean(parameter);//确是否转换
            if (value.ToString().Contains("成功"))
                return Brushes.Green;
            else
                return Brushes.Red;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
