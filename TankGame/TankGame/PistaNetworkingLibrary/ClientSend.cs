using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

namespace PistaNetworkLibrary
{
    public partial class MyClient
    {
        public static void Send(ClientPacket _packet)
        {

        }

        public static void Send(PacketBuilder _packet, Channel _channel = Channel.TPC)
        {
            if (_channel == Channel.TPC) SendTCPData(_packet);
            else SendUDPData(_packet);
        }
        public static void SendTCPData(PacketBuilder _packet)
        {
            _packet.WriteLength();
            MyClient.instance.tcp.SendData(_packet);
        }
        private static void SendUDPData(PacketBuilder _packet)
        {
            _packet.WriteLength();
            MyClient.instance.udp.SendData(_packet);
        }
    }
}
