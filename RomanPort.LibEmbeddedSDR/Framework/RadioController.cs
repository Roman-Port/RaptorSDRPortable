using RomanPort.LibSDR.Demodulators;
using RomanPort.LibSDR.Extras;
using RomanPort.LibSDR.Framework;
using RomanPort.LibSDR.Framework.FFT;
using RomanPort.LibSDR.Framework.Multithreading;
using RomanPort.LibSDR.Framework.Resamplers.Decimators;
using RomanPort.LibSDR.Framework.Util;
using RomanPort.LibSDR.Sources;
using RomanPort.LibSDR.Sources.Hardware;
using RomanPort.LibSDR.Sources.Hardware.RTLSDR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Radio
{
    public unsafe class RadioController
    {
        public readonly int bufferSize;
        public readonly int fftScaledSize;

        public ComplexFftView fft;

        private MultithreadWorker worker;

        public IHardwareSource source;
        public float sourceSampleRate;
        public int sourceIqDecimationRate;
        public float demodulationSampleRate;
        public float decimatedSourceSampleRate;
        public float audioSampleRate;

        private SdrComplexDecimator iqDecimator;
        private IQFirFilter iqFilter;

        private UnsafeBuffer iqBuffer;
        private Complex* iqBufferPtr;

        private UnsafeBuffer fftBuffer;
        private float* fftBufferPtr;
        private UnsafeBuffer fftScaledBuffer;
        private float* fftScaledBufferPtr;

        //Audio related stuff
        private UnsafeBuffer audioBufferA;
        private float* audioBufferAPtr;
        private UnsafeBuffer audioBufferB;
        private float* audioBufferBPtr;
        private UnsafeBuffer audioBufferC;
        private float* audioBufferCPtr;

        private SdrFloatDecimator audioDecimatorL;
        private SdrFloatDecimator audioDecimatorR;
        private FloatArbResampler audioResamplerL;
        private FloatArbResampler audioResamplerR;

        public WbFmDemodulator demodulatorWbFm;

        private WavEncoder test2;

        public RadioController(int bufferSize, int fftScaledSize)
        {
            this.bufferSize = bufferSize;
            this.fftScaledSize = fftScaledSize;
            demodulationSampleRate = 250000;
            audioSampleRate = 32000;
        }

        public void OpenSource()
        {
            //Open source
            source = new RtlSdrSource(0, 900001, 8);
            sourceSampleRate = source.Open(bufferSize);
        }

        public void OpenBuffers()
        {
            //Create FFT
            fft = new ComplexFftView(512, 40);

            //Create IQ decimator
            sourceIqDecimationRate = SdrFloatDecimator.CalculateDecimationRate(sourceSampleRate, demodulationSampleRate, out decimatedSourceSampleRate);
            iqDecimator = new SdrComplexDecimator(sourceIqDecimationRate);

            //Create IQ filter
            var coefficients = FilterBuilder.MakeBandPassKernel(sourceSampleRate, 5, 0, (int)(demodulationSampleRate / 2), WindowType.BlackmanHarris4);
            iqFilter = new IQFirFilter(coefficients, worker, sourceIqDecimationRate);

            //Create IQ buffers
            iqBuffer = UnsafeBuffer.Create(bufferSize, sizeof(Complex));
            iqBufferPtr = (Complex*)iqBuffer;

            //Create FFT buffers
            fftBuffer = UnsafeBuffer.Create(fft.fftBins, sizeof(float));
            fftBufferPtr = (float*)fftBuffer;
            fftScaledBuffer = UnsafeBuffer.Create(fftScaledSize, sizeof(float));
            fftScaledBufferPtr = (float*)fftScaledBuffer;

            //Create audio buffers
            audioBufferA = UnsafeBuffer.Create(bufferSize, sizeof(float));
            audioBufferB = UnsafeBuffer.Create(bufferSize, sizeof(float));
            audioBufferC = UnsafeBuffer.Create(bufferSize, sizeof(float));
            audioBufferAPtr = (float*)audioBufferA;
            audioBufferCPtr = (float*)audioBufferB;
            audioBufferBPtr = (float*)audioBufferC;

            //Determine source's audio decimation rate
            int sourceAudioDecimationRate = SdrFloatDecimator.CalculateDecimationRate(decimatedSourceSampleRate, audioSampleRate, out float decimatedAudioSampleRate);

            //Create audio resamplers to resample from the decimated rate to the actual rate
            audioResamplerL = new FloatArbResampler(decimatedAudioSampleRate, audioSampleRate, 1, 0);
            audioResamplerR = new FloatArbResampler(decimatedAudioSampleRate, audioSampleRate, 1, 0);

            //Create demodulators
            demodulatorWbFm = new WbFmDemodulator(sourceAudioDecimationRate, true);
            demodulatorWbFm.OnAttached(bufferSize);
            demodulatorWbFm.OnInputSampleRateChanged(decimatedSourceSampleRate);

            try
            {
                test2 = new WavEncoder(new FileStream("/home/pi/test.wav", FileMode.Create), 32000, 2, 16);
            } catch
            {
                test2 = new WavEncoder(new FileStream("E:\\test.wav", FileMode.Create), 32000, 2, 16);
            }
        }

        public int ProcessBlock()
        {
            //Read
            int read = source.Read(iqBufferPtr, bufferSize);

            //Process FFT
            /*for (int i = 0; i < read; i += fft.fftBins * 5)
                fft.ProcessSamples(iqBufferPtr + i);*/
            for(int i = 0; i<3; i++)
                fft.ProcessSamples(iqBufferPtr);

            //Filter and decimate
            iqFilter.Process(iqBufferPtr, read);
            int decimatedRead = read / sourceIqDecimationRate;

            //Demodulate into audio A and B buffers
            int demodulatedRead = demodulatorWbFm.DemodulateStereo(iqBufferPtr, audioBufferAPtr, audioBufferBPtr, decimatedRead);

            //Decimate and resample A -> C
            int audioResampledRead = audioResamplerL.Process(audioBufferAPtr, demodulatedRead, audioBufferCPtr, bufferSize, false);

            //Decimate and resample B -> A
            audioResamplerR.Process(audioBufferBPtr, demodulatedRead, audioBufferAPtr, bufferSize, false);

            test2.Write(audioBufferCPtr, audioBufferAPtr, audioResampledRead);

            return read;
        }

        public void ClearBuffers()
        {

        }

        public float* GetScaledFFTFrame()
        {
            //Read FFT frame
            fft.GetFFTSnapshot(fftBufferPtr);

            //Scale
            float scale = fft.fftBins / (float)fftScaledSize;
            for (int i = 0; i < fftScaledSize; i++)
                fftScaledBufferPtr[i] = fftBufferPtr[(int)(i * scale)];

            return fftScaledBufferPtr;
        }
    }
}
