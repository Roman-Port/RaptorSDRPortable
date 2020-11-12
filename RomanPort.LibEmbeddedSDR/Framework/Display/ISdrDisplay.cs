using RomanPort.LibEmbeddedSDR.Framework.Display;
using RomanPort.LibSDR.Framework.Util;
using System;

namespace RomanPort.LibEmbeddedSDR.Framework
{
    /// <summary>
    /// The actual display for rendering graphics
    /// </summary>
    public abstract unsafe class ISdrDisplay
    {
        public readonly int width;
        public readonly int height;
        public readonly UnsafeBuffer buffer;
        public readonly DisplayPixel* bufferPtr;

        public ISdrDisplay(int width, int height)
        {
            this.width = width;
            this.height = height;
            buffer = UnsafeBuffer.Create(width * height, sizeof(DisplayPixel));
            bufferPtr = (DisplayPixel*)buffer;
        }

        public abstract void Apply();
    }
}
