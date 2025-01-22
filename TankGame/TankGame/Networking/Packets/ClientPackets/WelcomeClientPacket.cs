using System;
using System.Collections.Generic;
using System.Linq;
using PistaNetworkLibrary;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Networking.Packets.ClientPackets
{
    public class WelcomeClientPacket : ClientPacket
    {
        public WelcomeClientPacket()
        {
        }

        public WelcomeClientPacket(string _username)
        {
            Write(_username);
        }

        public override void Handle(byte _fromClient, PacketBuilder _packet)
        {
            string username = _packet.ReadString();

            MyDebugger.WriteLine($"Player [{_fromClient}] joined with username: [{username}].");

            Server.clients[_fromClient].ConfirmConnection(username);
            Server.Joined?.Invoke(_fromClient);
        }

        public override void Write(params object[] _data)
        {
            using (PacketBuilder _packet = new PacketBuilder(Id))
            {
                _packet.Write((string)_data[0]);

                MyClient.Send(_packet);
            }
        }
    }
}
