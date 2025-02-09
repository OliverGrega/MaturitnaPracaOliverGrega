using PistaNetworkLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.PistaNetworkingLibrary
{
    public enum ClientPackets : byte
    {
        HandshakeConfirm,
        Input
    }

    public enum ServerPackets : byte
    {

    }

    public struct InputPayload
    {
        public uint tick;
        public byte input;        
    }   

    
}
