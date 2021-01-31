using NAudio.Wave;
using RomanPort.LibEmbeddedSDR.Framework;
using RomanPort.LibEmbeddedSDR.Framework.Input;
using RomanPort.LibSDR.Components.IO.WAV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RomanPort.LibEmbeddedSDR.EmulatedScreen
{
    public partial class Form1 : Form, IDeviceNatives
    {
        private Thread worker;
        
        public Form1()
        {
            InitializeComponent();
            SDR.Configure(emulatedDisplay1, this);
            worker = new Thread(SDR.Run);
            worker.Name = "SDR Worker";
            worker.IsBackground = true;
            worker.Start();
        }

        private void btnForceRedraw_Click(object sender, EventArgs e)
        {
            
        }

        private void OnInputControlBtnClicked(object sender, EventArgs e)
        {
            Enum.TryParse<UserInputKey>((string)((Button)sender).Tag, out UserInputKey key);
            SDR.OnInput(new UserInputEventArgs
            {
                down = true,
                key = key
            });
        }

        private void btnScreencap_Click(object sender, EventArgs e)
        {
            //Copy
            Bitmap b = emulatedDisplay1.GetSnapshot();

            //Prompt
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "Screenshot Output";
            dialog.Filter = "PNG Files (*.png)|*.png";
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            //Write
            using(FileStream fs = new FileStream(dialog.FileName, FileMode.Create))
                b.Save(fs, System.Drawing.Imaging.ImageFormat.Png);

            //Clean up
            b.Dispose();
        }

        public float GetTempC()
        {
            return 0;
        }

        private BufferedWaveProvider audioProvider;
        private WaveOut audioOutput;
        private byte[] audioOutputBuffer;
        private GCHandle audioOutputHandle;
        private unsafe float* audioOutputPtr;
        private float audioVolume = 0.8f;

        public unsafe void InitAudio(int sampleRate, int bufferSize)
        {
            //Set up audio provider
            audioProvider = new BufferedWaveProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 2));
            audioOutput = new WaveOut();
            audioOutput.Init(audioProvider);
            audioOutput.Play();

            //Create buffer
            audioOutputBuffer = new byte[bufferSize * 2 * sizeof(float)];
            audioOutputHandle = GCHandle.Alloc(audioOutputBuffer, GCHandleType.Pinned);
            audioOutputPtr = (float*)audioOutputHandle.AddrOfPinnedObject();

            //Create WAV file
            test = new WavFileWriter("F:\\test_audio.wav", FileMode.Create, sampleRate, 2, 16, bufferSize);
        }

        private WavFileWriter test;

        public unsafe void WriteAudio(float* left, float* right, int count)
        {
            //Write to file
            test.Write(left, right, count);
            
            //Copy and zipper
            for(int i = 0; i<count; i++)
            {
                audioOutputPtr[(i * 2)] = *left * audioVolume;
                audioOutputPtr[(i * 2) + 1] = *right * audioVolume;
                left++;
                right++;
            }

            //Write
            audioProvider.AddSamples(audioOutputBuffer, 0, count * 2 * sizeof(float));
        }

        private void volumeBar_Scroll(object sender, EventArgs e)
        {
            audioVolume = ((float)volumeBar.Value) / 10f;
        }
    }
}
