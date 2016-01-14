using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using LuniaAssembly.Packet;

namespace LuniaAssembly.Packet
{
    public class LCAuthentication : IPacket
    {
        public short ID => 1;

        public string Salt { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public LCAuthentication(string salt, string username, string password)
        {
            Salt = salt;
            Username = username;
            Password = password;
        }

        public LCAuthentication()
        {
            
        }

        public void Read(BinaryReader stream)
        {
            //Salt
            Salt = stream.ReadString();

            //Username
            Username = stream.ReadString();

            //Password 
            Password = stream.ReadString();
        }

        public void Write(BinaryWriter stream)
        {
            //Salt
            stream.Write(Salt);

            //Username
            stream.Write(Username);

            //Password
            stream.Write(Password);
        }
    }
}