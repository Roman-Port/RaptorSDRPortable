using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Rendering.Text
{
    public class SdrFontPack : BaseFontPack
    {
        public const int FILE_HEADER_SIZE = 76;
        public const int FILE_ENTRY_HEADER_SIZE = 4;

        public Dictionary<char, SdrFontPackCharacter> characters;

        public static SdrFontPack LoadFromBytes(byte[] data)
        {
            //Verify type
            if (data[0] != (byte)'S' || data[1] != (byte)'D' || data[2] != (byte)'F' || data[3] != (byte)'T')
                throw new Exception("This is not an SDR Font File. You need to convert fonts to this special format.");

            //Read in header
            byte version = data[4];
            byte charCount = data[5];
            byte realWidth = data[6];
            byte realHeight = data[7];
            byte maxOverdrawTop = data[8];
            byte maxOverdrawBottom = data[9];
            byte height = data[10];
            byte style = data[11];
            string name = Encoding.ASCII.GetString(data, 12, 64);

            //Create pack
            SdrFontPack pack = new SdrFontPack
            {
                characters = new Dictionary<char, SdrFontPackCharacter>(),
                fullHeight = realHeight,
                height = height,
                width = realWidth,
                characterCount = charCount,
                maxOverdrawBottom = maxOverdrawBottom,
                maxOverdrawTop = maxOverdrawTop,
                name = name
            };

            //Read in characters
            for(int i = 0; i<charCount; i++)
            {
                //Get index
                int index = FILE_HEADER_SIZE + (((realWidth * realHeight) + FILE_ENTRY_HEADER_SIZE) * i);

                //Read header
                char character = (char)data[index + 0];
                byte overdrawTop = data[index + 2];
                byte overdrawBottom = data[index + 3];

                //Read payload
                byte[] payload = new byte[realWidth * realHeight];
                Array.Copy(data, index + FILE_ENTRY_HEADER_SIZE, payload, 0, payload.Length);

                //Set
                pack.characters.Add(character, new SdrFontPackCharacter
                {
                    character = character,
                    overdrawTop = overdrawTop,
                    overdrawBottom = overdrawBottom,
                    payload = payload
                });
            }

            return pack;
        }

        public override void RenderCharacter(IDrawingContext ctx, char character, byte r, byte g, byte b)
        {
            //Try to find character data
            if(character == (char)0x00)
            {
                //Ignore. This is a null character
            } else if (characters.ContainsKey(character))
            {
                //Copy pixel data
                var cha = characters[character];
                for (int i = 0; i < width * fullHeight; i++)
                {
                    ctx.WritePixel(i % width, i / width, new Display.DisplayPixel(r, g, b, cha.payload[i]));
                }
            }
            else
            {
                //Write a blank spot, as this character isn't found
                RenderNullCharacter(ctx, r, g, b);
            }
        }
    }
}
