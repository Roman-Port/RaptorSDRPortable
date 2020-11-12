using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Rendering.Text
{
    public class SdrFontPackCharacter
    {
        public char character;
        public byte overdrawTop;
        public byte overdrawBottom;
        public byte[] payload;
    }
}
