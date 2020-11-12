using RomanPort.LibEmbeddedSDR.Framework.Display;
using RomanPort.LibEmbeddedSDR.Framework.Input;
using RomanPort.LibEmbeddedSDR.Framework.Rendering;
using RomanPort.LibEmbeddedSDR.Framework.UI.Components.Common;
using RomanPort.LibEmbeddedSDR.Framework.UI.Components.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.UI.User
{
    public class RadioControlBarMini : BaseRenderingView
    {
        private RectangleView background;
        private SinglelineTextView freqLabel;
        private RadioControlFreq freqDisplay;

        private SdrSession session;

        private int selectedId = 0; //0-11 freq

        public const int MINI_BAR_HEIGHT = 100;
        public const int MAX_SELECTED_ID = 11;

        private const int PADDING_TOP_LABEL = 11;
        private const int PADDING_TOP_OPTION = 30;
        private const int PADDING = 10;
        
        public RadioControlBarMini(IRenderingContext parent, int x, int y, int width, int height, SdrSession session) : base(parent, x, y, width, height)
        {
            this.session = session;
            GetCanvas().PushInputHandlerToStack(this);

            var labelColor = new DisplayPixel(210, 210, 210);
            background = new RectangleView(this, 0, 1, width, height-1, StaticColors.COLOR_FOREGROUND_BG);
            freqLabel = new SinglelineTextView(this, PADDING, PADDING_TOP_LABEL, 100, 100, GetCanvas().GetFontPack(RenderingFontTag.SYSTEM_15), "TUNING", labelColor);
            freqDisplay = new RadioControlFreq(this, PADDING, PADDING_TOP_OPTION, 100, 100, session.radio.source);
        }

        public override void FullRedrawThis()
        {
            DrawBorder();
        }

        private unsafe void DrawBorder()
        {
            var borderPtr = GetPixelPointer(0, 0);
            for (int i = 0; i < width; i++)
                borderPtr[i] = StaticColors.COLOR_FOREGROUND_BORDER;
        }

        public override void FullRedrawThisPost()
        {
            base.FullRedrawThisPost();
            freqDisplay.DrawNumberOutline(selectedId);
        }

        public override void OnUserInput(UserInputKey key)
        {
            //We have this implimented our own way, which is kind of gross. selectedId updates and then when we redraw it'll be updated
            if(key == UserInputKey.RIGHT)
            {
                selectedId++;
                if (selectedId > MAX_SELECTED_ID)
                    selectedId = 0;
                Invalidate();
            } else if (key == UserInputKey.LEFT)
            {
                selectedId--;
                if (selectedId < 0)
                    selectedId = MAX_SELECTED_ID - 1;
                Invalidate();
            } else if(key == UserInputKey.UP || key == UserInputKey.DOWN)
            {
                int dir = key == UserInputKey.UP ? 1 : -1;
                freqDisplay.UpdateNumber(selectedId, dir);
                Invalidate();
            }
        }
    }
}
