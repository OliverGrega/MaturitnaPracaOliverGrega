using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TankGame_Server
{
    public class Settings
    {
        public static Settings instance;

        public int MaxPlayers { get; set; } = 8;
        public const int TICK_RATE = 30;
        public const float MS_PER_TICK = 1000f / TICK_RATE;
        public const float MIN_TIME_BETWEEN_TICKS = 1f / TICK_RATE;
        public float PlayerMoveSpeed { get; set; } = 100;
        public float PlayerTurnSpeed { get; set; } = 2;
        public float PlayerShootRange { get; set; } = 200;
        public float PlayerReloadTime { get; set; } = 3;
        public int PlayerHealth { get; set; } = 3;

        public Settings()
        {
            instance = this;
            //Init();
        }
        public static void Init()
        {
            Create();
            if (File.Exists("settings.json"))
            {
                Load();
                Draw.WriteLine("Loaded settings!", DrawFlags.Important);
            }
            else
            {
                Draw.WriteLine("No settings file found. Creating new one!", DrawFlags.Important);
                Create();
            }
        }

        private static void Load()
        {
            string jsonString = File.ReadAllText("settings.json");
            instance = JsonSerializer.Deserialize<Settings>(jsonString);
        }

        private static void Create()
        {
            using(StreamWriter sw = new StreamWriter("settings.json"))
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(instance, options);
                sw.Write(jsonString);
            }
        }
    }
}
