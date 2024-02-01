namespace VisionShowLib.UserControls
{
    partial class VisionShowTool
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VisionShowTool));
            toolStrip1 = new ToolStrip();
            lblTitleName = new ToolStripLabel();
            toolStripSeparator1 = new ToolStripSeparator();
            无操作toolStripButton = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            放大toolStripButton = new ToolStripButton();
            toolStripSeparator3 = new ToolStripSeparator();
            缩小toolStripButton = new ToolStripButton();
            toolStripSeparator4 = new ToolStripSeparator();
            平移toolStripButton = new ToolStripButton();
            toolStripSeparator5 = new ToolStripSeparator();
            自适应toolStripButton = new ToolStripButton();
            toolStripSeparator8 = new ToolStripSeparator();
            图像旋转toolStripButton = new ToolStripButton();
            toolStripSeparator10 = new ToolStripSeparator();
            图像采集toolStripButton = new ToolStripButton();
            toolStripSeparator11 = new ToolStripSeparator();
            运行toolStripButton = new ToolStripButton();
            toolStripSeparator12 = new ToolStripSeparator();
            停止toolStripButton = new ToolStripButton();
            toolStripSeparator13 = new ToolStripSeparator();
            toolStripButton1 = new ToolStripButton();
            toolTip1 = new ToolTip(components);
            contextMenuStrip1 = new ContextMenuStrip(components);
            无操作ToolStripMenuItem = new ToolStripMenuItem();
            平移ToolStripMenuItem = new ToolStripMenuItem();
            缩放ToolStripMenuItem = new ToolStripMenuItem();
            适应窗口ToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator6 = new ToolStripSeparator();
            加载图片ToolStripMenuItem = new ToolStripMenuItem();
            保存图片ToolStripMenuItem = new ToolStripMenuItem();
            保存窗体图片ToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator7 = new ToolStripSeparator();
            文本区域清除ToolStripMenuItem = new ToolStripMenuItem();
            显示中心十字坐标ToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator9 = new ToolStripSeparator();
            彩色显示ToolStripMenuItem = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            LocationLabel = new ToolStripStatusLabel();
            toolStripStatusLabel2 = new ToolStripStatusLabel();
            GrayLabel = new ToolStripStatusLabel();
            toolStripStatusLabel4 = new ToolStripStatusLabel();
            TimeLabel = new ToolStripStatusLabel();
            h_Disp = new HalconDotNet.HWindowControl();
            toolStrip1.SuspendLayout();
            contextMenuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.AutoSize = false;
            toolStrip1.BackColor = Color.FromArgb(255, 109, 60);
            toolStrip1.Font = new Font("Calibri", 10.8F, FontStyle.Regular, GraphicsUnit.Point);
            toolStrip1.ImageScalingSize = new Size(20, 20);
            toolStrip1.Items.AddRange(new ToolStripItem[] { lblTitleName, toolStripSeparator1, 无操作toolStripButton, toolStripSeparator2, 放大toolStripButton, toolStripSeparator3, 缩小toolStripButton, toolStripSeparator4, 平移toolStripButton, toolStripSeparator5, 自适应toolStripButton, toolStripSeparator8, 图像旋转toolStripButton, toolStripSeparator10, 图像采集toolStripButton, toolStripSeparator11, 运行toolStripButton, toolStripSeparator12, 停止toolStripButton, toolStripSeparator13, toolStripButton1 });
            toolStrip1.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            toolStrip1.Location = new Point(1, 1);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(598, 38);
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "toolStrip1";
            // 
            // lblTitleName
            // 
            lblTitleName.BackColor = Color.Transparent;
            lblTitleName.Font = new Font("Calibri", 10.8F, FontStyle.Regular, GraphicsUnit.Point);
            lblTitleName.ForeColor = Color.White;
            lblTitleName.Name = "lblTitleName";
            lblTitleName.Size = new Size(54, 35);
            lblTitleName.Text = "CAM1";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.BackColor = Color.FromArgb(255, 109, 60);
            toolStripSeparator1.ForeColor = Color.White;
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 38);
            // 
            // 无操作toolStripButton
            // 
            无操作toolStripButton.BackgroundImageLayout = ImageLayout.Stretch;
            无操作toolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            无操作toolStripButton.Image = (Image)resources.GetObject("无操作toolStripButton.Image");
            无操作toolStripButton.ImageScaling = ToolStripItemImageScaling.None;
            无操作toolStripButton.ImageTransparentColor = Color.Magenta;
            无操作toolStripButton.Name = "无操作toolStripButton";
            无操作toolStripButton.Size = new Size(36, 35);
            无操作toolStripButton.Text = "无选择";
            无操作toolStripButton.Click += 无操作toolStripButton_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 38);
            // 
            // 放大toolStripButton
            // 
            放大toolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            放大toolStripButton.Image = (Image)resources.GetObject("放大toolStripButton.Image");
            放大toolStripButton.ImageScaling = ToolStripItemImageScaling.None;
            放大toolStripButton.ImageTransparentColor = Color.Magenta;
            放大toolStripButton.Name = "放大toolStripButton";
            放大toolStripButton.Size = new Size(36, 35);
            放大toolStripButton.Text = "放大";
            放大toolStripButton.Click += 放大toolStripButton_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 38);
            // 
            // 缩小toolStripButton
            // 
            缩小toolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            缩小toolStripButton.Image = (Image)resources.GetObject("缩小toolStripButton.Image");
            缩小toolStripButton.ImageScaling = ToolStripItemImageScaling.None;
            缩小toolStripButton.ImageTransparentColor = Color.Magenta;
            缩小toolStripButton.Name = "缩小toolStripButton";
            缩小toolStripButton.Size = new Size(36, 35);
            缩小toolStripButton.Text = "缩小";
            缩小toolStripButton.Click += 缩小toolStripButton_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(6, 38);
            // 
            // 平移toolStripButton
            // 
            平移toolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            平移toolStripButton.Image = (Image)resources.GetObject("平移toolStripButton.Image");
            平移toolStripButton.ImageScaling = ToolStripItemImageScaling.None;
            平移toolStripButton.ImageTransparentColor = Color.Magenta;
            平移toolStripButton.Name = "平移toolStripButton";
            平移toolStripButton.Size = new Size(36, 35);
            平移toolStripButton.Text = "平移";
            平移toolStripButton.Click += 平移toolStripButton_Click;
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(6, 38);
            // 
            // 自适应toolStripButton
            // 
            自适应toolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            自适应toolStripButton.Image = (Image)resources.GetObject("自适应toolStripButton.Image");
            自适应toolStripButton.ImageScaling = ToolStripItemImageScaling.None;
            自适应toolStripButton.ImageTransparentColor = Color.Magenta;
            自适应toolStripButton.Name = "自适应toolStripButton";
            自适应toolStripButton.Size = new Size(36, 35);
            自适应toolStripButton.Text = "自适应";
            自适应toolStripButton.Click += 自适应toolStripButton_Click;
            // 
            // toolStripSeparator8
            // 
            toolStripSeparator8.Name = "toolStripSeparator8";
            toolStripSeparator8.Size = new Size(6, 38);
            // 
            // 图像旋转toolStripButton
            // 
            图像旋转toolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            图像旋转toolStripButton.Image = (Image)resources.GetObject("图像旋转toolStripButton.Image");
            图像旋转toolStripButton.ImageScaling = ToolStripItemImageScaling.None;
            图像旋转toolStripButton.ImageTransparentColor = Color.Magenta;
            图像旋转toolStripButton.Name = "图像旋转toolStripButton";
            图像旋转toolStripButton.Size = new Size(36, 35);
            图像旋转toolStripButton.Text = "图像旋转";
            图像旋转toolStripButton.Click += 图像旋转toolStripButton_Click;
            // 
            // toolStripSeparator10
            // 
            toolStripSeparator10.Name = "toolStripSeparator10";
            toolStripSeparator10.Size = new Size(6, 38);
            // 
            // 图像采集toolStripButton
            // 
            图像采集toolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            图像采集toolStripButton.Image = (Image)resources.GetObject("图像采集toolStripButton.Image");
            图像采集toolStripButton.ImageScaling = ToolStripItemImageScaling.None;
            图像采集toolStripButton.ImageTransparentColor = Color.Magenta;
            图像采集toolStripButton.Name = "图像采集toolStripButton";
            图像采集toolStripButton.Size = new Size(36, 35);
            图像采集toolStripButton.Text = "图像采集";
            图像采集toolStripButton.Click += 图像采集toolStripButton_Click;
            // 
            // toolStripSeparator11
            // 
            toolStripSeparator11.Name = "toolStripSeparator11";
            toolStripSeparator11.Size = new Size(6, 38);
            // 
            // 运行toolStripButton
            // 
            运行toolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            运行toolStripButton.Image = (Image)resources.GetObject("运行toolStripButton.Image");
            运行toolStripButton.ImageScaling = ToolStripItemImageScaling.None;
            运行toolStripButton.ImageTransparentColor = Color.Magenta;
            运行toolStripButton.Name = "运行toolStripButton";
            运行toolStripButton.Size = new Size(39, 35);
            运行toolStripButton.Text = "连续运行";
            运行toolStripButton.Click += 运行toolStripButton_Click;
            // 
            // toolStripSeparator12
            // 
            toolStripSeparator12.Name = "toolStripSeparator12";
            toolStripSeparator12.Size = new Size(6, 38);
            // 
            // 停止toolStripButton
            // 
            停止toolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            停止toolStripButton.Image = (Image)resources.GetObject("停止toolStripButton.Image");
            停止toolStripButton.ImageScaling = ToolStripItemImageScaling.None;
            停止toolStripButton.ImageTransparentColor = Color.Magenta;
            停止toolStripButton.Name = "停止toolStripButton";
            停止toolStripButton.Size = new Size(37, 35);
            停止toolStripButton.Text = "停止运行";
            停止toolStripButton.Click += 停止toolStripButton_Click;
            // 
            // toolStripSeparator13
            // 
            toolStripSeparator13.Name = "toolStripSeparator13";
            toolStripSeparator13.Size = new Size(6, 38);
            // 
            // toolStripButton1
            // 
            toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButton1.Image = (Image)resources.GetObject("toolStripButton1.Image");
            toolStripButton1.ImageScaling = ToolStripItemImageScaling.None;
            toolStripButton1.ImageTransparentColor = Color.Magenta;
            toolStripButton1.Name = "toolStripButton1";
            toolStripButton1.Size = new Size(39, 35);
            toolStripButton1.Text = "参数折叠";
            toolStripButton1.ToolTipText = "参数折叠";
            toolStripButton1.Click += toolStripButton1_Click;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            contextMenuStrip1.ImageScalingSize = new Size(24, 24);
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { 无操作ToolStripMenuItem, 平移ToolStripMenuItem, 缩放ToolStripMenuItem, 适应窗口ToolStripMenuItem, toolStripSeparator6, 加载图片ToolStripMenuItem, 保存图片ToolStripMenuItem, 保存窗体图片ToolStripMenuItem, toolStripSeparator7, 文本区域清除ToolStripMenuItem, 显示中心十字坐标ToolStripMenuItem, toolStripSeparator9, 彩色显示ToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(169, 262);
            // 
            // 无操作ToolStripMenuItem
            // 
            无操作ToolStripMenuItem.Name = "无操作ToolStripMenuItem";
            无操作ToolStripMenuItem.Size = new Size(168, 24);
            无操作ToolStripMenuItem.Text = "无操作";
            无操作ToolStripMenuItem.Click += 无操作ToolStripMenuItem_Click;
            // 
            // 平移ToolStripMenuItem
            // 
            平移ToolStripMenuItem.Name = "平移ToolStripMenuItem";
            平移ToolStripMenuItem.Size = new Size(168, 24);
            平移ToolStripMenuItem.Text = "平移";
            平移ToolStripMenuItem.Click += 平移ToolStripMenuItem_Click;
            // 
            // 缩放ToolStripMenuItem
            // 
            缩放ToolStripMenuItem.Name = "缩放ToolStripMenuItem";
            缩放ToolStripMenuItem.Size = new Size(168, 24);
            缩放ToolStripMenuItem.Text = "缩放";
            缩放ToolStripMenuItem.Click += 缩放ToolStripMenuItem_Click;
            // 
            // 适应窗口ToolStripMenuItem
            // 
            适应窗口ToolStripMenuItem.Name = "适应窗口ToolStripMenuItem";
            适应窗口ToolStripMenuItem.Size = new Size(168, 24);
            适应窗口ToolStripMenuItem.Text = "适应窗口";
            适应窗口ToolStripMenuItem.Click += 适应窗口ToolStripMenuItem_Click;
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new Size(165, 6);
            // 
            // 加载图片ToolStripMenuItem
            // 
            加载图片ToolStripMenuItem.Name = "加载图片ToolStripMenuItem";
            加载图片ToolStripMenuItem.Size = new Size(168, 24);
            加载图片ToolStripMenuItem.Text = "加载图片";
            加载图片ToolStripMenuItem.Click += 加载图片ToolStripMenuItem_Click;
            // 
            // 保存图片ToolStripMenuItem
            // 
            保存图片ToolStripMenuItem.Name = "保存图片ToolStripMenuItem";
            保存图片ToolStripMenuItem.Size = new Size(168, 24);
            保存图片ToolStripMenuItem.Text = "保存图片";
            保存图片ToolStripMenuItem.Click += 保存图片ToolStripMenuItem_Click;
            // 
            // 保存窗体图片ToolStripMenuItem
            // 
            保存窗体图片ToolStripMenuItem.Name = "保存窗体图片ToolStripMenuItem";
            保存窗体图片ToolStripMenuItem.Size = new Size(168, 24);
            保存窗体图片ToolStripMenuItem.Text = "保存窗体图片";
            保存窗体图片ToolStripMenuItem.Click += 保存窗体图片ToolStripMenuItem_Click;
            // 
            // toolStripSeparator7
            // 
            toolStripSeparator7.Name = "toolStripSeparator7";
            toolStripSeparator7.Size = new Size(165, 6);
            // 
            // 文本区域清除ToolStripMenuItem
            // 
            文本区域清除ToolStripMenuItem.Name = "文本区域清除ToolStripMenuItem";
            文本区域清除ToolStripMenuItem.Size = new Size(168, 24);
            文本区域清除ToolStripMenuItem.Text = "文本区域清除";
            文本区域清除ToolStripMenuItem.Click += 文本区域清除ToolStripMenuItem_Click;
            // 
            // 显示中心十字坐标ToolStripMenuItem
            // 
            显示中心十字坐标ToolStripMenuItem.Name = "显示中心十字坐标ToolStripMenuItem";
            显示中心十字坐标ToolStripMenuItem.Size = new Size(168, 24);
            显示中心十字坐标ToolStripMenuItem.Text = "显示十字中心";
            显示中心十字坐标ToolStripMenuItem.Click += 显示中心十字坐标ToolStripMenuItem_Click;
            // 
            // toolStripSeparator9
            // 
            toolStripSeparator9.Name = "toolStripSeparator9";
            toolStripSeparator9.Size = new Size(165, 6);
            // 
            // 彩色显示ToolStripMenuItem
            // 
            彩色显示ToolStripMenuItem.Name = "彩色显示ToolStripMenuItem";
            彩色显示ToolStripMenuItem.Size = new Size(168, 24);
            彩色显示ToolStripMenuItem.Text = "彩色显示";
            彩色显示ToolStripMenuItem.Click += 彩色显示ToolStripMenuItem_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.AutoSize = false;
            statusStrip1.BackColor = Color.FromArgb(255, 109, 60);
            statusStrip1.Font = new Font("Calibri", 10.8F, FontStyle.Regular, GraphicsUnit.Point);
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { LocationLabel, toolStripStatusLabel2, GrayLabel, toolStripStatusLabel4, TimeLabel });
            statusStrip1.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            statusStrip1.Location = new Point(1, 411);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 16, 0);
            statusStrip1.Size = new Size(598, 38);
            statusStrip1.Stretch = false;
            statusStrip1.TabIndex = 3;
            statusStrip1.Text = "statusStrip1";
            // 
            // LocationLabel
            // 
            LocationLabel.ForeColor = Color.White;
            LocationLabel.Image = (Image)resources.GetObject("LocationLabel.Image");
            LocationLabel.ImageScaling = ToolStripItemImageScaling.None;
            LocationLabel.Name = "LocationLabel";
            LocationLabel.Size = new Size(64, 32);
            LocationLabel.Text = "0,0";
            // 
            // toolStripStatusLabel2
            // 
            toolStripStatusLabel2.ForeColor = Color.White;
            toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            toolStripStatusLabel2.Size = new Size(18, 32);
            toolStripStatusLabel2.Text = "|";
            // 
            // GrayLabel
            // 
            GrayLabel.ForeColor = Color.White;
            GrayLabel.Image = (Image)resources.GetObject("GrayLabel.Image");
            GrayLabel.ImageScaling = ToolStripItemImageScaling.None;
            GrayLabel.Name = "GrayLabel";
            GrayLabel.Size = new Size(51, 32);
            GrayLabel.Text = "0";
            // 
            // toolStripStatusLabel4
            // 
            toolStripStatusLabel4.ForeColor = Color.White;
            toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            toolStripStatusLabel4.Size = new Size(18, 32);
            toolStripStatusLabel4.Text = "|";
            // 
            // TimeLabel
            // 
            TimeLabel.ForeColor = Color.White;
            TimeLabel.Image = (Image)resources.GetObject("TimeLabel.Image");
            TimeLabel.ImageScaling = ToolStripItemImageScaling.None;
            TimeLabel.Name = "TimeLabel";
            TimeLabel.Size = new Size(72, 32);
            TimeLabel.Text = "0ms";
            // 
            // h_Disp
            // 
            h_Disp.BackColor = Color.FromArgb(255, 109, 60);
            h_Disp.BorderColor = Color.FromArgb(255, 109, 60);
            h_Disp.Dock = DockStyle.Fill;
            h_Disp.Font = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            h_Disp.ImagePart = new Rectangle(0, 0, 640, 480);
            h_Disp.Location = new Point(1, 39);
            h_Disp.Margin = new Padding(2, 3, 2, 3);
            h_Disp.Name = "h_Disp";
            h_Disp.Padding = new Padding(2, 3, 2, 3);
            h_Disp.Size = new Size(598, 372);
            h_Disp.TabIndex = 5;
            h_Disp.WindowSize = new Size(598, 372);
            h_Disp.SizeChanged += h_Disp_SizeChanged;
            h_Disp.MouseDown += h_Disp_MouseDown;
            h_Disp.MouseEnter += h_Disp_MouseEnter;
            h_Disp.MouseMove += h_Disp_MouseMove;
            h_Disp.MouseUp += h_Disp_MouseUp;
            // 
            // VisionShowTool
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Transparent;
            Controls.Add(h_Disp);
            Controls.Add(statusStrip1);
            Controls.Add(toolStrip1);
            ForeColor = Color.Silver;
            Margin = new Padding(2, 3, 2, 3);
            Name = "VisionShowTool";
            Padding = new Padding(1);
            Size = new Size(600, 450);
            Load += VisionShowTool_Load;
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            contextMenuStrip1.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ToolStrip toolStrip1;
        private ToolStripLabel lblTitleName;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton 无操作toolStripButton;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton 放大toolStripButton;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton 缩小toolStripButton;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripButton 平移toolStripButton;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripButton 自适应toolStripButton;
        private ToolTip toolTip1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem 无操作ToolStripMenuItem;
        private ToolStripMenuItem 平移ToolStripMenuItem;
        private ToolStripMenuItem 缩放ToolStripMenuItem;
        private ToolStripMenuItem 适应窗口ToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripMenuItem 加载图片ToolStripMenuItem;
        private ToolStripMenuItem 保存图片ToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripMenuItem 文本区域清除ToolStripMenuItem;
        private ToolStripMenuItem 显示中心十字坐标ToolStripMenuItem;
        private StatusStrip statusStrip1;
        private HalconDotNet.HWindowControl h_Disp;
        private ToolStripStatusLabel LocationLabel;
        private ToolStripStatusLabel toolStripStatusLabel2;
        private ToolStripStatusLabel GrayLabel;
        private ToolStripStatusLabel toolStripStatusLabel4;
        private ToolStripStatusLabel TimeLabel;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripButton 图像旋转toolStripButton;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripMenuItem 彩色显示ToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator10;
        private ToolStripButton 图像采集toolStripButton;
        private ToolStripMenuItem 保存窗体图片ToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator11;
        private ToolStripButton 运行toolStripButton;
        private ToolStripSeparator toolStripSeparator12;
        private ToolStripButton 停止toolStripButton;
        private ToolStripSeparator toolStripSeparator13;
        private ToolStripButton toolStripButton1;
    }
}
