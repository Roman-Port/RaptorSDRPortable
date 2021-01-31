using RomanPort.LibEmbeddedSDR.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Drawing
{
    public delegate void RenderComponentChildEnumerable(BaseRenderComponent child);

    public abstract class BaseRenderComponent : IDrawingContext
    {
        public BaseRenderComponent(IDrawingContext parent)
        {
            parentDrawingContext = parent;
        }
        
        private IDrawingContext parentDrawingContext;
        private bool drawingInvalidated = true;
        private bool layoutInvalidated = true;

        private BaseRenderComponent parent; //May be null
        private int width;
        private int height;
        private int x;
        private int y;
        private bool hidden;
        private DisplayPixel background = DisplayPixel.TRANSPARENT;

        protected IDrawingContext DrawingContext => parentDrawingContext;
        protected bool IsSelected { get; private set; }

        public int Width
        {
            get => width;
            set
            {
                width = value;
                LayoutInvalidate();
            }
        }
        public int Height
        {
            get => height;
            set
            {
                height = value;
                LayoutInvalidate();
            }
        }
        public int X
        {
            get => x;
            set
            {
                if (y != value)
                    ParentInvalidate();
                x = value;
            }
        }
        public int Y
        {
            get => y;
            set
            {
                if (y != value)
                    ParentInvalidate();
                y = value;
            }
        }
        public bool Hidden
        {
            get => hidden;
            set
            {
                hidden = value;
                ParentLayoutInvalidate();
            }
        }
        public DisplayPixel BackgroundColor
        {
            get => background;
            set
            {
                background = value;
                Invalidate();
            }
        }
        public bool IsMenu { get; set; }

        protected unsafe void UtilFill(DisplayPixel pixel)
        {
            UtilFill(pixel, 0, 0, Width, Height);
        }

        protected unsafe void UtilFill(DisplayPixel pixel, int offsetX, int offsetY, int width, int height)
        {
            for (int y = 0; y < height; y++)
            {
                DisplayPixel* ptr = GetPixelPointer(offsetX, offsetY + y);
                for (int x = 0; x < width; x++)
                    ptr[x] = pixel;
            }
        }

        protected unsafe void UtilOutline(DisplayPixel pixel, int offsetX, int offsetY, int width, int height, int thickness)
        {
            //Draw top and bottom
            for (int t = 0; t < thickness; t++)
            {
                DisplayPixel* ptrTop = GetPixelPointer(offsetX, offsetY + t);
                DisplayPixel* ptrBottom = GetPixelPointer(offsetX, offsetY + height - 1 - t);
                for (int x = 0; x < width; x++)
                {
                    ptrTop[x] = pixel;
                    ptrBottom[x] = pixel;
                }
            }

            //Draw left and right
            for(int y = 1; y<height - 1; y++)
            {
                int offset = offsetY + y;
                DisplayPixel* ptrLeft = GetPixelPointer(offsetX, offset);
                DisplayPixel* ptrRight = GetPixelPointer(offsetX + width - 1, offset);
                for(int t = 0; t<thickness; t++)
                {
                    ptrLeft[t] = pixel;
                    ptrRight[-t] = pixel;
                }
            }
        }

        public void DrawingTick(bool force = false)
        {
            if (layoutInvalidated || force)
                FullLayoutView();
            if (drawingInvalidated || force)
                FullRedrawView();
            EnumerateChildren((BaseRenderComponent c) => c.DrawingTick(force));
        }

        public void SetParent(BaseRenderComponent parent)
        {
            parentDrawingContext = parent;
            this.parent = parent;
        }

        public void Invalidate()
        {
            drawingInvalidated = true;
        }

        public void LayoutInvalidate()
        {
            layoutInvalidated = true;
            drawingInvalidated = true;
        }

        protected void ParentInvalidate()
        {
            parent?.Invalidate();
        }

        protected void ParentLayoutInvalidate()
        {
            parent?.LayoutInvalidate();
        }

        protected virtual void EnumerateChildren(RenderComponentChildEnumerable callback)
        {
            //No children unless implimented
        }

        private int menuSelectedChild = 0;

        public virtual UserInputResult OnInput(UserInputEventArgs input)
        {
            if(!IsMenu)
            {
                //Pass events through
                UserInputResult state = UserInputResult.Unhandled;
                EnumerateChildren((BaseRenderComponent child) =>
                {
                    if (state == UserInputResult.Unhandled)
                        state = child.OnInput(input);
                });
                return state;
            } else
            {
                //Send the event to the current target
                UserInputResult state = UserInputResult.Unhandled;
                int childIndex = 0;
                List<BaseRenderComponent> children = new List<BaseRenderComponent>();
                EnumerateChildren((BaseRenderComponent child) =>
                {
                    if (childIndex == menuSelectedChild)
                        state = child.OnInput(input);
                    childIndex++;
                    children.Add(child);
                });
                if (children.Count == 0)
                    throw new Exception("Empty menu");

                //Act on this accordingly
                if(state == UserInputResult.MoveNext || state == UserInputResult.MovePrevious)
                {
                    //Get direction
                    int dir = state == UserInputResult.MoveNext ? 1 : -1;

                    //Deselect current child
                    if(menuSelectedChild >= 0 && menuSelectedChild < children.Count)
                    {
                        children[menuSelectedChild].OnDeselected();
                        children[menuSelectedChild].IsSelected = false;
                    }

                    //Loop until we find a sutible child
                    for(int i = 0; i<children.Count; i++)
                    {
                        //Update direction
                        menuSelectedChild = (menuSelectedChild + dir);
                        if (menuSelectedChild >= children.Count)
                            menuSelectedChild = 0;
                        if (menuSelectedChild < 0)
                            menuSelectedChild = children.Count - 1;

                        //Attempt to select child
                        if (children[menuSelectedChild].OnSelected(dir))
                        {
                            children[menuSelectedChild].IsSelected = true;
                            return UserInputResult.HandledSelf;
                        }
                    }

                    //Failed to find a child to use. Go back
                    return state;
                }

                return state;
            }
        }

        protected virtual bool OnSelected(int dir)
        {
            bool selected = false;
            EnumerateChildren((BaseRenderComponent child) =>
            {
                if (selected)
                    return;
                if(child.OnSelected(dir))
                {
                    selected = true;
                    child.IsSelected = true;
                }
            });
            IsSelected = selected;
            return selected;
        }

        protected virtual void OnDeselected()
        {
            bool skip = false;
            IsSelected = false;
            EnumerateChildren((BaseRenderComponent child) =>
            {
                if (!skip && child.IsSelected)
                {
                    child.OnDeselected();
                    child.IsSelected = false;
                    skip = true;
                } 
            });
        }

        protected abstract void LayoutView();
        protected abstract void RedrawView();

        private void FullLayoutView()
        {
            //Layout this
            LayoutView();

            //Force children to relayout
            EnumerateChildren((BaseRenderComponent c) => c.FullLayoutView());

            //Update state
            layoutInvalidated = false;
        }

        private void FullRedrawView()
        {
            //Redraw background
            if(background.a == byte.MaxValue)
                UtilFill(background);
            
            //Redraw this
            RedrawView();

            //Force children to redraw
            EnumerateChildren((BaseRenderComponent c) => c.FullRedrawView());

            //Update state
            drawingInvalidated = false;
        }

        public void WritePixel(int offsetX, int offsetY, DisplayPixel pixel)
        {
            parentDrawingContext.WritePixel(offsetX + X, offsetY + Y, pixel);
        }

        public unsafe DisplayPixel* GetPixelPointer(int offsetX, int offsetY)
        {
            return parentDrawingContext.GetPixelPointer(offsetX + X, offsetY + Y);
        }

        public IDrawingContext GetOffsetContext(int offsetX, int offsetY)
        {
            return new OffsetDrawingContext(offsetX, offsetY, this);
        }
    }
}
