using ControlShareResources.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlueDetectionLib.窗体.Models
{
    public class ErosionModel : NotifyBase
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
        private string selectImageName;
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
        /// 掩膜宽度集合
        /// </summary>
        private ObservableCollection<int> maskWidthList = new ObservableCollection<int>(new int[] { 1, 3, 5, 7, 9, 11 });
        public ObservableCollection<int> MaskWidthList
        {
            get { return this.maskWidthList; }
            set
            {
                maskWidthList = value;
                DoNotify();
            }
        }
        /// <summary>
        ///选择掩膜宽度
        /// </summary>
        private int selectMaskWidth;
        public int SelectMaskWidth
        {
            get { return this.selectMaskWidth; }
            set
            {
                selectMaskWidth = value;
                DoNotify();
            }
        }
        /// <summary>
        ///选择掩膜宽度索引编号
        /// </summary>
        private int selectMaskWidthIndex;
        public int SelectMaskWidthIndex
        {
            get { return this.selectMaskWidthIndex; }
            set
            {
                selectMaskWidthIndex = value;
                DoNotify();
            }
        }

        /// <summary>
        /// 掩膜高度集合
        /// </summary>
        private ObservableCollection<int> maskHeightList = new ObservableCollection<int>(new int[] { 1, 3, 5, 7, 9, 11 });
        public ObservableCollection<int> MaskHeightList
        {
            get { return this.maskWidthList; }
            set
            {
                maskWidthList = value;
                DoNotify();
            }
        }
        /// <summary>
        ///选择掩膜高度
        /// </summary>
        private int selectMaskHeight;
        public int SelectMaskHeight
        {
            get { return this.selectMaskHeight; }
            set
            {
                selectMaskHeight = value;
                DoNotify();
            }
        }
        /// <summary>
        ///选择掩膜高度索引编号
        /// </summary>
        private int selectMaskHeightIndex;
        public int SelectMaskHeightIndex
        {
            get { return this.selectMaskHeightIndex; }
            set
            {
                selectMaskHeightIndex = value;
                DoNotify();
            }
        }
    }
}
