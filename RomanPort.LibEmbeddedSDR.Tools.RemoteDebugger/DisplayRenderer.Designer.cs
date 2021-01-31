
namespace RomanPort.LibEmbeddedSDR.Tools.RemoteDebugger
{
    partial class DisplayRenderer
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
            this.btnCaptureScreenshot = new System.Windows.Forms.Button();
            this.btnRecord = new System.Windows.Forms.Button();
            this.frame = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.frame)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCaptureScreenshot
            // 
            this.btnCaptureScreenshot.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCaptureScreenshot.Location = new System.Drawing.Point(152, 72);
            this.btnCaptureScreenshot.Name = "btnCaptureScreenshot";
            this.btnCaptureScreenshot.Size = new System.Drawing.Size(135, 23);
            this.btnCaptureScreenshot.TabIndex = 0;
            this.btnCaptureScreenshot.Text = "Capture Screenshot";
            this.btnCaptureScreenshot.UseVisualStyleBackColor = true;
            // 
            // btnRecord
            // 
            this.btnRecord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRecord.Location = new System.Drawing.Point(11, 72);
            this.btnRecord.Name = "btnRecord";
            this.btnRecord.Size = new System.Drawing.Size(135, 23);
            this.btnRecord.TabIndex = 1;
            this.btnRecord.Text = "Start Recording";
            this.btnRecord.UseVisualStyleBackColor = true;
            // 
            // frame
            // 
            this.frame.Location = new System.Drawing.Point(12, 12);
            this.frame.Name = "frame";
            this.frame.Size = new System.Drawing.Size(100, 50);
            this.frame.TabIndex = 2;
            this.frame.TabStop = false;
            // 
            // DisplayRenderer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(299, 107);
            this.Controls.Add(this.frame);
            this.Controls.Add(this.btnRecord);
            this.Controls.Add(this.btnCaptureScreenshot);
            this.Name = "DisplayRenderer";
            this.Text = "DisplayRenderer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DisplayRenderer_FormClosing);
            this.Load += new System.EventHandler(this.DisplayRenderer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.frame)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCaptureScreenshot;
        private System.Windows.Forms.Button btnRecord;
        private System.Windows.Forms.PictureBox frame;
    }
}