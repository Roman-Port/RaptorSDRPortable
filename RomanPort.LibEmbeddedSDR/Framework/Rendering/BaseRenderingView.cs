using RomanPort.LibEmbeddedSDR.Framework.Display;
using RomanPort.LibEmbeddedSDR.Framework.Input;
using RomanPort.LibEmbeddedSDR.Framework.Rendering.DrawingContexts;
using RomanPort.LibEmbeddedSDR.Framework.Rendering.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Rendering
{
    public abstract class BaseRenderingView : IRenderingContext
    {
        public readonly IRenderingContext parent;
        public int x; //Reletive to the parent
        public int y; //Reletive to the parent
        public int width;
        public int height;
        public List<BaseRenderingView> children;

        public bool invalidated;
        public List<BaseRenderingView> invalidatedChildren;

        public bool isSelectionHovering;
        public bool isSelectionSelected;

        public BaseRenderingView(IRenderingContext parent, int x, int y, int width, int height)
        {
            this.parent = parent;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            children = new List<BaseRenderingView>();
            invalidatedChildren = new List<BaseRenderingView>();
            invalidated = true;
            parent.AddChild(this);
            parent.InvalidateChild(this);
        }

        public int absoluteX => parent.absoluteX + x;

        public int absoluteY => parent.absoluteY + y;

        public void FullRedraw(bool force)
        {
            //Draw this
            if(invalidated || force)
                FullRedrawThis();

            //Draw children
            foreach (var c in children)
                if(c.NeedsRedraw() || force)
                    c.FullRedraw(force);

            //Post draw this
            if (invalidated || force)
                FullRedrawThisPost();

            //Draw effects
            if (isSelectionHovering)
                DrawSelectionHoveringOutline();

            //Update state
            invalidated = false;
            invalidatedChildren.Clear();
        }

        public abstract void FullRedrawThis();
        public virtual void FullRedrawThisPost() { }

        public virtual void DrawSelectionHoveringOutline()
        {
            //Draw the yellow outline around this object while it is being selected
            UtilDrawOutlineRectangle(this, width, height, StaticColors.COLOR_SELECTION);
        }

        public virtual void OnSelectedLocked() { }
        public virtual void OnSelectedUnlocked() { }

        /// <summary>
        /// Requests that this be redrawn when we can
        /// </summary>
        public void Invalidate()
        {
            invalidated = true;
            parent.InvalidateChild(this);
            foreach (var c in children)
                c.Invalidate();
        }

        public void InvalidateChild(BaseRenderingView view)
        {
            invalidatedChildren.Add(view);
            parent.InvalidateChild(this);
        }

        public bool NeedsRedraw()
        {
            return invalidated || invalidatedChildren.Count > 0;
        }

        public void WritePixel(int x, int y, DisplayPixel pixel)
        {
            parent.WritePixel(x + this.x, y + this.y, pixel);
        }

        public unsafe DisplayPixel* GetPixelPointer(int offsetX, int offsetY)
        {
            return parent.GetPixelPointer(offsetX + this.x, offsetY + this.y);
        }

        public IDrawingContext GetOffsetContext(int offsetX, int offsetY)
        {
            return new OffsetDrawingContext(offsetX, offsetY, this);
        }

        public void AddChild(BaseRenderingView view)
        {
            if(!children.Contains(view))
                children.Add(view);
        }

        public RenderingCanvas GetCanvas()
        {
            return parent.GetCanvas();
        }

        public virtual bool TrySelect()
        {
            //Users wil override this if wanted
            return false;
        }

        public virtual void OnUserInput(UserInputKey key)
        {
            //Users wil override this if wanted
        }

        public static void UtilDrawOutlineRectangle(IDrawingContext ctx, int width, int height, DisplayPixel color)
        {
            for(int i = 0; i < width; i++)
            {
                ctx.WritePixel(i, 0, color);
                ctx.WritePixel(i, height - 1, color);
            }
            for (int i = 0; i < height; i++)
            {
                ctx.WritePixel(0, i, color);
                ctx.WritePixel(width - 1, i, color);
            }
        }

        public static unsafe void UtilDrawFilledRectangle(IDrawingContext ctx, int width, int height, DisplayPixel color)
        {
            for (int y = 0; y < height; y++)
            {
                var ptr = ctx.GetPixelPointer(0, y);
                for (int i = 0; i < width; i++)
                    ptr[i] = color;
            }
        }
    }
}
