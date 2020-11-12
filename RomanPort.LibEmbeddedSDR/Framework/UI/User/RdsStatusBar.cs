using RomanPort.LibEmbeddedSDR.Framework.Display;
using RomanPort.LibEmbeddedSDR.Framework.Rendering;
using RomanPort.LibEmbeddedSDR.Framework.Rendering.Text;
using RomanPort.LibEmbeddedSDR.Framework.UI.Components.Common;
using RomanPort.LibSDR.Demodulators;
using RomanPort.LibSDR.Extras.RDS;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.UI.User
{
    class RdsStatusBar : BaseRenderingView
    {
        private WbFmDemodulator demodulator;
        private RDSClient rds;
        private BaseFontPack font;

        private RectangleView viewBackground;
        private RectangleView viewPsBorder;
        private SinglelineTextView viewPs;
        private SinglelineTextView viewRt;

        private const int PADDING = 15;

        public const int HEIGHT = PADDING + PADDING + 15 + 1;
        
        public RdsStatusBar(IRenderingContext parent, int x, int y, int width, int height, WbFmDemodulator demodulator) : base(parent, x, y, width, height)
        {
            this.demodulator = demodulator;
            rds = demodulator.UseRds();
            rds.OnPsBufferUpdated += Rds_OnPsBufferUpdated;
            rds.OnRtBufferUpdated += Rds_OnRtBufferUpdated;

            font = GetCanvas().GetFontPack(RenderingFontTag.SYSTEM_15);
            int psWidth = font.CalculateWidth(8);
            int minusTop = font.maxOverdrawTop;

            viewBackground = new RectangleView(this, 0, 0, width, height - 1, StaticColors.COLOR_FOREGROUND_BG);
            viewPsBorder = new RectangleView(this, PADDING + psWidth + PADDING, 1, 1, height - 1, StaticColors.COLOR_FOREGROUND_BORDER);
            viewPs = new SinglelineTextView(this, PADDING, PADDING - minusTop, psWidth, 20, font, "        ", new DisplayPixel(255, 255, 255));
            viewRt = new SinglelineTextView(this, PADDING + 1 + PADDING + psWidth + PADDING, PADDING - minusTop, width - (PADDING * 4) - psWidth - 1, 20, font, "", new DisplayPixel(220, 220, 220));
        }

        private void Rds_OnRtBufferUpdated(RDSClient client, char[] buffer)
        {
            viewRt.text = buffer;
            Invalidate();
        }

        private void Rds_OnPsBufferUpdated(RDSClient client, char[] buffer)
        {
            viewPs.text = buffer;
            Invalidate();
        }

        public override void FullRedrawThis()
        {
            //Draw border
            DrawBorder();
        }

        private unsafe void DrawBorder()
        {
            var borderPtr = GetPixelPointer(0, height - 1);
            for (int i = 0; i < width; i++)
                borderPtr[i] = StaticColors.COLOR_FOREGROUND_BORDER;
        }
    }
}
