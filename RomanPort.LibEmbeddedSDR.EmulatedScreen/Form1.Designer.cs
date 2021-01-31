using RomanPort.LibEmbeddedSDR.Framework.Input;

namespace RomanPort.LibEmbeddedSDR.EmulatedScreen
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
            this.btnForceRedraw = new System.Windows.Forms.Button();
            this.btnControlDown = new System.Windows.Forms.Button();
            this.btnControlLeft = new System.Windows.Forms.Button();
            this.btnControlRight = new System.Windows.Forms.Button();
            this.btnControlUp = new System.Windows.Forms.Button();
            this.btnControlA = new System.Windows.Forms.Button();
            this.btnControlB = new System.Windows.Forms.Button();
            this.btnControlC = new System.Windows.Forms.Button();
            this.emulatedDisplay1 = new RomanPort.LibEmbeddedSDR.EmulatedScreen.EmulatedDisplay();
            this.btnScreencap = new System.Windows.Forms.Button();
            this.volumeBar = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.volumeBar)).BeginInit();
            this.SuspendLayout();
            // 
            // btnForceRedraw
            // 
            this.btnForceRedraw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnForceRedraw.Location = new System.Drawing.Point(266, 826);
            this.btnForceRedraw.Name = "btnForceRedraw";
            this.btnForceRedraw.Size = new System.Drawing.Size(118, 23);
            this.btnForceRedraw.TabIndex = 1;
            this.btnForceRedraw.Text = "Force Redraw";
            this.btnForceRedraw.UseVisualStyleBackColor = true;
            this.btnForceRedraw.Click += new System.EventHandler(this.btnForceRedraw_Click);
            // 
            // btnControlDown
            // 
            this.btnControlDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnControlDown.Location = new System.Drawing.Point(51, 876);
            this.btnControlDown.Name = "btnControlDown";
            this.btnControlDown.Size = new System.Drawing.Size(75, 23);
            this.btnControlDown.TabIndex = 2;
            this.btnControlDown.Tag = "DOWN";
            this.btnControlDown.Text = "\\/";
            this.btnControlDown.UseVisualStyleBackColor = true;
            this.btnControlDown.Click += new System.EventHandler(this.OnInputControlBtnClicked);
            // 
            // btnControlLeft
            // 
            this.btnControlLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnControlLeft.Location = new System.Drawing.Point(12, 851);
            this.btnControlLeft.Name = "btnControlLeft";
            this.btnControlLeft.Size = new System.Drawing.Size(75, 23);
            this.btnControlLeft.TabIndex = 3;
            this.btnControlLeft.Tag = "LEFT";
            this.btnControlLeft.Text = "<-";
            this.btnControlLeft.UseVisualStyleBackColor = true;
            this.btnControlLeft.Click += new System.EventHandler(this.OnInputControlBtnClicked);
            // 
            // btnControlRight
            // 
            this.btnControlRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnControlRight.Location = new System.Drawing.Point(89, 851);
            this.btnControlRight.Name = "btnControlRight";
            this.btnControlRight.Size = new System.Drawing.Size(75, 23);
            this.btnControlRight.TabIndex = 4;
            this.btnControlRight.Tag = "RIGHT";
            this.btnControlRight.Text = "->";
            this.btnControlRight.UseVisualStyleBackColor = true;
            this.btnControlRight.Click += new System.EventHandler(this.OnInputControlBtnClicked);
            // 
            // btnControlUp
            // 
            this.btnControlUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnControlUp.Location = new System.Drawing.Point(51, 826);
            this.btnControlUp.Name = "btnControlUp";
            this.btnControlUp.Size = new System.Drawing.Size(75, 23);
            this.btnControlUp.TabIndex = 5;
            this.btnControlUp.Tag = "UP";
            this.btnControlUp.Text = "/\\";
            this.btnControlUp.UseVisualStyleBackColor = true;
            this.btnControlUp.Click += new System.EventHandler(this.OnInputControlBtnClicked);
            // 
            // btnControlA
            // 
            this.btnControlA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnControlA.Location = new System.Drawing.Point(462, 826);
            this.btnControlA.Name = "btnControlA";
            this.btnControlA.Size = new System.Drawing.Size(30, 73);
            this.btnControlA.TabIndex = 6;
            this.btnControlA.Tag = "A";
            this.btnControlA.Text = "A";
            this.btnControlA.UseVisualStyleBackColor = true;
            this.btnControlA.Click += new System.EventHandler(this.OnInputControlBtnClicked);
            // 
            // btnControlB
            // 
            this.btnControlB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnControlB.Location = new System.Drawing.Point(426, 826);
            this.btnControlB.Name = "btnControlB";
            this.btnControlB.Size = new System.Drawing.Size(30, 73);
            this.btnControlB.TabIndex = 7;
            this.btnControlB.Tag = "B";
            this.btnControlB.Text = "B";
            this.btnControlB.UseVisualStyleBackColor = true;
            this.btnControlB.Click += new System.EventHandler(this.OnInputControlBtnClicked);
            // 
            // btnControlC
            // 
            this.btnControlC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnControlC.Location = new System.Drawing.Point(390, 826);
            this.btnControlC.Name = "btnControlC";
            this.btnControlC.Size = new System.Drawing.Size(30, 73);
            this.btnControlC.TabIndex = 8;
            this.btnControlC.Tag = "C";
            this.btnControlC.Text = "C";
            this.btnControlC.UseVisualStyleBackColor = true;
            this.btnControlC.Click += new System.EventHandler(this.OnInputControlBtnClicked);
            // 
            // emulatedDisplay1
            // 
            this.emulatedDisplay1.BackColor = System.Drawing.Color.Black;
            this.emulatedDisplay1.Location = new System.Drawing.Point(12, 12);
            this.emulatedDisplay1.Name = "emulatedDisplay1";
            this.emulatedDisplay1.Size = new System.Drawing.Size(480, 802);
            this.emulatedDisplay1.TabIndex = 0;
            // 
            // btnScreencap
            // 
            this.btnScreencap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnScreencap.Location = new System.Drawing.Point(266, 851);
            this.btnScreencap.Name = "btnScreencap";
            this.btnScreencap.Size = new System.Drawing.Size(118, 23);
            this.btnScreencap.TabIndex = 9;
            this.btnScreencap.Text = "Capture Screenshot";
            this.btnScreencap.UseVisualStyleBackColor = true;
            this.btnScreencap.Click += new System.EventHandler(this.btnScreencap_Click);
            // 
            // volumeBar
            // 
            this.volumeBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.volumeBar.Location = new System.Drawing.Point(266, 876);
            this.volumeBar.Maximum = 12;
            this.volumeBar.Name = "volumeBar";
            this.volumeBar.Size = new System.Drawing.Size(118, 45);
            this.volumeBar.TabIndex = 10;
            this.volumeBar.Value = 8;
            this.volumeBar.Scroll += new System.EventHandler(this.volumeBar_Scroll);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 907);
            this.Controls.Add(this.volumeBar);
            this.Controls.Add(this.btnScreencap);
            this.Controls.Add(this.btnControlC);
            this.Controls.Add(this.btnControlB);
            this.Controls.Add(this.btnControlA);
            this.Controls.Add(this.btnControlUp);
            this.Controls.Add(this.btnControlRight);
            this.Controls.Add(this.btnControlLeft);
            this.Controls.Add(this.btnControlDown);
            this.Controls.Add(this.btnForceRedraw);
            this.Controls.Add(this.emulatedDisplay1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.volumeBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private EmulatedDisplay emulatedDisplay1;
        private System.Windows.Forms.Button btnForceRedraw;
        private System.Windows.Forms.Button btnControlDown;
        private System.Windows.Forms.Button btnControlLeft;
        private System.Windows.Forms.Button btnControlRight;
        private System.Windows.Forms.Button btnControlUp;
        private System.Windows.Forms.Button btnControlA;
        private System.Windows.Forms.Button btnControlB;
        private System.Windows.Forms.Button btnControlC;
        private System.Windows.Forms.Button btnScreencap;
        private System.Windows.Forms.TrackBar volumeBar;
    }
}

