using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Drawing
{
    [StructLayout(LayoutKind.Explicit, Size = 4, CharSet = CharSet.Ansi)]
    public struct DisplayPixel
    {
        //Configuration for writing to the canvas in emulated mode
        [FieldOffset(3)] public byte a;
        [FieldOffset(2)] public byte r;
        [FieldOffset(1)] public byte g;
        [FieldOffset(0)] public byte b;

        public static DisplayPixel TRANSPARENT = new DisplayPixel(0, 0, 0, 0);
        public static DisplayPixel BLACK = new DisplayPixel(0);
        public static DisplayPixel WHITE = new DisplayPixel(byte.MaxValue);
        public static DisplayPixel RED = new DisplayPixel(255, 0, 0);
        public static DisplayPixel GREEN = new DisplayPixel(0, 255, 0);
        public static DisplayPixel BLUE = new DisplayPixel(0, 0, 255);
        public static DisplayPixel GREY = new DisplayPixel(210);

        public DisplayPixel(byte brightness)
        {
            r = brightness;
            g = brightness;
            b = brightness;
            a = byte.MaxValue;
        }

        public DisplayPixel(byte r, byte g, byte b, byte a = byte.MaxValue)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public static unsafe void Mix(DisplayPixel* a, DisplayPixel* b, float aPercent, DisplayPixel* output)
        {
            float bPercent = 1 - aPercent;
            byte* cA = (byte*)a;
            byte* cB = (byte*)b;
            byte* cO = (byte*)output;
            for (int i = 0; i < 4; i++)
                cO[i] = (byte)((aPercent * cA[i]) + (bPercent * cB[i]));
        }
    }
}
