using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PistaNetworkLibrary
{
    
    public abstract class ClientPacket
    {
        /// <summary>Returns packet's Id</summary>
        protected byte Id => NetworkManager.GetClientPacketId(this);

        /// <summary>Writes packet on the client</summary>
        public abstract void Write(params object[] _data);

        /// <summary>Reads packet on the server</summary>
        public abstract void Handle(byte _fromClient, PacketBuilder _packet);
    }
}
