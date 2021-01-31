using RomanPort.LibSDR.Components.IO.WAV;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.LinuxCMD
{
    public class EmbeddedDevice : IDeviceNatives
    {
        public float GetTempC()
        {
            return 0;
        }

        private WavFileWriter test;

        public void InitAudio(int sampleRate, int bufferSize)
        {
            test = new WavFileWriter("/home/pi/test.wav", FileMode.Create, sampleRate, 2, 16, bufferSize);
        }

        public unsafe void WriteAudio(float* left, float* right, int count)
        {
            test.Write(left, right, count);
        }
    }
}
