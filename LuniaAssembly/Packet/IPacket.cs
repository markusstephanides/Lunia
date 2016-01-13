using System.IO;
using System.Net.Sockets;

namespace LuniaAssembly.Packet
{
    public interface IPacket
    {
        short ID { get; }

        void Read(BinaryReader stream);
        void Write(BinaryWriter stream);
    }
}