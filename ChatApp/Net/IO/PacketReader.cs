﻿using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ChatClient.Net.IO
{
    class PacketReader : BinaryReader
    {
        private NetworkStream _ns;
        public PacketReader(NetworkStream ns) : base(ns)
        {
            _ns = ns;
        }

        public string ReadMessage()
        {
            byte[] msgBuffer;
            int length = ReadInt32() * 2;
            msgBuffer = new byte[length];
            _ns.Read(msgBuffer, 0, length);
            var msg = Encoding.Unicode.GetString(msgBuffer);


            return msg;
        }
    }
}
