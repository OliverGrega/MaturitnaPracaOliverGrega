using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PistaNetworkLibrary
{
    public enum Channel
    {
        TPC,
        UDP
    }

    public partial class Server
    {
        public static void Send(ServerPacket _msg)
        {

        }
        public static void Send(int _toClient, PacketBuilder _packet, Channel _channel = Channel.TPC)
        {
            if (_channel == Channel.TPC)
            {
                SendTCPData(_toClient, _packet);
            }
            else
            {
                SendUDPData(_toClient, _packet);
            }
        }
        public static void SendAll(PacketBuilder _packet, Channel _channel = Channel.TPC)
        {
            if (_channel == Channel.TPC)
            {
                SendTCPDataToAll(_packet);
            }
            else
            {
                SendUDPDataToAll(_packet);
            }
        }
        public static void SendAll(PacketBuilder _packet,byte exceptId, Channel _channel = Channel.TPC)
        {
            if (_channel == Channel.TPC)
            {
                SendTCPDataToAll(exceptId,_packet);
            }
            else
            {
                SendUDPDataToAll(exceptId,_packet);
            }
        }

        #region TCP
        private static void SendTCPData(int _toClient, PacketBuilder _packet)
        {
            _packet.WriteLength();
            Server.clients[_toClient].tcp.SendData(_packet);
        }


        private static void SendTCPDataToAll(PacketBuilder _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i < Server.MaxPlayer; i++)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }
        private static void SendTCPDataToAll(int _exceptClient, PacketBuilder _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i < Server.MaxPlayer; i++)
            {
                if (i == _exceptClient) continue;
                Server.clients[i].tcp.SendData(_packet);
            }
        }
        #endregion

        #region UDP

        private static void SendUDPData(int _toClient, PacketBuilder _packet)
        {
            _packet.WriteLength();
            Server.clients[_toClient].tcp.SendData(_packet);
        }

        private static void SendUDPDataToAll(PacketBuilder _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i < Server.MaxPlayer; i++)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }
        private static void SendUDPDataToAll(int _exceptClient, PacketBuilder _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i < Server.MaxPlayer; i++)
            {
                if (i == _exceptClient) continue;
                Server.clients[i].tcp.SendData(_packet);
            }
        }

        #endregion
    }
}
