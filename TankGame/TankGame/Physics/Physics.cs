using System.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Physics
{
    public static class Physics
    {
        public static RayHit RayIntersectsCollider(Ray ray, Collider collider)
        {
            RayHit hit = new RayHit(false, 0, Vector2.Zero);
            Vector2 min, max;
            collider.GetMinMax(out min, out max);
            float lastEntry = float.NegativeInfinity;
            float firstExit = float.PositiveInfinity;
            if (ray.Direction.X != 0)
            {
                float t1 = (min.X - ray.Position.X) / ray.Direction.X;
                float t2 = (max.X - ray.Position.X) / ray.Direction.X;

                lastEntry = Math.Max(lastEntry, Math.Min(t1, t2));
                firstExit = Math.Min(firstExit, Math.Max(t1, t2));
            }
            else if (ray.Position.X <= min.X || ray.Position.X >= max.X)
            {
                return hit; 
            }

            if (ray.Direction.Y != 0)
            {
                float t1 = (min.Y - ray.Position.Y) / ray.Direction.Y;
                float t2 = (max.Y - ray.Position.Y) / ray.Direction.Y;

                lastEntry = Math.Max(lastEntry, Math.Min(t1, t2));
                firstExit = Math.Min(firstExit, Math.Max(t1, t2));
            }
            else if (ray.Position.Y <= min.Y || ray.Position.Y >= max.Y)
            {
                return hit;
            }

            if (firstExit > lastEntry && firstExit > 0 && lastEntry < 1)
            {
                hit.Position = ray.Position + ray.Direction * lastEntry;

                hit.IsHit = true;
                hit.Time = lastEntry;

                float dx = hit.Position.X - collider.Position.X;
                float dy = hit.Position.Y - collider.Position.Y;
                float px = collider.HalfSize.X - Math.Abs(dx);
                float py = collider.HalfSize.Y - Math.Abs(dy);

                if (px < py)
                {
                    hit.Normal = new Vector2((dx > 0 ? 1 : 0) - (dx < 0 ? 1 : 0),0);
                }
                else
                {
                    hit.Normal = new Vector2(0, (dy > 0 ? 1 : 0) - (dy < 0 ? 1 : 0));
                }
            }
            return hit;

        }
    }
}
