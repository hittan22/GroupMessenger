using ChatClient.Net.IO;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ChatClient.Net
{
    class Server
    {
        TcpClient _client;
        public PacketReader packetReader;

        public event Action connectedEvent;
        public event Action msgReceivedEvent;
        public event Action UserDisconnectedEvent;
        public Server()
        {
            _client = new TcpClient();
        }

        public void ConnectToServer(string username)
        {
            if (!_client.Connected)
            {
                _client.Connect(IPAddress.Parse("127.0.0.1"), 7262);
                packetReader = new PacketReader(_client.GetStream());

                if (!string.IsNullOrEmpty(username))
                {
                    var connectPacket = new PacketBuilder();
                    connectPacket.WriteOpCode(0);
                    connectPacket.WriteMessage(username);
                    _client.Client.Send(connectPacket.GetPacketBytes());
                    var opcode = packetReader.ReadByte();

                }
                ReadPackets();

            }
        }

        private void ReadPackets()
        {
            Task.Run(() =>
            {
                while (0 == 0)
                {
                    var opcode = packetReader.ReadByte();
                    switch (opcode)
                    {
                        case 1:
                            connectedEvent?.Invoke();
                            break;
                        case 5:
                            msgReceivedEvent?.Invoke();
                            break;
                        case 10:
                            UserDisconnectedEvent?.Invoke();
                            break;
                        default:
                            Console.WriteLine("no.");
                            break;
                    }
                }
            });
        }

        public void SendMessageToServer(string message)
        {
            var messagePacker = new PacketBuilder();
            messagePacker.WriteOpCode(5);
            messagePacker.WriteMessage(message);
            _client.Client.Send(messagePacker.GetPacketBytes());
        }
    }
}
