using RomanPort.LibEmbeddedSDR.Framework.Drawing.Components.Primitive;
using RomanPort.LibEmbeddedSDR.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Drawing.Components.Macros
{
    public class DropdownControlComponent : BaseRenderComponent
    {
        public DropdownControlComponent(IDrawingContext parent, int currentOption, params string[] options) : base(parent)
        {
            this.currentOption = currentOption;
            this.options = options;
            foreach (var c in options)
                maxChars = Math.Max(maxChars, c.Length);
            Width = (maxChars * FontStore.SYSTEM_BOLD_15.width) + PADDING_FULL;
            Height = FontStore.SYSTEM_REGULAR_15.height + PADDING_FULL;
        }

        public delegate void DropdownSelectorEventArgs(int index);

        public event DropdownSelectorEventArgs OnValueChanged;

        private int maxChars;
        private string[] options;
        private int currentOption;

        private const int PADDING = 8;
        private const int PADDING_FULL = PADDING * 2;

        public DropdownControlComponent SetValueChangeEvent(DropdownSelectorEventArgs callback)
        {
            OnValueChanged += callback;
            return this;
        }

        protected override void LayoutView()
        {
            
        }

        protected override void RedrawView()
        {
            UtilFill(StaticColors.COLOR_FOREGROUND_INPUT_BG, 0, 0, Width, Height);
            FontStore.SYSTEM_BOLD_15.RenderPretty(this, options[currentOption].ToCharArray(), DisplayPixel.WHITE, AlignHorizontal.Center, AlignVertical.Center, Width, Height);
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
            if (input.down && input.key == UserInputKey.UP)
                currentOption++;
            if (input.down && input.key == UserInputKey.DOWN)
                currentOption--;

            //Clamp
            if (currentOption >= options.Length)
                currentOption = 0;
            if (currentOption < 0)
                currentOption = options.Length - 1;

            //Invalidate
            Invalidate();
            if (input.down && (input.key == UserInputKey.DOWN || input.key == UserInputKey.UP))
                OnValueChanged?.Invoke(currentOption);

            //Return OK
            return UserInputResult.HandledSelf;
        }
    }
}
