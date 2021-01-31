using RomanPort.LibEmbeddedSDR.Frames.MenuTabs;
using RomanPort.LibEmbeddedSDR.Framework;
using RomanPort.LibEmbeddedSDR.Framework.Drawing;
using RomanPort.LibEmbeddedSDR.Framework.Drawing.Frames;
using RomanPort.LibEmbeddedSDR.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Frames
{
    public class MenuFrame : BaseRenderComponent, ISdrFrame
    {
        public MenuFrame(IDrawingContext parent) : base(parent)
        {
            Height = parent.Height;
            Width = parent.Width;

            tabs = new BaseMenuTab[]
            {
                new RecordingTab(this),
                new RadioTab(this)
            };
        }

        public static void ShowMenu()
        {
            SDR.SwitchActiveFrame(new MenuFrame(SDR.Display));
        }

        private BaseMenuTab[] tabs;

        private int currentTab;
        private bool currentTabSelected;

        private const int MENU_HEIGHT = 300;
        private const int TAB_LABEL_WIDTH = 120;
        private const int TAB_LABEL_HEIGHT = 30;
        private const int PADDING = 10;

        public bool IsSemiTransparent => true;

        public void OnClosed()
        {
            
        }

        public void OnOpened()
        {
            
        }

        public void Tick(bool forceRedraw)
        {
            DrawingTick(forceRedraw);
        }

        protected override void LayoutView()
        {
            
        }

        protected override unsafe void RedrawView()
        {
            //Darken the frame
            for (int i = 0; i<Height - MENU_HEIGHT; i++)
            {
                DisplayPixel* line = GetPixelPointer(0, i);
                for(int x = 0; x<Width; x++)
                {
                    line[x].r /= 3;
                    line[x].g /= 3;
                    line[x].b /= 3;
                }
            }

            //Draw the background
            UtilFill(StaticColors.COLOR_FOREGROUND_BG, 0, Height - MENU_HEIGHT, Width, MENU_HEIGHT);

            //Draw the tab labels
            for(int i = 0; i<tabs.Length; i++)
            {
                int offsetX = PADDING;
                int offsetY = (Height - MENU_HEIGHT) + PADDING + (i * TAB_LABEL_HEIGHT);
                if (i == currentTab)
                    UtilFill(StaticColors.COLOR_FOREGROUND_INPUT_BG, offsetX, offsetY, TAB_LABEL_WIDTH, TAB_LABEL_HEIGHT);
                if (i == currentTab && !currentTabSelected)
                    UtilOutline(StaticColors.COLOR_SELECTION, offsetX, offsetY, TAB_LABEL_WIDTH, TAB_LABEL_HEIGHT, 1);
                FontStore.SYSTEM_REGULAR_15.RenderPretty(GetOffsetContext(offsetX + 5, offsetY), tabs[i].DisplayName.ToCharArray(), DisplayPixel.WHITE, AlignHorizontal.Left, AlignVertical.Center, TAB_LABEL_WIDTH - 10, TAB_LABEL_HEIGHT);
            }
        }

        public override UserInputResult OnInput(UserInputEventArgs input)
        {
            if (input.down && input.key == UserInputKey.UP)
                currentTab = Math.Max(0, currentTab - 1);
            if (input.down && input.key == UserInputKey.DOWN)
                currentTab = Math.Max(tabs.Length - 1, currentTab + 1);
            if (input.down && input.key == UserInputKey.B)
            {
                SDR.CloseActiveFrame();
                return UserInputResult.HandledSelf;
            }
            Invalidate();
            return UserInputResult.HandledSelf;
        }
    }
}
