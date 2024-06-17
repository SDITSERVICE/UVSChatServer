using LumiSoft.Net.Media.Codec.Audio;
using Microsoft.AspNetCore.SignalR.Client;
using System.Diagnostics;
using UVSChatMedia.Media.Wave;
using UVSMedia.Media;


namespace SampleClient
{
    internal class Program
    {
        private static HubConnection m_connection;
        private static WaveOut m_WaveOut;
        private static WaveIn m_WaveIn;
        //private static FileStream m_pRecordStream = null;


        static async Task Main(string[] args)
        {
            Console.WriteLine("Enter ip address. Default 127.0.0.1");
            var inputIp = Console.ReadLine();
            var ip = string.IsNullOrEmpty(inputIp) ? "127.0.0.1" : inputIp;


            Console.WriteLine("Enter port. Default 5000");
            var inputPortp = Console.ReadLine();
            var port = string.IsNullOrEmpty(inputPortp) ? "5000" : inputPortp;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Connecting");


            await Task.Delay(500);
            Console.Write(".");
            await Task.Delay(500);
            Console.Write(".");
            await Task.Delay(500);
            Console.Write(".");


            Console.ForegroundColor = ConsoleColor.White;


            m_connection = new HubConnectionBuilder()
                .WithUrl($"http://{ip}:{port}/chat") // replace your URL
                .Build();

            m_connection.On<byte[]>(nameof(ClientCmd.ReceiveAudio), audioData =>
            {

                Debug.WriteLine($"Recive fragment {Utils.ByteArrayToHexString(audioData)} frLeg {audioData.Length}");
                m_PacketReceived(audioData);

            });


            await Task.Delay(500);
            Console.Write(".");
            await Task.Delay(500);
            Console.WriteLine(".");

            try
            {
                await m_connection.StartAsync();
            }

            catch (Exception ex) { Console.WriteLine(ex.Message); }

            #region Show devices

            Console.WriteLine();
            Console.WriteLine($"Find devices");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            //TODO : Create choise
            WaveIn.Devices
                .ToList()
                .ForEach(device =>
                    Console.WriteLine($"Mic: name {device.Name}")
                );

            WaveOut.Devices
                .ToList()
                .ForEach(device =>
                    Console.WriteLine($"Speak's: name {device.Name}")
                );
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;

            #endregion

            if (m_connection.State == HubConnectionState.Connected)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Connection successful.");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Нажми любую клавишу чтобы выйти...");

            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Connection to server refused");
                Console.ForegroundColor = ConsoleColor.White;
            }

            await Task.Delay(2000);

            //m_pRecordStream = File.Create($"D:\\test{new Random().Next(1, 10000)}.wav");

            m_WaveOut = new WaveOut(WaveOut.Devices[0], 16000, 16, 1);
            m_WaveIn = new WaveIn(WaveIn.Devices[0], 16000, 16, 1, 256);
            m_WaveIn.BufferFull += new BufferFullHandler(m_pWaveIn_BufferFull);
            m_WaveIn.Start();


            Console.ReadLine();
        }

        private static void m_pWaveIn_BufferFull(byte[] buffer)
        {
            PCMA pcma = new PCMA();
            // Compress data. 
            byte[] encodedData = null;

            //encodedData = G711.Encode_aLaw(buffer, 0, buffer.Length);
            encodedData = pcma.Encode(buffer, 0, buffer.Length);


            m_connection.SendAsync(nameof(ServerCmd.SendAudio), encodedData);

        }

        //PATCH : Change codec as PCMA
        private static void m_PacketReceived(byte[] e)
        {
            PCMA pcma = new PCMA();
            // Decompress data.
            byte[] decodedData = null;

            //decodedData = G711.Decode_aLaw(e, 0, e.Length);
            decodedData = pcma.Decode(e, 0, e.Length);

            // We just play received packet.
            m_WaveOut.Play(decodedData, 0, decodedData.Length);

            //TODO: ?
            // Record if recoring enabled.
            //if (m_pRecordStream != null)
            //{
            //    m_pRecordStream.Write(decodedData, 0, decodedData.Length);
            //}
        }
    }
}
