using RomanPort.LibSDR.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Drawing.Components.Advanced
{
    public unsafe class FftSpectrumComponent : BaseRenderComponent
    {
        public FftSpectrumComponent(IDrawingContext parent) : base(parent)
        {
        }

        private UnsafeBuffer fftBuffer;
        private float* fftPtr;

        private DisplayPixel[] gradient;
        private DisplayPixel[] gradientDark;

        public static readonly float[] SPECTRUM_TOP_COLOR = new float[] { 112, 180, 255 };
        public static readonly float[] SPECTRUM_BOTTOM_COLOR = new float[] { 0, 0, 80 };

        public float fftOffset = 0;
        public float fftRange = 80;

        protected override void LayoutView()
        {
            
        }

        protected override void RedrawView()
        {
            //Check if the height has changed
            if(gradient == null || gradient.Length != Height)
            {
                PrecomputeGradients();
            }

            //Make sure buffer is OK
            if (fftBuffer == null || fftBuffer.Length - 2 != Width)
                return;

            //Get base pixel
            var ptr = GetPixelPointer(0, 0);

            //Loop
            for (var x = 0; x < Width; x += 1)
            {
                //Get where this pixel is
                var max = Math.Max(fftPtr[x - 1], Math.Max(fftPtr[x], fftPtr[x + 1]));
                var value = fftPtr[x];
                var min = Math.Min(fftPtr[x - 1], Math.Min(fftPtr[x], fftPtr[x + 1]));

                //Loop
                for (var y = 0; y < Height; y++)
                {
                    //Get offset
                    var offset = ((y * Width) + x);

                    //Determine color
                    if (y > max)
                    {
                        //Full gradient
                        ptr[offset] = gradient[y];
                    }
                    else if (y == value)
                    {
                        //Point
                        ptr[offset] = new DisplayPixel(255, 255, 255);
                    }
                    else if (y <= max && y > value)
                    {
                        //Interp top
                        var c = InterpColor((float)Math.Pow((y - value) / (max - value), 3), gradient[y], new DisplayPixel(255, 255, 255));
                        ptr[offset + 0] = c;
                    }
                    else if (y < value && y >= min)
                    {
                        //Intep bottom
                        var c = InterpColor((float)Math.Pow((y - min) / (value - min), 3), new DisplayPixel(255, 255, 255), gradientDark[y]);
                        ptr[offset + 0] = c;
                    }
                    else
                    {
                        //Dark gradient
                        ptr[offset] = gradientDark[y];
                    }
                }
            }
        }

        public void AddScaledFrame(float* ptr, int width)
        {
            //Ensure
            if (width != Width)
                return;

            //If the array size is incorrect, create
            if(fftBuffer == null || fftBuffer.Length - 2 != width)
            {
                fftBuffer?.Dispose();
                fftBuffer = UnsafeBuffer.Create(width + 2, sizeof(int));
                fftPtr = ((float*)fftBuffer) + 1;
            }
            
            //Copy FFT frame
            Utils.Memcpy(fftPtr, ptr, Width * sizeof(float));

            //Convert and fill padding
            for (int i = 0; i < Width; i++)
                fftPtr[i] = ((Math.Abs(fftPtr[i]) + fftOffset) / fftRange) * Height;
            fftPtr[-1] = fftPtr[0];
            fftPtr[Width] = fftPtr[Width - 1];

            //Invalidate
            Invalidate();
        }

        private static DisplayPixel InterpColor(float percent, DisplayPixel c1, DisplayPixel c2)
        {
            var invPercent = 1 - percent;
            return new DisplayPixel(
                (byte)((c1.r * percent) + (c2.r * invPercent)),
                (byte)((c1.g * percent) + (c2.g * invPercent)),
                (byte)((c1.b * percent) + (c2.b * invPercent))
            );
        }

        private void PrecomputeGradients()
        {
            //Precompute spectrum gradient
            gradient = new DisplayPixel[Height];
            gradientDark = new DisplayPixel[Height];
            for (int i = 0; i < Height; i++)
            {
                float scale = i * (1 / (float)Height);
                gradient[i] = new DisplayPixel(0, 0, 0);
                gradientDark[i] = new DisplayPixel(0, 0, 0);

                //R
                gradient[i].r = (byte)(((1 - scale) * SPECTRUM_TOP_COLOR[0]) + (scale * SPECTRUM_BOTTOM_COLOR[0]));
                gradientDark[i].r = (byte)(gradient[i].r / 4);

                //G
                gradient[i].g = (byte)(((1 - scale) * SPECTRUM_TOP_COLOR[1]) + (scale * SPECTRUM_BOTTOM_COLOR[1]));
                gradientDark[i].g = (byte)(gradient[i].g / 4);

                //B
                gradient[i].b = (byte)(((1 - scale) * SPECTRUM_TOP_COLOR[2]) + (scale * SPECTRUM_BOTTOM_COLOR[2]));
                gradientDark[i].b = (byte)(gradient[i].b / 4);
            }

        }
    }
}
