using PistaNetworkLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.PistaNetworkingLibrary
{

    public static class ClientHandler
    {
        private delegate void PacketHandler(PacketBuilder _packet);
        private static Dictionary<byte, PacketHandler> packetHandlers = new Dictionary<byte, PacketHandler>()
        {

        };
    }
}
