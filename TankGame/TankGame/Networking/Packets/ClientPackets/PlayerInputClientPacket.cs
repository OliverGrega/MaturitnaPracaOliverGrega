using PistaNetworkLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Networking.Packets.ClientPackets
{
    public class PlayerInputClientPacket : ClientPacket
    {
        public PlayerInputClientPacket()
        {
        }

        public PlayerInputClientPacket(byte inputData)
        {
            Write(inputData);
        }

        public override void Handle(byte _fromClient, PacketBuilder _packet)
        {
           byte inputData = _packet.ReadByte();

           Server.clients[_fromClient].player.HandleMovementInput(inputData);
        }

        public override void Write(params object[] _data)
        {
            using (PacketBuilder _packet = new PacketBuilder(Id))
            {
                _packet.Write((byte)_data[0]);
                MyClient.Send(_packet, Channel.UDP);
            }
        }
    }
}
