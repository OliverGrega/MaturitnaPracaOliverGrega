using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using TankGame.Networking;
using System.Threading.Tasks;

namespace TankGame.Networking.Server
{
    public class Client
    {
        public static int dataBufferSize = 4096;
        public byte id;
        public TCP tcp;
        public UDP udp;

        public bool tryKick = false;

        public Client(byte _clientId)
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

                Server.Active.OnConnected(socket.Client, Server.Active.clients[id]);
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
                    if (Server.Active.clients[id].tryKick)
                    {
                        //Server.Active.clients[id].Disconnect("Kicked!");
                        return;
                    }
                    int byteLength = stream.EndRead(_result);
                    MyDebugger.WriteLine($"Read packet length: {byteLength}");
                    if (byteLength <= 0)
                    {
                        Server.Active.clients[id].Disconnect("Received corrupted data");
                        return;
                    }

                    byte[] data = new byte[byteLength];
                    Array.Copy(receiveBuffer, data, byteLength);

                    receivedData.Reset(HandleData(data));
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                catch (Exception _ex)
                {
                    Server.Active.clients[id].Disconnect("Lost connection");
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

                    if(Server.Active.SimulateLatency) Task.Run(async () => { await Task.Delay(Server.Active.Latency); ThreadManager.ExecuteOnMainThread(() => Server.Active.OnMessage(packedBytes, packetLength,id, Channel.TCP)); });
                    else ThreadManager.ExecuteOnMainThread(() => Server.Active.OnMessage(packedBytes, packetLength, id, Channel.TCP));

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
                Server.Active.SendUDPData(endPoint, _packet);
            }

            public void HandleData(PacketBuilder _packetData)
            {
                int packetLength = _packetData.ReadInt();
                byte[] packetBytes = _packetData.ReadBytes(packetLength);

                if(Server.Active.SimulateLatency) Task.Run(async () => { await Task.Delay(Server.Active.Latency); ThreadManager.ExecuteOnMainThread(() => Server.Active.OnMessage(packetBytes, packetLength, id, Channel.UDP)); });
                else ThreadManager.ExecuteOnMainThread(() => Server.Active.OnMessage(packetBytes, packetLength, id, Channel.UDP));
            }

            public void Disconnect()
            {
                endPoint = null;
            }
        }

        public void Disconnect(string reason)
        {
            Server.Active.OnDisconnected(tcp.socket.Client, this, reason);

            tcp.Disconnect();
            udp.Disconnect();
        }
    }

    public enum Channel
    {
        TCP,
        UDP
    }
}

