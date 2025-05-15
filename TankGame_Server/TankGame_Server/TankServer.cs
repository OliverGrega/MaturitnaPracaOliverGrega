using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TankGame;
using TankGame.Networking;
using TankGame.Networking.Server;

namespace TankGame_Server
{
    public class TankServer : Server
    {
        public TankServer(int _maxPlayers, int _port) : base(_maxPlayers, _port)
        {
            SimulateLatency = false;
        }

        public override void OnConnected(Socket socket, Client client)
        {
            ServerSend.SendHandshake(client.id, GameLogic.currentTick);
        }

        public override void OnDisconnected(Socket socket, Client client, string reason)
        {
            MyDebugger.WriteLine($"{socket.RemoteEndPoint} has disconnected. Reason: {reason}");
            GameLogic.DespawnPlayer(client.id);
        }

        public override void OnMessage(byte[] data, int size,byte fromClient, Channel channel)
        {            
            using (PacketBuilder _packet = new PacketBuilder(data))
            {
                //byte fromClient = 
                ClientPackets receivedPacket = (ClientPackets)_packet.ReadByte();
                switch(receivedPacket)
                {
                    case ClientPackets.HandshakeReceived:

                        string username = _packet.ReadString();
                        MyDebugger.WriteLine("Player confirmed handshake with username: " +username);
                        GameLogic.SpawnPlayer(fromClient, username);
                        ServerSend.SendSettings(fromClient);
                        foreach(var ply in GameLogic.players)
                        {
                            using (PacketBuilder syncPlayers = new PacketBuilder((byte)ServerPackets.PlayerJoined))
                            {
                                syncPlayers.Write(ply.Key);
                                syncPlayers.Write(ply.Value.username);
                                syncPlayers.Write(ply.Value.position);
                                Send(fromClient, syncPlayers);
                            }
                        }

                        using (PacketBuilder syncPlayers = new PacketBuilder((byte)ServerPackets.PlayerJoined))
                        {
                            syncPlayers.Write(fromClient);
                            syncPlayers.Write(username);
                            syncPlayers.Write(new Vector2(200,200));
                            SendAll(syncPlayers, fromClient);
                        }

                        break;
                    case ClientPackets.Input:
                        Vector2 moveInput = new Vector2();
                        uint tick = _packet.ReadUint();
                        byte input = _packet.ReadByte();
                        if (input.Get(7)) moveInput.Y = 1;
                        else if (input.Get(6)) moveInput.Y = -1;
                        if (input.Get(5)) moveInput.X = 1;
                        else if (input.Get(4)) moveInput.X = -1;

                        GameLogic.HandleInput(fromClient,new InputPayload { tick = tick, input = moveInput });

                        break;
                    case ClientPackets.Shoot:

                        uint attackTick = _packet.ReadUint();
                        GameLogic.PlayerShoot(fromClient, attackTick);
                        break;
                }
            }
        }
    }
}
