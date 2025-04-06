using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TankGame.Particles
{
    public class ParticleEmitter
    {
        private readonly ParticleEmitterData data;
        private float intervalLeft;
        private float timeLeft;
        private readonly IEmitter emitter;

        public ParticleEmitter(IEmitter _emitter, ParticleEmitterData _data)
        {
            emitter = _emitter;
            data = _data;
            intervalLeft = data.interval;
            timeLeft = data.time;
            
        }
        private void Emit(Vector2 _pos)
        {
            if (emitter.CanEmit == false) return;
            ParticleData d = data.particleData;
            d.lifeSpan = Global.RandomFloat(data.lifeSpanMin, data.lifeSpanMax);
            d.speed = Global.RandomFloat(data.speedMin, data.speedMax);
            float r = (float)(Random.Shared.NextDouble() * 2) - 1;
            d.angle = (Global.RandomFloat(-data.angleVariance,data.angleVariance)) + emitter.EmitRotation;

            Particle p = new(_pos, d);
            ParticleManager.AddParticle(p);
        }

        public void Play()
        {
            var pos = emitter.EmitPosition;
            for (int i = 0; i < data.emitCount; i++)
            {
                Emit(pos);
            }
        }

        public void Update()
        {
            if (data.oneShot)
            {
                Play();
                ParticleManager.RemoveParticleEmitter(this);
                return;
            }
            intervalLeft -= Global.DeltaTime;

            if (!data.loop)
            {
                if(timeLeft != float.PositiveInfinity)
                {
                    timeLeft -= Global.DeltaTime;
                    if (timeLeft <= 0) ParticleManager.RemoveParticleEmitter(this);
                }
            }
            
            while (intervalLeft <= 0)
            {
                intervalLeft += data.interval;
                Play();
            }           

        }
    }
}
