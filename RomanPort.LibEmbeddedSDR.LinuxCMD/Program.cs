using System;

namespace RomanPort.LibEmbeddedSDR.LinuxCMD
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create display
            FramebufferDisplay display = new FramebufferDisplay();

            //Create session
            SdrSession session = new SdrSession(display, 16384);

            //Run
            session.Run();
        }
    }
}
