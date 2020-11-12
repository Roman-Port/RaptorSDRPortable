using RomanPort.LibEmbeddedSDR.Framework.Display;
using RomanPort.LibSDR.Framework.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Rendering.Image
{
    public unsafe class SdrImgFile : IDisposable
    {
        private readonly UnsafeBuffer imgBuffer;
        public readonly DisplayPixel* imgBufferPtr;
        public readonly ushort width;
        public readonly ushort height;

        public SdrImgFile(ushort width, ushort height)
        {
            this.imgBuffer = UnsafeBuffer.Create(width * height, sizeof(DisplayPixel));
            this.imgBufferPtr = (DisplayPixel*)imgBuffer;
            this.width = width;
            this.height = height;
        }

        public static SdrImgFile LoadFromAsset(string name)
        {
            SdrImgFile f;
            using (Stream s = SdrSession.UtilGetAssetStream(name))
                f = LoadFromStream(s);
            return f;
        }

        public static SdrImgFile LoadFromStream(Stream src)
        {
            //Read header
            byte[] buffer = new byte[10];
            src.Read(buffer, 0, 10);

            //Verify type
            if (buffer[0] != (byte)'S' || buffer[1] != (byte)'D' || buffer[2] != (byte)'I' || buffer[3] != (byte)'M')
                throw new Exception("This is not an SDR Image File.");

            //Get data
            byte version = buffer[4];
            byte flags = buffer[5];
            ushort width = BitConverter.ToUInt16(buffer, 6);
            ushort height = BitConverter.ToUInt16(buffer, 8);

            //Open image
            SdrImgFile img = new SdrImgFile(width, height);

            //Begin reading channels
            byte[] channelBuffer = new byte[width * height];
            for(int i = 0; i<4; i++)
            {
                //Read length
                long beginningOffset = src.Position;
                src.Read(buffer, 0, 4);
                int length = BitConverter.ToInt32(buffer, 0);

                //Read channel data
                int read;
                using (GZipStream gz = new GZipStream(src, CompressionMode.Decompress, true))
                    read = gz.Read(channelBuffer, 0, width * height);

                //Validate
                if (read != channelBuffer.Length)
                    throw new Exception($"GZIP Decompression did not return required number of bytes; Got {read}, expected {channelBuffer.Length}");

                //Unpack. We can't memcopy because our pixel format MAY not match that of the file
                for(int p = 0; p<channelBuffer.Length; p++)
                {
                    switch(i)
                    {
                        case 0: img.imgBufferPtr[p].a = channelBuffer[p]; break;
                        case 1: img.imgBufferPtr[p].r = channelBuffer[p]; break;
                        case 2: img.imgBufferPtr[p].g = channelBuffer[p]; break;
                        case 3: img.imgBufferPtr[p].b = channelBuffer[p]; break;
                    }
                }

                //Jump to next stream
                src.Position = beginningOffset + 4 + length;
            }

            return img;
        }

        public void Dispose()
        {
            imgBuffer.Dispose();
        }

        public DisplayPixel* GetPointer(int offsetX, int offsetY)
        {
            return imgBufferPtr + (offsetY * width) + offsetX;
        }
    }
}
