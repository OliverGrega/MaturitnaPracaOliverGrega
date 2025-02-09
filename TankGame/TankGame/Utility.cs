using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankGame.Physics;

namespace TankGame
{
    public static class Utility
    {
        static Texture2D whitePixel;

        public static void Setup()
        {
            whitePixel = new Texture2D(Global.SpriteBatch.GraphicsDevice, 1, 1);
            whitePixel.SetData(new Color[] { Color.White });
        }

        public static void DrawCollider(Collider collider, Color color, int width)
        {
            Rectangle rect = new Rectangle(new Point((int)(collider.Position.X - collider.HalfSize.X), (int)(collider.Position.Y - collider.HalfSize.Y)), new Point((int)collider.HalfSize.X*2, (int)(collider.HalfSize.Y*2)));
            DrawRectangle(rect, color, width);
        }

        public static void DrawRectangle(Rectangle rect, Color color, int width)
        {
            Global.SpriteBatch.Draw(whitePixel, new Rectangle(rect.X, rect.Y, rect.Width, width), color);
            Global.SpriteBatch.Draw(whitePixel, new Rectangle(rect.X, rect.Y, width, rect.Height), color);
            Global.SpriteBatch.Draw(whitePixel, new Rectangle(rect.X + rect.Width - width, rect.Y, width, rect.Height), color);
            Global.SpriteBatch.Draw(whitePixel, new Rectangle(rect.X, rect.Y + rect.Height - width, rect.Width, width), color);
        }

        public static void DrawCircle(Vector2 center, float radius, Color color, int width = 2, int segments = 16)
        {
            Vector2[] vertex = new Vector2[segments];

            float increment = MathF.PI * 2.0f / segments;
            float theta = 0.0f;

            for(int i = 0; i < segments; i++)
            {
                vertex[i] = center + radius  * new Vector2(MathF.Cos(theta), MathF.Sin(theta));
                theta += increment;
            }

            DrawPolygon(vertex, segments, color, width);
        }

        public static void DrawPolygon(Vector2[] vertex, int count, Color color, int lineWidth)
        {
            if (count > 0)
            {
                for (int i = 0; i < count - 1; i++)
                {
                    DrawLineSegment(vertex[i], vertex[i + 1], color, lineWidth);
                }
                DrawLineSegment(vertex[count - 1], vertex[0], color, lineWidth);
            }
        }
        public static void DrawLineSegment(Vector2 point1, Vector2 point2, Color color, int lineWidth)
        {
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float length = Vector2.Distance(point1, point2);

            Global.SpriteBatch.Draw(whitePixel, point1, null, color,
            angle, Vector2.Zero, new Vector2(length, lineWidth),
            SpriteEffects.None, 0f);
        }
    
        public static void Set(this ref byte byte1, int pos, bool value)
        {
            if (value)
            {
                byte1 = (byte)(byte1 | (1 << pos));
            }
            else
            {
                byte1 = (byte)(byte1 & ~(1 << pos));
            }
        }

        public static bool Get(this byte byte1, int pos)
        {
            return (byte1 & (1 << pos)) != 0;
        }
    }
}
