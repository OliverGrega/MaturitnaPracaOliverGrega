using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
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
    }
}
