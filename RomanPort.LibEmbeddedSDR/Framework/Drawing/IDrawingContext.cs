using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Drawing
{
    /// <summary>
    /// An interface used by components for writing
    /// </summary>
    public interface IDrawingContext
    {
        int Width { get; }
        int Height { get; }
        void WritePixel(int offsetX, int offsetY, DisplayPixel pixel);
        unsafe DisplayPixel* GetPixelPointer(int offsetX, int offsetY);
        IDrawingContext GetOffsetContext(int offsetX, int offsetY);
    }
}
