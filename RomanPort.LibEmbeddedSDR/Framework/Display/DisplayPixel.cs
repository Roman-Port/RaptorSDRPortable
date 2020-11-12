using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Display
{
    [StructLayout(LayoutKind.Explicit, Size = 4, CharSet = CharSet.Ansi)]
    public struct DisplayPixel
    {
        //Configuration for writing to the canvas in emulated mode
        [FieldOffset(3)] public byte a;
        [FieldOffset(2)] public byte r;
        [FieldOffset(1)] public byte g;
        [FieldOffset(0)] public byte b;

        public DisplayPixel(byte r, byte g, byte b, byte a = byte.MaxValue)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
    }
}
