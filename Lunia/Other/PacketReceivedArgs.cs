using System;
using LuniaAssembly.Packet;

namespace Lunia
{
    public class PacketReceivedArgs : EventArgs
    {
        public IPacket Packet { get; set; }
    }
}