using ChatServer.Net.IO;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ChatServer
{
    class Client
    {
        public string Username { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }

        PacketReader _packetReader;
        PacketBuilder _packetBuilder;

        public Client(TcpClient client)
        {
            ClientSocket = client;
            UID = Guid.NewGuid();
            _packetReader = new PacketReader(ClientSocket.GetStream());
            _packetBuilder = new PacketBuilder();

            var opcode = _packetReader.ReadByte();
            Username = _packetReader.ReadMessage();

            _packetBuilder.WriteOpCode(8);
            ClientSocket.Client.Send(_packetBuilder.GetPacketBytes());
            Console.WriteLine($"[{DateTime.Now}]: Client has connected with the username: {Username}");

            Task.Run(() => Process());
        }

        void Process()
        {
            while (0 == 0)
            {
                try
                {
                    var opcode = _packetReader.ReadByte();
                    switch (opcode)
                    {
                        case 5:
                            var msg = _packetReader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now}] {Username}: {msg}");
                            Program.BroadcastMessage($"[{DateTime.Now}] {Username}: {msg}");
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine($"[{UID.ToString()}]: {Username} disconnected.");
                    Program.BroadcastDisconnect(UID.ToString());
                    ClientSocket.Close();
                    break;
                }
            }
        }
    }
}
