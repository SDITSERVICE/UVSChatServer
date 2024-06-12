using Microsoft.AspNetCore.SignalR;
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

        //[HubMethodName(nameof(ServerCmd.SendAudio))]
        //public async Task SendAudio(PacketAudio audioData)
        //{
        //    //Console.WriteLine($"Data={ByteArrayToHexString(audioData.packet ?? new byte[] {0xAA})}\r\nLength={audioData.count}");
        //    await Clients.Others.SendAsync("ReceiveAudio", audioData);
        //}

        [HubMethodName(nameof(ServerCmd.SendAudio))]
        public async Task SendAudio(byte[] audioData)
        {
            Debug.WriteLine($"Data={Utils.ByteArrayToHexString(audioData)}\r\nLength={audioData.Length}");
            await Clients.Others.SendAsync(nameof(ClientCmd.ReceiveAudio), audioData);
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
