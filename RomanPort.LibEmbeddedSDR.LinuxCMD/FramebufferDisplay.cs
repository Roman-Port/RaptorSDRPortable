using RomanPort.LibEmbeddedSDR.Framework;
using RomanPort.LibEmbeddedSDR.Framework.Display;
using RomanPort.LibSDR.Framework.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.LinuxCMD
{
    public unsafe class FramebufferDisplay : ISdrDisplay
    {
        private FileStream fb;
        private byte[] managedBuffer;
        
        public FramebufferDisplay(string path = "/dev/fb0") : base(480, 800)
        {
            fb = new FileStream(path, FileMode.Open);
            managedBuffer = new byte[width * sizeof(DisplayPixel)];
        }

        public override void Apply()
        {
            fixed(byte* ptr = managedBuffer)
            {
                fb.Position = 0;
                for (int i = 0; i < height; i++)
                {
                    //Copy to managed memory
                    Utils.Memcpy(ptr, bufferPtr + (width * i), width * sizeof(DisplayPixel));

                    //Write
                    fb.Write(managedBuffer, 0, managedBuffer.Length);
                }
            }
        }
    }
}
