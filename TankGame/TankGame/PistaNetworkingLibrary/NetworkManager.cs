using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;

namespace PistaNetworkLibrary
{
    public enum ItemNetworkAction : byte
    {
        spawn = 0,
        drop,
        pickup,
        use,
        equip,
        destroy
    }    

    public class NetworkManager
    {
        public static Dictionary<byte, ClientPacket> clientPackets;
        public static Dictionary<byte, ServerPacket> serverPackets;

        public NetworkManager()
        {
            clientPackets = new Dictionary<byte, ClientPacket>();
            serverPackets = new Dictionary<byte, ServerPacket>();

            var assembly = Assembly.GetCallingAssembly();
            Type[] types = assembly
                .GetTypes()
                .Where(x => x.IsSubclassOf(typeof(ClientPacket)))
                .OrderBy(q => q.GetType().Name).ToArray();

            MyDebugger.WriteLine($"REGISTERING CLIENT PACKETS FROM [{assembly.FullName}]");
            Debug.WriteLine($"REGISTERING CLIENT PACKETS FROM [{assembly.FullName}]");
            for (byte i = 0; i < types.Length; i++)
            {
                var initPacket = Activator.CreateInstance(types[i]) as ClientPacket;
                MyDebugger.WriteLine($"Registered client packet: [{clientPackets.Count},{initPacket.GetType().Name}]");
                Debug.WriteLine($"Registered client packet: [{clientPackets.Count},{initPacket.GetType().Name}]");
                clientPackets.Add(i, initPacket);
            }

            types = assembly
                .GetTypes()
                .Where(x => x.IsSubclassOf(typeof(ServerPacket)))
                .OrderBy(q => q.GetType().Name).ToArray();


            MyDebugger.WriteLine($"REGISTERING SERVER PACKETS FROM [{assembly.FullName}]");
            Debug.WriteLine($"REGISTERING SERVER PACKETS FROM [{assembly.FullName}]");
            for (byte i = 0; i < types.Length; i++)
            {
                var initPacket = Activator.CreateInstance(types[i]) as ServerPacket;
                MyDebugger.WriteLine($"Registered server packet: [{serverPackets.Count},{initPacket.GetType().Name}]");
                Debug.WriteLine($"Registered server packet: [{serverPackets.Count},{initPacket.GetType().Name}]");
                serverPackets.Add(i, initPacket);
            }
        }

        public static byte GetClientPacketId(ClientPacket _packet)
        {
            return clientPackets.FirstOrDefault(x => x.Value.GetType() == _packet.GetType()).Key;
        }
        public static byte GetServerPacketId(ServerPacket _packet)
        {
            return serverPackets.FirstOrDefault(x => x.Value.GetType() == _packet.GetType()).Key;
        }        
    }
}
