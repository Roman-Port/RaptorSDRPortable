using RomanPort.LibEmbeddedSDR.Framework.Display;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Rendering
{
    /// <summary>
    /// An interface used by components for writing
    /// </summary>
    public interface IDrawingContext
    {
        void WritePixel(int offsetX, int offsetY, DisplayPixel pixel);
        unsafe DisplayPixel* GetPixelPointer(int offsetX, int offsetY);
        IDrawingContext GetOffsetContext(int offsetX, int offsetY);
    }
}
