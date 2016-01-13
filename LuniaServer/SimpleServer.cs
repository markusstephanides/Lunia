using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using LuniaAssembly;
using LuniaAssembly.Packet;

namespace LuniaServer
{
    public class SimpleServer
    {
        //TODO Replace callbacks with events

        private TCPServer tcpServer;
        private Dictionary<short, Action<Connection, IPacket>> handlers;
        private HashSet<Connection> connections;



        public SimpleServer(int port)
        {
            tcpServer = new TCPServer(port, ClientConnectedCallback, ClientDisconnectedCallback, PacketReceivedCallback);
            handlers = new Dictionary<short, Action<Connection, IPacket>>();
            connections = new HashSet<Connection>();

            //Start server
            tcpServer.StartListening();
        }

        public void RegisterHandler(short packetID, Action<Connection, IPacket> handler)
        {
            handlers.Add(packetID, handler);
        }


        private void PacketReceivedCallback(Socket socket, byte[] bytes)
        {
            try
            {
                BinaryReader reader = new BinaryReader(new MemoryStream(bytes));

                //Get the packet id
                short packetID = reader.ReadInt16();

                if (packetID == -1 || !handlers.ContainsKey(packetID)) return;

                //Convert to packet
                IPacket packet = (IPacket)Activator.CreateInstance(Protocol.GetByID(packetID));
                packet.Read(reader);

                //Get Connection
                Connection connection = GetConnection(socket);

                //Invoke handler
                handlers[packetID].Invoke(connection, packet);
            }
            catch
            {
                // ignored
            }
        }

        public Connection GetConnection(Socket socket)
        {
            foreach (var _connection in connections)
            {
                if (_connection.Socket == socket) return _connection;
            }

            Connection connection = new Connection(socket);
            return connection;
        }

        private void ClientDisconnectedCallback()
        {

        }

        private void ClientConnectedCallback()
        {

        }
    }
}