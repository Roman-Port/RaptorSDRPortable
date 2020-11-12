using RomanPort.LibEmbeddedSDR.Framework.Display;
using RomanPort.LibEmbeddedSDR.Framework.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.UI.User
{
    public class RadioStatusBar : BaseRenderingView
    {
        private SdrSession session;
        
        public const int STATUS_BAR_HEIGHT = 40;
        public const int PADDING = 10;
        public const int PADDING_FULL = PADDING + PADDING;

        public const int SIGNAL_BARS_COUNT = 5;
        public const int SIGNAL_BARS_WIDTH = 6;
        public const int SIGNAL_BARS_MARGIN = 1;

        public float snr = 34;
        
        public RadioStatusBar(IRenderingContext parent, int x, int y, int width, int height, SdrSession session) : base(parent, x, y, width, height)
        {
            this.session = session;
            session.OnSyncTimeUpdated += Session_OnSyncTimeUpdated;
        }

        private void Session_OnSyncTimeUpdated(DateTime time)
        {
            Invalidate();
        }

        public override void FullRedrawThis()
        {
            //Draw background
            UtilDrawFilledRectangle(this, width, height, StaticColors.COLOR_FOREGROUND_BG);

            //Render parts
            DrawBars();
            DrawTime();
            DrawBorder();
        }

        private void DrawBars()
        {
            float height = this.height - PADDING_FULL;
            float factor = 1f - (1f / (SIGNAL_BARS_COUNT - 1));
            for (int i = 0; i < SIGNAL_BARS_COUNT; i++)
            {
                int drawHeight = (int)height;
                UtilDrawFilledRectangle(GetOffsetContext(PADDING + (i * (SIGNAL_BARS_WIDTH + SIGNAL_BARS_MARGIN)), PADDING + ((this.height - PADDING_FULL) - drawHeight)), SIGNAL_BARS_WIDTH, drawHeight, new DisplayPixel(byte.MaxValue, byte.MaxValue, byte.MaxValue));
                height *= factor;
            }
        }

        private void DrawTime()
        {
            string timeText;
            if (session.TryGetTime(out DateTime time))
                timeText = $"{time.Day.ToString().PadLeft(2, '0')}/{time.Month.ToString().PadLeft(2, '0')}/{time.Year} {(((time.Hour - 1) % 12) + 1).ToString().PadLeft(2, '0')}:{time.Minute.ToString().PadLeft(2, '0')} {(time.Hour >= 12 ? "PM" : "AM")}";
            else
                timeText = "--:--";
            var font = GetCanvas().GetFontPack(RenderingFontTag.SYSTEM_15);
            font.RenderLinearString(GetOffsetContext(width - font.CalculateWidth(timeText.Length, -1) - PADDING, PADDING), -1, timeText, 255, 255, 255);
        }

        private unsafe void DrawBorder()
        {
            var borderPtr = GetPixelPointer(0, height - 1);
            for (int i = 0; i < width; i++)
                borderPtr[i] = StaticColors.COLOR_FOREGROUND_BORDER;
        }
    }
}
