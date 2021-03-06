﻿using RomanPort.LibEmbeddedSDR;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Drawing
{
    public class OffsetDrawingContext : IDrawingContext
    {
        public int offsetX;
        public int offsetY;
        public readonly IDrawingContext parent;

        public int Width => parent.Width;

        public int Height => parent.Height;

        public OffsetDrawingContext(int offsetX, int offsetY, IDrawingContext parent)
        {
            this.offsetX = offsetX;
            this.offsetY = offsetY;
            this.parent = parent;
        }

        public void WritePixel(int offsetX, int offsetY, DisplayPixel pixel)
        {
            parent.WritePixel(offsetX + this.offsetX, offsetY + this.offsetY, pixel);
        }

        public unsafe DisplayPixel* GetPixelPointer(int offsetX, int offsetY)
        {
            return parent.GetPixelPointer(offsetX + this.offsetX, offsetY + this.offsetY);
        }

        public IDrawingContext GetOffsetContext(int offsetX, int offsetY)
        {
            return new OffsetDrawingContext(offsetX, offsetY, this);
        }
    }
}
