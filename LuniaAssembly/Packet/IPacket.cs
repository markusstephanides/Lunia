using System.IO;
using System.Net.Sockets;

namespace LuniaAssembly.Packet
{
    public interface IPacket
    {

        void Read(BinaryReader stream);
        void Write(BinaryWriter stream);
    }
}