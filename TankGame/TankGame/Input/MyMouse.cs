using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Input
{
    public static class MyMouse
    {
        public static bool dragging = false, rightDrag = false;
        public static Vector2 newMousePos, oldMousePos;
        public static MouseState newState, oldState;

        public static Rectangle MouseRect { get => new Rectangle((int)MouseWorldPosition.X, (int)MouseWorldPosition.Y, 1, 1); }
        public static Vector2 MouseWorldPosition { get => Vector2.Transform(new Vector2(newState.X, newState.Y), Matrix.Invert(Camera.Transform)); }

        public static void Update()
        {
            newState = Mouse.GetState();
            newMousePos = newState.Position.ToVector2();

            if(newState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Released)
            {
                
            }
        }

        public static void UpdateOld()
        {
            oldState = newState;
            oldMousePos = oldState.Position.ToVector2();
        }

        public static bool LeftButton()
        {
            return newState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Pressed;
        }

        public static bool LeftButtonDown()
        {
            return newState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Released;
        }

        public static bool LeftButtonUp()
        {
            return newState.LeftButton == ButtonState.Released && oldState.LeftButton == ButtonState.Pressed;
        }
    }
}
