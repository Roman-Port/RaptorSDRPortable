using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Drawing.Components.Containers
{
    public class HorizontalComponentContainer : BasePaddingRenderComponent
    {
        public HorizontalComponentContainer(IDrawingContext ctx) : base(ctx)
        {
        }

        private List<ComponentChild> children = new List<ComponentChild>();

        protected override void LayoutView()
        {
            //Begin layout of top and bottom elements
            int offsetLeft = PaddingLeft;
            int offsetRight = PaddingRight;
            int widthRemaining = Width - PaddingLeft - PaddingRight;
            int fillComponentCount = 0;
            foreach (var c in children)
            {
                if (c.component.Hidden)
                    continue;
                switch (c.alignment)
                {
                    case AlignHorizontal.Left:
                        c.component.Y = PaddingTop;
                        c.component.X = offsetLeft;
                        offsetLeft += c.component.Width;
                        widthRemaining -= c.component.Width;
                        break;
                    case AlignHorizontal.Right:
                        c.component.Y = PaddingTop;
                        c.component.X = Width - offsetRight - c.component.Width;
                        offsetRight += c.component.Width;
                        widthRemaining -= c.component.Width;
                        break;
                    case AlignHorizontal.Center:
                        fillComponentCount++;
                        break;
                }
                c.component.Height = Height - PaddingTop - PaddingBottom;
            }

            //Layout fill components
            if (fillComponentCount != 0)
            {
                widthRemaining /= fillComponentCount;
                foreach (var c in children)
                {
                    if (c.component.Hidden || c.alignment != AlignHorizontal.Center)
                        continue;
                    c.component.X = offsetLeft;
                    offsetLeft += widthRemaining;
                    c.component.Width = widthRemaining;
                }
            }
        }

        protected override void RedrawView()
        {

        }

        protected override void EnumerateChildren(RenderComponentChildEnumerable callback)
        {
            foreach (var c in children)
                callback(c.component);
        }

        public BaseRenderComponent AddChild(BaseRenderComponent child, AlignHorizontal alignment, int width)
        {
            child.Width = width;
            return AddChild(child, alignment);
        }

        public BaseRenderComponent AddChild(BaseRenderComponent child, AlignHorizontal alignment)
        {
            child.Height = Height;
            children.Add(new ComponentChild
            {
                alignment = alignment,
                component = child
            });
            child.SetParent(this);
            LayoutInvalidate();
            return child;
        }

        class ComponentChild
        {
            public BaseRenderComponent component;
            public AlignHorizontal alignment;
        }
    }
}
