using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Drawing.Components.Macros
{
    public class LabeledControlComponent : BaseRenderComponent
    {
        public LabeledControlComponent(IDrawingContext parent, string label, BaseRenderComponent component) : base(parent)
        {
            this.label = label.ToCharArray();
            this.component = component;
            component.SetParent(this);
            Width = component.Width + PADDING_RIGHT;
        }

        private char[] label;
        private BaseRenderComponent component;

        private const int PADDING_TOP = 20;
        private const int PADDING_RIGHT = 5;

        protected override void EnumerateChildren(RenderComponentChildEnumerable callback)
        {
            callback(component);
        }

        protected override void LayoutView()
        {
            component.Height = Height - PADDING_TOP;
            component.Width = Width - PADDING_RIGHT;
            component.Y = PADDING_TOP;
        }

        protected override void RedrawView()
        {
            FontStore.SYSTEM_COMPACT_15.RenderLine(this, label, DisplayPixel.WHITE);
        }
    }
}
