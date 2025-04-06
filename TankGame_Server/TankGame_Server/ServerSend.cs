using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TankGame;
using TankGame.Networking;
using TankGame.Networking.Server;

namespace TankGame_Server
{
    public static class ServerSend
    {
        public static void SendHandshake(byte _toClient, uint _currentTick)
        {
            using(PacketBuilder _packet = new PacketBuilder((byte)ServerPackets.Handshake))
            {
                _packet.Write(_toClient);
                _packet.Write(_currentTick);
                Server.Send(_toClient, _packet);
            }
        }

        public static void SendPlayerState(byte playerId, StatePayload statePayload)
        {
            using (PacketBuilder _packet = new PacketBuilder((byte)ServerPackets.PlayerPos))
            {
                _packet.Write(playerId);
                _packet.Write(statePayload.tick);
                _packet.Write(statePayload.position);
                _packet.Write(statePayload.rotation);

                Server.SendAll(_packet, Channel.UDP);
            }
        }

        public static void SendPlayerShoot(byte attacker, Vector2 from, Vector2 to)
        {
            using(PacketBuilder _packet = new PacketBuilder((byte)ServerPackets.PlayerShoot))
            {
                _packet.Write(attacker);
                _packet.Write(from);
                _packet.Write(to);

                Server.SendAll(_packet, Channel.UDP);
            }
        }

        public static void DespawnPlayer(byte ownerId)
        {
            using (PacketBuilder _packet = new PacketBuilder((byte)ServerPackets.PlayerLeft))
            {
                _packet.Write(ownerId);

                Server.SendAll(_packet, Channel.UDP);
            }
        }

        public static void SendPlayerDamage(byte newHp, byte ownerId)
        {
            using (PacketBuilder _packet = new PacketBuilder((byte)ServerPackets.PlayerHit))
            {
                _packet.Write(ownerId);
                _packet.Write(newHp);

                Server.SendAll(_packet, Channel.UDP);
            }
        }

        public static void PlayerKilled(byte targetId, byte attackerId)
        {
            using (PacketBuilder _packet = new PacketBuilder((byte)ServerPackets.PlayerKilled))
            {
                _packet.Write(targetId);
                _packet.Write(attackerId);

                Server.SendAll(_packet, Channel.UDP);
            }
        }

        public static void PlayerReloaded(byte playerId)
        {
            using (PacketBuilder _packet = new PacketBuilder((byte)ServerPackets.PlayerReload))
            {
                _packet.Write(playerId);

                Server.SendAll(_packet, Channel.UDP);
            }
        }

        public static void RespawnPlayer(byte playerId, Vector2 newPos)
        {
            using (PacketBuilder _packet = new PacketBuilder((byte)ServerPackets.PlayerRespawn))
            {
                _packet.Write(playerId);
                _packet.Write(newPos);

                Server.SendAll(_packet, Channel.UDP);
            }
        }

        public static void RoundEnded(byte _topKiller, byte killCount)
        {
            using (PacketBuilder _packet = new PacketBuilder((byte)ServerPackets.RoundEnd))
            {
                _packet.Write(_topKiller);
                _packet.Write(killCount);

                Server.SendAll(_packet, Channel.TCP);
            }
        }

        public static void NewRound()
        {
            using (PacketBuilder _packet = new PacketBuilder((byte)ServerPackets.NewRound))
            {
                Server.SendAll(_packet, Channel.TCP);
            }
        }

        public static void SendSettings(byte playerId)
        {
            using (PacketBuilder _packet = new PacketBuilder((byte)ServerPackets.SyncServerSettings))
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(Settings.instance, options);
                byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (GZipStream gzip = new GZipStream(ms, CompressionMode.Compress))
                    {
                        gzip.Write(jsonBytes, 0, jsonBytes.Length);
                    }

                    byte[] compressedData = ms.ToArray();
                    MyDebugger.WriteLine($"Sending settings to player {playerId} file size: {compressedData.Length}");
                    _packet.Write(compressedData.Length);
                    _packet.Write(compressedData);
                }

                Server.Send(playerId,_packet, Channel.TCP);
            }
        }
    }
}
