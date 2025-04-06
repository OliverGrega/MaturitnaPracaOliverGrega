using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Particles
{
    public class StaticEmitter : IEmitter
    {
        public Vector2 EmitPosition { get; }

        public float EmitRotation { get; }
        public bool CanEmit { get; set; } = true;
        public StaticEmitter(Vector2 _pos, float emitRotation)
        {
            EmitPosition = _pos;
            EmitRotation = emitRotation;
        }
    }
}
