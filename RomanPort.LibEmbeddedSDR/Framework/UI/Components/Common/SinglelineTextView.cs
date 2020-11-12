using RomanPort.LibEmbeddedSDR.Framework.Display;
using RomanPort.LibEmbeddedSDR.Framework.Rendering;
using RomanPort.LibEmbeddedSDR.Framework.Rendering.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.UI.Components.Common
{
    public class SinglelineTextView : BaseRenderingView
    {
        public BaseFontPack font;
        public TextViewAlign align;
        public char[] text;
        public int characterSpacing;
        public byte colorR;
        public byte colorG;
        public byte colorB;

        public SinglelineTextView(IRenderingContext parent, int x, int y, int width, int height, BaseFontPack font, string text, DisplayPixel color, TextViewAlign align = TextViewAlign.Left, int characterSpacing = -1) : base(parent, x, y, width, height)
        {
            this.font = font;
            this.text = text.ToCharArray();
            this.characterSpacing = characterSpacing;
            this.align = align;
            colorR = color.r;
            colorG = color.g;
            colorB = color.b;
        }

        public void SetText(string text)
        {
            this.text = text.ToCharArray();
        }

        public override void FullRedrawThis()
        {
            int charsToDraw = Math.Min(text.Length, width / (font.width + characterSpacing));
            if(align == TextViewAlign.Left)
                font.RenderLinearString(this, characterSpacing, text, charsToDraw, colorR, colorG, colorB);
            else if (align == TextViewAlign.Center)
                font.RenderLinearString(GetOffsetContext((width - (text.Length * (font.width + characterSpacing))) / 2, 0), characterSpacing, text, charsToDraw, colorR, colorG, colorB);
            else if (align == TextViewAlign.Right)
                font.RenderLinearString(GetOffsetContext(width - font.CalculateWidth(text.Length, characterSpacing), 0), characterSpacing, text, charsToDraw, colorR, colorG, colorB);
        }

        public enum TextViewAlign
        {
            Left,
            Center,
            Right
        }
    }
}
