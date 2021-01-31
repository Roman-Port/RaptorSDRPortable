using RomanPort.LibEmbeddedSDR.Framework.Drawing.Fonts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomanPort.LibEmbeddedSDR.Tools.FontPackGenerator
{
    class Program
    {
        public const byte ASCII_START = 32;
        public const byte ASCII_END = 127;

        static void Main(string[] args)
        {
            ProcessPack(10, "RobotoMono-Regular.ttf", "SYSTEM_REGULAR_10.sdrf");
            ProcessPack(15, "RobotoMono-Regular.ttf", "SYSTEM_REGULAR_15.sdrf");
            ProcessPack(20, "RobotoMono-Regular.ttf", "SYSTEM_REGULAR_20.sdrf");
            ProcessPack(10, "RobotoMono-Bold.ttf", "SYSTEM_BOLD_10.sdrf");
            ProcessPack(15, "RobotoMono-Bold.ttf", "SYSTEM_BOLD_15.sdrf");
            ProcessPack(20, "RobotoMono-Bold.ttf", "SYSTEM_BOLD_20.sdrf");
        }

        static void ProcessPack(int height, string fontName, string outputName)
        {
            //Load font
            PrivateFontCollection fontCollection = new PrivateFontCollection();
            fontCollection.AddFontFile(fontName);
            FontFamily fontFamily = fontCollection.Families[0];
            Font font = new Font(fontFamily, height, FontStyle.Regular, GraphicsUnit.Pixel, 0, false);

            //Render all
            Bitmap[] chars = new Bitmap[ASCII_END - ASCII_START];
            for (int i = 0; i < chars.Length; i++)
                chars[i] = GenerateCharacter(font, (char)(i + ASCII_START), height);

            //Measure font
            int actualHeight = 0;
            int actualWidth = 0;
            int offsetX = int.MaxValue;
            int offsetY = int.MaxValue;
            for (int i = 0; i < chars.Length; i++)
            {
                int top = TestTopBounds(chars[i]);
                int bottom = TestBottomBounds(chars[i]);
                int left = TestLeftBounds(chars[i]);
                int right = TestRightBounds(chars[i]);
                if (left == -1 || right == -1 || top == -1 || bottom == -1)
                    continue;
                actualHeight = Math.Max(actualHeight, bottom - top);
                actualWidth = Math.Max(actualWidth, right - left);
                offsetX = Math.Min(offsetX, left);
                offsetY = Math.Min(offsetY, top);
            }
            offsetY += (actualHeight - height) / 2;
            actualHeight = height;

            //Begin converting
            FileStream output = new FileStream(@"C:\Users\Roman\source\repos\RomanPort.EmbeddedSDR\RomanPort.LibEmbeddedSDR\Assets\" + outputName, FileMode.Create);
            byte[] buffer = new byte[Math.Max(actualHeight * actualWidth, Math.Max(SdrFontPack.CHAR_HEADER_SIZE, SdrFontPack.HEADER_SIZE))];

            //Create header
            buffer[0] = (byte)'S';
            buffer[1] = (byte)'D';
            buffer[2] = (byte)'R';
            buffer[3] = (byte)'F';
            buffer[4] = 0;
            buffer[5] = (byte)chars.Length;
            buffer[6] = (byte)actualWidth;
            buffer[7] = (byte)actualHeight;
            output.Write(buffer, 0, SdrFontPack.HEADER_SIZE);

            //Write characters
            for (int i = 0; i < chars.Length; i++)
            {
                //Write header
                buffer[0] = (byte)(i + ASCII_START);
                buffer[1] = 0;
                output.Write(buffer, 0, SdrFontPack.CHAR_HEADER_SIZE);

                //Convert content
                int bufferIndex = 0;
                double max = 1;
                for (int y = 0; y < actualHeight; y++)
                {
                    for (int x = 0; x < actualWidth; x++)
                    {
                        buffer[bufferIndex] = chars[i].GetPixel(x + offsetX, y + offsetY).A;
                        max = Math.Max(max, buffer[bufferIndex]);
                        bufferIndex++;
                    }
                }

                //Normalize content
                for (int j = 0; j < buffer.Length; j++)
                    buffer[j] = (byte)Math.Min(byte.MaxValue, byte.MaxValue * (buffer[j] / max));

                //Write content
                output.Write(buffer, 0, actualHeight * actualWidth);
            }

            //Clean up
            output.Flush();
            output.Close();
            output.Dispose();
            for (int i = 0; i < chars.Length; i++)
                chars[i].Dispose();
            font.Dispose();
            fontFamily.Dispose();
            fontCollection.Dispose();
        }

        static int TestTopBounds(Bitmap c)
        {
            int y;
            for(y = 0; y<c.Height; y += 1)
            {
                for (int x = 0; x < c.Width; x++)
                {
                    if (c.GetPixel(x, y).A != 0)
                        return y - 1;
                }
            }
            return -1;
        }

        static int TestBottomBounds(Bitmap c)
        {
            int y;
            for (y = c.Height - 1; y > 0; y -= 1)
            {
                for (int x = 0; x < c.Width; x++)
                {
                    if (c.GetPixel(x, y).A != 0)
                        return y + 1;
                }
            }
            return -1;
        }

        static int TestLeftBounds(Bitmap c)
        {
            int x;
            for (x = 0; x < c.Width; x += 1)
            {
                for (int y = 0; y < c.Height; y++)
                {
                    if (c.GetPixel(x, y).A != 0)
                        return x - 1;
                }
            }
            return -1;
        }

        static int TestRightBounds(Bitmap c)
        {
            int x;
            for (x = c.Width - 1; x > 0; x -= 1)
            {
                for (int y = 0; y < c.Height; y++)
                {
                    if (c.GetPixel(x, y).A != 0)
                        return x + 1;
                }
            }
            return -1;
        }

        static Bitmap GenerateCharacter(Font font, char c, int height)
        {
            Bitmap bmp = new Bitmap(height * 4, height * 4);
            Graphics g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            g.DrawString(new string(new char[] { c }), font, Brushes.Black, height, height);
            g.Flush();
            return bmp;
        }
    }
}
