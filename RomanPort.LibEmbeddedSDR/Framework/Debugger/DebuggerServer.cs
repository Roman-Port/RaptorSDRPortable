using RomanPort.LibEmbeddedSDR.Framework.Debugger.Stream;
using RomanPort.LibEmbeddedSDR.Framework.Drawing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Debugger
{
    public class DebuggerServer : DebuggerBase
    {
        public DebuggerServer() : base(UDP_SERVER_PORT)
        {
            audioStreamer = new StreamServer(this, OP_AUDIO_DATA);
            screenStreamer = new StreamServer(this, OP_SCREENCAP_DATA);
        }

        private StreamServer audioStreamer;
        private StreamServer screenStreamer;

        public void SendCommand(ushort opcode, int payloadLength, IPAddress serverAddress)
        {
            //Write opcode
            BitConverter.GetBytes(opcode).CopyTo(buffer, 0);

            //Transmit
            debugger.SendTo(buffer, payloadLength + HEADER_LENGTH, System.Net.Sockets.SocketFlags.None, new IPEndPoint(serverAddress, UDP_CLIENT_PORT));
        }

        public unsafe void ReportAudio(float* left, float* right, int count)
        {
            audioStreamer.ReportData(count, left, right);
        }

        public unsafe void ReportFrame(DisplayPixel* pixel, int width, int height)
        {
            if(screenStreamer.HasClient)
            {
                BitConverter.GetBytes(width).CopyTo(buffer, HEADER_LENGTH + 0);
                BitConverter.GetBytes(height).CopyTo(buffer, HEADER_LENGTH + 4);
                SendCommand(OP_SCREENCAP_FRAME, 8, screenStreamer.Client);
                screenStreamer.ReportData(width * height, pixel);
            }
        }

        protected override void ProcessPayload(int len)
        {
            //Get the opcode
            ushort opcode = BitConverter.ToUInt16(buffer, 0);

            //Switch on this
            switch(opcode)
            {
                case OP_BUTTON_PRESS:
                    SDR.OnInput(new Input.UserInputEventArgs
                    {
                        down = true,
                        key = (Input.UserInputKey)BitConverter.ToUInt16(buffer, HEADER_LENGTH + 0)
                    });
                    break;
                case OP_AUDIO_SUBCRIBE:
                    audioStreamer.SubscribeEvent();
                    break;
                case OP_AUDIO_UNSUBCRIBE:
                    audioStreamer.UnsubscribeEvent();
                    break;
                case OP_SCREENCAP_SUBSCRIBE:
                    screenStreamer.SubscribeEvent();
                    break;
                case OP_SCREENCAP_UNSUBSCRIBE:
                    screenStreamer.UnsubscribeEvent();
                    break;
            }
        }
    }
}
