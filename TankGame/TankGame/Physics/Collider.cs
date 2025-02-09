using System.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Physics
{
    public struct Collider
    {
        public Vector2 Position { get; set; }
        public Vector2 HalfSize { get; set; }

        public Collider(Vector2 _pos,  Vector2 _halfSize)
        {
            Position = _pos;
            HalfSize = _halfSize;
        }

        public void GetMinMax(out Vector2 min, out Vector2 max)
        {
            min = Position - HalfSize;
            max = Position + HalfSize;
        }

        public static Collider GetSum(Collider col, Vector2 halfSize)
        {
            return new Collider(col.Position, col.HalfSize + halfSize);
        }
    }
}
