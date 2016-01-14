using System;

namespace LuniaAssembly.Packet
{
    public class Protocol
    {
        public static readonly Protocol Authentication = new Protocol(1, typeof(LCAuthentication));
        public static readonly Protocol AuthenticationResponse = new Protocol(2, typeof(LCAuthenticationResponse));
        public static readonly Protocol StateSwitch = new Protocol(3, typeof(LCStateSwitch));
        public static readonly Protocol CharacterList = new Protocol(4, typeof(LCCharacterList));

        public short PacketID { get; set; }
        public Type Type { get; set; }

        public Protocol(short packetId, Type type)
        {
            PacketID = packetId;
            Type = type;
        }

        //TODO Optimize (Caching)
        public static Type GetTypeByID(short id)
        {
            foreach (var field in typeof(Protocol).GetFields())
            {
                if (field.FieldType == typeof (Protocol))
                {
                    Protocol protocol = (Protocol) field.GetValue(null);

                    if (protocol.PacketID == id) return protocol.Type;
                }
            }

            return null;
        }

        public static short GetIDByType(Type type)
        {
            foreach (var field in typeof(Protocol).GetFields())
            {
                if (field.FieldType == typeof(Protocol))
                {
                    Protocol protocol = (Protocol)field.GetValue(null);
                    if (protocol.Type == type) return protocol.PacketID;
                }
            }

            return -1;
        }
    }
}