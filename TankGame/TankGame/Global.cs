using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
    public class Global
    {
        public delegate void PassObject(object i);
        public delegate void DestroyObject(object i);
        public static float DeltaTime { get; set; }
        public static float TotalTime { get; set; }
        public static SpriteBatch SpriteBatch { get; set; }
        public static GraphicsDeviceManager GraphicsDeviceManager { get; set; }
        public static SpriteFont basicFont;
        public static TankClient Client { get; set; }
        public static string GameVersion { get; } = "0.0.1";

        private static Dictionary<string, Texture2D> loadedTextures = new Dictionary<string, Texture2D>();

        public const float TIME_BETWEEN_TICKS = 1f / 30f;

        public static Texture2D LoadTexture(string path)
        {
            if(loadedTextures.ContainsKey(path))
            {
                return loadedTextures[path];
            }
            else
            {
                loadedTextures.Add(path, LoadTexturePng(path));
                return loadedTextures[path];
            }
        }

        private static Texture2D LoadTexturePng(string path)
        {
            FileStream stream;
            if (!File.Exists(path))
            {

                Debug.WriteLine($"Error: Could not find texture at {path}.");
                stream = new FileStream("Content/MissingTexture.png", FileMode.Open);
            }
            else
            {
                stream = new FileStream(path, FileMode.Open);
            }

            Texture2D tex = Texture2D.FromStream(SpriteBatch.GraphicsDevice, stream);
            Debug.WriteLine($"Loaded texture at {path}.");
            stream.Dispose();
            return tex;
        }

    }
}
