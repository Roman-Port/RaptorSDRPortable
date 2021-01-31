using RomanPort.LibEmbeddedSDR.Framework.Drawing;
using RomanPort.LibEmbeddedSDR.Framework.Drawing.Components.Containers;
using RomanPort.LibEmbeddedSDR.Framework.Drawing.Components.Primitive;
using RomanPort.LibEmbeddedSDR.Framework.Drawing.Frames;
using RomanPort.LibSDR.Sources.Hardware;
using RomanPort.LibSDR.Sources.Hardware.RTLSDR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RomanPort.LibEmbeddedSDR.Frames
{
    public class StartupFrame : CenteringComponentContainer, ISdrFrame
    {
        public StartupFrame(IDrawingContext ctx) : base(ctx)
        {
            Height = ctx.Height;
            Width = ctx.Width;

            BackgroundColor = DisplayPixel.BLACK;

            text = new TextComponent(ctx, FontStore.SYSTEM_REGULAR_15, GetTextString(), DisplayPixel.WHITE, AlignHorizontal.Center, AlignVertical.Center);
            text.BackgroundColor = DisplayPixel.BLACK;
            AddChild(text, Width, 100);

            worker = new Thread(Work);
            worker.Name = "Startup Worker";
            worker.IsBackground = true;
            worker.Start();
        }

        private TextComponent text;
        private int frameCount;
        private Thread worker;

        private static readonly char[] SPINNER = new char[] { '|', '/', '-', '\\' };

        public bool IsSemiTransparent => false;

        public void OnClosed()
        {
            Hidden = true;
        }

        public void OnOpened()
        {
            Hidden = false;
        }

        public void Tick(bool forceRedraw)
        {
            text.SetText(GetTextString());
            frameCount++;
            DrawingTick(forceRedraw);
        }

        private string GetTextString()
        {
            return $"[{SPINNER[(frameCount / 2) % SPINNER.Length]}] RaptorSDR v{SDR.VERSION_MAJOR}.{SDR.VERSION_MINOR}";
        }

        private void Work()
        {
            //Open and set
            try
            {
                SDR.Source = new RtlSdrSource(0, 900001);
            } catch (HardwareNotFoundException)
            {
                SDR.ShowFatalError("Radio Not Connected", "Make sure there is a radio connected and restart.");
                return;
            } catch (Exception ex)
            {
                SDR.ShowFatalError("Unknown Radio Opening Error", ex);
                return;
            }

            //Configure
            SDR.CenterFrequency = 93700000;
            SDR.ManualGainLevel = 20;
            SDR.SwitchActiveFrame(new SpectrumFrame(SDR.Display));
        }
    }
}
