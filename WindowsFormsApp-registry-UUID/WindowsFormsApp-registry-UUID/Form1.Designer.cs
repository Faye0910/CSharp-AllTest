﻿namespace WindowsFormsApp_registry_UUID
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.registrybtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // registrybtn
            // 
            this.registrybtn.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.registrybtn.Location = new System.Drawing.Point(22, 27);
            this.registrybtn.Name = "registrybtn";
            this.registrybtn.Size = new System.Drawing.Size(150, 58);
            this.registrybtn.TabIndex = 0;
            this.registrybtn.Text = "Registry";
            this.registrybtn.UseVisualStyleBackColor = true;
            this.registrybtn.Click += new System.EventHandler(this.registrybtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(195, 107);
            this.Controls.Add(this.registrybtn);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button registrybtn;
    }
}

