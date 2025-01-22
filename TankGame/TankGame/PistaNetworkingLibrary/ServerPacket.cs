using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PistaNetworkLibrary
{
    public abstract class ServerPacket
    {
        /// <summary>Returns packet's id</summary>
        protected byte Id => NetworkManager.GetServerPacketId(this);

        /// <summary>Writes the packet on the server</summary>
        public abstract void Write(params object[] _data);

        /// <summary>Reads the packet on the client</summary>
        public abstract void Handle(PacketBuilder _packet);
    }
}
