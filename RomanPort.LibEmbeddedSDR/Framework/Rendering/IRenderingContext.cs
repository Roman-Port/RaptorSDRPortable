using RomanPort.LibEmbeddedSDR.Framework.Display;
using RomanPort.LibEmbeddedSDR.Framework.Input;
using RomanPort.LibEmbeddedSDR.Framework.Rendering.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Rendering
{
    public interface IRenderingContext : IDrawingContext, IUserInputHandler
    {
        int absoluteX { get; }
        int absoluteY { get; }

        void AddChild(BaseRenderingView view);
        void InvalidateChild(BaseRenderingView view);
        RenderingCanvas GetCanvas();
        void FullRedraw(bool force);

        bool TrySelect(); //Called when this object is attempted to be selected. Returns true/false if it was actually selected
    }
}
