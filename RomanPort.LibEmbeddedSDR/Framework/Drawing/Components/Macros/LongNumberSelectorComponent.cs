using RomanPort.LibEmbeddedSDR.Framework.Drawing.Fonts;
using RomanPort.LibEmbeddedSDR.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Drawing.Components.Macros
{
    public class LongNumberSelectorComponent : BaseRenderComponent
    {
        public LongNumberSelectorComponent(IDrawingContext parent, int numberCount, long value) : base(parent)
        {
            this.numberCount = numberCount;
            this.value = value;
            Width = (font.width * numberCount) + PADDING_H_TOTAL + (GROUP_PADDING * ((numberCount / 3) - 1));
            minValue = 0;
            maxValue = (long)Math.Pow(10, numberCount) - 1;
        }

        public delegate void NumberSelectorEventArgs(long value);

        public event NumberSelectorEventArgs OnValueChanged;

        public LongNumberSelectorComponent SetMinValue(int min)
        {
            minValue = min;
            if (value < minValue)
                value = minValue;
            return this;
        }

        public LongNumberSelectorComponent SetMaxValue(int max)
        {
            maxValue = max;
            if (value > maxValue)
                value = maxValue;
            return this;
        }

        public LongNumberSelectorComponent SetValueChangeEvent(NumberSelectorEventArgs callback)
        {
            OnValueChanged += callback;
            return this;
        }

        private long minValue;
        private long maxValue;
        private long value;
        private int numberCount;
        private int selectedDigit = -1;

        private readonly static SdrFontPack font = FontStore.SYSTEM_REGULAR_20;
        private readonly static DisplayPixel COLOR_LIT = DisplayPixel.WHITE;
        private readonly static DisplayPixel COLOR_UNLIT = new DisplayPixel(180);

        private const int PADDING_H = 5;
        private const int PADDING_H_TOTAL = PADDING_H * 2;
        private const int GROUP_PADDING = 4;

        protected override bool OnSelected(int dir)
        {
            selectedDigit = dir != -1 ? numberCount - 1 : 0;
            Invalidate();
            return true;
        }

        public override UserInputResult OnInput(UserInputEventArgs input)
        {
            //Apply
            if (input.down && input.key == UserInputKey.LEFT)
                selectedDigit++;
            if (input.down && input.key == UserInputKey.RIGHT)
                selectedDigit--;
            if (input.down && input.key == UserInputKey.UP)
                value += (long)Math.Pow(10, selectedDigit);
            if (input.down && input.key == UserInputKey.DOWN)
                value -= (long)Math.Pow(10, selectedDigit);

            //Make sure it's within bounds
            if (value < minValue)
                value = minValue;
            if (value > maxValue)
                value = maxValue;

            //Invalidate
            Invalidate();
            if (input.down && (input.key == UserInputKey.UP || input.key == UserInputKey.DOWN))
                OnValueChanged?.Invoke(value);

            //Return result
            if (selectedDigit < 0)
                return UserInputResult.MoveNext;
            else if (selectedDigit >= numberCount)
                return UserInputResult.MovePrevious;
            else
                return UserInputResult.HandledSelf;
        }

        protected override void LayoutView()
        {
            
        }

        protected override void RedrawView()
        {
            //Render the dark padding
            UtilFill(StaticColors.COLOR_FOREGROUND_INPUT_BG, 0, 0, PADDING_H, Height);
            UtilFill(StaticColors.COLOR_FOREGROUND_INPUT_BG, Width - PADDING_H, 0, PADDING_H, Height);

            //Render digits
            for (int i = 0; i < numberCount; i++)
                RenderDigit(i);

            //Render decimals
            for (int i = 3; i < numberCount; i += 3)
            {
                UtilFill(StaticColors.COLOR_FOREGROUND_INPUT_BG, GetDigitPixel(i - 1) - GROUP_PADDING, 0, GROUP_PADDING, Height);
                UtilFill(GetColor(i), GetDigitPixel(i - 1) - 3, Height - 10, 2, 2);
            }
        }

        private void RenderDigit(int index)
        {
            //Get offset
            int pixel = GetDigitPixel(index);

            //Clear
            UtilFill(StaticColors.COLOR_FOREGROUND_INPUT_BG, pixel, 0, font.width, Height);
            
            //Render character
            int d = (int)(value / (long)Math.Pow(10, index)) % 10;
            
            font.RenderCharacter(GetOffsetContext(pixel, (Height - font.height) / 2), (char)(48 + d), GetColor(index));

            //If this is selected, draw outline
            if (selectedDigit == index)
                UtilOutline(StaticColors.COLOR_SELECTION, pixel, 0, font.width, Height, 1);
        }

        private int GetDigitPixel(int index)
        {
            return Width - PADDING_H - ((index + 1) * font.width) - ((index / 3) * GROUP_PADDING);
        }

        private DisplayPixel GetColor(int index)
        {
            return (value / (long)Math.Pow(10, index)) > 0 ? COLOR_LIT : COLOR_UNLIT;
        }
    }
}
