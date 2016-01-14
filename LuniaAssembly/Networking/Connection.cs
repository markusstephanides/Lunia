using System;
using System.IO;
using System.Net.Sockets;
using LuniaAssembly.Packet;


namespace LuniaAssembly
{
    public class Connection
    {
        public Socket Socket { get; set; }
        public Guid Guid { get; set; }

        public Connection(Socket socket)
        {
            Socket = socket;
            Guid = Guid.NewGuid();
        }

        public void Send(IPacket packet)
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(memoryStream);

            //Write id
            binaryWriter.Write(Protocol.GetIDByType(packet.GetType()));

            //Write packet
            packet.Write(binaryWriter);

            //Send
            Socket.Send(memoryStream.GetBuffer());
        }
    }
}