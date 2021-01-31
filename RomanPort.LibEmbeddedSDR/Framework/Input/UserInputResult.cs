using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Input
{
    public enum UserInputResult
    {
        Unhandled,
        HandledSelf,
        MoveNext,
        MovePrevious
    }
}
