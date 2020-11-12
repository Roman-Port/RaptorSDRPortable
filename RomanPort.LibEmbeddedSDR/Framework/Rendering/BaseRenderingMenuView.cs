using RomanPort.LibEmbeddedSDR.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Rendering
{
    public class BaseRenderingMenuView : BaseRenderingView
    {
        public BaseRenderingMenuView(IRenderingContext parent, int x, int y, int width, int height) : base(parent, x, y, width, height)
        {
        }

        public int selectedChildIndex = -1; //-1 if nothing is selected
        public bool selectionLocked = false; //If locked, pass events through. Otherwise, use us as the menu

        public override void FullRedrawThis()
        {
            
        }

        public override void OnUserInput(UserInputKey key)
        {
            //Dispatch this to a child if selected
            if (selectionLocked)
            {
                children[selectedChildIndex].OnUserInput(key);
                return;
            }

            //Act as a menu
            if(key == UserInputKey.A)
            {
                //Lock onto the current selection
                if(selectedChildIndex != -1)
                {
                    selectionLocked = true;
                    children[selectedChildIndex].isSelectionSelected = true;
                    children[selectedChildIndex].OnSelectedLocked();
                    Invalidate();
                }
            } else if (key == UserInputKey.RIGHT || key == UserInputKey.DOWN || key == UserInputKey.UP || key == UserInputKey.LEFT)
            {
                //Get next
                int next = FindNextSelectableChildIndex((key == UserInputKey.RIGHT || key == UserInputKey.DOWN) ? 1 : -1);
                if(next != -1)
                {
                    if (selectedChildIndex != -1)
                        children[selectedChildIndex].isSelectionHovering = false;
                    selectedChildIndex = next;
                    children[selectedChildIndex].isSelectionHovering = true;
                    Invalidate();
                }
            }
        }

        private int FindNextSelectableChildIndex(int direction)
        {
            for (int i = 0; i < children.Count; i += direction)
            {
                int actualIndex = (i - Math.Min(0, selectedChildIndex)) % children.Count;
                if (actualIndex == selectedChildIndex)
                    continue;
                if (children[actualIndex].TrySelect())
                    return actualIndex;
            }
            return -1;
        }
    }
}
