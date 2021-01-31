using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.LibEmbeddedSDR
{
    public interface IDeviceNatives
    {
        float GetTempC();
        void InitAudio(int sampleRate, int bufferSize);
        unsafe void WriteAudio(float* left, float* right, int count);
    }
}
