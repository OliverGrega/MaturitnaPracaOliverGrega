using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Particles
{
    public class Particle
    {
        public ParticleData data;
        private Vector2 position;
        private float lifespanLeft;
        private float lifespanAmount;
        private Color color;
        private float opacity;
        public bool isFinised = false;
        private float scale;
        private float rotaion;
        private Vector2 origin;
        private Vector2 direction;

        public Particle(Vector2 _pos, ParticleData _data)
        {
            data = _data;
            lifespanLeft = data.lifeSpan;
            lifespanAmount = 1;
            position = _pos;
            color = data.colorStart;
            opacity = data.opacityStart;
            origin = data.sourceRect.Size.ToVector2() / 2;

            if (data.speed != 0)
            {
                //data.angle = MathHelper.ToRadians(data.angle);
                direction = new((float)Math.Sin(data.angle),-(float)Math.Cos(data.angle));
            }
            else
            {
                direction = Vector2.Zero;
            }
        }

        public void Update()
        {
            lifespanLeft -= Global.DeltaTime;
            if(lifespanLeft <= 0 )
            {
                isFinised = true;
                return;
            }

            lifespanAmount = MathHelper.Clamp(lifespanLeft / data.lifeSpan,0, 1);
            color = Color.Lerp(data.colorEnd, data.colorStart, lifespanAmount);
            opacity = MathHelper.Clamp(MathHelper.Lerp(data.opacityEnd,data.opacityStart, lifespanAmount), 0, 1);
            scale = MathHelper.Lerp(data.sizeEnd,data.sizeStart, lifespanAmount);
            rotaion = MathHelper.Lerp(data.rotationEnd,data.rotationStart, lifespanAmount);
            position += direction * data.speed * Global.DeltaTime;
            if (data.rotateToVelocity) rotaion = data.angle;
        }

        public void Draw()
        {
            Global.SpriteBatch.Draw(data.texture, position, data.sourceRect, color * opacity, rotaion, origin,scale, SpriteEffects.None, data.depth);
        }
    }
}
