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
        SendAudioUnity,
        SendAudioFromId,
        SendAudioFromIds,
        SendAudioCaller,
        SendMessage,

    }

    public enum ClientCmd
    {
        ReceiveAudio,
        ReceiveAudioFromId,
        ReceiveAudioFromIds,
        ReceiveAudioCaller,
    }

}
