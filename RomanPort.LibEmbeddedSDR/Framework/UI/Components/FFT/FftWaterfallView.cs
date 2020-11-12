using RomanPort.LibEmbeddedSDR.Framework.Display;
using RomanPort.LibEmbeddedSDR.Framework.Rendering;
using RomanPort.LibSDR.Framework.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.UI.Components.FFT
{
    public unsafe class FftWaterfallView : BaseRenderingView
    {
        private UnsafeBuffer waterfallBuffer;
        private DisplayPixel* waterfallBufferPtr;
        private UnsafeBuffer waterfallQueueBuffer;
        private DisplayPixel* waterfallQueueBufferPtr;
        private int waterfallBufferIndex;
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

        public FftWaterfallView(IRenderingContext parent, int x, int y, int width, int height) : base(parent, x, y, width, height)
        {
            waterfallBuffer = UnsafeBuffer.Create(width * height, sizeof(DisplayPixel));
            waterfallBufferPtr = (DisplayPixel*)waterfallBuffer;
            waterfallQueueBuffer = UnsafeBuffer.Create(width, sizeof(DisplayPixel));
            waterfallQueueBufferPtr = (DisplayPixel*)waterfallQueueBuffer;
        }

        public void AddScaledFrame(float* scaledFftFrame)
        {
            isFrameQueued = false;
            for (int x = 0; x < width; x++)
            {
                waterfallQueueBufferPtr[x] = GetGradientColor(scaledFftFrame[x] / minDb);
            }
            isFrameQueued = true;

            //Invalidate
            Invalidate();
        }

        public override void FullRedrawThis()
        {
            //We don't need to do anything if we have no frames drawn
            if (!isFrameQueued)
                return;

            //Move all pixels downwards by one, starting from the bottom
            for(int i = height - 1; i > 0; i--)
                Utils.Memcpy(GetPixelPointer(0, i), GetPixelPointer(0, i - 1), width * sizeof(DisplayPixel));

            //Copy queued frame to video buffer
            Utils.Memcpy(GetPixelPointer(0, 0), waterfallQueueBufferPtr, width * sizeof(DisplayPixel));
            isFrameQueued = false;
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
            return new DisplayPixel (
                (byte)(Math.Ceiling((mix1[0] * ratio) + (mix2[0] * (1 - ratio)))),
                (byte)(Math.Ceiling((mix1[1] * ratio) + (mix2[1] * (1 - ratio)))),
                (byte)(Math.Ceiling((mix1[2] * ratio) + (mix2[2] * (1 - ratio))))
            );
        }
    }
}
