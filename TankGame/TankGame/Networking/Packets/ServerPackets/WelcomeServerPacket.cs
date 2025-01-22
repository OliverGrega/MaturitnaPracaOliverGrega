using PistaNetworkLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TankGame.Networking.Packets.ClientPackets;

namespace TankGame.Networking.Packets.ServerPackets
{
    public class WelcomeServerPacket : ServerPacket
    {
        public WelcomeServerPacket()
        {
        }

        public WelcomeServerPacket(byte _toClient)
        {
            Write(_toClient);
        }

        public override void Handle(PacketBuilder _packet)
        {
            string _msg = _packet.ReadString();
            byte _myId = _packet.ReadByte();
            MyClient.instance.myId = _myId;
            MyDebugger.WriteLine(_msg);

            MyClient.Send(new WelcomeClientPacket("Tank #"+_myId));

            MyClient.instance.udp.Connect(((IPEndPoint)MyClient.instance.tcp.socket.Client.LocalEndPoint).Port);
        }

        public override void Write(params object[] _data)
        {
            using (PacketBuilder _packet = new PacketBuilder(Id))
            {
                int _toClient = (byte)_data[0];
                _packet.Write("Welcome to server!");
                _packet.Write(_toClient);

                MyDebugger.WriteLine("Sending welcome packet!");

                Server.Send(_toClient, _packet);
            }
        }
    }
}
