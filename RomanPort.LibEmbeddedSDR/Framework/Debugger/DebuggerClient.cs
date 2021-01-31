using RomanPort.LibEmbeddedSDR.Framework.Input;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Debugger
{
    public class DebuggerClient : DebuggerBase
    {
        public DebuggerClient(IPAddress ip) : base(UDP_CLIENT_PORT)
        {
            ServerAddress = ip;
        }

        public IPAddress ServerAddress { get; set; }

        public event StreamDataEventArgs OnAudioData;
        public event StreamDataEventArgs OnScreencapData;
        public event ScreencapFrameStartedArgs OnScreencapFrameStarted;

        public delegate void StreamDataEventArgs(byte[] buffer, int bufferOffset, int count);
        public delegate void ScreencapFrameStartedArgs(int width, int height);

        private void SendCommand(ushort opcode, int payloadLength)
        {
            //Write opcode
            BitConverter.GetBytes(opcode).CopyTo(buffer, 0);

            //Transmit
            debugger.SendTo(buffer, payloadLength + HEADER_LENGTH, System.Net.Sockets.SocketFlags.None, new IPEndPoint(ServerAddress, UDP_SERVER_PORT));
        }

        public void SendKeyDown(UserInputKey key)
        {
            BitConverter.GetBytes((ushort)key).CopyTo(buffer, HEADER_LENGTH + 0);
            SendCommand(OP_BUTTON_PRESS, 2);
        }

        public void BeginAudioStreaming()
        {
            GetLocalAddress().GetAddressBytes().CopyTo(buffer, HEADER_LENGTH);
            SendCommand(OP_AUDIO_SUBCRIBE, 4);
        }

        public void EndAudioStreaming()
        {
            SendCommand(OP_AUDIO_UNSUBCRIBE, 0);
        }

        public void BeginScreenStreaming()
        {
            GetLocalAddress().GetAddressBytes().CopyTo(buffer, HEADER_LENGTH);
            SendCommand(OP_SCREENCAP_SUBSCRIBE, 4);
        }

        public void EndScreenStreaming()
        {
            SendCommand(OP_SCREENCAP_UNSUBSCRIBE, 0);
        }

        protected override void ProcessPayload(int read)
        {
            //Get the opcode
            ushort opcode = BitConverter.ToUInt16(buffer, 0);

            //Switch on this
            switch (opcode)
            {
                case OP_AUDIO_DATA:
                    OnAudioData?.Invoke(buffer, HEADER_LENGTH, read - HEADER_LENGTH);
                    break;
                case OP_SCREENCAP_DATA:
                    OnScreencapData?.Invoke(buffer, HEADER_LENGTH, read - HEADER_LENGTH);
                    break;
                case OP_SCREENCAP_FRAME:
                    OnScreencapFrameStarted?.Invoke(BitConverter.ToInt32(buffer, HEADER_LENGTH + 0), BitConverter.ToInt32(buffer, HEADER_LENGTH + 4));
                    break;
            }
        }
    }
}
