using System.Diagnostics;
using LumiSoft.Net.Codec;
using LumiSoft.Net.UDP;
using Microsoft.AspNetCore.SignalR.Client;
using UVSChatMedia.Media.Wave;
using UVSMedia.Media;


namespace SampleClient
{
    internal class Program
    {
        private static int m_Codec = 0;
        private static HubConnection m_connection;
        private static WaveOut m_WaveOut;
        private static WaveIn m_WaveIn;
        //private static FileStream m_pRecordStream = null;


        static async Task Main(string[] args)
        {
            m_connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/chat") // replace your URL
                .Build();

            m_connection.On<byte[]>(nameof(ClientCmd.ReceiveAudio), audioData =>
            {
                
                Debug.WriteLine($"Recive fragment {Utils.ByteArrayToHexString(audioData)} frLeg {audioData.Length}");
                m_PacketReceived(audioData);
               
            });

            await m_connection.StartAsync();

            #region Show devices

            //TODO : Create choise
            // WaveIn.Devices
            //     .ToList()
            //     .ForEach(device =>
            //         Console.WriteLine($"In: Name {device.Name}")
            //     );

            // WaveOut.Devices
            //     .ToList()
            //     .ForEach(device =>
            //         Console.WriteLine($"Out: Name {device.Name}")
            //     );
            

            #endregion

            Console.WriteLine("Connection started. Wait other user.\r\nPress any key to exit...");
            await Task.Delay(2000);

            //m_pRecordStream = File.Create($"D:\\test{new Random().Next(1, 10000)}.wav");

            m_WaveOut = new WaveOut(WaveOut.Devices[0], 8000, 16, 1);
            m_WaveIn = new WaveIn(WaveIn.Devices[0], 8000, 16, 1, 256);
            m_WaveIn.BufferFull += new BufferFullHandler(m_pWaveIn_BufferFull);
            m_WaveIn.Start();


            Console.ReadLine();
        }

        private static void m_pWaveIn_BufferFull(byte[] buffer)
        {
            // Compress data. 
            byte[] encodedData = null;
            if (m_Codec == 0)
            {
                encodedData = G711.Encode_aLaw(buffer, 0, buffer.Length);
            }
            else if (m_Codec == 1)
            {
                encodedData = G711.Encode_uLaw(buffer, 0, buffer.Length);
            }

            m_connection.SendAsync(nameof(ServerCmd.SendAudio), encodedData);
            
        }

        //TODO : Change codec as PCMA
        private static void m_PacketReceived(byte[] e)
        {
            // Decompress data.
            byte[] decodedData = null;
            if (m_Codec == 0)
            {
                decodedData = G711.Decode_aLaw(e, 0, e.Length);
            }
            else if (m_Codec == 1)
            {
                decodedData = G711.Decode_uLaw(e, 0, e.Length);
            }

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
