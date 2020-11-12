using RomanPort.LibEmbeddedSDR.Framework.Display;
using RomanPort.LibSDR.Framework.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Rendering.Backgrounds
{
    public class BlackBackground : ICanvasBackgroundProvider
    {
        public unsafe void RenderCanvasBackground(RenderingCanvas canvas)
        {
            byte* ptr = (byte*)canvas.display.bufferPtr;
            int len = canvas.display.width * canvas.display.height * sizeof(DisplayPixel);
            for (int i = 0; i < len; i++)
                ptr[i] = 0x00;
        }
    }
}
