using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Debugger
{
    public abstract class DebuggerBase
    {
        public const int UDP_SERVER_PORT = 43166;
        public const int UDP_CLIENT_PORT = 43165;
        public const int HEADER_LENGTH = 2;
        public const int PAYLOAD_LENGTH = 16384;
        public const int BUFFER_LENGTH = HEADER_LENGTH + PAYLOAD_LENGTH;

        public const ushort OP_BUTTON_PRESS = 1;
        public const ushort OP_AUDIO_SUBCRIBE = 2;
        public const ushort OP_AUDIO_UNSUBCRIBE = 3;
        public const ushort OP_AUDIO_DATA = 4;
        public const ushort OP_SCREENCAP_SUBSCRIBE = 5;
        public const ushort OP_SCREENCAP_UNSUBSCRIBE = 6;
        public const ushort OP_SCREENCAP_DATA = 7;
        public const ushort OP_SCREENCAP_FRAME = 8;

        public IPEndPoint endpoint;
        public Socket debugger;
        public byte[] buffer;

        public DebuggerBase(int port)
        {
            buffer = new byte[BUFFER_LENGTH];
            endpoint = new IPEndPoint(IPAddress.Any, port);
            debugger = new Socket(SocketType.Dgram, ProtocolType.Udp);
            debugger.Bind(endpoint);
            debugger.BeginReceive(buffer, 0, BUFFER_LENGTH, SocketFlags.None, OnDebuggerReceiveData, null);
        }

        protected IPAddress GetLocalAddress()
        {
            IPAddress addr;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                addr = endPoint.Address;
            }
            return addr;
        }

        void OnDebuggerReceiveData(IAsyncResult ar)
        {
            //Get
            int read = debugger.EndReceive(ar);

            //Process
            ProcessPayload(read);

            //Read
            debugger.BeginReceive(buffer, 0, BUFFER_LENGTH, SocketFlags.None, OnDebuggerReceiveData, null);
        }

        protected abstract void ProcessPayload(int read);
    }
}
