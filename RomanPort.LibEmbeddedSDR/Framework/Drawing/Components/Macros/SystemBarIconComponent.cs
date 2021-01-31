using RomanPort.LibEmbeddedSDR.Framework.Drawing.Components.Advanced;
using RomanPort.LibEmbeddedSDR.Framework.Drawing.Fonts;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Drawing.Components.Macros
{
    public class SystemBarIconComponent : BaseRenderComponent
    {
        public SystemBarIconComponent(IDrawingContext parent, string label) : base(parent)
        {
            this.label = label;
            font = FontStore.SYSTEM_COMPACT_15;
            Width = PADDING_FULL + MARGIN_RIGHT + (label.Length * font.width);
            marginTop = (SystemBarComponent.SYSTEMBAR_HEIGHT - PADDING_FULL - font.height) / 2;
            textCtx = new OffsetDrawingContext(PADDING, marginTop + PADDING, this);
        }

        private const int PADDING = 5;
        private const int PADDING_FULL = PADDING + PADDING;
        private const int MARGIN_RIGHT = 7;

        private SdrFontPack font;
        private string label;
        private int marginTop;
        private OffsetDrawingContext textCtx;
        private bool lit;

        private readonly static DisplayPixel COLOR_LIT = DisplayPixel.WHITE;
        private readonly static DisplayPixel COLOR_UNLIT = new DisplayPixel(100);

        public bool IconLit
        {
            get => lit;
            set
            {
                lit = value;
                Invalidate();
            }
        }

        public SystemBarIconComponent SetIconLit(bool lit)
        {
            IconLit = lit;
            return this;
        }

        protected override void LayoutView()
        {
            
        }

        protected override void RedrawView()
        {
            UtilFill(lit ? COLOR_LIT : COLOR_UNLIT, 0, marginTop, Width - MARGIN_RIGHT, PADDING_FULL + font.height);
            font.RenderPretty(textCtx, label.ToCharArray(), DisplayPixel.BLACK, AlignHorizontal.Center, AlignVertical.Center, font.width * label.Length, font.height);
        }
    }
}
