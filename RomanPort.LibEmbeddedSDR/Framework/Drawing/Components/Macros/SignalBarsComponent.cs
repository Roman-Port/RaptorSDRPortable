using RomanPort.LibSDR.Components.Misc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace RomanPort.LibEmbeddedSDR.Framework.Drawing.Components.Macros
{
    public class SignalBarsComponent : BaseRenderComponent
    {
        public SignalBarsComponent(IDrawingContext parent) : base(parent)
        {
            //Precompute heights
            SIGNAL_BAR_HEIGHTS_PRECOMPUTED = new int[SIGNAL_BARS_COUNT];
            SIGNAL_BAR_OFFSETS_PRECOMPUTED = new int[SIGNAL_BARS_COUNT];
            for (int i = 0; i < SIGNAL_BARS_COUNT; i++)
            {
                double height = SIGNAL_BARS_MIN_HEIGHT + (Math.Pow((double)(i + SIGNAL_BARS_POW_OFFSET) / (SIGNAL_BARS_COUNT + SIGNAL_BARS_POW_OFFSET - 1), 2) * (STATUS_BAR_HEIGHT - PADDING_FULL - SIGNAL_BARS_MIN_HEIGHT));
                SIGNAL_BAR_HEIGHTS_PRECOMPUTED[i] = (int)height;
                SIGNAL_BAR_OFFSETS_PRECOMPUTED[i] = STATUS_BAR_HEIGHT - (int)height - PADDING;
            }

            //Bind
            SDR.OnSnrReading += SDR_OnSnrReading;

            //Set up
            Height = STATUS_BAR_HEIGHT;
            Width = SIGNAL_BARS_TOTAL_WIDTH + PADDING_FULL;
            BackgroundColor = StaticColors.COLOR_FOREGROUND_BG;
        }

        private int signalBarsLit;

        private const int STATUS_BAR_HEIGHT = 40;
        private const int PADDING = 10;
        private const int PADDING_FULL = PADDING + PADDING;

        private const int SIGNAL_BARS_COUNT = 5;
        private const int SIGNAL_BARS_WIDTH = 5;
        private const int SIGNAL_BARS_MARGIN = 1;
        private const int SIGNAL_BARS_SPACING = SIGNAL_BARS_MARGIN + SIGNAL_BARS_WIDTH;
        private const int SIGNAL_BARS_TOTAL_WIDTH = (SIGNAL_BARS_WIDTH + SIGNAL_BARS_MARGIN) * SIGNAL_BARS_COUNT;
        private const int SIGNAL_BARS_UNLIT_HEIGHT = 2;
        private const int SIGNAL_BARS_UNLIT_OFFSET = STATUS_BAR_HEIGHT - PADDING - SIGNAL_BARS_UNLIT_HEIGHT;
        private const int SIGNAL_BARS_MIN_HEIGHT = 4;
        private const int SIGNAL_BARS_POW_OFFSET = 1;

        private const float GOOD_SNR = 45;
        private const float BAD_SNR = 8;
        private const float SNR_RATING_RANGE = GOOD_SNR - BAD_SNR;

        private static int[] SIGNAL_BAR_HEIGHTS_PRECOMPUTED;
        private static int[] SIGNAL_BAR_OFFSETS_PRECOMPUTED;

        protected override void LayoutView()
        {
            
        }

        protected override void RedrawView()
        {
            //Draw filled bars
            for (int i = 0; i < Math.Min(signalBarsLit, SIGNAL_BARS_COUNT); i++)
            {
                UtilFill(DisplayPixel.WHITE,
                    PADDING + (i * SIGNAL_BARS_SPACING), SIGNAL_BAR_OFFSETS_PRECOMPUTED[i],
                    SIGNAL_BARS_WIDTH, SIGNAL_BAR_HEIGHTS_PRECOMPUTED[i]);
            }

            //Draw unfilled bars
            for (int i = signalBarsLit; i < SIGNAL_BARS_COUNT; i++)
            {
                UtilFill(new DisplayPixel(180),
                    PADDING + (i * SIGNAL_BARS_SPACING), SIGNAL_BARS_UNLIT_OFFSET,
                    SIGNAL_BARS_WIDTH, SIGNAL_BARS_UNLIT_HEIGHT);
            }
        }

        private void SDR_OnSnrReading(SnrReading reading)
        {
            //Determine the number of bars lit
            signalBarsLit = (int)((((reading.snr - BAD_SNR) / SNR_RATING_RANGE) * SIGNAL_BARS_COUNT) + 0.5f);

            //Invalidate
            Invalidate();
        }
    }
}
