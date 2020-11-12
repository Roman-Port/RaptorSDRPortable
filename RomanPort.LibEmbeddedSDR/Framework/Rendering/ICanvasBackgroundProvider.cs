using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Rendering
{
    public interface ICanvasBackgroundProvider
    {
        void RenderCanvasBackground(RenderingCanvas canvas);
    }
}
