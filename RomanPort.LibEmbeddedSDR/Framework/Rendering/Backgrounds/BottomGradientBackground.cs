using RomanPort.LibEmbeddedSDR.Framework.Display;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Rendering.Backgrounds
{
    public class BottomGradientBackground : ICanvasBackgroundProvider
    {
        public readonly byte startR;
        public readonly byte startG;
        public readonly byte startB;
        public readonly int gradientHeight;

        private DisplayPixel[] cache;

        public BottomGradientBackground(int gradientHeight, byte startR, byte startG, byte startB)
        {
            this.gradientHeight = gradientHeight;
            this.startR = startR;
            this.startG = startG;
            this.startB = startB;
            UpdateCachedPixels();
        }
        
        public unsafe void RenderCanvasBackground(RenderingCanvas canvas)
        {
            int offset = canvas.display.height - gradientHeight;
            for(int i = 0; i<offset * canvas.display.width; i++)
            {
                canvas.display.bufferPtr[i] = new DisplayPixel(0, 0, 0);
            }
            for (int i = 0; i < cache.Length; i++)
            {
                DisplayPixel* ptr = canvas.GetPixelPointer(0, i + offset);
                for (int x = 0; x < canvas.display.width; x++)
                    ptr[x] = cache[i];
            }
        }

        private void UpdateCachedPixels()
        {
            cache = new DisplayPixel[gradientHeight];
            for(int i = 0; i < gradientHeight; i++)
            {
                float scale = (float)i / gradientHeight;
                cache[i] = new DisplayPixel(
                    (byte)(scale * startR),
                    (byte)(scale * startG),
                    (byte)(scale * startB)
                );
            }
        }
    }
}
