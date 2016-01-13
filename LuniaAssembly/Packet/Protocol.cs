﻿using System;

namespace LuniaAssembly.Packet
{
    public class Protocol
    {
        public static readonly Protocol Authentication = new Protocol(1, typeof(LCAuthentication));
        public static readonly Protocol AuthenticationResponse = new Protocol(2, typeof(LCAuthenticationResponse));

        public short PacketID { get; set; }
        public Type Type { get; set; }

        public Protocol(short packetId, Type type)
        {
            PacketID = packetId;
            Type = type;
        }

        //TODO Optimize (Caching)
        public static Type GetByID(short id)
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

        public static int GetByType(Type type)
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