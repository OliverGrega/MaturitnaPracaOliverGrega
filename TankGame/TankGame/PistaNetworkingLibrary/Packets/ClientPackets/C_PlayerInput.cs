using PistaNetworkLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.PistaNetworkingLibrary.Packets.ClientPackets
{
    public class C_PlayerInput : ClientPacket
    {
        private byte input;

        public C_PlayerInput()
        {
        }

        public C_PlayerInput(byte inputData)
        {
            input = inputData;
        }

        public override void Handle(byte _fromClient, PacketBuilder _packet)
        {
            byte inputData = _packet.ReadByte();

            Server.clients[_fromClient].player.HandleMovementInput(inputData);
        }

        public override void Write()
        {
            using (PacketBuilder _packet = new PacketBuilder(Id))
            {
                _packet.Write(input);
                MyClient.Send(_packet, Channel.UDP);
            }
        }
    }
}
