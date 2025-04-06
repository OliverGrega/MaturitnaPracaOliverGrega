using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TankGame.Particles
{
    public interface IEmitter
    {
        Vector2 EmitPosition { get; }
        float EmitRotation { get; }
        public bool CanEmit { get; set; }
    }
}
