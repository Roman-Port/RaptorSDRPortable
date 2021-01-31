using RomanPort.LibEmbeddedSDR.Framework.Debugger;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace RomanPort.LibEmbeddedSDR.LinuxCMD
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create parts
            FramebufferDisplay display = new FramebufferDisplay();
            EmbeddedDevice device = new EmbeddedDevice();

            //Create session
            SDR.Configure(display, device);

            //Go
            SDR.Run();
        }
    }
}
