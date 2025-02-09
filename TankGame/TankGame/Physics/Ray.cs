using System.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Physics
{
    public struct Ray
    {
        public Vector2 Position { get; set; }
        public Vector2 Direction { get; set; }

        public Ray(Vector2 position, Vector2 direction)
        {
            Position = position;
            Direction = direction;
        }

    }
}
