using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.UI
{
    public class CanvasObject : GameObject
    {
        public bool Visible { get; set; } = true;
        public Vector2 Position { get; set; } = Vector2.Zero;
        public Color Color { get; set; } = Color.White;
        public float Rotation { get; set; } = 0;
        public float Scale { get; set; } = 1;
        public virtual Vector2 Dimensions { get; }
        public float LayerDepth { get; set; }
    }
}
