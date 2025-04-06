using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Particles
{
    public struct ParticleEmitterData
    {
        public ParticleData particleData = new();
        public bool oneShot = false;
        public bool loop = false;
        public float angle = 0;
        public float angleVariance = 45;
        public float lifeSpanMin = 0.1f;
        public float lifeSpanMax = 2f;
        public float speedMin = 10;
        public float speedMax = 100;
        public float interval = 1;
        public int emitCount = 1;
        public float time = 1;

        public ParticleEmitterData()
        {

        }
    }
}
