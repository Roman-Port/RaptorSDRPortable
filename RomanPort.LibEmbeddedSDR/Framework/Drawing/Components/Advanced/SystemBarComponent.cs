using RomanPort.LibEmbeddedSDR.Framework.Drawing.Components.Containers;
using RomanPort.LibEmbeddedSDR.Framework.Drawing.Components.Macros;
using RomanPort.LibEmbeddedSDR.Framework.Drawing.Components.Primitive;
using RomanPort.LibSDR.Components.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Drawing.Components.Advanced
{
    public class SystemBarComponent : HorizontalComponentContainer
    {
        public SystemBarComponent(IDrawingContext parent) : base(parent)
        {
            Height = SYSTEMBAR_HEIGHT;
            Width = parent.Width;
            BackgroundColor = StaticColors.COLOR_FOREGROUND_BG;

            AddChild(new SignalBarsComponent(this), AlignHorizontal.Left);
            snrText = (TextComponent)AddChild(new TextComponent(this, FontStore.SYSTEM_REGULAR_15, "", DisplayPixel.WHITE, AlignHorizontal.Left, AlignVertical.Center), AlignHorizontal.Left, FontStore.SYSTEM_REGULAR_15.width * 5);
            snrText.BackgroundColor = BackgroundColor;

            iconStereo = (SystemBarIconComponent)AddChild(new SystemBarIconComponent(this, "STEREO").SetIconLit(SDR.DemodulatorWbFm.StereoDetected), AlignHorizontal.Right);
            iconRds = (SystemBarIconComponent)AddChild(new SystemBarIconComponent(this, "RDS").SetIconLit(SDR.DemodulatorWbFm.UseRds().IsRdsSynced), AlignHorizontal.Right);

            SDR.OnSnrReading += SDR_OnSnrReading;
            SDR.DemodulatorWbFm.UseRds().OnSyncStateChanged += (bool rds) => iconRds.IconLit = rds;
            SDR.DemodulatorWbFm.OnStereoDetected += (bool stereo) => iconStereo.IconLit = stereo;
        }

        public const int SYSTEMBAR_HEIGHT = 40;

        private TextComponent snrText;
        private SystemBarIconComponent iconStereo;
        private SystemBarIconComponent iconRds;

        private void SDR_OnSnrReading(SnrReading reading)
        {
            snrText.SetText((int)reading.snr + " dB");
        }
    }
}
