using RomanPort.LibEmbeddedSDR.Framework.Display;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Rendering.Backgrounds
{
    /// <summary>
    /// Draws black the first time it is called, and then never does drawing again
    /// </summary>
    public class NullBackground : ICanvasBackgroundProvider
    {
        public bool isFirstDraw = true;
        
        public unsafe void RenderCanvasBackground(RenderingCanvas canvas)
        {
            if(isFirstDraw)
            {
                byte* ptr = (byte*)canvas.display.bufferPtr;
                int len = canvas.display.width * canvas.display.height * sizeof(DisplayPixel);
                for (int i = 0; i < len; i++)
                    ptr[i] = 0x00;
                isFirstDraw = false;
            }
        }
    }
}
