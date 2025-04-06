using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Particles
{
    public struct ParticleData
    {
        public Texture2D texture = ContentLoader.LoadTexture("Content/Textures/particles.png");
        public Rectangle sourceRect = new Rectangle(12, 0, 4, 4);
        public float lifeSpan = 2f;
        public Color colorStart = Color.Yellow;
        public Color colorEnd = Color.Red;
        public float opacityStart = 1f;
        public float opacityEnd = 0f;
        public float sizeStart = 1;
        public float sizeEnd = 0;
        public float rotationStart = 0f;
        public float rotationEnd = 6.28f;
        public float speed = 0.5f;
        public float angle = 0;
        public bool rotateToVelocity = false;
        public float depth = 0;

        public ParticleData()
        {

        }
    }
}
