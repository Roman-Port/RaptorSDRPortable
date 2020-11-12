using RomanPort.LibEmbeddedSDR.Framework.Display;
using RomanPort.LibEmbeddedSDR.Framework.Rendering;
using RomanPort.LibEmbeddedSDR.Framework.Rendering.Image;
using RomanPort.LibSDR.Framework.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.UI.Components.Common
{
    public unsafe class ImageView : BaseRenderingView
    {
        private SdrImgFile img;
        
        public ImageView(IRenderingContext parent, int x, int y, int width, int height, SdrImgFile img) : base(parent, x, y, width, height)
        {
            this.img = img;
        }

        public override void FullRedrawThis()
        {
            //Render line by line
            for(int i = 0; i<Math.Min(img.height, height); i++)
            {
                //Get pointers
                DisplayPixel* dstPtr = GetPixelPointer(0, i);
                DisplayPixel* srcPtr = img.GetPointer(0, i);

                //Copy
                Utils.Memcpy(dstPtr, srcPtr, Math.Min(img.width, width) * sizeof(DisplayPixel));
            }
        }
    }
}
