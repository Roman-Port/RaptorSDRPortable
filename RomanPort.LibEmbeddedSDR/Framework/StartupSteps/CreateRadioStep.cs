using RomanPort.LibEmbeddedSDR.Framework.Input;
using RomanPort.LibEmbeddedSDR.Framework.Radio;
using RomanPort.LibSDR.Framework.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.StartupSteps
{
    public class CreateRadioStep : IStartupStep
    {
        public string GetErrorTitle()
        {
            return "Error Creating Radio";
        }

        public string GetName()
        {
            return "Opening RTL-SDR";
        }

        public void OnUserInput(UserInputKey key)
        {
            
        }

        public bool Work(SdrSession session, out string errorText)
        {
            //Create radio
            session.radio = new RadioController(session.bufferSize, session.display.width);

            //Open device
            try
            {
                session.radio.OpenSource();
            }
            catch (DllNotFoundException)
            {
                errorText = "The DLL files required for communicating with the radio couldn't be found. Make sure you've installed the required DLLs for this radio.";
                return false;
            }
            catch (BadImageFormatException)
            {
                errorText = "The DLL files required for communicating with the radio do not match this platform. The LIKELY issue is that you are using a 32 bit version of librtlsdr, while this software is compiled in 64 bit.";
                return false;
            }
            catch (RadioNotFoundException)
            {
                errorText = "No attached radios found.";
                return false;
            }
            catch (RtlDeviceErrorException e)
            {
                errorText = $"RTL-SDR returned error {e.rtlStatusCode} while opening. Invalid parameter?";
                return false;
            }

            //Open buffers
            session.radio.OpenBuffers();

            errorText = null;
            return true;
        }
    }
}
