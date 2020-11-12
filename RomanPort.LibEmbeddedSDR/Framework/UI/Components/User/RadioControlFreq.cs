using RomanPort.LibEmbeddedSDR.Framework.Display;
using RomanPort.LibEmbeddedSDR.Framework.Radio;
using RomanPort.LibEmbeddedSDR.Framework.Rendering;
using RomanPort.LibEmbeddedSDR.Framework.Rendering.Text;
using RomanPort.LibSDR.Sources.Hardware;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.UI.Components.User
{
    public class RadioControlFreq : BaseRenderingView
    {
        public RadioControlFreq(IRenderingContext parent, int x, int y, int width, int height, IHardwareSource src) : base(parent, x, y, width, height)
        {
            font = GetCanvas().GetFontPack(RenderingFontTag.SYSTEM_20);
            this.src = src;
        }

        private IHardwareSource src;

        public BaseFontPack font;
        public static readonly DisplayPixel ACTIVE_NUM_COLOR = new DisplayPixel(255, 255, 255);
        public static readonly DisplayPixel DEACTIVE_NUM_COLOR = new DisplayPixel(200, 200, 200);
        public const int NUMBER_PADDING = 16;
        public const int SEGMENT_PADDING = 8;
        public const int BORDER_SIZE_OFFSET = 1;
        public const int PADDING = 1;

        public long freq { get => src.CenterFrequency; set => src.CenterFrequency = value; }

        public override void FullRedrawThis()
        {
            //Render numbers
            for(int i = 0; i<12; i++)
            {
                int value = (int)(freq / ((long)Math.Pow(10, 11 - i)) % 10);
                DisplayPixel color = freq / (long)Math.Pow(10, 11 - i) == 0 ? DEACTIVE_NUM_COLOR : ACTIVE_NUM_COLOR;
                font.RenderCharacter(GetOffsetContext(PADDING + (i * NUMBER_PADDING) + ((i / 3) * SEGMENT_PADDING), PADDING), (char)(value + 48), color.r, color.g, color.b);
            }

            //Render segments
            for(int i = 0; i<3; i++)
            {
                font.RenderCharacter(GetOffsetContext(PADDING + ((i + 1) * 3 * NUMBER_PADDING) + (i * SEGMENT_PADDING) - (SEGMENT_PADDING / 2), PADDING), '.', ACTIVE_NUM_COLOR.r, ACTIVE_NUM_COLOR.g, ACTIVE_NUM_COLOR.b);
            }
        }

        public void DrawNumberOutline(int charIndex)
        {
            int x = PADDING + (charIndex * NUMBER_PADDING) + ((charIndex / 3) * SEGMENT_PADDING) - BORDER_SIZE_OFFSET;
            int y = PADDING - BORDER_SIZE_OFFSET;
            UtilDrawOutlineRectangle(GetOffsetContext(x, y), NUMBER_PADDING + (BORDER_SIZE_OFFSET * 2), font.fullHeight + (BORDER_SIZE_OFFSET * 2), StaticColors.COLOR_SELECTION);
        }

        public void UpdateNumber(int charIndex, int dir)
        {
            freq += dir * (long)Math.Pow(10, 11 - charIndex);
            if (freq > 999999999000)
                freq = 999999999000;
            if (freq < 0)
                freq = 0;
        }
    }
}
