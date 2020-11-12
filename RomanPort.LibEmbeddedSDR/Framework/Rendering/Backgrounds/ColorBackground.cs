using RomanPort.LibEmbeddedSDR.Framework.Display;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Rendering.Backgrounds
{
    public class ColorBackground : ICanvasBackgroundProvider
    {
        public DisplayPixel color;

        public ColorBackground(DisplayPixel color)
        {
            this.color = color;
        }

        public unsafe void RenderCanvasBackground(RenderingCanvas canvas)
        {
            for (int i = 0; i < canvas.display.width * canvas.display.height; i++)
                canvas.display.bufferPtr[i] = color;
        }
    }
}
