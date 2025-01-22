using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
    public class Display
    {
        public static int ScreenWidth { get; private set; }
        public static int ScreenHeight { get; private set; }

        public static Vector2 ScreenSize { get => new Vector2(ScreenWidth, ScreenHeight); }

        private static bool isFullscreen, isBorderless = true;

        //public static Canvas canvas;

        private static int defaultWidth,defaultHeight;

        

        public static void Init(int _screenWidth, int _screenHeight)
        {
            //canvas = new Canvas(Global.graphicsManager.GraphicsDevice, _screenWidth, _screenHeight);
            ChangeResolution(_screenWidth, _screenHeight);
            defaultWidth = _screenWidth;
            defaultHeight = _screenHeight;            
        }

        public static void ChangeResolution(int _screenWidth, int _screenHeight)
        {            
            ScreenWidth = _screenWidth;
            ScreenHeight = _screenHeight;
            Global.GraphicsDeviceManager.PreferredBackBufferHeight = ScreenHeight;
            Global.GraphicsDeviceManager.PreferredBackBufferWidth = ScreenWidth;
            Global.GraphicsDeviceManager.ApplyChanges();
            //canvas.SetDestRect();
        }

        public static void ToggleFullscreen()
        {
            Global.GraphicsDeviceManager.ToggleFullScreen();
            Global.GraphicsDeviceManager.ApplyChanges();
            isFullscreen = !isFullscreen;
            //canvas.SetDestRect();            
        }       

        
    }
}
