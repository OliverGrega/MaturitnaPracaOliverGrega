using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
    public class StackedSprite
    {
        Texture2D Texture { get; set; }

        private Rectangle[] stacks;
        public float StackOffset { get; set; }
        public Vector2 Origin { get => stacks[0].Size.ToVector2() / 2; }
        public float Depth { get; set; } = 0.5f;

        public bool HasTexture { get =>  Texture != null; }
        public Rectangle DestRect { get; private set; }

        public StackedSprite(string texturePath, float stackOffset, Rectangle[] stacks)
        {
            Texture = ContentLoader.LoadTexture(texturePath);
            StackOffset = stackOffset;
            this.stacks = stacks;
        }
        public StackedSprite(string texturePath, float stackOffset)
        {
            Texture = ContentLoader.LoadTexture(texturePath);
            StackOffset = stackOffset;
        }

        public void Draw(Vector2 pos, float rot, Vector2 scale)
        {
            Draw(pos, rot, scale, Color.White);
        }

        public void Draw(Vector2 pos, float rot, Vector2 scale, Color color)
        {
            if (!HasTexture || stacks.Length == 0) return;

            DestRect = new Rectangle((int)pos.X - (int)stacks[0].Width, (int)pos.Y - (int)stacks[0].Height, stacks[0].Width * (int)scale.X, stacks[0].Height * (int)scale.Y);
            for (int i = 0; i < stacks.Length; i++)
            {
                Global.SpriteBatch.Draw(Texture, pos - new Vector2(0, i * StackOffset), stacks[i], color, rot, Origin, scale, SpriteEffects.None, Depth + i * 0.001f);
            }
        }

        public void ChangeStacks(Rectangle[] stacks)
        {
            this.stacks = stacks;
        }
    }
}
