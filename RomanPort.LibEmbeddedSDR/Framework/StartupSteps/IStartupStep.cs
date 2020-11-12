using RomanPort.LibEmbeddedSDR.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.StartupSteps
{
    public interface IStartupStep : IUserInputHandler
    {
        string GetName();
        string GetErrorTitle();
        bool Work(SdrSession session, out string errorText);
    }
}
