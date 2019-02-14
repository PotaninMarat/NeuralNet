namespace TestConvLayer
{
    partial class Form1
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
            this.zedGraphControl1 = new ZedGraph.ZedGraphControl();
            this.buttonSin = new System.Windows.Forms.Button();
            this.zedGraphControl2 = new ZedGraph.ZedGraphControl();
            this.buttonRect = new System.Windows.Forms.Button();
            this.buttonNoise = new System.Windows.Forms.Button();
            this.buttonTrain = new System.Windows.Forms.Button();
            this.buttonTest = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // zedGraphControl1
            // 
            this.zedGraphControl1.Location = new System.Drawing.Point(13, 13);
            this.zedGraphControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.zedGraphControl1.Name = "zedGraphControl1";
            this.zedGraphControl1.ScrollGrace = 0D;
            this.zedGraphControl1.ScrollMaxX = 0D;
            this.zedGraphControl1.ScrollMaxY = 0D;
            this.zedGraphControl1.ScrollMaxY2 = 0D;
            this.zedGraphControl1.ScrollMinX = 0D;
            this.zedGraphControl1.ScrollMinY = 0D;
            this.zedGraphControl1.ScrollMinY2 = 0D;
            this.zedGraphControl1.Size = new System.Drawing.Size(376, 227);
            this.zedGraphControl1.TabIndex = 0;
            // 
            // buttonSin
            // 
            this.buttonSin.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonSin.Location = new System.Drawing.Point(13, 257);
            this.buttonSin.Name = "buttonSin";
            this.buttonSin.Size = new System.Drawing.Size(81, 34);
            this.buttonSin.TabIndex = 1;
            this.buttonSin.Text = "sin";
            this.buttonSin.UseVisualStyleBackColor = true;
            this.buttonSin.Click += new System.EventHandler(this.buttonSin_Click);
            // 
            // zedGraphControl2
            // 
            this.zedGraphControl2.Location = new System.Drawing.Point(397, 13);
            this.zedGraphControl2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.zedGraphControl2.Name = "zedGraphControl2";
            this.zedGraphControl2.ScrollGrace = 0D;
            this.zedGraphControl2.ScrollMaxX = 0D;
            this.zedGraphControl2.ScrollMaxY = 0D;
            this.zedGraphControl2.ScrollMaxY2 = 0D;
            this.zedGraphControl2.ScrollMinX = 0D;
            this.zedGraphControl2.ScrollMinY = 0D;
            this.zedGraphControl2.ScrollMinY2 = 0D;
            this.zedGraphControl2.Size = new System.Drawing.Size(342, 346);
            this.zedGraphControl2.TabIndex = 2;
            // 
            // buttonRect
            // 
            this.buttonRect.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonRect.Location = new System.Drawing.Point(110, 257);
            this.buttonRect.Name = "buttonRect";
            this.buttonRect.Size = new System.Drawing.Size(81, 34);
            this.buttonRect.TabIndex = 3;
            this.buttonRect.Text = "rect";
            this.buttonRect.UseVisualStyleBackColor = true;
            this.buttonRect.Click += new System.EventHandler(this.buttonRect_Click);
            // 
            // buttonNoise
            // 
            this.buttonNoise.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonNoise.Location = new System.Drawing.Point(208, 257);
            this.buttonNoise.Name = "buttonNoise";
            this.buttonNoise.Size = new System.Drawing.Size(81, 34);
            this.buttonNoise.TabIndex = 4;
            this.buttonNoise.Text = "noise";
            this.buttonNoise.UseVisualStyleBackColor = true;
            this.buttonNoise.Click += new System.EventHandler(this.buttonNoise_Click);
            // 
            // buttonTrain
            // 
            this.buttonTrain.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonTrain.Location = new System.Drawing.Point(308, 325);
            this.buttonTrain.Name = "buttonTrain";
            this.buttonTrain.Size = new System.Drawing.Size(81, 34);
            this.buttonTrain.TabIndex = 5;
            this.buttonTrain.Text = "train";
            this.buttonTrain.UseVisualStyleBackColor = true;
            this.buttonTrain.Click += new System.EventHandler(this.buttonTrain_Click);
            // 
            // buttonTest
            // 
            this.buttonTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonTest.Location = new System.Drawing.Point(208, 325);
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.Size = new System.Drawing.Size(81, 34);
            this.buttonTest.TabIndex = 6;
            this.buttonTest.Text = "test";
            this.buttonTest.UseVisualStyleBackColor = true;
            this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(753, 372);
            this.Controls.Add(this.buttonTest);
            this.Controls.Add(this.buttonTrain);
            this.Controls.Add(this.buttonNoise);
            this.Controls.Add(this.buttonRect);
            this.Controls.Add(this.zedGraphControl2);
            this.Controls.Add(this.buttonSin);
            this.Controls.Add(this.zedGraphControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private ZedGraph.ZedGraphControl zedGraphControl1;
        private System.Windows.Forms.Button buttonSin;
        private ZedGraph.ZedGraphControl zedGraphControl2;
        private System.Windows.Forms.Button buttonRect;
        private System.Windows.Forms.Button buttonNoise;
        private System.Windows.Forms.Button buttonTrain;
        private System.Windows.Forms.Button buttonTest;
    }
}

