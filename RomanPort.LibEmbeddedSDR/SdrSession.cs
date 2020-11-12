using RomanPort.LibEmbeddedSDR.Framework;
using RomanPort.LibEmbeddedSDR.Framework.Input;
using RomanPort.LibEmbeddedSDR.Framework.Radio;
using RomanPort.LibEmbeddedSDR.Framework.Rendering;
using RomanPort.LibEmbeddedSDR.Views;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace RomanPort.LibEmbeddedSDR
{
    public delegate void SdrSessionTimeUpdated(DateTime time);
    
    public unsafe class SdrSession : IUserInputHandler
    {
        public readonly int bufferSize;
        public ISdrDisplay display;
        public RenderingFontStore fontStore;
        public RadioController radio;

        public event SdrSessionTimeUpdated OnSyncTimeUpdated;
        public bool isTimeSet;

        private IView activeView;

        private Stopwatch timeSync;
        private DateTime latestTime;

        public SdrSession(ISdrDisplay display, int bufferSize)
        {
            this.display = display;
            this.bufferSize = bufferSize;

            //Create font store and load fonts
            fontStore = new RenderingFontStore();
            fontStore.LoadFontPackFromResource(RenderingFontTag.SYSTEM_20, "FONT_SYSTEM_REGULAR_20.sdrfnt");
            fontStore.LoadFontPackFromResource(RenderingFontTag.SYSTEM_15, "FONT_SYSTEM_REGULAR_15.sdrfnt");

            //Create stopwatch for keeping the time
            timeSync = new Stopwatch();
            timeSync.Start();

            //Set to startup view. This will create everything for us
            activeView = new StartupView(this);
        }

        public void SwitchActiveView(IView view)
        {
            this.activeView = view;
        }

        public void AbortWithError(string title, string message)
        {
            SwitchActiveView(new ErrorView(this, title, message));
        }

        public void AbortWithError(string title, Exception ex)
        {
            AbortWithError(title, $"Unhandled exception \"{ex.GetType().Name}\": {ex.Message}{ex.StackTrace}");
        }

        public RenderingCanvas GetNewCanvas(ICanvasBackgroundProvider background)
        {
            return new RenderingCanvas(display, background, fontStore);
        }

        public void OnUserInput(UserInputKey key)
        {
            activeView.OnUserInput(key);
        }

        public void SetTime(DateTime localTime)
        {
            timeSync.Restart();
            latestTime = localTime;
            isTimeSet = true;
            OnSyncTimeUpdated?.Invoke(localTime);
        }

        public bool TryGetTime(out DateTime time)
        {
            if(!isTimeSet)
            {
                time = new DateTime(2020, 1, 1);
                return false;
            }
            time = latestTime.Add(timeSync.Elapsed);
            return true;
        }

        public void TuneRadio(long freq)
        {
            radio.source.CenterFrequency = freq;
            radio.demodulatorWbFm.UseRds().Reset();
        }

        public void Run()
        {
            while(true)
            {
                activeView.Tick();
                if(activeView.canvas.invalidated)
                    activeView.UiTick();
            }
        }

        public static Stream UtilGetAssetStream(string name)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("RomanPort.LibEmbeddedSDR.Assets." + name);
        }

        public static byte[] UtilGetAssetBytes(string name)
        {
            byte[] data;
            using (Stream stream = UtilGetAssetStream(name))
            {
                data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
            }
            return data;
        }
    }
}
