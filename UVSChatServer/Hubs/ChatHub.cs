using Microsoft.AspNetCore.SignalR;
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

        public async Task SendAudio(PacketAudio audioData)
        {
            await Clients.Others.SendAsync("ReceiveAudio", audioData);
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

        public static string ByteArrayToHexString(byte[] byteArray)
        {
            StringBuilder hexString = new StringBuilder(byteArray.Length * 4);
            foreach (byte b in byteArray)
            {
                hexString.AppendFormat("0x{0:X2} ", b);
            }
            return hexString.ToString().Trim();
        }
    }
}
