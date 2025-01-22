using PistaNetworkLibrary;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Networking.Packets.ServerPackets
{
    public class SpawnPlayerServerPacket : ServerPacket
    {
        public SpawnPlayerServerPacket()
        {

        }
        public SpawnPlayerServerPacket(byte _toClient,byte _newPlayerId, string _username, Vector2 _spawn)
        {
            Write(_toClient, _newPlayerId, _username, _spawn);
        }

        public override void Handle(PacketBuilder _packet)
        {
            byte newId = _packet.ReadByte();
            string username = _packet.ReadString();
            Vector2 position = _packet.ReadVector2();

            GameManager.instance.SpawnPlayer(newId, username, position);
        }

        public override void Write(params object[] _data)
        {
            using (PacketBuilder _packet = new PacketBuilder(Id))
            {
                byte toClient = (byte)_data[0];
                byte newClientId = (byte)_data[1];
                string username = (string)_data[2];
                Vector2 pos = (Vector2)_data[3];
                _packet.Write(newClientId);
                _packet.Write(username);
                _packet.Write(pos);

                Server.Send(toClient, _packet);
            }
        }
    }
}
