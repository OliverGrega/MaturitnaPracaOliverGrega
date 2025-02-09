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
        public Allignment Allignment { get; set; } = Allignment.Right;
        public virtual Vector2 Dimensions { get; }
        public float LayerDepth { get; set; }

        public Vector2 GetCenter(Allignment allignment, Vector2 dims)
        {
            switch(allignment)
            {
                case Allignment.Center:
                    return dims / 2;
                case Allignment.Right:
                    return Vector2.Zero;
                case Allignment.Left:
                    return new Vector2(dims.X, 0);
            }
            return Vector2.Zero;
        }
    }

    public enum Allignment
    {
        Center,
        Left,
        Right
    }
}
