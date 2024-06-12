using System;
using System.Collections.Generic;
using System.Text;

namespace UVSMedia.Media
{
    [Serializable]
    public struct PacketAudio
    {
        public byte[] packet;
        public int offset;
        public int count;
    }

    public enum ServerCmd
    {
        SendAudio,

    }

    public enum ClientCmd
    {
        ReceiveAudio
    }
}
