
namespace RomanPort.LibEmbeddedSDR.Tools.RemoteDebugger
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
            this.btnControlC = new System.Windows.Forms.Button();
            this.btnControlB = new System.Windows.Forms.Button();
            this.btnControlA = new System.Windows.Forms.Button();
            this.btnControlUp = new System.Windows.Forms.Button();
            this.btnControlRight = new System.Windows.Forms.Button();
            this.btnControlLeft = new System.Windows.Forms.Button();
            this.btnControlDown = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnStopAudio = new System.Windows.Forms.Button();
            this.btnStartAudio = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.serverAddress = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnBeginScreenStreaming = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnControlC
            // 
            this.btnControlC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnControlC.Location = new System.Drawing.Point(182, 17);
            this.btnControlC.Name = "btnControlC";
            this.btnControlC.Size = new System.Drawing.Size(30, 73);
            this.btnControlC.TabIndex = 15;
            this.btnControlC.Tag = "C";
            this.btnControlC.Text = "C";
            this.btnControlC.UseVisualStyleBackColor = true;
            this.btnControlC.Click += new System.EventHandler(this.buttonControl_Click);
            // 
            // btnControlB
            // 
            this.btnControlB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnControlB.Location = new System.Drawing.Point(218, 17);
            this.btnControlB.Name = "btnControlB";
            this.btnControlB.Size = new System.Drawing.Size(30, 73);
            this.btnControlB.TabIndex = 14;
            this.btnControlB.Tag = "B";
            this.btnControlB.Text = "B";
            this.btnControlB.UseVisualStyleBackColor = true;
            this.btnControlB.Click += new System.EventHandler(this.buttonControl_Click);
            // 
            // btnControlA
            // 
            this.btnControlA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnControlA.Location = new System.Drawing.Point(254, 17);
            this.btnControlA.Name = "btnControlA";
            this.btnControlA.Size = new System.Drawing.Size(30, 73);
            this.btnControlA.TabIndex = 13;
            this.btnControlA.Tag = "A";
            this.btnControlA.Text = "A";
            this.btnControlA.UseVisualStyleBackColor = true;
            this.btnControlA.Click += new System.EventHandler(this.buttonControl_Click);
            // 
            // btnControlUp
            // 
            this.btnControlUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnControlUp.Location = new System.Drawing.Point(45, 17);
            this.btnControlUp.Name = "btnControlUp";
            this.btnControlUp.Size = new System.Drawing.Size(75, 23);
            this.btnControlUp.TabIndex = 12;
            this.btnControlUp.Tag = "UP";
            this.btnControlUp.Text = "/\\";
            this.btnControlUp.UseVisualStyleBackColor = true;
            this.btnControlUp.Click += new System.EventHandler(this.buttonControl_Click);
            // 
            // btnControlRight
            // 
            this.btnControlRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnControlRight.Location = new System.Drawing.Point(83, 42);
            this.btnControlRight.Name = "btnControlRight";
            this.btnControlRight.Size = new System.Drawing.Size(75, 23);
            this.btnControlRight.TabIndex = 11;
            this.btnControlRight.Tag = "RIGHT";
            this.btnControlRight.Text = "->";
            this.btnControlRight.UseVisualStyleBackColor = true;
            this.btnControlRight.Click += new System.EventHandler(this.buttonControl_Click);
            // 
            // btnControlLeft
            // 
            this.btnControlLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnControlLeft.Location = new System.Drawing.Point(6, 42);
            this.btnControlLeft.Name = "btnControlLeft";
            this.btnControlLeft.Size = new System.Drawing.Size(75, 23);
            this.btnControlLeft.TabIndex = 10;
            this.btnControlLeft.Tag = "LEFT";
            this.btnControlLeft.Text = "<-";
            this.btnControlLeft.UseVisualStyleBackColor = true;
            this.btnControlLeft.Click += new System.EventHandler(this.buttonControl_Click);
            // 
            // btnControlDown
            // 
            this.btnControlDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnControlDown.Location = new System.Drawing.Point(45, 67);
            this.btnControlDown.Name = "btnControlDown";
            this.btnControlDown.Size = new System.Drawing.Size(75, 23);
            this.btnControlDown.TabIndex = 9;
            this.btnControlDown.Tag = "DOWN";
            this.btnControlDown.Text = "\\/";
            this.btnControlDown.UseVisualStyleBackColor = true;
            this.btnControlDown.Click += new System.EventHandler(this.buttonControl_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnControlA);
            this.groupBox1.Controls.Add(this.btnControlUp);
            this.groupBox1.Controls.Add(this.btnControlC);
            this.groupBox1.Controls.Add(this.btnControlRight);
            this.groupBox1.Controls.Add(this.btnControlB);
            this.groupBox1.Controls.Add(this.btnControlLeft);
            this.groupBox1.Controls.Add(this.btnControlDown);
            this.groupBox1.Location = new System.Drawing.Point(12, 268);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(290, 98);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Remote Control";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btnStopAudio);
            this.groupBox2.Controls.Add(this.btnStartAudio);
            this.groupBox2.Location = new System.Drawing.Point(12, 211);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(290, 51);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Audio";
            // 
            // btnStopAudio
            // 
            this.btnStopAudio.Location = new System.Drawing.Point(111, 19);
            this.btnStopAudio.Name = "btnStopAudio";
            this.btnStopAudio.Size = new System.Drawing.Size(99, 23);
            this.btnStopAudio.TabIndex = 1;
            this.btnStopAudio.Text = "Stop Audio";
            this.btnStopAudio.UseVisualStyleBackColor = true;
            this.btnStopAudio.Click += new System.EventHandler(this.btnStopAudio_Click);
            // 
            // btnStartAudio
            // 
            this.btnStartAudio.Location = new System.Drawing.Point(6, 19);
            this.btnStartAudio.Name = "btnStartAudio";
            this.btnStartAudio.Size = new System.Drawing.Size(99, 23);
            this.btnStartAudio.TabIndex = 0;
            this.btnStartAudio.Text = "Start Audio";
            this.btnStartAudio.UseVisualStyleBackColor = true;
            this.btnStartAudio.Click += new System.EventHandler(this.btnStartAudio_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 20);
            this.label1.TabIndex = 18;
            this.label1.Text = "Server Address";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // serverAddress
            // 
            this.serverAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.serverAddress.Location = new System.Drawing.Point(95, 9);
            this.serverAddress.Name = "serverAddress";
            this.serverAddress.Size = new System.Drawing.Size(207, 20);
            this.serverAddress.TabIndex = 19;
            this.serverAddress.Text = "10.0.1.55";
            this.serverAddress.TextChanged += new System.EventHandler(this.serverAddress_TextChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.btnBeginScreenStreaming);
            this.groupBox3.Location = new System.Drawing.Point(12, 154);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(290, 51);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Display";
            // 
            // btnBeginScreenStreaming
            // 
            this.btnBeginScreenStreaming.Location = new System.Drawing.Point(6, 19);
            this.btnBeginScreenStreaming.Name = "btnBeginScreenStreaming";
            this.btnBeginScreenStreaming.Size = new System.Drawing.Size(204, 23);
            this.btnBeginScreenStreaming.TabIndex = 0;
            this.btnBeginScreenStreaming.Text = "Begin Streaming";
            this.btnBeginScreenStreaming.UseVisualStyleBackColor = true;
            this.btnBeginScreenStreaming.Click += new System.EventHandler(this.btnBeginScreenStreaming_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 378);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.serverAddress);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnControlC;
        private System.Windows.Forms.Button btnControlB;
        private System.Windows.Forms.Button btnControlA;
        private System.Windows.Forms.Button btnControlUp;
        private System.Windows.Forms.Button btnControlRight;
        private System.Windows.Forms.Button btnControlLeft;
        private System.Windows.Forms.Button btnControlDown;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnStopAudio;
        private System.Windows.Forms.Button btnStartAudio;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox serverAddress;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnBeginScreenStreaming;
    }
}

