using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace PistaNetworkLibrary
{
    public partial class MyClient
    {
        public static MyClient instance;
        public static int dataBufferSize = 4096;

        public string ip = "127.0.0.1";
        public int port = 7777;
        public int myId = 0;
        public TCP tcp;
        public UDP udp;

        public bool isConnected { get { if (tcp.socket == null) return false; else { return tcp.socket.Connected; } } }

        public MyClient()
        {
            instance = this;

            tcp = new TCP();
            udp = new UDP();
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

        public void ConnectToServer(string _ip,int _port)
        {
            ip = _ip;
            port = _port;

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
                //isConnected = false;
                tcp.socket.Close();
                udp.socket.Close();

                MyClient.OnLeft?.Invoke(reason);
                Console.WriteLine("Disconnected");
            }
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
                socket.BeginConnect(instance.ip,instance.port, ConnectCallback, socket);
            }

            private void ConnectCallback(IAsyncResult _result)
            {
                try
                {
                    socket.EndConnect(_result);
                    
                    if(!socket.Connected)
                    {
                        return;
                    }
                    
                    stream = socket.GetStream();
                    
                    receivedData = new PacketBuilder();
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                    MyClient.OnJoined?.Invoke();
                }
                catch (Exception _ex)
                {
                    Debug.WriteLine($"Failed to connect to server: {_ex}");
                    MyClient.OnFailedToConnect?.Invoke();
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
                        instance.Disconnect("Server error");
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
                    Disconnect("TCP Timed out!");
                }
            }

            private bool HandleData(byte[] _data)
            {
                int packetLength = 0;

                receivedData.SetBytes(_data);

                if(receivedData.UnreadLength() >= 4)
                {
                    packetLength = receivedData.ReadInt();

                    if (packetLength <= 0) return true;
                }

                while (packetLength >0 && packetLength <= receivedData.UnreadLength())
                {
                    byte[] packedBytes = receivedData.ReadBytes(packetLength);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (PacketBuilder _packet = new PacketBuilder(packedBytes))
                        {
                            byte packetId = _packet.ReadByte();
                            NetworkManager.serverPackets[packetId].Handle(_packet);
                        }
                    });

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
                instance.Disconnect(reason);

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
                endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
            }

            public void Connect(int _localPort)
            {
                socket = new UdpClient(_localPort);

                socket.Connect(endPoint);
                socket.BeginReceive(ReceiveCallback,null);

                using(PacketBuilder _packet = new PacketBuilder())
                {
                    SendData(_packet);
                }
            }

            public void SendData(PacketBuilder _packet)
            {
                try
                {
                    _packet.InsertInt(instance.myId);
                    if(socket!= null)
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

                    if(data.Length < 4)
                    {
                        instance.Disconnect("Server error");
                        return;
                    }

                    HandleData(data);
                }
                catch(Exception _ex)
                {
                    Debug.WriteLine($"UDP stopped receiveing packets: {_ex}");
                    Disconnect("UDP Timed out!");
                }
            }

            private void HandleData(byte[] _data)
            {
                int packetLength = 0;
                using(PacketBuilder _packet = new PacketBuilder(_data))
                {
                    packetLength = _packet.ReadInt();
                    _data = _packet.ReadBytes(packetLength);
                }

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (PacketBuilder _packet = new PacketBuilder(_data))
                    {
                        byte packetId = _packet.ReadByte();
                        NetworkManager.serverPackets[packetId].Handle(_packet);
                    }
                });
            }

            private void Disconnect(string reason)
            {
                instance.Disconnect(reason);

                endPoint = null;
                socket = null;

                
            }
        }
    }
}
