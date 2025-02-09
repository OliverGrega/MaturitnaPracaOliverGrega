using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
    public class Entity : GameObject
    {
        protected StackedSprite Sprite { get; }

        public Entity(string texturePath, Vector2 pos,float stackOffset)
        {
            Sprite = new StackedSprite(texturePath, stackOffset);
            Position = pos;
        }

        public override void Update()
        {

        }

        public override void Draw()
        {
            if (smooothing)
            {
                Sprite.Draw(Position, Rotation - defaultRot, Scale);
            }
            else
            {
                Sprite.Draw(Position, Rotation - defaultRot,Scale);
            }
        }

        #region TRANSFORM
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; } = Vector2.One;
        public Vector2 Velocity { get; set; } = Vector2.Zero;
        public Vector2 Forward
        {
            get
            {
                var dir = new Vector2(MathF.Cos(Rotation), MathF.Sin(Rotation));
                return dir;
            }
        }

        public Vector2 Right
        {
            get
            {
                var dir = new Vector2(MathF.Cos(Rotation + MathHelper.PiOver2), MathF.Sin(Rotation + MathHelper.PiOver2));
                return dir;
            }
        }


        protected bool smooothing;
        float smoothAmount = 20;
        protected float defaultRot;

        public virtual void SetPosRot(Vector2 pos, float rot)
        {
            Position = pos;
            Rotation = MathHelper.WrapAngle(rot);
        }

        #endregion
    }
}
