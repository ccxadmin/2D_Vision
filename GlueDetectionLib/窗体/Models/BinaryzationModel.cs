using ControlShareResources.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlueDetectionLib.窗体.Models
{
    public class BinaryzationModel : NotifyBase
    {
       /// <summary>
       /// 窗体标题
       /// </summary>
        private string titleName = "参数设置窗体";
        public string TitleName
        {
            get { return this.titleName; }
            set
            {
                titleName = value;
                DoNotify();
            }
        }
        /// <summary>
        /// 图像源集合
        /// </summary>
        private ObservableCollection<string> imageList = new ObservableCollection<string>();
        public ObservableCollection<string> ImageList
        {
            get { return this.imageList; }
            set
            {
                imageList = value;
                DoNotify();
            }
        }
        /// <summary>
        ///选择图像名称
        /// </summary>
        private string selectImageName ;
        public string SelectImageName
        {
            get { return this.selectImageName; }
            set
            {
                selectImageName = value;
                DoNotify();
            }
        }
        /// <summary>
        ///选择图像索引编号
        /// </summary>
        private int selectImageIndex;
        public int SelectImageIndex
        {
            get { return this.selectImageIndex; }
            set
            {
                selectImageIndex = value;
                DoNotify();
            }
        }
     
        /// <summary>
        /// 最小灰度值
        /// </summary>
        private byte grayMin;
        public byte GrayMin
        {
            get { return this.grayMin; }
            set
            {
                grayMin = value;
                DoNotify();
            }
        }

        //private Action numGrayMinValueChangeAction;
        //public Action NumGrayMinValueChangeAction
        //{
        //    get { return numGrayMinValueChangeAction; }
        //    set
        //    {
        //        numGrayMinValueChangeAction = value;
        //        DoNotify();
        //    }
        //}

        /// <summary>
        /// 最大灰度值
        /// </summary>
        private byte grayMax;
        public byte GrayMax
        {
            get { return this.grayMax; }
            set
            {
                grayMax = value;
                DoNotify();
            }
        }

        //private Action numGrayMaxValueChangeAction;
        //public Action NumGrayMaxValueChangeAction
        //{
        //    get { return numGrayMaxValueChangeAction; }
        //    set
        //    {
        //        numGrayMaxValueChangeAction = value;
        //        DoNotify();
        //    }
        //}
        /// <summary>
        /// 是否图像反转
        /// </summary>
        private bool isInvertImage;
        public bool IsInvertImage
        {
            get { return this.isInvertImage; }
            set
            {
                isInvertImage = value;
                DoNotify();
            }
        }

    }


}
