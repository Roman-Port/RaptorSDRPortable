using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Drawing.Components.Containers
{
    public class CenteringComponentContainer : BaseRenderComponent
    {
        public CenteringComponentContainer(IDrawingContext parent) : base(parent)
        {
        }

        public void AddChild(BaseRenderComponent child)
        {
            children.Add(child);
            LayoutInvalidate();
        }

        public void AddChild(BaseRenderComponent child, int width, int height)
        {
            child.Width = width;
            child.Height = height;
            AddChild(child);
        }

        private List<BaseRenderComponent> children = new List<BaseRenderComponent>();

        protected override void EnumerateChildren(RenderComponentChildEnumerable callback)
        {
            foreach (var c in children)
                callback(c);
        }

        protected override void LayoutView()
        {
            //Total size of all children
            int totalHeight = 0;
            foreach(var c in children)
                totalHeight += c.Height;

            //Position all
            int offsetTop = (Height - totalHeight) / 2;
            foreach(var c in children)
            {
                c.X = (Width - c.Width) / 2;
                c.Y = offsetTop;
                offsetTop += c.Height;
            }
        }

        protected override void RedrawView()
        {
            
        }
    }
}
