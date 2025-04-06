using PistaNetworkLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Networking.Server
{
    public partial class Server
    {
        public int MaxPlayers { get; set; }
        public int Port { get; set; }

        public int ConnectedPlayers
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

        public TcpListener tcpListener;
        public UdpClient udpListener;

        public Dictionary<byte, Client> clients;

        public static Server Active;

        public bool SimulateLatency = false;
        public int Latency = 500;

        public Server(int _maxPlayers, int _port)
        {
            Active = this;

            MaxPlayers = _maxPlayers;
            Port = _port;

            clients = new Dictionary<byte, Client>();

            for (byte i = 1; i <= MaxPlayers; i++)
            {
                clients.Add(i, new Client(i));
            }

            MyDebugger.WriteLine("Trying to start server...");
            Stopwatch sw = new Stopwatch();

            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            udpListener = new UdpClient(Port);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            MyDebugger.WriteLine($"Server started on {Port}! Elapsed time: [{sw.Elapsed}]");
        }

        private void TCPConnectCallback(IAsyncResult _result)
        {
            TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            for (byte i = 1; i <= MaxPlayers; i++)
            {
                if (clients[i].tcp.socket == null)
                {
                    OnConnecting(_client.Client, clients[i]);
                    clients[i].tcp.Connect(_client);
                    return;
                }
            }

            MyDebugger.WriteLine($"Failed to connect from {_client.Client.RemoteEndPoint}: Server is full!");
        }
        private void UDPReceiveCallback(IAsyncResult _result)
        {
            try
            {
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpListener.EndReceive(_result, ref clientEndPoint);
                udpListener.BeginReceive(UDPReceiveCallback, null);

                if (data.Length < 4 || data == null)
                {
                    return;
                }

                using (PacketBuilder _packet = new PacketBuilder(data))
                {
                    byte clientId = _packet.ReadByte();
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
            catch(SocketException ex) when (ex.SocketErrorCode == SocketError.ConnectionReset)
            {
                MyDebugger.WriteLine($"UDP Clients Disconnects: {ex.Message}");
                udpListener.BeginReceive(UDPReceiveCallback, null);
            }
            catch (Exception _ex)
            {
                MyDebugger.WriteLine($"Error receiving UDP data: {_ex}");
            }
        }

        public void SendUDPData(IPEndPoint _clientEndPoint, PacketBuilder _packet)
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

        public virtual void OnConnecting(Socket socket, Client client)
        {
            MyDebugger.WriteLine($"Incoming connection from {socket.RemoteEndPoint}...");
        }
        public virtual void OnConnected(Socket socket, Client client)
        {
            MyDebugger.WriteLine($"Connection from {socket.RemoteEndPoint} connected!");
        }
        public virtual void OnMessage(byte[] data, int size, byte fromClient, Channel channel)
        {

        }
        public virtual void OnDisconnected(Socket socket, Client client, string reason)
        {
            MyDebugger.WriteLine($"Stop connection from {socket.RemoteEndPoint}!");
        }
    }
}
