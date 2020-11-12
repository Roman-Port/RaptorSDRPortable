using RomanPort.LibEmbeddedSDR.Framework.Display;
using RomanPort.LibEmbeddedSDR.Framework.Rendering;
using RomanPort.LibSDR.Framework.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.UI.Components.FFT
{
    public unsafe class FftSpectrumView : BaseRenderingView
    {
        private UnsafeBuffer spectrumBuffer;
        private int* spectrumBufferPtr;
        private int firstUsedLine;

        private DisplayPixel[] spectrumGradient;
        private DisplayPixel[] spectrumGradientHalf;

        public static readonly float[] SPECTRUM_TOP_COLOR = new float[] { 112, 180, 255 };
        public static readonly float[] SPECTRUM_BOTTOM_COLOR = new float[] { 0, 0, 80 };

        public static readonly DisplayPixel PIXEL_WHITE = new DisplayPixel(255, 255, 255);

        public float minDb = -80;
        public float spectrumScale { get { return height / minDb; } }

        public FftSpectrumView(IRenderingContext parent, int x, int y, int width, int height) : base(parent, x, y, width, height)
        {
            spectrumBuffer = UnsafeBuffer.Create(width + 2, sizeof(int));
            spectrumBufferPtr = (int*)spectrumBuffer;
            PrecomputeGradients();
        }

        public override void FullRedrawThis()
        {
            for (int y = 0; y < height; y++)
            {
                //Get pointer to these pixels for fast drawing
                var pixelPtr = GetPixelPointer(0, y);

                //If this line isn't used at all, quickly fill it
                if (y < firstUsedLine)
                {
                    for (int x = 0; x < width; x++)
                        pixelPtr[x] = spectrumGradientHalf[y];
                    continue;
                }

                //Loop across
                for (int x = 0; x < width; x++)
                {
                    if (spectrumBufferPtr[x + 1] == y)
                    {
                        //Draw white
                        pixelPtr[x] = PIXEL_WHITE;
                    }
                    else if (spectrumBufferPtr[x + 1] > y)
                    {
                        //Draw spectrum BG
                        pixelPtr[x] = spectrumGradientHalf[y];
                    }
                    else if (spectrumBufferPtr[x + 1] < y)
                    {
                        //Draw spectrum FG
                        pixelPtr[x] = spectrumGradient[y];
                    }
                }
            }
        }

        public void AddScaledFrame(float* fftBufferPtr)
        {
            //Crunch
            for (int i = 0; i < width; i++)
            {
                spectrumBufferPtr[i + 1] = (int)((fftBufferPtr[i] * spectrumScale) + 0.5f);
            }

            //Determine the first used line by finding the min pixel
            firstUsedLine = int.MaxValue;
            for (int i = 0; i < width - 1; i++)
                firstUsedLine = Math.Min(firstUsedLine, spectrumBufferPtr[i + 1]);

            //Write first and last element to the space we allocated for over/underflows. This just lets us save some CPU cycles later
            spectrumBufferPtr[0] = spectrumBufferPtr[1];
            spectrumBufferPtr[width - 1] = spectrumBufferPtr[width - 2];

            //Invalidate
            Invalidate();
        }

        private void PrecomputeGradients()
        {
            //Precompute spectrum gradient
            spectrumGradient = new DisplayPixel[height];
            spectrumGradientHalf = new DisplayPixel[height];
            for (int i = 0; i < height; i++)
            {
                float scale = i * (1 / (float)height);
                spectrumGradient[i] = new DisplayPixel(0, 0, 0);
                spectrumGradientHalf[i] = new DisplayPixel(0, 0, 0);

                //R
                spectrumGradient[i].r = (byte)(((1 - scale) * SPECTRUM_TOP_COLOR[0]) + (scale * SPECTRUM_BOTTOM_COLOR[0]));
                spectrumGradientHalf[i].r = (byte)(spectrumGradient[i].r / 4);

                //G
                spectrumGradient[i].g = (byte)(((1 - scale) * SPECTRUM_TOP_COLOR[1]) + (scale * SPECTRUM_BOTTOM_COLOR[1]));
                spectrumGradientHalf[i].g = (byte)(spectrumGradient[i].g / 4);

                //B
                spectrumGradient[i].b = (byte)(((1 - scale) * SPECTRUM_TOP_COLOR[2]) + (scale * SPECTRUM_BOTTOM_COLOR[2]));
                spectrumGradientHalf[i].b = (byte)(spectrumGradient[i].b / 4);
            }
            
        }

        /*private void RenderSpectrum()
        {
            //Render
            for (int x = 0; x < width; x++)
            {
                //Get points
                int pointMax = Math.Max(Math.Max(spectrumBufferPtr[x], spectrumBufferPtr[x + 2]), spectrumBufferPtr[x + 1]);
                int pointMin = Math.Min(Math.Min(spectrumBufferPtr[x], spectrumBufferPtr[x + 2]), spectrumBufferPtr[x + 1]);

                //Compute
                for (int y = 0; y < height; y++)
                {
                    if (y > pointMin && y < pointMax)
                    {
                        WritePixel(x, y, new DisplayPixel(byte.MaxValue, byte.MaxValue, byte.MaxValue));
                    }
                    else if (spectrumBufferPtr[x + 1] > y)
                    {
                        WritePixel(x, y, spectrumGradientHalf[y]);
                    }
                    else if (spectrumBufferPtr[x + 1] < y)
                    {
                        WritePixel(x, y, spectrumGradient[y]);
                    }
                }
            }
        }*/
    }
}
