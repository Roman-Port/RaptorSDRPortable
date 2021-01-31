using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Drawing
{
    public interface ISdrDisplay : IDrawingContext
    {
        void Apply();
    }
}
