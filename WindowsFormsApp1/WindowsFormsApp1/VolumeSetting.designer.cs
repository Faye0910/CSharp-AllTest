namespace WindowsFormsApp1
{
    partial class Volume
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Volume40 = new System.Windows.Forms.Button();
            this.CloseFormbtn = new System.Windows.Forms.Button();
            this.Volume60 = new System.Windows.Forms.Button();
            this.Volume80 = new System.Windows.Forms.Button();
            this.Volume100 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // Volume40
            // 
            this.Volume40.Font = new System.Drawing.Font("微軟正黑體", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Volume40.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Volume40.Location = new System.Drawing.Point(52, 183);
            this.Volume40.Margin = new System.Windows.Forms.Padding(12, 13, 12, 13);
            this.Volume40.Name = "Volume40";
            this.Volume40.Size = new System.Drawing.Size(200, 100);
            this.Volume40.TabIndex = 10;
            this.Volume40.Text = "40";
            this.Volume40.UseVisualStyleBackColor = true;
            this.Volume40.Click += new System.EventHandler(this.Volume40_Click);
            // 
            // CloseFormbtn
            // 
            this.CloseFormbtn.Font = new System.Drawing.Font("微軟正黑體", 27F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.CloseFormbtn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.CloseFormbtn.Location = new System.Drawing.Point(724, 425);
            this.CloseFormbtn.Margin = new System.Windows.Forms.Padding(12, 13, 12, 13);
            this.CloseFormbtn.Name = "CloseFormbtn";
            this.CloseFormbtn.Size = new System.Drawing.Size(250, 70);
            this.CloseFormbtn.TabIndex = 11;
            this.CloseFormbtn.Text = "Close";
            this.CloseFormbtn.UseVisualStyleBackColor = true;
            this.CloseFormbtn.Click += new System.EventHandler(this.CloseFormbtn_Click);
            // 
            // Volume60
            // 
            this.Volume60.Font = new System.Drawing.Font("微軟正黑體", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Volume60.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Volume60.Location = new System.Drawing.Point(276, 183);
            this.Volume60.Margin = new System.Windows.Forms.Padding(12, 13, 12, 13);
            this.Volume60.Name = "Volume60";
            this.Volume60.Size = new System.Drawing.Size(200, 100);
            this.Volume60.TabIndex = 10;
            this.Volume60.Text = "60";
            this.Volume60.UseVisualStyleBackColor = true;
            this.Volume60.Click += new System.EventHandler(this.Volume60_Click);
            // 
            // Volume80
            // 
            this.Volume80.Font = new System.Drawing.Font("微軟正黑體", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Volume80.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Volume80.Location = new System.Drawing.Point(500, 183);
            this.Volume80.Margin = new System.Windows.Forms.Padding(12, 13, 12, 13);
            this.Volume80.Name = "Volume80";
            this.Volume80.Size = new System.Drawing.Size(200, 100);
            this.Volume80.TabIndex = 10;
            this.Volume80.Text = "80";
            this.Volume80.UseVisualStyleBackColor = true;
            this.Volume80.Click += new System.EventHandler(this.Volume80_Click);
            // 
            // Volume100
            // 
            this.Volume100.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Volume100.Font = new System.Drawing.Font("微軟正黑體", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Volume100.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Volume100.Location = new System.Drawing.Point(724, 183);
            this.Volume100.Margin = new System.Windows.Forms.Padding(12, 13, 12, 13);
            this.Volume100.Name = "Volume100";
            this.Volume100.Size = new System.Drawing.Size(200, 100);
            this.Volume100.TabIndex = 10;
            this.Volume100.Text = "100";
            this.Volume100.UseVisualStyleBackColor = false;
            this.Volume100.Click += new System.EventHandler(this.Volume100_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(110)))), ((int)(((byte)(184)))));
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1021, 95);
            this.pictureBox1.TabIndex = 12;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(110)))), ((int)(((byte)(184)))));
            this.pictureBox2.Location = new System.Drawing.Point(-14, 412);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(1021, 95);
            this.pictureBox2.TabIndex = 13;
            this.pictureBox2.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Volume
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(985, 506);
            this.Controls.Add(this.CloseFormbtn);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.Volume100);
            this.Controls.Add(this.Volume80);
            this.Controls.Add(this.Volume60);
            this.Controls.Add(this.Volume40);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Volume";
            this.Text = "Volume";
            this.Load += new System.EventHandler(this.Volume_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Button Volume40;
        public System.Windows.Forms.Button CloseFormbtn;
        public System.Windows.Forms.Button Volume60;
        public System.Windows.Forms.Button Volume80;
        public System.Windows.Forms.Button Volume100;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Timer timer1;
    }
}