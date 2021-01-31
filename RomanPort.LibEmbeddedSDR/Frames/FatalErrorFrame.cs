using RomanPort.LibEmbeddedSDR.Framework.Drawing;
using RomanPort.LibEmbeddedSDR.Framework.Drawing.Frames;
using RomanPort.LibSDR.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Frames
{
    public unsafe class FatalErrorFrame : BaseRenderComponent, ISdrFrame
    {
        public FatalErrorFrame(IDrawingContext parent, string title, string message) : base(parent)
        {
            Width = parent.Width;
            Height = parent.Height;
            this.title = title;
            this.message = message;
        }

        private string title;
        private string message;

        private UnsafeBuffer backgroundBuffer;
        private DisplayPixel* backgroundBufferPtr;

        private const int WINDOW_WIDTH = 400;
        private const int WINDOW_HEIGHT = 300;
        private const int WINDOW_PADDING = 10;

        public bool IsSemiTransparent => false;

        public void OnClosed()
        {
            backgroundBuffer.Dispose();
            backgroundBuffer = null;
        }

        public void OnOpened()
        {
            
        }

        public void Tick(bool forceRedraw)
        {
            DrawingTick(forceRedraw);
        }

        protected override void LayoutView()
        {
            
        }

        protected override void RedrawView()
        {
            //Save or rerender buffer
            if(backgroundBuffer == null || backgroundBuffer.Length != Width * Height)
            {
                backgroundBuffer?.Dispose();
                backgroundBuffer = UnsafeBuffer.Create(Width * Height, out backgroundBufferPtr);
                byte* bufferPtr = (byte*)GetPixelPointer(0, 0);
                for (int i = 0; i < Width * Height * 4; i++)
                    bufferPtr[i] /= 2;
                Utils.Memcpy(backgroundBufferPtr, GetPixelPointer(0, 0), Width * Height * sizeof(DisplayPixel));
            } else
            {
                Utils.Memcpy(GetPixelPointer(0, 0), backgroundBufferPtr, Width * Height * sizeof(DisplayPixel));
            }

            //Calculate offsets
            int y = (Height - WINDOW_HEIGHT) / 2;
            int x = (Width - WINDOW_WIDTH) / 2;

            //Draw background
            UtilFill(DisplayPixel.BLACK, x, y, WINDOW_WIDTH, WINDOW_HEIGHT);
            UtilFill(DisplayPixel.RED, x, y + 40, WINDOW_WIDTH, 1);
            UtilOutline(DisplayPixel.RED, x, y, WINDOW_WIDTH, WINDOW_HEIGHT, 1);

            //Draw text
            FontStore.SYSTEM_REGULAR_15.RenderPretty(GetOffsetContext(x + WINDOW_PADDING, y), title.ToCharArray(), DisplayPixel.RED, AlignHorizontal.Left, AlignVertical.Center, WINDOW_WIDTH - WINDOW_PADDING - WINDOW_PADDING, 40);
            FontStore.SYSTEM_REGULAR_15.RenderPretty(GetOffsetContext(x + WINDOW_PADDING, y + 42 + WINDOW_PADDING), message.ToCharArray(), DisplayPixel.WHITE, AlignHorizontal.Left, AlignVertical.Top, WINDOW_WIDTH - WINDOW_PADDING - WINDOW_PADDING, WINDOW_HEIGHT - WINDOW_PADDING - 42);
        }
    }
}
