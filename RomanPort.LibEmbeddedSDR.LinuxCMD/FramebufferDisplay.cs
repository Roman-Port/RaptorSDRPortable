using RomanPort.LibEmbeddedSDR.Framework;
using RomanPort.LibEmbeddedSDR.Framework.Drawing;
using RomanPort.LibSDR.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.LinuxCMD
{
    public unsafe class FramebufferDisplay : ISdrDisplay
    {
        private FileStream fb;
        private UnsafeBuffer buffer;
        private DisplayPixel* bufferPtr;

        public const int FRAME_WIDTH = 480;
        public const int FRAME_HEIGHT = 800;
        public const int FRAME_BYTES_TOTAL = FRAME_WIDTH * FRAME_HEIGHT * 4;

        public FramebufferDisplay(string path = "/dev/fb0")
        {
            fb = new FileStream(path, FileMode.Open);
            buffer = UnsafeBuffer.Create(FRAME_WIDTH * FRAME_HEIGHT, out bufferPtr);
        }

        public int Width => FRAME_WIDTH;

        public int Height => FRAME_HEIGHT;

        public void Apply()
        {
            fb.Position = 0;
            buffer.CopyToStream(fb, FRAME_BYTES_TOTAL);
        }

        public IDrawingContext GetOffsetContext(int offsetX, int offsetY)
        {
            return new OffsetDrawingContext(offsetX, offsetY, this);
        }

        public DisplayPixel* GetPixelPointer(int offsetX, int offsetY)
        {
            return bufferPtr + offsetX + (offsetY * FRAME_WIDTH);
        }

        public void WritePixel(int offsetX, int offsetY, DisplayPixel pixel)
        {
            bufferPtr[offsetX + (offsetY * FRAME_WIDTH)] = pixel;
        }
    }
}
