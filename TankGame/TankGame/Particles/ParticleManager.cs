using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Particles
{
    public static class ParticleManager
    {
        private static readonly List<Particle> particles = new();
        private static readonly List<ParticleEmitter> particleEmitters = new();

        public static void AddParticle(Particle particle)
        {
            particles.Add(particle);
        }
        public static void AddParticleEmitter(ParticleEmitter emitter)
        {
            particleEmitters.Add(emitter);
        }

        public static void RemoveParticleEmitter(ParticleEmitter emitter)
        {
            particleEmitters.Remove(emitter);
        }
        private static void UpdateParticles()
        {
            foreach (var particle in particles)
            {
                particle.Update();
            }

            particles.RemoveAll(x => x.isFinised);
        }
        private static void UpdateEmitters()
        {
            for(int i = particleEmitters.Count-1; i >= 0; i--)
            {
                particleEmitters[i].Update();
            }
        }

        public static void ClearAll()
        {
            particles.Clear();
            particleEmitters.Clear();
        }

        public static void Update()
        {
            UpdateParticles();
            UpdateEmitters();
        }
        public static void Draw()
        {
            foreach (var particle in particles)
            {
                particle.Draw();
            }
        }
    }
}
