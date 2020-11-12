using RomanPort.LibEmbeddedSDR.Framework.Rendering.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RomanPort.LibEmbeddedSDR.Tools.FontPackGenerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            charMap = GenerateCharMap();
        }

        public const int PREVIEW_SCALE = 4;

        public const byte ASCII_START = 32;
        public const byte ASCII_END = 127;

        public const int FILE_HEADER_SIZE = SdrFontPack.FILE_HEADER_SIZE;
        public const int FILE_ENTRY_HEADER_SIZE = SdrFontPack.FILE_ENTRY_HEADER_SIZE;

        public const int FONT_PADDING_TOP = 5;
        public const int FONT_PADDING_BOTTOM = 5;
        public const int FONT_PADDING_TOTAL = FONT_PADDING_BOTTOM + FONT_PADDING_TOP;

        public char[] charMap;

        public char[] GenerateCharMap()
        {
            char[] map = new char[ASCII_END - ASCII_START];
            for (byte i = 0; i < map.Length; i++)
                map[i] = (char)(i + ASCII_START);
            return map;
        }

        public Bitmap GenerateFontPackImage(Font font, int fontHeight)
        {
            int offset = (font.Height - fontHeight) / 2;
            Bitmap bmp = new Bitmap(fontHeight * 2, charMap.Length * (fontHeight + FONT_PADDING_TOTAL));
            Graphics g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            for (int i = 0; i<charMap.Length; i++)
                g.DrawString(new string(new char[] { charMap[i] }), font, Brushes.Black, 0, (i * (fontHeight + FONT_PADDING_TOTAL)) + FONT_PADDING_TOP - offset);
            g.Flush();

            return bmp;
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            CreatePackage(15, "C:\\Users\\Roman\\Downloads\\RobotoMono-Regular.ttf", "");
        }

        private void CreatePackage(int fontHeight, string ttfPath, string outputPath)
        {
            //Load fonts from file
            PrivateFontCollection fontCollection = new PrivateFontCollection();
            fontCollection.AddFontFile(ttfPath);
            FontFamily fontFamily = fontCollection.Families[0];
            Font font = new Font(fontFamily, fontHeight);

            //Generate
            Bitmap bmp = GenerateFontPackImage(font, fontHeight);

            //Find the max and min pixels on the X axis where data exists
            int xMax = 0;
            int xMin = int.MaxValue;
            bool foundStart = false;
            for(int x = 0; x<bmp.Width; x++)
            {
                bool dataExists = false;
                for (int y = 0; y < bmp.Height; y++) {
                    var p = bmp.GetPixel(x, y);
                    dataExists = p.A > 0;
                    if (dataExists)
                        break;
                }
                if(dataExists && !foundStart)
                {
                    xMin = x;
                    foundStart = true;
                } else if (dataExists && foundStart)
                {
                    xMax = x;
                }
            }

            //Get font overdraw (how much we draw into the padding) for each
            int[] overdrawTop = new int[charMap.Length];
            int[] overdrawBottom = new int[charMap.Length];
            int maxOverdrawTop = 0;
            int maxOverdrawBottom = 0;
            for(int i = 0; i<charMap.Length; i++)
            {
                //Get offset
                int offset = (i * (fontHeight + FONT_PADDING_TOTAL));

                //Determine top overdraw
                for (int o = FONT_PADDING_TOP - 1; o >= 0; o--)
                {
                    bool hasData = false;
                    for (int x = 0; x < bmp.Width; x++)
                        hasData = hasData || bmp.GetPixel(x, offset + o).A > 0;
                    if(!hasData)
                    {
                        //This is our overdraw
                        overdrawTop[i] = FONT_PADDING_TOP - (o + 1);
                        maxOverdrawTop = Math.Max(maxOverdrawTop, FONT_PADDING_TOP - (o + 1));
                        break;
                    }
                }

                //Determine bottom overdraw
                for (int o = 0; o<FONT_PADDING_BOTTOM; o++)
                {
                    bool hasData = false;
                    for (int x = 0; x < bmp.Width; x++)
                        hasData = hasData || bmp.GetPixel(x, offset + FONT_PADDING_TOP + fontHeight + o).A > 0;
                    if (!hasData)
                    {
                        //This is our overdraw
                        overdrawBottom[i] = o;
                        maxOverdrawBottom = Math.Max(maxOverdrawBottom, o);
                        break;
                    }
                }
            }

            //Calculate real font width and height
            int realFontWidth = xMax - xMin;
            int realFontHeight = fontHeight + maxOverdrawTop + maxOverdrawBottom;

            //Generate font pack. Open buffer
            //4     Sign    Always "SDFT" (SDR Font)
            //1     Byte    Version. Always 1
            //1     Byte    Character count
            //1     Byte    Font width
            //1     Byte    Font Height (includes overdraw)
            //1     Byte    Max overdraw top
            //1     Byte    Max overdrop bottom
            //1     Byte    Font Size
            //1     Byte    Font Style
            //64    Char[]  Font Name
            //   === BEGIN ARRAY OF STRUCT ===
            //1     Byte    ASCII character this entry represents
            //1     Byte    <Reserved>
            //1     Byte    Padding top
            //1     Byte    Padding bottom
            //W*H   Blob    The blob of ALPHA CHANNELS from the pixel. This is image data for the character, but it only uses the alpha channel
            byte[] buffer = new byte[FILE_HEADER_SIZE + (((realFontWidth * realFontHeight) + FILE_ENTRY_HEADER_SIZE) * charMap.Length)];

            //Write file header
            buffer[0] = (byte)'S';
            buffer[1] = (byte)'D';
            buffer[2] = (byte)'F';
            buffer[3] = (byte)'T';
            buffer[4] = 0x01;
            buffer[5] = (byte)charMap.Length;
            buffer[6] = (byte)realFontWidth;
            buffer[7] = (byte)realFontHeight;
            buffer[8] = (byte)maxOverdrawTop;
            buffer[9] = (byte)maxOverdrawBottom;
            buffer[10] = (byte)fontHeight;
            buffer[11] = (byte)0x00;
            char[] fontName = font.Name.ToCharArray();
            Encoding.ASCII.GetBytes(fontName, 0, Math.Min(fontName.Length, 64), buffer, 12);

            //Write each character
            for(int i = 0; i<charMap.Length; i++)
            {
                //Get index
                int index = FILE_HEADER_SIZE + (((realFontWidth * realFontHeight) + FILE_ENTRY_HEADER_SIZE) * i);

                //Write header
                buffer[index + 0] = (byte)charMap[i];
                buffer[index + 1] = 0x00;
                buffer[index + 2] = (byte)overdrawTop[i];
                buffer[index + 3] = (byte)overdrawBottom[i];

                //Copy pixels
                for (int p = 0; p<realFontWidth * realFontHeight; p++)
                {
                    int pX = p % realFontWidth;
                    int pY = p / realFontWidth;
                    int readX = pX + xMin;
                    int readY = FONT_PADDING_TOP + pY + (i * (fontHeight + FONT_PADDING_TOTAL)) - maxOverdrawTop;
                    byte pixel = bmp.GetPixel(readX, readY).A;
                    buffer[index + FILE_ENTRY_HEADER_SIZE + p] = pixel;
                }
            }

            //Test the file by loading it
            SdrFontPack pack = SdrFontPack.LoadFromBytes(buffer);
            Bitmap previewBitmap = new Bitmap(pack.characterCount * pack.width * PREVIEW_SCALE, pack.fullHeight * PREVIEW_SCALE);
            for(int i = 0; i<pack.characterCount; i++)
            {
                var ch = pack.characters[(char)(i + ASCII_START)];
                for (int p = 0; p<pack.fullHeight * pack.width; p++)
                {
                    int x = ((p % pack.width) + (i * pack.width)) * PREVIEW_SCALE;
                    int y = (p / pack.width) * PREVIEW_SCALE;
                    Color c = Color.FromArgb(ch.payload[p], 0, 0, 0);
                    for(int wX = 0; wX < PREVIEW_SCALE; wX++)
                        for(int wY = 0; wY < PREVIEW_SCALE; wY++)
                            previewBitmap.SetPixel(x + wX, y + wY, c);
                }
            }

            //Save
            File.WriteAllBytes("E:\\test.sdrfnt", buffer);
            fontPreview.Height = previewBitmap.Height;
            fontPreview.Image = previewBitmap;
        }
    }
}
