using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Numerics;
using TankGame.Networking.Packets.ServerPackets;

namespace PistaNetworkLibrary
{ 
    public partial class ServerClient
    {
        public static int dataBufferSize = 4096;
        public byte id;
        public PlayerInfo player;
        public TCP tcp;
        public UDP udp;

        public ServerClient(byte _clientId)
        {
            id = _clientId;
            tcp = new TCP(id);
            udp = new UDP(id);
        }

        public class TCP
        {
            public TcpClient socket;

            private readonly byte id;
            private NetworkStream stream;
            private PacketBuilder receivedData;
            private byte[] receiveBuffer;

            public TCP(byte _id)
            {
                id = _id;
            }

            public void Connect(TcpClient _socket)
            {
                socket = _socket;
                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;

                stream = socket.GetStream();

                receivedData = new PacketBuilder();
                receiveBuffer = new byte[dataBufferSize];

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                MyDebugger.WriteLine($"Connection from {socket.Client.RemoteEndPoint} is receiving stream...");
                Server.Joining?.Invoke(id);                
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
                    MyDebugger.WriteLine($"Error sending data to player {id} via TCP: {_ex}");
                }
            }

            private void ReceiveCallback(IAsyncResult _result)
            {
                try
                {

                    int byteLength = stream.EndRead(_result);
                    MyDebugger.WriteLine($"Read packet length: {byteLength}");
                    if (byteLength <= 0)
                    {
                        Server.clients[id].Disconnect("Received packet is corrupted");
                        return;
                    }

                    byte[] data = new byte[byteLength];
                    Array.Copy(receiveBuffer, data, byteLength);

                    receivedData.Reset(HandleData(data));
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                catch (Exception _ex)
                {                    
                    Server.clients[id].Disconnect("Error trying to receive packets");
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
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (PacketBuilder _packet = new PacketBuilder(packedBytes))
                        {
                            byte packetId = _packet.ReadByte();
                            NetworkManager.clientPackets[packetId].Handle(id, _packet);
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

            public void Disconnect()
            {
                socket.Close();
                stream = null;
                receivedData = null;
                receiveBuffer = null;
                socket = null;
            }
        }

        public class UDP
        {
            public IPEndPoint endPoint;

            private byte id;

            public UDP(byte _id)
            {
                id = _id;
            }

            public void Connect(IPEndPoint _endPoint)
            {
                endPoint = _endPoint;
            }

            public void SendData(PacketBuilder _packet)
            {
                Server.SendUDPData(endPoint, _packet);
            }

            public void HandleData(PacketBuilder _packetData)
            {
                int packetLength = _packetData.ReadInt();
                byte[] packetBytes = _packetData.ReadBytes(packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (PacketBuilder _packet = new PacketBuilder(packetBytes))
                    {
                        byte packetId = _packet.ReadByte();
                        NetworkManager.clientPackets[packetId].Handle(id, _packet);
                    }
                });
            }

            public void Disconnect()
            {
                endPoint = null;
            }
        }

        public void ConfirmConnection(string _playerName)
        {
            player = new PlayerInfo(id, _playerName);

            foreach (ServerClient _client in Server.clients.Values)
            {
                if (_client.player != null)
                {
                    if (_client.id != id)
                    {
                        Server.Send(new SpawnPlayerServerPacket(id, _client.player.Id, _client.player.Username, new Vector2(50, 50)));
                    }
                }
            }
            foreach (ServerClient _client in Server.clients.Values)
            {
                if (_client.player != null)
                {
                    Server.Send(new SpawnPlayerServerPacket(_client.id, player.Id, player.Username, new Vector2(50, 50)));
                }
            }
        }

        private void Disconnect(string reason)
        {
            MyDebugger.WriteLine($"{tcp.socket.Client.RemoteEndPoint} has disconnected. Reason: {reason}");

            Server.Left?.Invoke(id);
            player = null;

            tcp.Disconnect();
            udp.Disconnect();
        }
    }
}
