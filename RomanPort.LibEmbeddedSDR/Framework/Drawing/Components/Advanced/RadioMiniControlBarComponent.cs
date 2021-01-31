using RomanPort.LibEmbeddedSDR.Frames;
using RomanPort.LibEmbeddedSDR.Framework.Drawing.Components.Containers;
using RomanPort.LibEmbeddedSDR.Framework.Drawing.Components.Macros;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Drawing.Components.Advanced
{
    public class RadioMiniControlBarComponent : HorizontalComponentContainer
    {
        public RadioMiniControlBarComponent(IDrawingContext ctx) : base(ctx)
        {
            BackgroundColor = StaticColors.COLOR_FOREGROUND_BG;
            Height = 70;
            Padding = 10;
            IsMenu = true;

            //Make parts
            freqControl = AddLabeledComponent("Tuning", new LongNumberSelectorComponent(this, 3 * 4, SDR.CenterFrequency))
                .SetMaxValue(int.MaxValue)
                .SetMinValue(10)
                .SetValueChangeEvent((long freq) => SDR.CenterFrequency = freq);
            bandwidthControl = AddLabeledComponent("Bandwidth", new LongNumberSelectorComponent(this, 3 * 2, (long)SDR.Bandwidth))
                .SetMaxValue(250000)
                .SetMinValue(1000)
                .SetValueChangeEvent((long bandwidth) => SDR.Bandwidth = bandwidth);
            demodControl = AddLabeledComponent("Mode", new DropdownControlComponent(this, (int)SDR.DemodMode, "WbFm", "AM"))
                .SetValueChangeEvent((int mode) => SDR.DemodMode = (SdrDemodMode)mode);
            AddChild(new ControlBarButtonComponent(this, "MORE", () => MenuFrame.ShowMenu()), AlignHorizontal.Right);
        }

        private T AddLabeledComponent<T>(string label, T component) where T : BaseRenderComponent
        {
            AddChild(new LabeledControlComponent(this, label, component), AlignHorizontal.Left);
            return component;
        }

        private LongNumberSelectorComponent freqControl;
        private LongNumberSelectorComponent bandwidthControl;
        private DropdownControlComponent demodControl;
    }
}
