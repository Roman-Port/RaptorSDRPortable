using RomanPort.LibEmbeddedSDR.Framework.Display;
using RomanPort.LibEmbeddedSDR.Framework.Rendering;
using RomanPort.LibEmbeddedSDR.Framework.Rendering.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.UI.Components.Common
{
    public class MultilineTextView : BaseRenderingView
    {
        public BaseFontPack font;
        public string text;
        public int characterSpacing;
        public int lineSpacing;
        public byte colorR;
        public byte colorG;
        public byte colorB;

        public MultilineTextView(IRenderingContext parent, int x, int y, int width, int height, BaseFontPack font, string text, DisplayPixel color, int characterSpacing = -1, int lineSpacing = 5) : base(parent, x, y, width, height)
        {
            this.font = font;
            this.text = text;
            this.characterSpacing = characterSpacing;
            this.lineSpacing = lineSpacing;
            colorR = color.r;
            colorG = color.g;
            colorB = color.b;
        }

        public override void FullRedrawThis()
        {
            font.RenderMultilineString(this, characterSpacing, lineSpacing, width, text, colorR, colorG, colorB);
        }
    }
}
