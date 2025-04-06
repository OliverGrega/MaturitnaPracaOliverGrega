using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Networking
{
    public class NetworkTimer
    {
        float timer;
        public float MinTimeBetweenTicks { get; }
        public uint currentTick;

        public NetworkTimer(float serverTickRate)
        {
            MinTimeBetweenTicks = 1f / serverTickRate;
        }

        public void Update(float deltaTime)
        {
            timer += deltaTime;
        }

        public bool ShouldTick()
        {
            if(timer >= MinTimeBetweenTicks)
            {
                timer -= MinTimeBetweenTicks;
                currentTick++;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            timer = 0;
            currentTick = 0;
        }
    }
}
