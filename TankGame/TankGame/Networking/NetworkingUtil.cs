using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Networking
{
    public static class NetworkingUtil
    {
        public static int BytesDown { get; set; }
        public static int BytesUp { get; set; }
        public static int PacketsDown { get; set; }
        public static int PacketsUp { get; set; }

        public static void ResetTimer()
        {
            BytesDown = 0;
            BytesUp = 0;
            PacketsDown = 0;
            PacketsUp = 0;
        }

        public static void PacketReceived(int length)
        {
            BytesDown += length;
            PacketsDown++;
        }
        public static void PacketSent(int length)
        {
            BytesUp += length;
            PacketsUp++;
        }
    }
}
