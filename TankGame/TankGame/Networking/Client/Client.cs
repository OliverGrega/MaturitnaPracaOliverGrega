using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TankGame.Networking.Server;

namespace TankGame.Networking.Client
{
    public class Client
    {
        public static Client Active;
        public static int dataBufferSize = 4096;

        public string ip = "127.0.0.1";
        public int port = 7777;
        public byte myId = 0;
        public TCP tcp;
        public UDP udp;

        public bool isConnected;

        public Client()
        {
            Active = this;
        }

        public bool TryToConnect(string _ip)
        {
            if (ValidateIPv4(_ip))
            {
                string[] parts = _ip.Split(':');
                ConnectToServer(parts[0], Int32.Parse(parts[1]));
                return true;
            }
            return false;
        }

        public void ConnectToServer(string _ip, int _port)
        {
            ip = _ip;
            port = _port;

            tcp = new TCP();
            udp = new UDP();

            isConnected = true;
            OnConnecting();
            tcp.Connect();
        }

        private bool ValidateIPv4(string _ipString)
        {
            if (String.IsNullOrWhiteSpace(_ipString))
            {
                return false;
            }

            string[] parts = _ipString.Split(':');

            if (parts.Length != 2)
            {
                return false;
            }

            string[] splitValues = parts[0].Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }

            byte tempForParsing;


            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }

        public void Disconnect(string reason)
        {
            if (isConnected)
            {
                isConnected = false;
                tcp.socket.Close();
                udp.socket.Close();

                OnDisconnect(reason);
            }
        }

        public virtual void OnConnecting()
        {

        }
        public virtual void OnConnected()
        {

        }

        public virtual void OnDisconnect(string reason)
        {

        }

        public virtual void OnMessage(byte[] data, int size, Channel channel)
        {

        }

        public void Send(PacketBuilder _packet, Channel _channel = Channel.TCP)
        {
            NetworkingUtil.PacketSent(_packet.Size);
            if (_channel == Channel.TCP) SendTCPData(_packet);
            else SendUDPData(_packet);
        }
        private void SendTCPData(PacketBuilder _packet)
        {
            _packet.WriteLength();
            tcp.SendData(_packet);
        }
        private void SendUDPData(PacketBuilder _packet)
        {
            _packet.WriteLength();
            udp.SendData(_packet);
        }


        public class TCP
        {
            public TcpClient socket;

            private NetworkStream stream;
            private PacketBuilder receivedData;
            private byte[] receiveBuffer;

            public void Connect()
            {
                socket = new TcpClient
                {
                    ReceiveBufferSize = dataBufferSize,
                    SendBufferSize = dataBufferSize,
                };

                receiveBuffer = new byte[dataBufferSize];
                socket.BeginConnect(Active.ip, Active.port, ConnectCallback, socket);
            }

            private void ConnectCallback(IAsyncResult _result)
            {
                try
                {
                    socket.EndConnect(_result);

                    if (!socket.Connected)
                    {
                        return;
                    }

                    stream = socket.GetStream();

                    receivedData = new PacketBuilder();
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                    Client.Active.OnConnected();
                }
                catch (Exception _ex)
                {
                    Client.Active.OnDisconnect($"Failed to connect to server");
                }

            }

            public void SendData(PacketBuilder _packet)
            {
                try
                {
                    if (socket != null)
                    {
                        stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                    }
                }
                catch (Exception _ex)
                {
                    Debug.WriteLine($"[{DateTime.Now}] Error sending data to server via TCP: {_ex}");
                }
            }

            private void ReceiveCallback(IAsyncResult _result)
            {
                try
                {
                    int byteLength = stream.EndRead(_result);
                    if (byteLength <= 0)
                    {
                        Active.Disconnect("Server error");
                        return;
                    }

                    byte[] data = new byte[byteLength];
                    Array.Copy(receiveBuffer, data, byteLength);

                    receivedData.Reset(HandleData(data));
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                catch (Exception _ex)
                {
                    Debug.WriteLine($"TCP stopped receiveing packets: {_ex}");
                    Disconnect("Lost connection to server!");
                }
            }

            private bool HandleData(byte[] _data)
            {
                int packetLength = 0;

                receivedData.SetBytes(_data);

                if (receivedData.UnreadLength() >= 4)
                {
                    packetLength = receivedData.ReadInt();

                    if (packetLength <= 0) return true;
                }

                while (packetLength > 0 && packetLength <= receivedData.UnreadLength())
                {
                    byte[] packedBytes = receivedData.ReadBytes(packetLength);
                    NetworkingUtil.PacketReceived(packedBytes.Length + 4);
                    ThreadManager.ExecuteOnMainThread(() => Active.OnMessage(packedBytes, packetLength, Channel.TCP));

                    packetLength = 0;

                    if (receivedData.UnreadLength() >= 4)
                    {
                        packetLength = receivedData.ReadInt();

                        if (packetLength <= 0) return true;
                    }
                }

                if (packetLength <= 1) return true;

                return false;
            }            

            private void Disconnect(string reason)
            {
                Active.Disconnect(reason);

                stream = null;
                receiveBuffer = null;
                receivedData = null;
                socket = null;
            }
        }
        public class UDP
        {
            public UdpClient socket;
            public IPEndPoint endPoint;

            public UDP()
            {
                endPoint = new IPEndPoint(IPAddress.Parse(Active.ip), Active.port);
            }

            public void Connect(int _localPort)
            {
                socket = new UdpClient(_localPort);

                socket.Connect(endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                using (PacketBuilder _packet = new PacketBuilder())
                {
                    SendData(_packet);
                }
            }

            public void SendData(PacketBuilder _packet)
            {
                try
                {
                    _packet.InsertByte(Active.myId);
                    if (socket != null)
                    {
                        socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
                    }
                }
                catch (Exception _ex)
                {
                    Console.WriteLine($"Error sending data to server via UDP: {_ex}");
                }
            }

            private void ReceiveCallback(IAsyncResult _result)
            {
                try
                {
                    byte[] data = socket.EndReceive(_result, ref endPoint);
                    socket.BeginReceive(ReceiveCallback, null);

                    if (data.Length < 4)
                    {
                        Active.Disconnect("Corrupted packet received");
                        return;
                    }

                    HandleData(data);
                }
                catch (Exception _ex)
                {
                    Debug.WriteLine($"UDP stopped receiveing packets: {_ex}");
                    Disconnect("UDP Timed out!");
                }
            }

            private void HandleData(byte[] _data)
            {
                int packetLength = 0;
                using (PacketBuilder _packet = new PacketBuilder(_data))
                {
                    packetLength = _packet.ReadInt();
                    _data = _packet.ReadBytes(packetLength);
                }
                NetworkingUtil.PacketReceived(_data.Length + 4);
                ThreadManager.ExecuteOnMainThread(() => Active.OnMessage(_data, packetLength, Channel.UDP));
            }

            private void Disconnect(string reason)
            {
                Active.Disconnect(reason);

                endPoint = null;
                socket = null;
            }
        }
    }
}
