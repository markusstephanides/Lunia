using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using LuniaAssembly;
using LuniaAssembly.Packet;

namespace Lunia.Networking
{
    // State object for receiving data from remote device.
    public class StateObject
    {
        // Client socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 256;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

    public class TCPClient
    {

        // ManualResetEvent instances signal completion.
        private static ManualResetEvent connectDone =
            new ManualResetEvent(false);

        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);

        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        // The response from the remote device.
        private static String response = String.Empty;

        private string hostname;
        private int port;
        private Action connectedCallback;

        private Socket client;

        public string NetworkSalt => Encrypter.GetHashed(hostname);


        public static event EventHandler<PacketReceivedArgs> PacketReceived;

        public TCPClient(string hostname, int port)
        {
            this.hostname = hostname;
            this.port = port;
        }

        public void StartClient(Action connectedCallback)
        {
            // Connect to a remote device.
            try
            {
                this.connectedCallback = connectedCallback;
                // Establish the remote endpoint for the socket.
                // The name of the 
                // remote device is "host.contoso.com".
                IPHostEntry ipHostInfo = Dns.Resolve(hostname);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.
                client = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.
                client.BeginConnect(remoteEP,
                new AsyncCallback(ConnectCallback), client);

                //// Send test data to the remote device.
                //Send(client, "This is a test<EOF>");
                //sendDone.WaitOne();

                //// Receive the response from the remote device.
                //Receive(client);
                //receiveDone.WaitOne();

                //// Write the response to the console.
                //Console.WriteLine("Response received : {0}", response);

                //// Release the socket.
                //client.Shutdown(SocketShutdown.Both);
                //client.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);

                //Start receiving
                receive();

                connectedCallback.Invoke();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void receive()
        {
            try
            {
                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                StateObject state = (StateObject) ar.AsyncState;

                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    //Decode
                    BinaryReader reader = new BinaryReader(new MemoryStream(state.buffer));

                    //Get the packet id
                    short packetID = reader.ReadInt16();

                    if (packetID == -1 && PacketReceived != null) return;

                    //Convert to packet
                    IPacket packet = (IPacket) Activator.CreateInstance(Protocol.GetByID(packetID));
                    packet.Read(reader);

                    PacketReceived?.Invoke(this, new PacketReceivedArgs() {Packet = packet});

                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void Send(IPacket packet)
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(memoryStream);

            //Write id
            binaryWriter.Write(packet.ID);

            //Write packet
            packet.Write(binaryWriter);

            // Begin sending the data to the remote device.
            client.BeginSend(memoryStream.GetBuffer(), 0, memoryStream.GetBuffer().Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}