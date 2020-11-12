using RomanPort.LibEmbeddedSDR.Framework;
using RomanPort.LibEmbeddedSDR.Framework.Display;
using RomanPort.LibEmbeddedSDR.Framework.Input;
using RomanPort.LibEmbeddedSDR.Framework.Radio;
using RomanPort.LibEmbeddedSDR.Framework.Rendering;
using RomanPort.LibEmbeddedSDR.Framework.Rendering.Backgrounds;
using RomanPort.LibEmbeddedSDR.Framework.UI.Components.FFT;
using RomanPort.LibEmbeddedSDR.Framework.UI.User;
using RomanPort.LibSDR.Framework.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Views
{
    public unsafe class SDRView : IView
    {
        private RadioStatusBar viewStatus;
        private RadioControlBarMini viewControlBar;
        private FftWaterfallView viewFftWaterfall;
        private FftSpectrumView viewFftSpectrum;
        private RdsStatusBar viewRdsStatus;

        private long samplesSinceLastFftFrame;
        private long targetSamplesPerFftFrame;

        public const int FFT_SPECTRUM_HEIGHT = 200;

        public SDRView(SdrSession session) : base(session, new NullBackground())
        {
            //Create views
            viewStatus = new RadioStatusBar(canvas, 0, 0, display.width, RadioStatusBar.STATUS_BAR_HEIGHT, session);
            viewControlBar = new RadioControlBarMini(canvas, 0, 0, display.width, RadioControlBarMini.MINI_BAR_HEIGHT, session);
            viewFftWaterfall = new FftWaterfallView(canvas, 0, 0, display.width, display.height - RadioControlBarMini.MINI_BAR_HEIGHT - RadioStatusBar.STATUS_BAR_HEIGHT - FFT_SPECTRUM_HEIGHT);
            viewFftSpectrum = new FftSpectrumView(canvas, 0, 0, display.width, FFT_SPECTRUM_HEIGHT);
            viewRdsStatus = new RdsStatusBar(canvas, 0, 0, display.width, RdsStatusBar.HEIGHT, session.radio.demodulatorWbFm);

            //Calculate
            UpdateViewLocations();
            targetSamplesPerFftFrame = (long)(radio.sourceSampleRate / 15);

            //Add events
            radio.demodulatorWbFm.UseRds().OnTimeUpdated += SDRView_OnTimeUpdated;
        }

        private void SDRView_OnTimeUpdated(LibSDR.Extras.RDS.RDSClient client, DateTime time, TimeSpan offset)
        {
            session.SetTime(time);
        }

        public void UpdateViewLocations()
        {
            int offsetTop = 0;
            int offsetBottom = 0;

            //Add main status bar
            viewStatus.y = offsetTop;
            offsetTop += viewStatus.height;

            //Add RDS bar
            viewRdsStatus.y = offsetTop;
            offsetTop += viewRdsStatus.height;

            //Add spectrum
            viewFftSpectrum.y = offsetTop;
            offsetTop += viewFftSpectrum.height;

            //Add bottom bar
            viewControlBar.y = display.height - offsetBottom - viewControlBar.height;
            offsetBottom += viewControlBar.height;

            //Add waterfall between
            viewFftWaterfall.height = display.height - offsetTop - offsetBottom;
            viewFftWaterfall.y = offsetTop;
        }
        
        public override void Tick()
        {
            //Process samples
            try
            {
                samplesSinceLastFftFrame += radio.ProcessBlock();
            } catch (RtlDeviceErrorException)
            {
                session.AbortWithError("Radio Disconnected", "There was a communication error with the radio. It was likely removed.");
                return;
            }

            //Process FFT
            if(samplesSinceLastFftFrame > targetSamplesPerFftFrame)
            {
                float* scaledFftFrame = radio.GetScaledFFTFrame();
                viewFftSpectrum.AddScaledFrame(scaledFftFrame);
                viewFftWaterfall.AddScaledFrame(scaledFftFrame);
                canvas.Invalidate();
                samplesSinceLastFftFrame = 0;
            }
        }

        public override void UiTick()
        {
            //Redraw canvas
            canvas.FullRedraw();
        }

        public override void OnUserInput(UserInputKey key)
        {
            canvas.OnUserInput(key);
        }
    }
}
