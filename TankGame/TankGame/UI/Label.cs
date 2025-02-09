using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.UI
{
    public class Label : CanvasObject
    {
        public string Content { get; }
        public override Vector2 Dimensions { get => Global.basicFont.MeasureString(Content); }
        private Vector2 Origin => GetCenter(Allignment, Dimensions);

        public Label(string _content, Vector2 _pos, Color _color, float _scale = 1)
        {
            Content = _content;
            Position = _pos;
            Color = _color;
            Scale = _scale;
        }

        public override void Draw()
        {
            if (Visible)
            {
                Global.SpriteBatch.DrawString(Global.basicFont,Content,Position,Color, Rotation, Origin, Scale,0,LayerDepth);
            }
        }
    }
}
