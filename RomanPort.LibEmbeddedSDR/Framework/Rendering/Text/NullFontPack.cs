using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Rendering.Text
{
    public class NullFontPack : BaseFontPack
    {
        public NullFontPack()
        {
            name = "UNKNOWN FONT";
            fullHeight = 15;
            height = 15;
            width = 10;
            characterCount = 0;
            maxOverdrawBottom = 0;
            maxOverdrawTop = 0;
        }
        
        public override void RenderCharacter(IDrawingContext ctx, char character, byte r, byte g, byte b)
        {
            RenderNullCharacter(ctx, r, g, b);
        }
    }
}
