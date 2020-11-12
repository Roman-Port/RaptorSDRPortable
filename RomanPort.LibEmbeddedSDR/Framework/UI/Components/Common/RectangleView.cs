using RomanPort.LibEmbeddedSDR.Framework.Display;
using RomanPort.LibEmbeddedSDR.Framework.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.UI.Components.Common
{
    public class RectangleView : BaseRenderingView
    {
        public DisplayPixel color;
        public bool fill;
        
        public RectangleView(IRenderingContext parent, int x, int y, int width, int height, DisplayPixel fillColor) : base(parent, x, y, width, height)
        {
            color = fillColor;
            fill = true;
        }

        public RectangleView(IRenderingContext parent, int x, int y, int width, int height) : base(parent, x, y, width, height)
        {
            fill = false;
        }

        public override unsafe void FullRedrawThis()
        {
            if(fill)
            {
                UtilDrawFilledRectangle(this, width, height, color);
            }
        }
    }
}
