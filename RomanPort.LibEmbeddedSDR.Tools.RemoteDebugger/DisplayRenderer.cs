using RomanPort.LibEmbeddedSDR.Framework.Debugger;
using RomanPort.LibEmbeddedSDR.Framework.Drawing;
using RomanPort.LibSDR.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RomanPort.LibEmbeddedSDR.Tools.RemoteDebugger
{
    public unsafe partial class DisplayRenderer : Form
    {
        public DisplayRenderer(DebuggerClient debugger)
        {
            InitializeComponent();
            this.debugger = debugger;
            
        }

        private void DisplayRenderer_Load(object sender, EventArgs e)
        {
            debugger.BeginScreenStreaming();
            debugger.OnScreencapFrameStarted += Debugger_OnScreencapFrameStarted;
            debugger.OnScreencapData += Debugger_OnScreencapData;
        }

        private void Debugger_OnScreencapFrameStarted(int width, int height)
        {
            if (width == this.width && height == this.height)
                return;
            imageIndex = 0;
            this.width = width;
            this.height = height;
            Invoke((MethodInvoker)delegate
            {
                imageBuffer = UnsafeBuffer.Create(width * height, out imagePtr);
                image = new Bitmap(width, height, width * sizeof(DisplayPixel), System.Drawing.Imaging.PixelFormat.Format32bppArgb, (IntPtr)imagePtr);
                frame.Width = width;
                frame.Height = height;
                frame.Image = image;
                imageSize = width * height;
                imageIndex = 0;
                Width = frame.Left + frame.Left + image.Width;
                Height = frame.Top + frame.Bottom + image.Height;
            });
        }

        private void Debugger_OnScreencapData(byte[] buffer, int bufferOffset, int count)
        {
            if (imagePtr == null)
                return;
            for(int i = 0; i<count; i += 4)
            {
                imagePtr[imageIndex] = new DisplayPixel(buffer[DebuggerBase.HEADER_LENGTH + i + 2], buffer[DebuggerBase.HEADER_LENGTH + i + 1], buffer[DebuggerBase.HEADER_LENGTH + i + 0], buffer[DebuggerBase.HEADER_LENGTH + i + 3]);
                imageIndex = (imageIndex + 1) % imageSize;
            }
            frame.Invalidate();
        }

        private Bitmap image;
        private UnsafeBuffer imageBuffer;
        private DisplayPixel* imagePtr;
        private int imageIndex;
        private int imageSize;

        private int width = -1;
        private int height = -1;
        private DebuggerClient debugger;

        private void DisplayRenderer_FormClosing(object sender, FormClosingEventArgs e)
        {
            debugger.EndScreenStreaming();
        }
    }
}
