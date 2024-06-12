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
        private static HubConnection _connection;
        private static WaveOut m_WaveOut;
        private static WaveIn m_WaveIn;
        private static FileStream m_pRecordStream = null;


        static async Task Main(string[] args)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/audioHub") // Замените на ваш URL
                .Build();

            connection.On<byte[]>("ReceiveAudio", audioData =>
            {
               // PlayAudio(audioData);
            });

            await connection.StartAsync();

            Console.WriteLine("Connection started. Press any key to exit...");

            foreach (WavInDevice device in WaveIn.Devices)
            {
                Console.WriteLine($"Name {device.Name} | Channel {device.Channels}");
            }

            m_pRecordStream = File.Create("D:\\test.wav");

            m_WaveOut = new WaveOut(WaveOut.Devices[0], 8000, 16, 1);
            m_WaveIn = new WaveIn(WaveIn.Devices[0], 8000, 16, 1, 400);
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

            PacketAudio packet = new PacketAudio();
            packet.packet = encodedData;
            packet.offset = 0;
            packet.count = encodedData.Length;

            _connection.SendAsync("SendAudio", packet);
            _connection.StopAsync();
            // We just sent buffer to target end point.
            //m_pUdpServer.SendPacket(encodedData, 0, encodedData.Length, m_pTargetEP);
        }

        private void m_pUdpServer_PacketReceived(PacketAudio e)
        {
            // Decompress data.
            byte[] decodedData = null;
            if (m_Codec == 0)
            {
                decodedData = G711.Decode_aLaw(e.packet, 0, e.packet.Length);
            }
            else if (m_Codec == 1)
            {
                decodedData = G711.Decode_uLaw(e.packet, 0, e.packet.Length);
            }

            // We just play received packet.
            m_WaveOut.Play(decodedData, 0, decodedData.Length);

            // Record if recoring enabled.
            if (m_pRecordStream != null)
            {
                m_pRecordStream.Write(decodedData, 0, decodedData.Length);
            }
        }
    }
}
