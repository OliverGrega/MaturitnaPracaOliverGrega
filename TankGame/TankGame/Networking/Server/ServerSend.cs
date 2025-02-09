using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Networking.Server
{
    public partial class Server
    {
        public static void Send(byte _toClient, PacketBuilder _packet, Channel _channel = Channel.TCP)
        {
            if (_channel == Channel.TCP)
            {
                Active.SendTCPData(_toClient, _packet);
            }
            else
            {
                Active.SendUDPData(_toClient, _packet);
            }
        }
        public static void SendAll(PacketBuilder _packet, Channel _channel = Channel.TCP)
        {
            if (_channel == Channel.TCP)
            {
                Active.SendTCPDataToAll(_packet);
            }
            else
            {
                Active.SendUDPDataToAll(_packet);
            }
        }
        public static void SendAll(PacketBuilder _packet, byte exceptId, Channel _channel = Channel.TCP)
        {
            if (_channel == Channel.TCP)
            {
                Active.SendTCPDataToAll(exceptId, _packet);
            }
            else
            {
                Active.SendUDPDataToAll(exceptId, _packet);
            }
        }

        #region TCP
        private void SendTCPData(byte _toClient, PacketBuilder _packet)
        {
            _packet.WriteLength();
            clients[_toClient].tcp.SendData(_packet);
        }


        private void SendTCPDataToAll(PacketBuilder _packet)
        {
            _packet.WriteLength();
            for (byte i = 1; i < MaxPlayers; i++)
            {
                clients[i].tcp.SendData(_packet);
            }
        }
        private void SendTCPDataToAll(byte _exceptClient, PacketBuilder _packet)
        {
            _packet.WriteLength();
            for (byte i = 1; i < MaxPlayers; i++)
            {
                if (i == _exceptClient) continue;
                clients[i].tcp.SendData(_packet);
            }
        }
        #endregion

        #region UDP

        private void SendUDPData(byte _toClient, PacketBuilder _packet)
        {
            _packet.WriteLength();
            clients[_toClient].udp.SendData(_packet);
        }

        private void SendUDPDataToAll(PacketBuilder _packet)
        {
            _packet.WriteLength();
            for (byte i = 1; i < MaxPlayers; i++)
            {
                clients[i].udp.SendData(_packet);
            }
        }
        private void SendUDPDataToAll(byte _exceptClient, PacketBuilder _packet)
        {
            _packet.WriteLength();
            for (byte i = 1; i < MaxPlayers; i++)
            {
                if (i == _exceptClient) continue;
                clients[i].udp.SendData(_packet);
            }
        }

        #endregion
    }
}
