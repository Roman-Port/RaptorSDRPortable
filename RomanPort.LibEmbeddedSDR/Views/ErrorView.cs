using RomanPort.LibEmbeddedSDR.Framework;
using RomanPort.LibEmbeddedSDR.Framework.Display;
using RomanPort.LibEmbeddedSDR.Framework.Input;
using RomanPort.LibEmbeddedSDR.Framework.Rendering;
using RomanPort.LibEmbeddedSDR.Framework.Rendering.Backgrounds;
using RomanPort.LibEmbeddedSDR.Framework.UI.Components.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Views
{
    public class ErrorView : IView
    {
        private SinglelineTextView viewTitle;
        private MultilineTextView viewText;

        public const int PADDING = 15;
        
        public ErrorView(SdrSession session, string title, string message) : base(session, new BottomGradientBackground(180, 255, 77, 77))
        {
            viewTitle = new SinglelineTextView(canvas, PADDING, PADDING, session.display.width - PADDING - PADDING, session.display.height - PADDING - PADDING, canvas.GetFontPack(RenderingFontTag.SYSTEM_20), title.ToUpper(), new DisplayPixel(255, 110, 110));
            viewText = new MultilineTextView(canvas, PADDING, PADDING + 30, session.display.width - PADDING - PADDING, session.display.height - PADDING - PADDING, canvas.GetFontPack(RenderingFontTag.SYSTEM_20), message, new DisplayPixel(255, 255, 255));
        }

        public override void OnUserInput(UserInputKey key)
        {
            
        }

        public override void Tick()
        {
            
        }

        public override void UiTick()
        {
            canvas.FullRedraw();
        }
    }
}
