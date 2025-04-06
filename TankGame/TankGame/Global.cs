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

        public static float RandomFloat(float min, float max)
        {
            return (float)(Random.Shared.NextDouble() * (max - min) + min);
        }

        public static void PlaySound(string soundId, float volume = 1, float minPitch = 0, float maxPitch = 0)
        {
            ContentLoader.LoadSound(soundId).Play(volume, RandomFloat(minPitch, maxPitch), 0);
        }
    }
}
