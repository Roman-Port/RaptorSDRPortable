using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Drawing
{
    public abstract class BasePaddingRenderComponent : BaseRenderComponent
    {
        public BasePaddingRenderComponent(IDrawingContext parent) : base(parent)
        {
            ctx = new OffsetDrawingContext(0, 0, this);
        }

        private OffsetDrawingContext ctx;
        private int paddingTop;
        private int paddingBottom;
        private int paddingLeft;
        private int paddingRight;

        public int PaddedWidth { get => Width - paddingLeft - paddingRight; }

        public int PaddedHeight { get => Height - paddingTop - paddingBottom; }

        public OffsetDrawingContext PaddedContext { get => ctx; }

        public int PaddingTop
        {
            get => paddingTop;
            set
            {
                paddingTop = value;
                ctx.offsetY = value;
                LayoutInvalidate();
            }
        }

        public int PaddingBottom
        {
            get => paddingBottom;
            set
            {
                paddingBottom = value;
                LayoutInvalidate();
            }
        }

        public int PaddingLeft
        {
            get => paddingLeft;
            set
            {
                paddingLeft = value;
                ctx.offsetX = value;
                LayoutInvalidate();
            }
        }

        public int PaddingRight
        {
            get => paddingRight;
            set
            {
                paddingRight = value;
                LayoutInvalidate();
            }
        }

        public int Padding
        {
            set
            {
                PaddingTop = value;
                PaddingBottom = value;
                PaddingLeft = value;
                PaddingRight = value;
            }
        }
    }
}
