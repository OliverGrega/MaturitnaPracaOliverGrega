using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TankGame.Particles
{ 
    public class MouseEmitter : IEmitter
    {
        Vector2 IEmitter.EmitPosition => Mouse.GetState().Position.ToVector2();

        public float EmitRotation => 0;
        public bool CanEmit { get; set; } = true;

    }
}
