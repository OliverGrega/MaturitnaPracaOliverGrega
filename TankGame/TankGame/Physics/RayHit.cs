using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Physics
{
    public struct RayHit
    {
        public bool IsHit { get; set; }
        public float Time { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Normal { get; set; }

        public RayHit(bool isHit, float time, Vector2 pos)
        {
            IsHit = isHit;
            Time = time;
            Position = pos;
            Normal = Vector2.Zero;
        }
    }
}
