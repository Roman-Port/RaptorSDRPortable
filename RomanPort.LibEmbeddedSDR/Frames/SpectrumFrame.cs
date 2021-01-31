using RomanPort.LibEmbeddedSDR.Framework.Drawing;
using RomanPort.LibEmbeddedSDR.Framework.Drawing.Components.Advanced;
using RomanPort.LibEmbeddedSDR.Framework.Drawing.Components.Containers;
using RomanPort.LibEmbeddedSDR.Framework.Drawing.Components.Primitive;
using RomanPort.LibEmbeddedSDR.Framework.Drawing.Frames;
using RomanPort.LibSDR.Components.FFT.Mutators;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Frames
{
    public class SpectrumFrame : VerticalComponentContainer, ISdrFrame
    {
        public SpectrumFrame(IDrawingContext ctx) : base(ctx)
        {
            Height = ctx.Height;
            Width = ctx.Width;

            //Get FFTs
            mainFft = new FFTResizer(SDR.MainFFT, Width);
            mpxFft = new FFTResizer(SDR.MpxFFT, Width);

            //Create components
            systemBar = new SystemBarComponent(this);
            rdsBar = new RdsBarComponent(this);
            fftMainSpectrum = new FftSpectrumComponent(this);
            fftMainWaterfall = new FftWaterfallComponent(this);
            fftMpxSpectrum = new FftSpectrumComponent(this);
            miniControlBar = new RadioMiniControlBarComponent(this);

            //Add components
            AddChild(systemBar, AlignVertical.Top);
            AddChild(rdsBar, AlignVertical.Top);
            AddChild(fftMainSpectrum, AlignVertical.Top, 200);
            AddChild(fftMainWaterfall, AlignVertical.Center, 40);
            AddChild(miniControlBar, AlignVertical.Bottom);
            AddChild(fftMpxSpectrum, AlignVertical.Bottom, 100);

            OnSelected(1);
        }

        private SystemBarComponent systemBar;
        private RdsBarComponent rdsBar;
        private FftSpectrumComponent fftMainSpectrum;
        private FftWaterfallComponent fftMainWaterfall;
        private FftSpectrumComponent fftMpxSpectrum;
        private RadioMiniControlBarComponent miniControlBar;

        private FFTResizer mainFft;
        private FFTResizer mpxFft;

        public bool IsSemiTransparent => false;

        public void OnClosed()
        {
            Hidden = true;
        }

        public void OnOpened()
        {
            Hidden = false;
        }

        protected override void LayoutView()
        {
            base.LayoutView();
            mainFft.OutputSize = Width;
            mpxFft.OutputSize = Width;
        }

        public unsafe void Tick(bool forceRedraw)
        {
            //Get FFT frames
            float* mainFftFrame = mainFft.ProcessFFT(out int fftWidth);
            float* mpxFftFrame = mpxFft.ProcessFFT(out int mpxFftWidth);

            //Add to FFTs
            fftMainSpectrum.AddScaledFrame(mainFftFrame, fftWidth);
            fftMainWaterfall.AddScaledFrame(mainFftFrame, fftWidth);
            fftMpxSpectrum.AddScaledFrame(mpxFftFrame, mpxFftWidth);

            //Draw
            DrawingTick(forceRedraw);
        }
    }
}
