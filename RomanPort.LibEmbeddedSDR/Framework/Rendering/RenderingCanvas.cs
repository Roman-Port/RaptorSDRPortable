using RomanPort.LibEmbeddedSDR.Framework.Display;
using RomanPort.LibEmbeddedSDR.Framework.Input;
using RomanPort.LibEmbeddedSDR.Framework.Rendering.DrawingContexts;
using RomanPort.LibEmbeddedSDR.Framework.Rendering.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Rendering
{
    public unsafe class RenderingCanvas : IRenderingContext
    {
        public readonly ISdrDisplay display;
        public ICanvasBackgroundProvider background;
        public List<BaseRenderingView> children;
        public List<BaseRenderingView> inputStack; //Latest added will get user input
        public bool invalidated = true;
        public bool firstDraw = true;
        public RenderingFontStore fontStore;

        public RenderingCanvas(ISdrDisplay display, ICanvasBackgroundProvider background, RenderingFontStore fontStore)
        {
            this.display = display;
            this.fontStore = fontStore;
            this.background = background;
            this.children = new List<BaseRenderingView>();
            this.inputStack = new List<BaseRenderingView>();
            invalidated = true;
        }

        public int absoluteX => 0;

        public int absoluteY => 0;

        public void FullRedraw(bool force = false)
        {
            //Draw background
            if(firstDraw || force)
                background.RenderCanvasBackground(this);

            //Draw children
            foreach (var c in children)
                if(c.NeedsRedraw() || force)
                    c.FullRedraw(force);

            //Refresh display
            display.Apply();

            //Update state
            invalidated = false;
            firstDraw = false;
        }

        public BaseFontPack GetFontPack(RenderingFontTag tag)
        {
            return fontStore.GetFontPack(tag);
        } 

        public void WritePixel(int x, int y, DisplayPixel pixel)
        {
            if (pixel.a == 0)
                return;
            else /*if (pixel.a == byte.MaxValue)*/
                display.bufferPtr[GetPixelBufferLocation(x, y)] = pixel;
            /*else
                WriteSemiTransparentPixel(x, y, pixel);*/
        }

        public DisplayPixel ReadPixel(int x, int y)
        {
            return display.bufferPtr[GetPixelBufferLocation(x, y)];
        }

        public DisplayPixel* GetPixelPointer(int offsetX, int offsetY)
        {
            return display.bufferPtr + GetPixelBufferLocation(offsetX, offsetY);
        }

        public int GetPixelBufferLocation(int x, int y)
        {
            return (y * display.width) + x;
        }

        public void WriteSemiTransparentPixel(int offsetX, int offsetY, DisplayPixel pixel)
        {
            //This is more expensive, as we need to mix the pixels
            //We'll have to mix the pixels now
            float mixNew = (float)pixel.a / byte.MaxValue;
            float mixOld = 1f - mixNew;

            //Get the pixel index
            int pixelIndex = GetPixelBufferLocation(offsetX, offsetY);

            //Read existing pixel
            DisplayPixel srcPixel = display.bufferPtr[pixelIndex];

            //Calculate
            display.bufferPtr[pixelIndex] = new DisplayPixel
            {
                r = (byte)((pixel.r * mixNew) + (srcPixel.r * mixOld)),
                g = (byte)((pixel.g * mixNew) + (srcPixel.g * mixOld)),
                b = (byte)((pixel.b * mixNew) + (srcPixel.b * mixOld)),
                a = (byte)((pixel.a * mixNew) + (srcPixel.a * mixOld))
            };
        }

        public IDrawingContext GetOffsetContext(int offsetX, int offsetY)
        {
            return new OffsetDrawingContext(offsetX, offsetY, this);
        }

        public void AddChild(BaseRenderingView view)
        {
            if (!children.Contains(view))
                children.Add(view);
        }

        public RenderingCanvas GetCanvas()
        {
            return this;
        }

        public void OnUserInput(UserInputKey key)
        {
            //Send this to the frontmost on the stack
            if (inputStack.Count > 0)
                inputStack[inputStack.Count - 1].OnUserInput(key);
        }

        public bool TrySelect()
        {
            throw new NotImplementedException();
        }
        
        public void Invalidate()
        {
            invalidated = true;
        }

        public void InvalidateChild(BaseRenderingView v)
        {
            Invalidate();
        }

        public void PushInputHandlerToStack(BaseRenderingView v)
        {
            inputStack.Add(v);
        }

        public int GetHorizCenteredLocation(int elementWidth)
        {
            return (display.width - elementWidth) / 2;
        }
    }
}
