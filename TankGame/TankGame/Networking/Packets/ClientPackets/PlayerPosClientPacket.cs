using Microsoft.Xna.Framework;
using PistaNetworkLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Networking.Packets.ClientPackets
{
    public class PlayerPosClientPacket : ClientPacket
    {
        public PlayerPosClientPacket()
        {
        }

        public PlayerPosClientPacket(Vector2 pos, float rot)
        {
            Write(pos,rot);
        }

        public override void Handle(byte _fromClient, PacketBuilder _packet)
        {
            Vector2 pos = _packet.ReadVector2();
            float rot = _packet.ReadFloat();            

            Server.clients[_fromClient].player.MoveRot(pos, rot);
        }

        public override void Write(params object[] _data)
        {
            using (PacketBuilder _packet = new PacketBuilder(Id))
            {
                Vector2 pos = (Vector2)_data[0];
                float rot = (float)_data[1];
                _packet.Write(pos.ToNumerics());
                _packet.Write(rot);

                MyClient.Send(_packet, Channel.UDP);
            }
        }
    }
}
