using NAudio.Wave;
using RomanPort.LibEmbeddedSDR.Framework.Debugger;
using RomanPort.LibEmbeddedSDR.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RomanPort.LibEmbeddedSDR.Tools.RemoteDebugger
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            standardColor = serverAddress.ForeColor;
            errorColor = Color.Red;

            //Set up debugger
            debugger = new DebuggerClient(IPAddress.Parse(serverAddress.Text));
            debugger.OnAudioData += Debugger_OnAudioData;

            //Set up audio provider
            audioProvider = new BufferedWaveProvider(WaveFormat.CreateIeeeFloatWaveFormat(48000, 2));
            audioOutput = new WaveOut();
            audioOutput.Init(audioProvider);
            audioOutput.Play();
        }

        private void Debugger_OnAudioData(byte[] buffer, int bufferOffset, int count)
        {
            audioProvider.AddSamples(buffer, bufferOffset, count);
        }

        private DebuggerClient debugger;

        private BufferedWaveProvider audioProvider;
        private WaveOut audioOutput;

        private Color standardColor;
        private Color errorColor;

        private void btnStartAudio_Click(object sender, EventArgs e)
        {
            debugger.BeginAudioStreaming();
        }

        private void btnStopAudio_Click(object sender, EventArgs e)
        {
            debugger.EndAudioStreaming();
        }

        private void serverAddress_TextChanged(object sender, EventArgs e)
        {
            try
            {
                debugger.ServerAddress = IPAddress.Parse(serverAddress.Text);
                serverAddress.ForeColor = standardColor;
            } catch
            {
                serverAddress.ForeColor = errorColor;
            }
        }

        private void buttonControl_Click(object sender, EventArgs e)
        {
            Enum.TryParse<UserInputKey>((string)((Button)sender).Tag, out UserInputKey key);
            debugger.SendKeyDown(key);
        }

        private DisplayRenderer preview;

        private void btnBeginScreenStreaming_Click(object sender, EventArgs e)
        {
            if(preview == null)
            {
                //Open
                preview = new DisplayRenderer(debugger);
                preview.FormClosed += (object s, FormClosedEventArgs ee) => btnBeginScreenStreaming_Click(preview, ee);
                preview.Show();
                btnBeginScreenStreaming.Text = "End Streaming";
            } else
            {
                if(sender != preview)
                    preview.Close();
                preview = null;
                btnBeginScreenStreaming.Text = "Begin Streaming";
            }
        }
    }
}
