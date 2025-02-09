using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Input
{
    public static class MyKeyboard
    {
        public static KeyboardState CurrentState { get { return currKeyboardState; } }

        private static KeyboardState prevKeyboardState;
        private static KeyboardState currKeyboardState;

        public static void Update()
        {
            prevKeyboardState = currKeyboardState;
            currKeyboardState = Keyboard.GetState();
        }

        public static bool IsKeyDown(Keys key)
        {
            return currKeyboardState.IsKeyDown(key);
        }
        public static bool IsKeyClicked(Keys key)
        {
            return currKeyboardState.IsKeyDown(key) & !prevKeyboardState.IsKeyDown(key);
        }
        public static bool IsKeyUp(Keys key)
        {
            return currKeyboardState.IsKeyUp(key) & prevKeyboardState.IsKeyDown(key);
        }
    }
}
