using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankGame.Input;

namespace TankGame.UI
{
    public class Button : CanvasObject
    {
        public Action OnClick { get; set; }
        public string Content { get; set; }

        private Rectangle ButtonRect;

        public override Vector2 Dimensions => Global.basicFont.MeasureString(Content);

        public Button(Action _onClick, string _content, Vector2 _pos, Color _color)
        {
            OnClick = _onClick;
            Content = _content;
            Position = _pos;
            Color = _color;

            ButtonRect = new Rectangle((int)Position.X - (int)Dimensions.X / 2, (int)Position.Y - (int)Dimensions.Y / 2, (int)Dimensions.X, (int)Dimensions.Y);
        }

        public override bool Hover()
        {
            if (ButtonRect.Contains(MyMouse.MouseWorldPosition))
            {
                return true;
            }
            return false;
        }

        private float goalSize = 1;
        public override void Update()
        {
            if (Hover())
            {
                goalSize = 1.2f;
                Color = Color.Yellow;
                if (MyMouse.LeftButtonDown())
                {
                    OnClick?.Invoke();
                    
                }
            }
            else
            {
                goalSize = 1;
                Color = Color.White;
            }
            Scale = MathHelper.Lerp(Scale, goalSize, Global.DeltaTime * 10);
            if(Hover() && MyMouse.LeftButtonDown())
            {

            }
        }

        public override void Draw()
        {
            if (Visible)
            {
                Global.SpriteBatch.DrawString(Global.basicFont, Content, Position, Color, Rotation, Dimensions/2, Scale, SpriteEffects.None, LayerDepth);
            }
        }
    }
}
