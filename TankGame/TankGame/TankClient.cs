using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
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
            if (MyKeyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Tab))
            {
                Utility.DrawRectangle(new Rectangle((int)(Camera.Center.X - Display.ScreenWidth * 0.25f), (int)(Camera.Center.Y - Display.ScreenHeight * 0.25f), (int)(Display.ScreenWidth * 0.5f), (int)(Display.ScreenHeight * 0.5f)), new Color(0, 0, 0, 100),0.89f);

                int i = 1;
                Global.SpriteBatch.DrawString(Global.basicFont, $"Username     Kills     Deaths", new Vector2(Camera.Center.X-(Display.ScreenWidth * 0.25f), Camera.Center.Y-(Display.ScreenHeight * 0.25f)), Color.White,0,Vector2.Zero,0.5f,0,0.9f);
                foreach(var n in GameManager.players)
                {
                    Global.SpriteBatch.DrawString(Global.basicFont, $"{n.Value.name}   {n.Value.kills}   {n.Value.deaths}", new Vector2(Camera.Center.X-(Display.ScreenWidth * 0.25f), (Camera.Center.Y - ((Display.ScreenHeight * 0.25f))+(30*i))), Color.White,0,Vector2.Zero,0.8f,0,0.9f+(i*0.01f));
                    i++;
                }
            }


            if (!debug) return;

            Global.SpriteBatch.DrawString(
                Global.basicFont,
                $"Bytes up: {NetworkingUtil.BytesUp}\nBytes down: {NetworkingUtil.BytesDown}\nPackets up: {NetworkingUtil.PacketsUp}\nPackets down: {NetworkingUtil.PacketsDown}",
                new Vector2(Camera.Center.X - 300, Camera.Center.Y -300), Color.White);
        }

        public static uint hideMvpTick;

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
                            handShakePacket.Write(ContentLoader.loadedData.PlayerName);
                            Send(handShakePacket);
                        }
                        GameManager.duringRound = true;
                        udp.Connect(((IPEndPoint)tcp.socket.Client.LocalEndPoint).Port);
                        break;
                    case ServerPackets.PlayerJoined:
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
                    case ServerPackets.PlayerLeft:
                        byte leavingPly = _packet.ReadByte();
                        GameManager.instance.RemovePlayer(leavingPly);
                        break;
                    case ServerPackets.PlayerHit:
                        byte hitPly = _packet.ReadByte();
                        byte newHp = _packet.ReadByte();
                        GameManager.instance.ChangePlayerHealth(hitPly, newHp);
                        break;
                    case ServerPackets.PlayerKilled:
                        byte targetPly = _packet.ReadByte();
                        byte attackerPly = _packet.ReadByte();
                        GameManager.instance.KillPlayer(targetPly, attackerPly);
                        break;
                    case ServerPackets.PlayerReload:
                        byte reloadingPly = _packet.ReadByte();
                        GameManager.instance.PlayerReloaded(reloadingPly);
                        break;
                    case ServerPackets.PlayerRespawn:
                        byte respawningPly = _packet.ReadByte();
                        Vector2 respawnPos = _packet.ReadVector2();
                        GameManager.instance.RespawnPlayer(respawningPly, respawnPos);
                        break;
                    case ServerPackets.SyncServerSettings:
                        using (MemoryStream ms = new MemoryStream())
                        {
                            int length = _packet.ReadInt();
                            byte[] bytes = _packet.ReadBytes(length);

                            ms.Write(bytes, 0, length);

                            ms.Seek(0, SeekOrigin.Begin);
                            using (GZipStream gzip = new GZipStream(ms, CompressionMode.Decompress))
                            using (MemoryStream decompressed = new MemoryStream())
                            {
                                gzip.CopyTo(decompressed);
                                string json = System.Text.Encoding.UTF8.GetString(decompressed.ToArray());
                                Settings.Load(json);
                            }
                        }
                        break;
                    case ServerPackets.RoundEnd:
                        byte topKiller = _packet.ReadByte();
                        byte kills = _packet.ReadByte();
                        GameManager.topFrag = topKiller;
                        GameManager.topFragKills = kills;
                        GameManager.duringRound = false;
                        hideMvpTick = tick + 180;

                        break;
                }
            }
        }
    }
}
