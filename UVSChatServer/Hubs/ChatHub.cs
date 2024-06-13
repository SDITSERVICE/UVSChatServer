﻿using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Text;
using System.Threading.Channels;
using UVSMedia.Media;

namespace UVSChatServer.Hubs
{
    public class ChatHub : Hub
    {
        public static List<string> _groups { get; set; } = new List<string>();
        public static List<string> AllMessages { get; set; } = new List<string>();

        public static Action<string>? ActionMessage;


        public override Task OnConnectedAsync()
        {
            Console.WriteLine(Context.ConnectionId + " connected");
            return base.OnConnectedAsync();
        }

       

        [HubMethodName(nameof(ServerCmd.SendAudio))]
        public async Task SendAudio(byte[] audioData)
        {
            Console.WriteLine($"Data={Utils.ByteArrayToHexString(audioData)}\r\nLength={audioData.Length}");
            //await Clients.Caller.SendAsync(nameof(ClientCmd.ReceiveAudio), audioData); //Отправить всем кто подписан на событие ReciveAudio
            await Clients.Others.SendAsync(nameof(ClientCmd.ReceiveAudio), audioData); //Отправить всем кто подписан на событие ReciveAudio
        }

        [HubMethodName(nameof(ServerCmd.SendAudioUnity))]
        public async Task SendAudioUnity(float[] audioData)
        {
            //// Открываем файл для записи в бинарном режиме
            //using (FileStream fs = new FileStream("D:\\test.raw", FileMode.Append, FileAccess.Write))
            //{
            //    // Записываем массив байт в файл
            //    fs.Write(ConvertFloatToByte(audioData), 0, ConvertFloatToByte(audioData).Length);
            //}

            //Console.WriteLine($"Data={string.Join(" ",audioData)}\r\nLength={audioData.Length}");
            await Clients.Caller.SendAsync(nameof(ClientCmd.ReceiveAudio), audioData); //Отправить всем кто подписан на событие ReciveAudio
            //await Clients.Others.SendAsync(nameof(ClientCmd.ReceiveAudio), audioData); //Отправить всем кто подписан на событие ReciveAudio
        }

        private byte[] ConvertFloatToByte(float[] array)
        {
            byte[] byteArr = new byte[array.Length * 4];
            for (int i = 0; i < array.Length; i++)
            {
                var bytes = BitConverter.GetBytes(array[i] * 0x80000000);
                Array.Copy(bytes, 0, byteArr, i * 4, bytes.Length);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(byteArr, i * 4, 4);
            }
            return byteArr;
        }

        public ChannelReader<byte[]> UploadStream(ChannelReader<byte[]> stream)
        {
            _ = Task.Run(async () =>
            {
                while (await stream.WaitToReadAsync())
                {
                    while (stream.TryRead(out var item))
                    {
                        // Отправить полученные данные всем клиентам
                        await Clients.Others.SendAsync("ReceiveAudioPacket", item);
                    }
                }
            });

            var channel = Channel.CreateUnbounded<byte[]>();
            return channel.Reader;
        }

    }
}
