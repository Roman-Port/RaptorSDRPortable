using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using RomanPort.LibEmbeddedSDR.Framework.Display;
using RomanPort.LibSDR.Framework.Util;
using System.Drawing.Drawing2D;

namespace RomanPort.LibEmbeddedSDR.EmulatedScreen
{
    public unsafe partial class EmulatedDisplay : UserControl
    {
        public EmulatedDisplay()
        {
            InitializeComponent();
            display = new EmulatedDisplaySession(ClientRectangle.Width, ClientRectangle.Height, this);
            buffer = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, PixelFormat.Format32bppArgb);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
        }

        public EmulatedDisplaySession display;
        private Bitmap buffer;

        public void UpdateBuffer(DisplayPixel* ptr)
        {
            Invoke((MethodInvoker)delegate
            {
                var bits = buffer.LockBits(new Rectangle(0, 0, buffer.Width, buffer.Height), ImageLockMode.ReadWrite, buffer.PixelFormat);
                Utils.Memcpy((void*)bits.Scan0, ptr, buffer.Width * buffer.Height * sizeof(DisplayPixel));
                buffer.UnlockBits(bits);
                Invalidate();
            });
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Invoke((MethodInvoker)delegate
            {
                ConfigureGraphics(e.Graphics);
                e.Graphics.DrawImageUnscaled(buffer, 0, 0);
            });
        }

        public static void ConfigureGraphics(Graphics graphics)
        {
            graphics.CompositingMode = CompositingMode.SourceOver;
            graphics.CompositingQuality = CompositingQuality.HighSpeed;
            graphics.SmoothingMode = SmoothingMode.None;
            graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            graphics.InterpolationMode = InterpolationMode.High;
        }

        public Bitmap GetBuffer()
        {
            return buffer;
        }
    }
}
