using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
        public float PlayerRespawnTime { get; set; } = 6;
        public int RoundDuration { get; set; } = 20;
        public int EndDuration { get; set; } = 6;
        public int MapWidth { get; set; } = 100;
        public int MapHeight { get; set; } = 100;
        public int PlayerHealth { get; set; } = 3;

        public int RoundDurationInTicks => RoundDuration * TICK_RATE;
        public int EndDurationInTicks => EndDuration * TICK_RATE;

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

        public static string Compress()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string uncompressedString = JsonSerializer.Serialize(instance, options);

            byte[] compressedBytes;

            using (var uncompressedStream = new MemoryStream(Encoding.UTF8.GetBytes(uncompressedString)))
            {
                using (var compressedStream = new MemoryStream())
                {
                    // setting the leaveOpen parameter to true to ensure that compressedStream will not be closed when compressorStream is disposed
                    // this allows compressorStream to close and flush its buffers to compressedStream and guarantees that compressedStream.ToArray() can be called afterward
                    // although MSDN documentation states that ToArray() can be called on a closed MemoryStream, I don't want to rely on that very odd behavior should it ever change
                    using (var compressorStream = new DeflateStream(compressedStream, CompressionLevel.Fastest, true))
                    {
                        uncompressedStream.CopyTo(compressorStream);
                    }

                    // call compressedStream.ToArray() after the enclosing DeflateStream has closed and flushed its buffer to compressedStream
                    compressedBytes = compressedStream.ToArray();
                }
            }

            return Convert.ToBase64String(compressedBytes);
        }

        public static string Decompress(string compressedString)
        {
            byte[] decompressedBytes;

            var compressedStream = new MemoryStream(Convert.FromBase64String(compressedString));

            using (var decompressorStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
            {
                using (var decompressedStream = new MemoryStream())
                {
                    decompressorStream.CopyTo(decompressedStream);

                    decompressedBytes = decompressedStream.ToArray();
                }
            }

            return Encoding.UTF8.GetString(decompressedBytes);
        }
    }
}
