using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GameServer
{
    public class Settings
    {
        public static Settings instance;

        public int Port { get; set; } = 7777;
        public int MaxPlayers { get; set; } = 8;
        public int ServerTickRate { get; set; } = 60;
        public float MsPerTick;

        public static void Init()
        {
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
            instance.MsPerTick = 1000 / instance.ServerTickRate;
        }

        private static void Load()
        {
            string jsonString = File.ReadAllText("settings.json");
            instance = JsonSerializer.Deserialize<Settings>(jsonString);
        }

        private static void Create()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(instance, options);
            File.Create("settings.json").Dispose();
            File.WriteAllText("settings.json", jsonString);

        }
    }
}
