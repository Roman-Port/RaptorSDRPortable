using RomanPort.LibSDR.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Drawing.Components.Advanced
{
    public unsafe class FftWaterfallComponent : BaseRenderComponent
    {
        public FftWaterfallComponent(IDrawingContext parent) : base(parent)
        {
            
        }

        private UnsafeBuffer waterfallBuffer;
        private DisplayPixel* waterfallBufferPtr;
        private UnsafeBuffer waterfallQueueBuffer;
        private DisplayPixel* waterfallQueueBufferPtr;
        private bool isFrameQueued;

        public float minDb = -80;

        public static readonly float[][] WATERFALL_COLORS = new float[][]
        {
            new float[] {0, 0, 32},
            new float[] {0, 0, 48},
            new float[] {0, 0, 80},
            new float[] {0, 0, 145},
            new float[] {30, 144, 255},
            new float[] {255, 255, 255},
            new float[] {255, 255, 0},
            new float[] {254, 109, 22},
            new float[] {255, 0, 0},
            new float[] {198, 0, 0},
            new float[] {159, 0, 0},
            new float[] {117, 0, 0},
            new float[] {74, 0, 0}
        };

        private void RecreateBuffers()
        {
            //Destory buffers
            waterfallBuffer?.Dispose();
            waterfallQueueBuffer?.Dispose();

            //Create buffers
            waterfallBuffer = UnsafeBuffer.Create(Width * Height, sizeof(DisplayPixel));
            waterfallBufferPtr = (DisplayPixel*)waterfallBuffer;
            waterfallQueueBuffer = UnsafeBuffer.Create(Width, sizeof(DisplayPixel));
            waterfallQueueBufferPtr = (DisplayPixel*)waterfallQueueBuffer;
        }

        protected override void LayoutView()
        {
            
        }

        protected override void RedrawView()
        {
            //Check
            if (waterfallQueueBuffer == null || waterfallBuffer == null || waterfallQueueBuffer.Length != Width || waterfallBuffer.Length != Width * Height)
            {
                RecreateBuffers();
                return;
            }

            //We don't need to do anything if we have no frames drawn
            if (!isFrameQueued)
                return;

            //Copy pixels from the buffer here
            Utils.Memcpy(GetPixelPointer(0, 1), waterfallBufferPtr, Width * (Height - 1) * sizeof(DisplayPixel));

            //Copy queued frame to video buffer
            Utils.Memcpy(GetPixelPointer(0, 0), waterfallQueueBufferPtr, Width * sizeof(DisplayPixel));

            //Copy back to the main buffer
            Utils.Memcpy(waterfallBufferPtr, GetPixelPointer(0, 0), Width * Height * sizeof(DisplayPixel));

            isFrameQueued = false;
        }

        public void AddScaledFrame(float* scaledFftFrame, int width)
        {
            //Check
            if (waterfallQueueBuffer == null || waterfallBuffer == null || waterfallQueueBuffer.Length != width)
                RecreateBuffers();

            isFrameQueued = false;
            for (int x = 0; x < Width; x++)
            {
                waterfallQueueBufferPtr[x] = GetGradientColor(scaledFftFrame[x] / minDb);
            }
            isFrameQueued = true;

            //Invalidate
            Invalidate();
        }

        static DisplayPixel GetGradientColor(float percent)
        {
            //Make sure percent is within range
            percent = 1 - percent;
            percent = Math.Max(0, percent);
            percent = Math.Min(1, percent);

            //Calculate
            var scale = WATERFALL_COLORS.Length - 1;

            //Get the two colors to mix
            var mix2 = WATERFALL_COLORS[(int)Math.Floor(percent * scale)];
            var mix1 = WATERFALL_COLORS[(int)Math.Ceiling(percent * scale)];
            if (mix2 == null || mix1 == null)
            {
                throw new Exception("Invalid color!");
            }

            //Get ratio
            var ratio = (percent * scale) - Math.Floor(percent * scale);

            //Mix
            return new DisplayPixel(
                (byte)(Math.Ceiling((mix1[0] * ratio) + (mix2[0] * (1 - ratio)))),
                (byte)(Math.Ceiling((mix1[1] * ratio) + (mix2[1] * (1 - ratio)))),
                (byte)(Math.Ceiling((mix1[2] * ratio) + (mix2[2] * (1 - ratio))))
            );
        }
    }
}
