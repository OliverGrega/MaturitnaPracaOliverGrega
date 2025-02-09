using PistaNetworkLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.PistaNetworkingLibrary
{
    public interface INetworkPayload
    {
        public uint Tick { get; set; }
        public void Write();
        public void Read();
    }
}
