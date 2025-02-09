using Microsoft.Xna.Framework;
using PistaNetworkLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.PistaNetworkingLibrary.Packets.ServerPackets
{
    public class PlayerPosServerPacket : ServerPacket
    {
        public PlayerPosServerPacket()
        {
        }

        public PlayerPosServerPacket(byte id, Vector2 pos, float rot, bool ignoreOwner = true)
        {
            Write(id, pos, rot, ignoreOwner);
        }

        public override void Handle(PacketBuilder _packet)
        {
            byte playerId = _packet.ReadByte();
            System.Numerics.Vector2 pos = _packet.ReadVector2();
            float rot = _packet.ReadFloat();
            Debug.WriteLine(pos);
            GameManager.players[playerId].SetPosRot(pos, rot);
        }

        public override void Write(params object[] _data)
        {
            using (PacketBuilder _packet = new PacketBuilder(Id))
            {
                byte playerId = (byte)_data[0];
                Vector2 pos = (Vector2)_data[1];
                float rot = (float)_data[2];
                _packet.Write(playerId);
                _packet.Write(pos.ToNumerics());
                _packet.Write(rot);
                if ((bool)_data[3])
                {
                    Server.SendAll(_packet, playerId, Channel.UDP);
                }
                else
                {
                    Server.SendAll(_packet, Channel.UDP);
                }
            }
        }
    }
}
