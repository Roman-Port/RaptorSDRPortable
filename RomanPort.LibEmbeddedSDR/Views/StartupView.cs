using RomanPort.LibEmbeddedSDR.Framework;
using RomanPort.LibEmbeddedSDR.Framework.Display;
using RomanPort.LibEmbeddedSDR.Framework.Input;
using RomanPort.LibEmbeddedSDR.Framework.Radio;
using RomanPort.LibEmbeddedSDR.Framework.Rendering;
using RomanPort.LibEmbeddedSDR.Framework.Rendering.Backgrounds;
using RomanPort.LibEmbeddedSDR.Framework.Rendering.Image;
using RomanPort.LibEmbeddedSDR.Framework.StartupSteps;
using RomanPort.LibEmbeddedSDR.Framework.UI.Components.Common;
using RomanPort.LibSDR.Framework.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Views
{
    public class StartupView : IView
    {
        private ImageView viewLogo;
        private ProgressBarView viewProgress;
        private SinglelineTextView viewStatus;

        private IStartupStep[] steps;
        private int currentSetupStepIndex = 0;

        public const int PROGRESS_WIDTH = 300;

        public StartupView(SdrSession session) : base(session, new BlackBackground())
        {
            //Create image
            var img = SdrImgFile.LoadFromAsset("IMG_STARTUP_LOGO.sdrimg");
            viewLogo = new ImageView(canvas, canvas.GetHorizCenteredLocation(img.width), 100, img.width, img.height, img);

            //Make steps
            steps = new IStartupStep[]
            {
                new CreateRadioStep(),
                new SyncTimeStep()
            };

            //Create other views
            viewProgress = new ProgressBarView(canvas, canvas.GetHorizCenteredLocation(PROGRESS_WIDTH), session.display.height - 130, PROGRESS_WIDTH, 30, new DisplayPixel(255, 255, 255), StaticColors.COLOR_ACCENT, steps.Length + 1);
            viewProgress.progress = 1;
            viewStatus = new SinglelineTextView(canvas, 0, session.display.height - 170, session.display.width, 15, canvas.GetFontPack(RenderingFontTag.SYSTEM_20), "Preparing...", new DisplayPixel(255, 255, 255), SinglelineTextView.TextViewAlign.Center);
        }
        
        public override void OnUserInput(UserInputKey key)
        {
            steps[currentSetupStepIndex].OnUserInput(key);
        }

        public override void Tick()
        {
            if(currentSetupStepIndex >= steps.Length)
            {
                //Finished
                session.SwitchActiveView(new SDRView(session));
                return;
            }
            
            viewProgress.progress = currentSetupStepIndex + 1;
            viewStatus.SetText(steps[currentSetupStepIndex].GetName());

            canvas.FullRedraw(true);

            if (!steps[currentSetupStepIndex].Work(session, out string errorText))
            {
                session.AbortWithError(steps[currentSetupStepIndex].GetErrorTitle(), errorText);
                return;
            }
            currentSetupStepIndex++;
        }

        public override void UiTick()
        {
            
        }

        private void CreateRadio()
        {
            
        }
    }
}
