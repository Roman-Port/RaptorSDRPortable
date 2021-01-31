using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Drawing.Components.Primitive
{
    public class BoxComponent : BaseRenderComponent
    {
        public BoxComponent(IDrawingContext ctx, DisplayPixel color) : base(ctx)
        {
            this.color = color;
        }

        private DisplayPixel color;

        protected override void LayoutView()
        {
            
        }

        protected override void RedrawView()
        {
            UtilFill(color);
        }
    }
}
