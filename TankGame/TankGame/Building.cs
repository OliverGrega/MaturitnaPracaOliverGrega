using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
    public class Building : Entity
    {
        public Building(Vector2 pos) : base("Content/Textures/Building.png", pos, 8)
        {
            Sprite.ChangeStacks(new Rectangle[6]
            {
                new Rectangle(0,0,8,7),
                new Rectangle(0,0,8,7),
                new Rectangle(8,0,8,7),
                new Rectangle(16,0,8,7),
                new Rectangle(16,0,8,7),
                new Rectangle(16,0,8,7)
            });

            Scale = new Vector2(8, 8);
        }

        public override void Update()
        {
            Rotation += 0.01f;
        }
    }
}
