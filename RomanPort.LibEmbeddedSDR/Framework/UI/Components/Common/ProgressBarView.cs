using RomanPort.LibEmbeddedSDR.Framework.Display;
using RomanPort.LibEmbeddedSDR.Framework.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.UI.Components.Common
{
    public class ProgressBarView : BaseRenderingView
    {
        public float max;
        public float min;
        public float progress;
        public DisplayPixel outlineColor;
        public DisplayPixel filledColor;
        
        public ProgressBarView(IRenderingContext parent, int x, int y, int width, int height, DisplayPixel outlineColor, DisplayPixel filledColor, float max, float min = 0) : base(parent, x, y, width, height)
        {
            this.outlineColor = outlineColor;
            this.filledColor = filledColor;
            this.max = max;
            this.min = min;
        }

        public override void FullRedrawThis()
        {
            float scale = Math.Min(1, Math.Max((progress - min) / (max - min), 0));
            UtilDrawFilledRectangle(this, (int)(width * scale), height, filledColor);
            UtilDrawOutlineRectangle(this, width, height, outlineColor);
        }
    }
}
