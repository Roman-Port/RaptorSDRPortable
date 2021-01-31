using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Drawing.Fonts
{
    public class SdrFontPack
    {
        public byte width;
        public byte height;
        public Dictionary<char, byte[]> font;

        public const int HEADER_SIZE = 8;
        public const int CHAR_HEADER_SIZE = 2;

        public SdrFontPack(byte width, byte height)
        {
            this.width = width;
            this.height = height;
            font = new Dictionary<char, byte[]>();
        }

        public static SdrFontPack FromResource(string id)
        {
            SdrFontPack pack;
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("RomanPort.LibEmbeddedSDR.Assets." + id + ".sdrf"))
                pack = FromStream(stream);
            return pack;
        }

        public static SdrFontPack FromStream(Stream s)
        {
            //Load header
            byte[] header = new byte[Math.Max(HEADER_SIZE, CHAR_HEADER_SIZE)];
            s.Read(header, 0, HEADER_SIZE);

            //Check magic
            if (header[0] != 'S' || header[1] != 'D' || header[2] != 'R' || header[3] != 'F')
                throw new Exception("Invalid file");

            //Read header parts
            byte version = header[4];
            byte count = header[5];
            byte width = header[6];
            byte height = header[7];

            //Create
            SdrFontPack pack = new SdrFontPack(width, height);

            //Load all fonts
            for(int i = 0; i<count; i++)
            {
                //Load font header
                s.Read(header, 0, CHAR_HEADER_SIZE);
                byte charCode = header[0];
                byte charFlags = header[1];

                //Read font data
                byte[] buffer = new byte[width * height];
                s.Read(buffer, 0, buffer.Length);
                pack.font.Add((char)charCode, buffer);
            }

            return pack;
        }

        public SdrFontPack MakeNarrowPack(float hScale)
        {
            //Make pack
            SdrFontPack pack = new SdrFontPack((byte)(width * hScale), height);

            //Convert characters
            foreach(var c in font)
            {
                byte[] data = new byte[pack.height * pack.width];
                for(int y = 0; y<pack.height; y++)
                {
                    int srcY = y;
                    for (int x = 0; x<pack.width; x++)
                    {
                        int srcX = (int)(x / hScale);
                        data[x + (y * pack.width)] = Math.Max(c.Value[srcX + (srcY * width)], c.Value[srcX + (srcY * width) + 1]);
                    }
                }
                pack.font.Add(c.Key, data);
            }

            return pack;
        }

        public unsafe void RenderCharacter(IDrawingContext ctx, char c, DisplayPixel color)
        {
            //Make sure we have this
            if (!font.ContainsKey(c))
                return;

            //Get lines
            byte[] data = font[c];

            //Copy
            for(int y = 0; y<height; y++)
            {
                //Get pointer and offset
                DisplayPixel* line = ctx.GetPixelPointer(0, y);
                int offset = width * y;
                
                //Transfer
                for(int x = 0; x<width; x++)
                {
                    //Cheap out
                    if (data[offset + x] == byte.MaxValue)
                        line[x] = color;
                    else if (data[offset + x] == 0)
                        continue;

                    //Mix
                    DisplayPixel.Mix(&color, line + x, data[offset + x] / 255f, line + x);
                }
            }
        }

        public void RenderLine(IDrawingContext ctx, char[] text, DisplayPixel color)
        {
            for (int i = 0; i < text.Length; i += 1)
                RenderCharacter(ctx.GetOffsetContext(i * width, 0), text[i], color);
        }

        public void RenderPretty(IDrawingContext ctx, char[][] text, DisplayPixel color, AlignHorizontal horizTextAlign, AlignVertical vertTextAlign, int boundsWidth, int boundsHeight)
        {
            //Determine Y offset
            int offsetY;
            switch(vertTextAlign)
            {
                case AlignVertical.Top: offsetY = 0; break;
                case AlignVertical.Bottom: offsetY = boundsHeight - (text.Length * height); break;
                case AlignVertical.Center: offsetY = (boundsHeight - (text.Length * height)) / 2; break;
                default: throw new Exception("Unknown alignment.");
            }

            //Draw
            for(int ln = 0; ln < text.Length; ln++)
            {
                //Calculate X offset
                int offsetX;
                switch (horizTextAlign)
                {
                    case AlignHorizontal.Left: offsetX = 0; break;
                    case AlignHorizontal.Right: offsetX = boundsWidth - (text[ln].Length * width); break;
                    case AlignHorizontal.Center: offsetX = (boundsWidth - (text[ln].Length * width)) / 2; break;
                    default: throw new Exception("Unknown alignment.");
                }

                //Write
                RenderLine(ctx.GetOffsetContext(offsetX, offsetY + (height * ln)), text[ln], color);
            }
        }

        public void RenderPretty(IDrawingContext ctx, char[] text, DisplayPixel color, AlignHorizontal horizTextAlign, AlignVertical vertTextAlign, int boundsWidth, int boundsHeight)
        {
            char[][] lines = SplitLines(text, boundsWidth, boundsHeight);
            RenderPretty(ctx, lines, color, horizTextAlign, vertTextAlign, boundsWidth, boundsHeight);
        }

        public char[][] SplitLines(char[] text, int boundsWidth, int boundsHeight)
        {
            int maxLines = boundsHeight / height;
            int maxCols = boundsWidth / width;
            List<char[]> lines = new List<char[]>();
            int offset = 0;
            while(offset < text.Length && lines.Count < maxLines)
            {
                int startIndex = offset;
                int maxReadable = Math.Min(maxCols, text.Length - offset);
                int lastSplit = maxReadable;
                /*for(int i = 1; i<maxReadable; i++)
                {
                    if (text[startIndex + i] == ' ')
                        lastSplit = i;
                }*/
                offset += lastSplit;
                char[] output = new char[lastSplit];
                Array.Copy(text, startIndex, output, 0, lastSplit);
                lines.Add(output);
            }
            return lines.ToArray();
        }

        private char[] FindLineSplit(char[] text, int offset, int maxWidth, out int read)
        {
            int lastSplit = maxWidth;
            int startIndex = offset;
            read = 0;
            for(int i = 0; i<maxWidth && offset < text.Length; i++)
            {
                if (text[offset] == ' ')
                    lastSplit = i;
                offset++;
            }
            char[] output = new char[lastSplit];
            Array.Copy(text, startIndex, output, 0, lastSplit);
            read = lastSplit;
            return output;
        }
    }
}
