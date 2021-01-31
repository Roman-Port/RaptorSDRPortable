using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Drawing.Components.Containers
{
    public class VerticalComponentContainer : BasePaddingRenderComponent
    {
        public VerticalComponentContainer(IDrawingContext ctx) : base(ctx)
        {
        }

        private List<ComponentChild> children = new List<ComponentChild>();

        protected override void LayoutView()
        {
            //Begin layout of top and bottom elements
            int offsetTop = PaddingTop;
            int offsetBottom = PaddingBottom;
            int heightRemaining = Height - PaddingTop - PaddingBottom;
            int fillComponentCount = 0;
            foreach(var c in children)
            {
                if (c.component.Hidden)
                    continue;
                switch(c.alignment)
                {
                    case AlignVertical.Top:
                        c.component.X = PaddingLeft;
                        c.component.Y = offsetTop;
                        offsetTop += c.component.Height;
                        heightRemaining -= c.component.Height;
                        break;
                    case AlignVertical.Bottom:
                        c.component.X = PaddingLeft;
                        c.component.Y = Height - offsetBottom - c.component.Height;
                        offsetBottom += c.component.Height;
                        heightRemaining -= c.component.Height;
                        break;
                    case AlignVertical.Center:
                        fillComponentCount++;
                        break;
                }
                c.component.Width = Width - PaddingLeft - PaddingRight;
            }

            //Layout fill components
            if (fillComponentCount != 0)
            {
                heightRemaining /= fillComponentCount;
                foreach (var c in children)
                {
                    if (c.component.Hidden || c.alignment != AlignVertical.Center)
                        continue;
                    c.component.Y = offsetTop;
                    offsetTop += heightRemaining;
                    c.component.Height = heightRemaining;
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

        public void AddChild(BaseRenderComponent child, AlignVertical alignment)
        {
            child.Width = Width;
            children.Add(new ComponentChild
            {
                alignment = alignment,
                component = child
            });
            child.SetParent(this);
            LayoutInvalidate();
        }

        public void AddChild(BaseRenderComponent child, AlignVertical alignment, int height)
        {
            child.Height = height;
            AddChild(child, alignment);
        }

        class ComponentChild
        {
            public BaseRenderComponent component;
            public AlignVertical alignment;
        }
    }
}
