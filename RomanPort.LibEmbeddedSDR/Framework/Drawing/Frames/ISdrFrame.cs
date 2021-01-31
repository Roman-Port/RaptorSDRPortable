using RomanPort.LibEmbeddedSDR.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Drawing.Frames
{
    public interface ISdrFrame
    {
        bool IsSemiTransparent { get; }
        void Tick(bool forceRedraw);
        void OnOpened();
        void OnClosed();
        UserInputResult OnInput(UserInputEventArgs input);
    }
}
