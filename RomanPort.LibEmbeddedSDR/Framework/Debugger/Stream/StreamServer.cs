using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace RomanPort.LibEmbeddedSDR.Framework.Debugger.Stream
{
    public class StreamServer
    {
        private IPAddress client;
        private DebuggerServer server;
        private ushort dataOpcode;

        public bool HasClient { get => client != null; }
        public IPAddress Client { get => client; }

        public StreamServer(DebuggerServer server, ushort dataOpcode)
        {
            this.server = server;
            this.dataOpcode = dataOpcode;
        }

        public void SubscribeEvent()
        {
            byte[] addr = new byte[4];
            Array.Copy(server.buffer, DebuggerBase.HEADER_LENGTH, addr, 0, 4);
            client = new IPAddress(addr);
        }

        public void UnsubscribeEvent()
        {
            client = null;
        }

        public unsafe void ReportData<T>(int count, params T*[] channels) where T : unmanaged
        {
            //Check
            if (!HasClient)
                return;
            
            //Get byte buffers
            byte*[] channelsB = new byte*[channels.Length];
            for (int i = 0; i < channels.Length; i++)
                channelsB[i] = (byte*)channels[i];
            count *= channels.Length;
            count *= sizeof(T);

            //Copy
            while (count > 0)
            {
                int copyable = Math.Min(count, DebuggerBase.PAYLOAD_LENGTH);
                copyable -= (copyable % (channels.Length * sizeof(T)));
                for (int i = 0; i < copyable; i += channels.Length * sizeof(T))
                {
                    for(int c = 0; c<channels.Length; c++)
                    {
                        int offset = DebuggerBase.HEADER_LENGTH + i + (sizeof(T) * c);
                        for (int j = 0; j < sizeof(T); j++)
                            server.buffer[offset + j] = channelsB[c][j];
                        channelsB[c] += sizeof(T);
                    }
                }
                count -= copyable;
                server.SendCommand(dataOpcode, copyable, client);
            }
        }
    }
}
