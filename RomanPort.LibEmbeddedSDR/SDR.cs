using RomanPort.LibEmbeddedSDR.Frames;
using RomanPort.LibEmbeddedSDR.Framework;
using RomanPort.LibEmbeddedSDR.Framework.Debugger;
using RomanPort.LibEmbeddedSDR.Framework.Drawing;
using RomanPort.LibEmbeddedSDR.Framework.Drawing.Frames;
using RomanPort.LibEmbeddedSDR.Framework.Input;
using RomanPort.LibSDR.Components;
using RomanPort.LibSDR.Components.Decimators;
using RomanPort.LibSDR.Components.FFT.Generators;
using RomanPort.LibSDR.Components.FFT.Mutators;
using RomanPort.LibSDR.Components.Misc;
using RomanPort.LibSDR.Components.Resamplers.Arbitrary;
using RomanPort.LibSDR.Demodulators;
using RomanPort.LibSDR.Demodulators.Analog;
using RomanPort.LibSDR.Demodulators.Analog.Broadcast;
using RomanPort.LibSDR.Sources.Hardware;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RomanPort.LibEmbeddedSDR
{
    public static unsafe class SDR
    {
        public const int BUFFER_SIZE = 65536;
        public const int OUTPUT_AUDIO_RATE = 48000;

        public const int VERSION_MAJOR = 0;
        public const int VERSION_MINOR = 1;

        static SDR()
        {
            InitRadio();
        }

        /* General */

        private static ISdrDisplay display;
        private static IDeviceNatives device;
        private static DebuggerServer debugger;

        public static ISdrDisplay Display { get => display; }
        public static IDeviceNatives Device { get => device; }

        public static void Configure(ISdrDisplay display, IDeviceNatives device)
        {
            //Set
            SDR.display = display;
            SDR.device = device;

            //Create debugger
            debugger = new DebuggerServer();

            //Init
            device.InitAudio(OUTPUT_AUDIO_RATE, BUFFER_SIZE);
        }

        public static void Run()
        {
            //Set frame
            SwitchActiveFrame(new StartupFrame(display));

            //Run
            while (true)
            {
                //Check if we need to enable special drawing
                foreach (var f in frames)
                    frameChanged = frameChanged || f.IsSemiTransparent;
                
                //Render all
                if(frameChanged)
                {
                    for (int i = 0; i < frames.Count; i++)
                        frames[i].Tick(frameChanged);
                } else
                {
                    ActiveFrame.Tick(frameChanged);
                }

                //Reset
                frameChanged = false;

                //Apply
                display.Apply();
                debugger.ReportFrame(display.GetPixelPointer(0, 0), display.Width, display.Height);

                //Wait
                Thread.Sleep(1000 / 30);
            }
        }

        /* Interface */

        private static List<ISdrFrame> frames = new List<ISdrFrame>();
        private static bool frameChanged;

        public static ISdrFrame ActiveFrame { get => frames.Count == 0 ? null : frames[frames.Count - 1]; }

        public static void ShowFatalError(string title, string body)
        {
            SwitchActiveFrame(new FatalErrorFrame(Display, title, body));
        }

        public static void ShowFatalError(string title, Exception ex)
        {
            string body = $"An unknown exception \"{ex.GetType().Name}\" was thrown: {ex.Message}\n\nThis exception was thrown at {ex.StackTrace}";
            ShowFatalError(title, body);
        }

        public static void SwitchActiveFrame(ISdrFrame frame)
        {
            ActiveFrame?.OnClosed();
            frames.Add(frame);
            frame.OnOpened();
            frameChanged = true;
        }

        public static void CloseActiveFrame()
        {
            ActiveFrame.OnClosed();
            frames.Remove(ActiveFrame);
            ActiveFrame.OnOpened();
            frameChanged = true;
        }

        public static void OnInput(UserInputEventArgs input)
        {
            ActiveFrame.OnInput(input);
        }

        /* Radio */

        public static event EventOnSdrSnrReadingArgs OnSnrReading;

        public static float SampleRate { get => source.SampleRate; set => source.SampleRate = value; }
        public static float Bandwidth { get => bandwidth; set
            {
                bandwidth = value;
                ConfigureRadio();
            }
        }
        public static long CenterFrequency { get => source.CenterFrequency; set => source.CenterFrequency = value; }
        public static int ManualGainLevel { get => source.ManualGainLevel; set => source.ManualGainLevel = value; }
        public static IHardwareSource Source
        {
            get => source;
            set
            {
                //Set
                source = value;

                //Register
                value.OnSampleRateChanged += (float sampleRate) => ConfigureRadio();
                value.OnSamplesAvailable += OnSamplesAvailable;

                //Open
                value.Open(BUFFER_SIZE);
                value.BeginStreaming();
            }
        }
        public static SdrDemodMode DemodMode
        {
            get
            {
                for(int i = 0; i<demodulators.Length; i++)
                {
                    if (activeDemodulator == demodulators[i])
                        return (SdrDemodMode)i;
                }
                throw new Exception("Invalid demodulator set.");
            }
            set
            {
                activeDemodulator = demodulators[(int)value];
                ConfigureRadio();
            }
        }
        public static FFTSmoothener MainFFT { get => fftSmoothener; }
        public static FFTSmoothener MpxFFT { get => mpxFftSmoothener; }
        public static WbFmDemodulator DemodulatorWbFm { get => (WbFmDemodulator)demodulators[0]; }
        public static AmDemodulator DemodulatorAm { get => (AmDemodulator)demodulators[1]; }

        private static IAudioDemodulator[] demodulators = new IAudioDemodulator[]
        {
            new WbFmDemodulator(),
            new AmDemodulator()
        };

        private static IHardwareSource source;
        private static IAudioDemodulator activeDemodulator = DemodulatorWbFm;

        private static float bandwidth = 200000;
        private static float decimatedSampleRate;
        private static float actualOutputAudioRate;

        private static ComplexDecimator iqDecimator;
        private static ArbitraryStereoResampler audioResampler;
        private static System.Timers.Timer snrQuery;

        private static FFTGenerator fft;
        private static FFTSmoothener fftSmoothener;
        private static FFTGenerator mpxFft;
        private static FFTSmoothener mpxFftSmoothener;

        private static Thread secondaryWorkerThread;
        private static AutoResetEvent threadASignal = new AutoResetEvent(true);
        private static AutoResetEvent threadBSignal = new AutoResetEvent(false);
        private static int threadBBufferUsage;

        private static UnsafeBuffer audioBufferA;
        private static float* audioBufferAPtr;
        private static UnsafeBuffer audioBufferB;
        private static float* audioBufferBPtr;
        private static UnsafeBuffer threadBBuffer;
        private static Complex* threadBBufferPtr;

        private static void InitRadio()
        {
            //Create buffers
            threadBBuffer = UnsafeBuffer.Create(BUFFER_SIZE, out threadBBufferPtr);
            audioBufferA = UnsafeBuffer.Create(BUFFER_SIZE, out audioBufferAPtr);
            audioBufferB = UnsafeBuffer.Create(BUFFER_SIZE, out audioBufferBPtr);

            //Create FFTs
            fft = new FFTGenerator(16384, false);
            mpxFft = DemodulatorWbFm.EnableMpxFFT(4096);
            fftSmoothener = new FFTSmoothener(fft);
            mpxFftSmoothener = new FFTSmoothener(mpxFft);

            //Create worker thread
            secondaryWorkerThread = new Thread(RunSecondaryWorkerThread);
            secondaryWorkerThread.IsBackground = true;
            secondaryWorkerThread.Name = "Secondary Worker Thread";
            secondaryWorkerThread.Start();

            //Set timer to query the SNR every once in a while
            snrQuery = new System.Timers.Timer(1000 / 5);
            snrQuery.Elapsed += SnrQuery_Elapsed;
            snrQuery.AutoReset = true;
            snrQuery.Start();
        }

        private static void ConfigureRadio()
        {
            //Set up decimator
            int iqDecimationRate = DecimationUtil.CalculateDecimationRate(SampleRate, bandwidth, out decimatedSampleRate);
            iqDecimator = new ComplexDecimator(SampleRate, bandwidth, iqDecimationRate, 20, bandwidth * 0.05f);

            //Configure demodulator
            actualOutputAudioRate = activeDemodulator.Configure(BUFFER_SIZE, decimatedSampleRate, OUTPUT_AUDIO_RATE);

            //Create resampler
            audioResampler = new ArbitraryStereoResampler(actualOutputAudioRate, OUTPUT_AUDIO_RATE, BUFFER_SIZE);
        }

        private static void OnSamplesAvailable(Complex* samples, int count)
        {
            //Process thread A
            count = ProcessThreadABlock(samples, count);

            //Wait for us to be able to safely transfer to the other buffer
            threadASignal.WaitOne();
            threadASignal.Reset();

            //Copy
            Utils.Memcpy(threadBBufferPtr, samples, count * sizeof(Complex));
            threadBBufferUsage = count;

            //Signal to the other thread that it can run
            threadBSignal.Set();
        }

        private static void RunSecondaryWorkerThread()
        {
            while (true)
            {
                //Wait for signal to become true
                threadBSignal.WaitOne();
                threadBSignal.Reset();

                //Run our thread
                ProcessThreadBBlock(threadBBufferPtr, threadBBufferUsage);

                //Signal to other thread that it can safely copy into our buffer
                threadASignal.Set();
            }
        }

        private static int ProcessThreadABlock(Complex* samples, int count)
        {
            //Add to FFT
            fft.AddSamples(samples, count);

            //Decimate + filter
            count = iqDecimator.Process(samples, count);

            return count;
        }

        private static void ProcessThreadBBlock(Complex* samples, int count)
        {
            //Demodulate
            count = activeDemodulator.DemodulateStereo(samples, audioBufferAPtr, audioBufferBPtr, count);

            //Resampler input
            audioResampler.Input(audioBufferAPtr, audioBufferBPtr, count);

            //Resampler output
            do
            {
                //Process
                count = audioResampler.Output(audioBufferAPtr, audioBufferBPtr, BUFFER_SIZE);

                //Output
                debugger.ReportAudio(audioBufferAPtr, audioBufferBPtr, count);
                device.WriteAudio(audioBufferAPtr, audioBufferBPtr, count);
            } while (count != 0);
        }

        private static void SnrQuery_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //If there's no demodulator, stop
            if (activeDemodulator == null)
                return;

            //Get SNR
            var snr = activeDemodulator.ReadAverageSnr();
            OnSnrReading?.Invoke(snr);
        }
    }

    public delegate void EventOnSdrSnrReadingArgs(SnrReading reading);
}
