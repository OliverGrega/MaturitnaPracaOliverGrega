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
            if(smooothing)
            {
                smoothPos = Vector2.Lerp(smoothPos, Position, Global.DeltaTime * smoothAmount);
                if(Rotation - smoothRot < -MathF.PI)
                {
                    Rotation += MathF.PI * 2;
                }
                if (Rotation - smoothRot > MathF.PI)
                {
                    Rotation = MathF.PI * 2;
                }
                smoothRot = MathHelper.Lerp(smoothRot,Rotation, Global.DeltaTime * smoothAmount);
            }
        }

        public override void Draw()
        {
            if (smooothing)
            {
                Sprite.Draw(smoothPos, smoothRot - defaultRot, Scale);
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
        float smoothAmount = 10;
        protected Vector2 smoothPos = Vector2.Zero;
        float smoothRot = 0;
        protected float defaultRot;

        public virtual void Move()
        {
            Position += Velocity;
        }

        public void SetPosRot(Vector2 pos, float rot)
        {
            Position = pos;
            Rotation = rot;
        }

        #endregion
    }
}
