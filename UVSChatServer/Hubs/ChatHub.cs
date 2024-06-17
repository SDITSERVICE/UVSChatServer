using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Text;
using System.Threading.Channels;
using UVSMedia.Media;

namespace UVSChatServer.Hubs
{
    public class ChatHub : Hub
    {
        #region TODO : Update

        public static List<string> _groups { get; set; } = new List<string>();
        public static List<string> AllMessages { get; set; } = new List<string>();

        public static Action<string>? ActionMessage;

        #endregion

        public override Task OnConnectedAsync()
        {
            Console.WriteLine(Context.ConnectionId + " connected");
            return base.OnConnectedAsync();
        }

       

        [HubMethodName(nameof(ServerCmd.SendAudio))]
        public async Task SendAudio(byte[] audioData)
        {
            Console.WriteLine($"Data={Utils.ByteArrayToHexString(audioData)}\r\nLength={audioData.Length}");
            await Clients.Others.SendAsync(nameof(ClientCmd.ReceiveAudio), audioData); //Send to everyone who is subscribed to the Receive Audio event
        }

        [HubMethodName(nameof(ServerCmd.SendAudioUnity))]
        public async Task SendAudioUnity(float[] audioData)
        {
            #region Test write to RAW file

            //// Opening a file for writing in binary mode
            //using (FileStream fs = new FileStream("D:\\test.raw", FileMode.Append, FileAccess.Write))
            //{
            //    // write to file
            //    fs.Write(ConvertFloatToByte(audioData), 0, ConvertFloatToByte(audioData).Length);
            //}

            #endregion

            await Clients.Caller.SendAsync(nameof(ClientCmd.ReceiveAudio), audioData);
           
        }

    }
}
