using System;
using System.IO;

namespace LuniaAssembly.Packet
{
    public class LCAuthenticationResponse : IPacket
    {
        public AuthenticationResult Result { get; set; }


        public void Read(BinaryReader stream)
        {
            Result = (AuthenticationResult) Enum.Parse(typeof(AuthenticationResult), stream.ReadString());
        }

        public void Write(BinaryWriter stream)
        {
           stream.Write(Result.ToString());
        }
    }
}