using RomanPort.LibEmbeddedSDR.Framework.Drawing.Components.Containers;
using RomanPort.LibEmbeddedSDR.Framework.Drawing.Components.Primitive;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Drawing.Components.Advanced
{
    public class RdsBarComponent : HorizontalComponentContainer
    {
        public RdsBarComponent(IDrawingContext ctx) : base(ctx)
        {
            psDisplay = new TextComponent(this, FontStore.SYSTEM_REGULAR_15, "", DisplayPixel.WHITE)
                .SetBackgroundColor(new DisplayPixel(21, 21, 23))
                .SetTextColor(DisplayPixel.WHITE)
                .SetTextHorizontalAlign(AlignHorizontal.Center)
                .SetTextVerticalAlign(AlignVertical.Center);
            rtDisplay = new TextComponent(this, FontStore.SYSTEM_COMPACT_15, "", DisplayPixel.WHITE)
                .SetTextColor(DisplayPixel.WHITE)
                .SetBackgroundColor(DisplayPixel.BLACK)
                .SetTextHorizontalAlign(AlignHorizontal.Left)
                .SetTextVerticalAlign(AlignVertical.Center);
            rtDisplay.PaddingLeft = H_PADDING;
            rtDisplay.PaddingRight = H_PADDING;

            AddChild(psDisplay, AlignHorizontal.Left, (FontStore.SYSTEM_REGULAR_15.width * 8) + H_PADDING_FULL);
            AddChild(rtDisplay, AlignHorizontal.Center, 0);

            SDR.DemodulatorWbFm.UseRds().OnPsBufferUpdated += RdsBarComponent_OnPsBufferUpdated;
            SDR.DemodulatorWbFm.UseRds().OnRtBufferUpdated += RdsBarComponent_OnRtBufferUpdated;

            Height = 40;
            BackgroundColor = DisplayPixel.BLACK;
        }

        private const int H_PADDING = 16;
        private const int H_PADDING_FULL = H_PADDING * 2;

        private TextComponent psDisplay;
        private TextComponent rtDisplay;

        private void RdsBarComponent_OnRtBufferUpdated(LibSDR.Components.Digital.RDS.RDSClient client, char[] buffer)
        {
            rtDisplay.SetText(buffer);
        }

        private void RdsBarComponent_OnPsBufferUpdated(LibSDR.Components.Digital.RDS.RDSClient client, char[] buffer)
        {
            psDisplay.SetText(buffer);
        }
    }
}
