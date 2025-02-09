using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Networking
{
    public class TickManager
    {
        public static TickManager Instance;

        private float timer;
        public uint tick;
        private readonly float minTimeBetweenTicks = 1f / SERVER_TICK_RATE;
        private const float SERVER_TICK_RATE = 30f;

        public TickManager()
        {
            Instance = this;

            tick = 0;
        }
    }
}
