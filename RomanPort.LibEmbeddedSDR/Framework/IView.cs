using RomanPort.LibEmbeddedSDR.Framework.Input;
using RomanPort.LibEmbeddedSDR.Framework.Radio;
using RomanPort.LibEmbeddedSDR.Framework.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework
{
    public abstract class IView : IUserInputHandler
    {
        public readonly SdrSession session;
        public readonly RenderingCanvas canvas;
        public readonly ISdrDisplay display;
        public readonly RadioController radio;

        public IView(SdrSession session, ICanvasBackgroundProvider background)
        {
            this.session = session;
            canvas = session.GetNewCanvas(background);
            display = session.display;
            radio = session.radio;
        }

        public abstract void OnUserInput(UserInputKey key);
        public abstract void Tick();
        public abstract void UiTick();
    }
}
