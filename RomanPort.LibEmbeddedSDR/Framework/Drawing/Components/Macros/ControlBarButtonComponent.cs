using RomanPort.LibEmbeddedSDR.Frames;
using RomanPort.LibEmbeddedSDR.Framework.Drawing.Components.Primitive;
using RomanPort.LibEmbeddedSDR.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Drawing.Components.Macros
{
    public class ControlBarButtonComponent : BaseRenderComponent
    {
        public ControlBarButtonComponent(IDrawingContext parent, string label, ButtonClickedEventArgs callback) : base(parent)
        {
            this.label = label.ToCharArray();
            this.callback = callback;
            Width = (label.Length * FontStore.SYSTEM_BOLD_15.width) + PADDING_FULL;
            Height = FontStore.SYSTEM_REGULAR_15.height + PADDING_FULL;
        }

        public delegate void ButtonClickedEventArgs();

        private char[] label;
        private ButtonClickedEventArgs callback;

        private const int PADDING = 8;
        private const int PADDING_FULL = PADDING * 2;

        protected override void LayoutView()
        {

        }

        protected override void RedrawView()
        {
            UtilFill(StaticColors.COLOR_FOREGROUND_INPUT_BG, 0, 0, Width, Height);
            FontStore.SYSTEM_BOLD_15.RenderPretty(this, label, DisplayPixel.WHITE, AlignHorizontal.Center, AlignVertical.Center, Width, Height);
            if (IsSelected)
                UtilOutline(StaticColors.COLOR_SELECTION, 0, 0, Width, Height, 1);
        }

        protected override bool OnSelected(int dir)
        {
            Invalidate();
            return true;
        }

        protected override void OnDeselected()
        {
            Invalidate();
        }

        public override UserInputResult OnInput(UserInputEventArgs input)
        {
            //Check if we're trying to move
            if (input.down && input.key == UserInputKey.RIGHT)
                return UserInputResult.MoveNext;
            if (input.down && input.key == UserInputKey.LEFT)
                return UserInputResult.MovePrevious;

            //Update
            if (input.down && input.key == UserInputKey.A)
                callback();

            //Return OK
            return UserInputResult.HandledSelf;
        }
    }
}

