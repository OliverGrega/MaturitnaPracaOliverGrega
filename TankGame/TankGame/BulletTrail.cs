using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
    public class BulletTrail : GameObject
    {
        public Vector2 Start { get; set; }
        public Vector2 End { get; set; }
        public float LifeTime { get; set; }
        private Color trailColor = Color.White;
        private Color endColor = new Color(1, 1, 1, 0);

        public BulletTrail(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
            LifeTime = 0;
        }

        public override void Update()
        {
            LifeTime += Global.DeltaTime;

            trailColor = Color.Lerp(Color.White, endColor, LifeTime / 2);
            if(LifeTime >= 2)
            {
                GameManager.Destroy(this);
            }
        }

        public override void Draw()
        {
            Utility.DrawLineSegment(Start, End, trailColor, 1);
        }
    }
}
