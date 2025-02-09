using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Net;

namespace PistaNetworkLibrary
{
    public partial class Server
    {
        public static int MaxPlayer { get; private set; }
        public static int Port { get; private set; }
        public static int ConnectedPlayers
        {
            get
            {
                int _playerCount = 0;
                foreach (var n in clients)
                {
                    if (n.Value.tcp.socket != null) _playerCount++;
                }
                return _playerCount;
            }
        }

        public static Dictionary<int, ConnectedClient> clients = new Dictionary<int, ConnectedClient>();
        public delegate void PacketHandler(int _fromClient, PacketBuilder _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;

        public static TcpListener tcpListener;
        public static UdpClient udpListener;

        public static void Start(int _maxPlayers, int _port)
        {
            MaxPlayer = _maxPlayers;
            Port = _port;

            MyDebugger.WriteLine("Trying to start server...");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            InitServerData();

            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            udpListener = new UdpClient(Port);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            sw.Stop();
            MyDebugger.WriteLine($"Server started on {Port}! Elapsed time: [{sw.Elapsed}]");
        }
        private static void TCPConnectCallback(IAsyncResult _result)
        {
            TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
            MyDebugger.WriteLine($"Incoming connection from {_client.Client.RemoteEndPoint}...");

            for (int i = 1; i <= MaxPlayer; i++)
            {
                if (clients[i].tcp.socket == null)
                {
                    clients[i].tcp.Connect(_client);
                    return;
                }
            }

            MyDebugger.WriteLine($"Failed to connect from {_client.Client.RemoteEndPoint}: Server is full!");
        }
        private static void UDPReceiveCallback(IAsyncResult _result)
        {
            try
            {
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpListener.EndReceive(_result, ref clientEndPoint);
                udpListener.BeginReceive(UDPReceiveCallback, null);

                if (data.Length < 4)
                {
                    return;
                }

                using (PacketBuilder _packet = new PacketBuilder(data))
                {
                    int clientId = _packet.ReadInt();
                    if (clientId == 0) return;

                    if (clients[clientId].udp.endPoint == null)
                    {
                        clients[clientId].udp.Connect(clientEndPoint);
                        return;
                    }

                    if (clients[clientId].udp.endPoint.ToString() == clientEndPoint.ToString())
                    {
                        clients[clientId].udp.HandleData(_packet);
                    }
                }
            }
            catch (Exception _ex)
            {
                MyDebugger.WriteLine($"Error receiving UDP data: {_ex}");
            }
        }
        public static void SendUDPData(IPEndPoint _clientEndPoint, PacketBuilder _packet)
        {
            try
            {
                if (_clientEndPoint != null)
                {
                    udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
                }
            }
            catch (Exception _ex)
            {
                MyDebugger.WriteLine($"Error sending data to {_clientEndPoint} via UDP: {_ex}");
            }
        }
        private static void InitServerData()
        {
            for (int i = 1; i <= MaxPlayer; i++)
            {
                clients.Add(i, new ConnectedClient((byte)i));
            }
            var networkManager = new NetworkManager();
            MyDebugger.WriteLine("Initialized server...");
        }
    }
}
