namespace BrokerageGather
{
    partial class FormMain
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnGetCityBlocks = new System.Windows.Forms.Button();
            this.txtCityName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCoordinateOffset = new System.Windows.Forms.TextBox();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.picMap = new System.Windows.Forms.PictureBox();
            this.btnSerialize = new System.Windows.Forms.Button();
            this.btnDserialize = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtQueryWord = new System.Windows.Forms.TextBox();
            this.btnQueryPOI = new System.Windows.Forms.Button();
            this.btnSaveResult = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picMap)).BeginInit();
            this.SuspendLayout();
            // 
            // btnGetCityBlocks
            // 
            this.btnGetCityBlocks.Location = new System.Drawing.Point(244, 34);
            this.btnGetCityBlocks.Name = "btnGetCityBlocks";
            this.btnGetCityBlocks.Size = new System.Drawing.Size(138, 23);
            this.btnGetCityBlocks.TabIndex = 0;
            this.btnGetCityBlocks.Text = "获取城市分块";
            this.btnGetCityBlocks.UseVisualStyleBackColor = true;
            this.btnGetCityBlocks.Click += new System.EventHandler(this.btnGetCityBlocks_Click);
            // 
            // txtCityName
            // 
            this.txtCityName.Location = new System.Drawing.Point(110, 36);
            this.txtCityName.Name = "txtCityName";
            this.txtCityName.Size = new System.Drawing.Size(128, 21);
            this.txtCityName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(50, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "城市名:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "坐标偏移量:";
            // 
            // txtCoordinateOffset
            // 
            this.txtCoordinateOffset.Location = new System.Drawing.Point(110, 63);
            this.txtCoordinateOffset.Name = "txtCoordinateOffset";
            this.txtCoordinateOffset.Size = new System.Drawing.Size(128, 21);
            this.txtCoordinateOffset.TabIndex = 4;
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(12, 121);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMessage.Size = new System.Drawing.Size(533, 165);
            this.txtMessage.TabIndex = 5;
            // 
            // picMap
            // 
            this.picMap.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.picMap.Location = new System.Drawing.Point(13, 293);
            this.picMap.Margin = new System.Windows.Forms.Padding(0);
            this.picMap.Name = "picMap";
            this.picMap.Size = new System.Drawing.Size(532, 400);
            this.picMap.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picMap.TabIndex = 6;
            this.picMap.TabStop = false;
            // 
            // btnSerialize
            // 
            this.btnSerialize.Location = new System.Drawing.Point(388, 34);
            this.btnSerialize.Name = "btnSerialize";
            this.btnSerialize.Size = new System.Drawing.Size(152, 23);
            this.btnSerialize.TabIndex = 7;
            this.btnSerialize.Text = "序列化块数据";
            this.btnSerialize.UseVisualStyleBackColor = true;
            this.btnSerialize.Click += new System.EventHandler(this.btnSerialize_Click);
            // 
            // btnDserialize
            // 
            this.btnDserialize.Location = new System.Drawing.Point(13, 7);
            this.btnDserialize.Name = "btnDserialize";
            this.btnDserialize.Size = new System.Drawing.Size(527, 23);
            this.btnDserialize.TabIndex = 8;
            this.btnDserialize.Text = "反序列化块数据";
            this.btnDserialize.UseVisualStyleBackColor = true;
            this.btnDserialize.Click += new System.EventHandler(this.btnDserialize_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "搜索关键字:";
            // 
            // txtQueryWord
            // 
            this.txtQueryWord.Location = new System.Drawing.Point(110, 94);
            this.txtQueryWord.Name = "txtQueryWord";
            this.txtQueryWord.Size = new System.Drawing.Size(230, 21);
            this.txtQueryWord.TabIndex = 10;
            // 
            // btnQueryPOI
            // 
            this.btnQueryPOI.Location = new System.Drawing.Point(346, 92);
            this.btnQueryPOI.Name = "btnQueryPOI";
            this.btnQueryPOI.Size = new System.Drawing.Size(96, 23);
            this.btnQueryPOI.TabIndex = 11;
            this.btnQueryPOI.Text = "开始爬POI数据";
            this.btnQueryPOI.UseVisualStyleBackColor = true;
            this.btnQueryPOI.Click += new System.EventHandler(this.btnQueryPOI_Click);
            // 
            // btnSaveResult
            // 
            this.btnSaveResult.Location = new System.Drawing.Point(448, 92);
            this.btnSaveResult.Name = "btnSaveResult";
            this.btnSaveResult.Size = new System.Drawing.Size(92, 23);
            this.btnSaveResult.TabIndex = 12;
            this.btnSaveResult.Text = "保存数据";
            this.btnSaveResult.UseVisualStyleBackColor = true;
            this.btnSaveResult.Click += new System.EventHandler(this.btnSaveResult_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(552, 706);
            this.Controls.Add(this.btnSaveResult);
            this.Controls.Add(this.btnQueryPOI);
            this.Controls.Add(this.txtQueryWord);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnDserialize);
            this.Controls.Add(this.btnSerialize);
            this.Controls.Add(this.picMap);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.txtCoordinateOffset);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtCityName);
            this.Controls.Add(this.btnGetCityBlocks);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.ShowIcon = false;
            this.Text = "采集POI数据";
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picMap)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGetCityBlocks;
        private System.Windows.Forms.TextBox txtCityName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtCoordinateOffset;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.PictureBox picMap;
        private System.Windows.Forms.Button btnSerialize;
        private System.Windows.Forms.Button btnDserialize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtQueryWord;
        private System.Windows.Forms.Button btnQueryPOI;
        private System.Windows.Forms.Button btnSaveResult;
    }
}

