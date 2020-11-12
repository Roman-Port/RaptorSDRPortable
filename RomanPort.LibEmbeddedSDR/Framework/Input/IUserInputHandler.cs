using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Input
{
    public interface IUserInputHandler
    {
        void OnUserInput(UserInputKey key);
    }
}
