using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Drawing.Components.Macros
{
    public class BatteryIndicatorComponent : BaseRenderComponent
    {
        public BatteryIndicatorComponent(IDrawingContext parent) : base(parent)
        {
            Width = 26;
            Height = 40;
        }

        private const int BATTERY_TIP_HEIGHT = 3;
        private const int PADDING_V = 5;
        private const int PADDING_H = 5;

        protected override void LayoutView()
        {
            
        }

        protected override void RedrawView()
        {
            //Draw outline and tip
            int tipWidth = (Width - PADDING_H - PADDING_H) / 2;
            UtilFill(DisplayPixel.WHITE, (Width - tipWidth) / 2, PADDING_V, tipWidth, BATTERY_TIP_HEIGHT);
            UtilOutline(DisplayPixel.WHITE, PADDING_H, BATTERY_TIP_HEIGHT + PADDING_V, Width - PADDING_H - PADDING_H, Height - BATTERY_TIP_HEIGHT - PADDING_V - PADDING_V, 1);
        }
    }
}
