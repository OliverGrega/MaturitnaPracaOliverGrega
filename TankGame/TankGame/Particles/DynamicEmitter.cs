using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Particles
{
    public class DynamicEmitter : IEmitter
    {
        public Vector2 EmitPosition { get => target.Position; } 
        public float EmitRotation { get => target.Rotation + (-MathF.PI / 2); }
        public bool CanEmit { get; set; } = true;

        private Entity target;
        public DynamicEmitter(Entity _target)
        {
            target = _target;
        }
    }
}
