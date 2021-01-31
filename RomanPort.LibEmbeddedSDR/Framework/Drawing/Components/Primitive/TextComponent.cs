using RomanPort.LibEmbeddedSDR.Framework.Drawing.Fonts;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Drawing.Components.Primitive
{
    public class TextComponent : BasePaddingRenderComponent
    {
        public TextComponent(IDrawingContext parent, SdrFontPack font, string text, DisplayPixel color, AlignHorizontal horizontalTextAlign = AlignHorizontal.Left, AlignVertical verticalTextAlign = AlignVertical.Top) : base(parent)
        {
            this.font = font;
            this.text = text.ToCharArray();
            this.color = color;
            this.horizontalTextAlign = horizontalTextAlign;
            this.verticalTextAlign = verticalTextAlign;
        }

        private SdrFontPack font;
        private DisplayPixel color;
        private char[] text;
        private AlignHorizontal horizontalTextAlign;
        private AlignVertical verticalTextAlign;

        public TextComponent SetText(string text)
        {
            this.text = text.ToCharArray();
            Invalidate();
            return this;
        }

        public TextComponent SetText(char[] text)
        {
            this.text = text;
            Invalidate();
            return this;
        }

        public TextComponent SetBackgroundColor(DisplayPixel color)
        {
            BackgroundColor = color;
            return this;
        }

        public TextComponent SetTextColor(DisplayPixel color)
        {
            TextColor = color;
            return this;
        }

        public TextComponent SetTextHorizontalAlign(AlignHorizontal align)
        {
            TextHorizontalAlign = align;
            return this;
        }

        public TextComponent SetTextVerticalAlign(AlignVertical align)
        {
            TextVerticalAlign = align;
            return this;
        }

        public DisplayPixel TextColor
        {
            get => color;
            set
            {
                color = value;
                Invalidate();
            }
        }

        public AlignHorizontal TextHorizontalAlign
        {
            get => horizontalTextAlign;
            set
            {
                horizontalTextAlign = value;
                Invalidate();
            }
        }

        public AlignVertical TextVerticalAlign
        {
            get => verticalTextAlign;
            set
            {
                verticalTextAlign = value;
                Invalidate();
            }
        }

        protected override void LayoutView()
        {
            
        }

        protected override void RedrawView()
        {
            font.RenderPretty(PaddedContext, text, color, horizontalTextAlign, verticalTextAlign, PaddedWidth, PaddedHeight);
        }
    }
}
