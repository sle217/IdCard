
namespace WindowsFormsApp2
{
    partial class Form1
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
            this.btnImageUrl = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lstBoxDirectory = new System.Windows.Forms.ListBox();
            this.lblIdCard = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnImageUrl
            // 
            this.btnImageUrl.Location = new System.Drawing.Point(240, 489);
            this.btnImageUrl.Name = "btnImageUrl";
            this.btnImageUrl.Size = new System.Drawing.Size(75, 23);
            this.btnImageUrl.TabIndex = 0;
            this.btnImageUrl.Text = "选择图片";
            this.btnImageUrl.UseVisualStyleBackColor = true;
            this.btnImageUrl.Click += new System.EventHandler(this.btnImageUrl_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(413, 490);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "开始识别";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(72, 490);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(162, 21);
            this.dateTimePicker1.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 494);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "过期时间";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(4, 530);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(494, 23);
            this.progressBar1.TabIndex = 7;
            // 
            // lstBoxDirectory
            // 
            this.lstBoxDirectory.FormattingEnabled = true;
            this.lstBoxDirectory.ItemHeight = 12;
            this.lstBoxDirectory.Location = new System.Drawing.Point(12, 36);
            this.lstBoxDirectory.Name = "lstBoxDirectory";
            this.lstBoxDirectory.Size = new System.Drawing.Size(486, 448);
            this.lstBoxDirectory.TabIndex = 8;
            // 
            // lblIdCard
            // 
            this.lblIdCard.AutoSize = true;
            this.lblIdCard.Location = new System.Drawing.Point(13, 12);
            this.lblIdCard.Name = "lblIdCard";
            this.lblIdCard.Size = new System.Drawing.Size(65, 12);
            this.lblIdCard.TabIndex = 9;
            this.lblIdCard.Text = "身份证信息";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(323, 489);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "保存目录";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(508, 553);
            this.Controls.Add(this.lblIdCard);
            this.Controls.Add(this.lstBoxDirectory);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnImageUrl);
            this.Name = "Form1";
            this.Text = "身份证识别系统";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnImageUrl;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ListBox lstBoxDirectory;
        private System.Windows.Forms.Label lblIdCard;
        private System.Windows.Forms.Button btnSave;
    }
}

