using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace TankGame
{
    public static class Camera
    {
        private static Matrix transform;
        public static Matrix Transform
        {
            get { return transform; }
        }
        public static Vector2 Center { get => center; }

        private static Vector2 center;
        private static Viewport viewport;

        private static float zoom = 1.5f;
        private static float rotation = 0;

       //public static Vector2 ToLocal(float _x, float _y)
       //{
       //    Vector2 offset = center - (Display.canvas.ScreenDimensions / 2);
       //    return offset += new Vector2(_x, _y);
       //}

        public static float X
        {
            get { return center.X; }
            set { center.X = value; }
        }
        public static float Y
        {
            get { return center.X; }
            set { center.X = value; }
        }
        public static float Zoom
        {
            get { return zoom; }
            set { zoom = value; if (zoom < 0.1f) zoom = 0.1f; }
        }
        public static float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        public static float MaxZoom { get; set; } = 0.7f;
        public static float MinZoom { get; set; } = 3f;

        public static Entity Target { get; set; }
        static float desiredZoom;
        static Vector2 desiredCenter;

        public static void Setup(Viewport newViewport)
        {
            viewport = newViewport;
        }

        public static void ChangeZoom(float amount)
        {
            zoom += amount;
            zoom = MathHelper.Clamp(zoom, MaxZoom, MinZoom);
        }

        public static void Update()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.C))
            {
                Zoom += 0.01f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.V))
            {
                Zoom -= 0.01f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                Rotation += 0.01f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.T))
            {
                Rotation -= 0.01f;
            }

            if (Target != null) center = new Vector2(Target.Position.X, Target.Position.Y);
            else center = new Vector2(Display.ScreenWidth / 2, Display.ScreenHeight / 2);

            desiredZoom = MathHelper.Lerp(desiredZoom, zoom, Global.DeltaTime * 10);

            transform = Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0)) *
                                                 Matrix.CreateRotationZ(rotation) *
                                                 Matrix.CreateScale(new Vector3((int)desiredZoom, (int)desiredZoom, 1)) *
                                                 Matrix.CreateTranslation(new Vector3(Display.ScreenWidth / 2, Display.ScreenHeight / 2, 0));
        }
    }
}
