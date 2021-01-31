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
using System.Drawing.Drawing2D;
using RomanPort.LibSDR.Components;
using RomanPort.LibEmbeddedSDR.Framework.Drawing;

namespace RomanPort.LibEmbeddedSDR.EmulatedScreen
{
    public unsafe partial class EmulatedDisplay : UserControl, ISdrDisplay
    {
        public EmulatedDisplay()
        {
            InitializeComponent();

            //Get size
            imageWidth = ClientRectangle.Width;
            imageHeight = ClientRectangle.Height;

            //Make buffer
            buffer = UnsafeBuffer.Create(imageWidth * imageHeight, sizeof(DisplayPixel));
            bufferPtr = (DisplayPixel*)buffer;

            //Make image
            image = new Bitmap(imageWidth, imageHeight, imageWidth * sizeof(DisplayPixel), PixelFormat.Format32bppArgb, (IntPtr)bufferPtr);

            //Configure
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
        }

        private UnsafeBuffer buffer;
        private DisplayPixel* bufferPtr;
        private Bitmap image;
        private int imageWidth;
        private int imageHeight;

        protected override void OnPaint(PaintEventArgs e)
        {
            Invoke((MethodInvoker)delegate
            {
                ConfigureGraphics(e.Graphics);
                e.Graphics.DrawImageUnscaled(image, 0, 0);
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

        public Bitmap GetSnapshot()
        {
            Bitmap bmp = new Bitmap(imageWidth, imageHeight, PixelFormat.Format32bppArgb);
            var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Utils.Memcpy((void*)data.Scan0, bufferPtr, imageWidth * imageHeight * sizeof(DisplayPixel));
            bmp.UnlockBits(data);
            return bmp;
        }

        public void Apply()
        {
            Invalidate();
        }

        int IDrawingContext.Width { get => imageWidth; }
        int IDrawingContext.Height { get => imageHeight; }

        public void WritePixel(int offsetX, int offsetY, DisplayPixel pixel)
        {
            bufferPtr[offsetX + (imageWidth * offsetY)] = pixel;
        }

        public DisplayPixel* GetPixelPointer(int offsetX, int offsetY)
        {
            return bufferPtr + offsetX + (imageWidth * offsetY);
        }

        public IDrawingContext GetOffsetContext(int offsetX, int offsetY)
        {
            return new OffsetDrawingContext(offsetX, offsetY, this);
        }
    }
}
