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

        public static List<string> Groups { get; set; } = new List<string>();
        public static List<string> Users { get; set; } = new List<string>();


        public static Action<string>? ActionMessage;

        #endregion

        public override Task OnConnectedAsync()
        {
            if(!Users.Contains(Context.ConnectionId))
                Users.Add(Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if (Users.Contains(Context.ConnectionId))
                Users.Remove(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        [HubMethodName(nameof(ServerCmd.SendAudio))]
        public async Task SendAudio(byte[] audioData)
        {
            Console.WriteLine($"Data={Utils.ByteArrayToHexString(audioData)}\r\nLength={audioData.Length}");
            await Clients.Others.SendAsync(nameof(ClientCmd.ReceiveAudio), audioData); //Send to everyone who is subscribed to the Receive Audio event
        }

        [HubMethodName(nameof(ServerCmd.SendAudioFromId))]
        public async Task SendAudio(byte[] audioData, string id)
        {
            await Clients.Client(id).SendAsync(nameof(ClientCmd.ReceiveAudio), audioData);
        }

        [HubMethodName(nameof(ServerCmd.SendAudioFromIds))]
        public async Task SendAudio(byte[] audioData, string[] ids)
        {
           ids
               .ToList()
               .ForEach(async _id  => 
               {
                   await Clients.Client(_id).SendAsync(nameof(ClientCmd.ReceiveAudio), audioData);
               });
        }


        [HubMethodName(nameof(ServerCmd.SendAudioCaller))]
        public async Task SendAudioCaller(byte[] audioData)
        {
            await Clients.Caller.SendAsync(nameof(ClientCmd.ReceiveAudio), audioData);
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
