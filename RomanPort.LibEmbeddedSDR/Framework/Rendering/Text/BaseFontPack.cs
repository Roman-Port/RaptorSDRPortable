using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Rendering.Text
{
    public abstract class BaseFontPack
    {
        public string name;
        public int fullHeight; //Includes overdraw. Also format of all payloads
        public int height;
        public int width;
        public int characterCount;
        public int maxOverdrawTop;
        public int maxOverdrawBottom;

        public abstract void RenderCharacter(IDrawingContext ctx, char character, byte r, byte g, byte b);

        public int CalculateWidth(int charCount, int characterSpacing = -1)
        {
            return charCount * (characterSpacing + width);
        }

        public void RenderNullCharacter(IDrawingContext ctx, byte r, byte g, byte b)
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < fullHeight; y++)
                    ctx.WritePixel(x, y, new Display.DisplayPixel(r, g, b, byte.MaxValue));
        }

        public void RenderLinearString(IDrawingContext ctx, int characterSpacing, string data, byte r, byte g, byte b)
        {
            char[] d = data.ToCharArray();
            RenderLinearString(ctx, characterSpacing, d, d.Length, r, g, b);
        }

        public void RenderLinearString(IDrawingContext ctx, int characterSpacing, char[] data, int charactersToDraw, byte r, byte g, byte b)
        {
            for (int i = 0; i < charactersToDraw; i++)
            {
                RenderCharacter(ctx.GetOffsetContext(i * (width + characterSpacing), 0), data[i], r, g, b);
            }
        }

        public void RenderMultilineString(IDrawingContext ctx, int characterSpacing, int lineSpacing, int width, string data, byte r, byte g, byte b)
        {
            RenderMultilineString(ctx, characterSpacing, lineSpacing, width, data.ToCharArray(), r, g, b);
        }

        public void RenderMultilineString(IDrawingContext ctx, int characterSpacing, int lineSpacing, int boxWidth, char[] data, byte r, byte g, byte b)
        {
            //Split by where it logically makes sense (spaces, for example)
            List<char[]> lines = new List<char[]>();
            char[] lineBuffer = new char[boxWidth / (width + characterSpacing)];
            int lineBufferIndex = 0;
            int lastValidSplit = -1;
            for (int i = 0; i < data.Length; i++)
            {
                //Update logical split
                if (data[i] == ' ')
                    lastValidSplit = lineBufferIndex;

                //Copy to buffer
                lineBuffer[lineBufferIndex] = data[i];
                lineBufferIndex++;

                //Check if we need to split
                if (lineBufferIndex == lineBuffer.Length)
                {
                    //Check if we have a last logical split
                    if (lastValidSplit != -1)
                    {
                        //We have a logical split!
                        //Clear out this line buffer from the split to here
                        for (int j = lastValidSplit + 1; j < lineBufferIndex; j++)
                            lineBuffer[j] = (char)0x00;

                        //Back up the reading index to this so we continue on the next line
                        i -= lineBufferIndex - lastValidSplit - 1;
                    }

                    //Write and reset
                    lastValidSplit = -1;
                    lineBufferIndex = 0;
                    lines.Add(lineBuffer);
                    lineBuffer = new char[lineBuffer.Length];
                }
            }

            //Add the last line, if needed
            if (lineBufferIndex != 0)
                lines.Add(lineBuffer);

            //We now have our lines. We'll write these out
            for (int i = 0; i < lines.Count; i++)
                RenderLinearString(ctx.GetOffsetContext(0, i * (height + lineSpacing)), characterSpacing, lines[i], lines[i].Length, r, g, b);
        }
    }
}
