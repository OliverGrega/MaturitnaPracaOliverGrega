using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TankGame.Input;
using TankGame.Networking;
using TankGame.Networking.Client;
using TankGame.Scene;

namespace TankGame
{
    public struct InputPayload
    {
        public uint tick;
        public System.Numerics.Vector2 input;
    }

    public struct StatePayload
    {
        public uint tick;
        public System.Numerics.Vector2 position;
        public float rotation;
    }

    public class TankClient : Client
    {
        public bool IsConnected { get; private set; }
        public uint tick => timer.currentTick;
        public NetworkTimer timer = new NetworkTimer(30);

        public static Action<uint> OnTick;

        public void Update()
        {
            timer.Update(Global.DeltaTime);
            if (timer.ShouldTick()) OnTick?.Invoke(tick);

            if (MyKeyboard.IsKeyClicked(Microsoft.Xna.Framework.Input.Keys.F3)) debug = !debug;
        }

        public override void OnDisconnect(string reason)
        {
            SceneManager.LoadScene(new MenuScene(), reason);
            IsConnected = false;
        }

        public override void OnConnected()
        {
            SceneManager.LoadScene(new GameScene());
            IsConnected = true;
        }

        public void SendInput(byte input)
        {
            using(PacketBuilder _packet = new PacketBuilder((byte)ClientPackets.Input))
            {
                _packet.Write(tick);
                _packet.Write(input);
                Send(_packet, Networking.Server.Channel.UDP);
            }
        }
        public void SendShoot()
        {
            using (PacketBuilder _packet = new PacketBuilder((byte)ClientPackets.Shoot))
            {            
                _packet.Write(tick);
                Send(_packet, Networking.Server.Channel.UDP);
            }
        }

        private void TrySyncTick(uint newTick)
        {
            if(tick < newTick) timer.currentTick = newTick;
        }

        private static bool debug = false;
        public static void DrawDebug()
        {
            if (!debug) return;

            Global.SpriteBatch.DrawString(
                Global.basicFont,
                $"Bytes up: {NetworkingUtil.BytesUp}\nBytes down: {NetworkingUtil.BytesDown}\nPackets up: {NetworkingUtil.PacketsUp}\nPackets down: {NetworkingUtil.PacketsDown}",
                new Vector2(100, 100), Color.White);
        }

        public override void OnMessage(byte[] data, int size, Networking.Server.Channel channel)
        {
            using(PacketBuilder _packet = new PacketBuilder(data))
            {
                ServerPackets packetType = (ServerPackets)_packet.ReadByte();

                switch(packetType)
                {
                    case ServerPackets.Handshake:

                        byte myId = _packet.ReadByte();
                        uint serverTick = _packet.ReadUint();
                        timer.currentTick = serverTick;
                        this.myId = myId;
                        using (PacketBuilder handShakePacket = new PacketBuilder((byte)ClientPackets.HandshakeReceived))
                        {
                            handShakePacket.Write("Player " + myId);
                            Send(handShakePacket);
                        }

                        udp.Connect(((IPEndPoint)tcp.socket.Client.LocalEndPoint).Port);
                        break;
                    case ServerPackets.SpawnPlayer:

                        byte playerId = _packet.ReadByte();
                        string username = _packet.ReadString();
                        Vector2 spawnPos = _packet.ReadVector2();
                        GameManager.instance.SpawnPlayer(playerId, username, spawnPos);
                        break;

                    case ServerPackets.PlayerPos:

                        byte plyToMove = _packet.ReadByte();
                        uint tickWhenMoved = _packet.ReadUint();
                        TrySyncTick(tickWhenMoved);
                        Vector2 plyPos = _packet.ReadVector2();
                        float plyRot = _packet.ReadFloat();
                        GameManager.instance.UpdatePlayerState(plyToMove, new StatePayload { tick = tickWhenMoved, position = plyPos.ToNumerics(), rotation = plyRot});
                        break;
                    case ServerPackets.PlayerShoot:
                        byte attacker = _packet.ReadByte();
                        Vector2 attackFrom = _packet.ReadVector2();
                        Vector2 attackTo = _packet.ReadVector2();
                        GameManager.instance.PlayerShoot(attacker, attackFrom, attackTo);
                        break;
                }
            }
        }
    }
}
