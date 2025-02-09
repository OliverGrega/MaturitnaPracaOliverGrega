using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TankGame
{
    public class ContentLoader
    {

        public ContentLoader()
        {
            CheckSettings();
            InitTextures();
            InitSounds();
        }

        #region TEXTURES

        static Dictionary<string, Texture2D> textures;
        string texturesPath = "Content/Textures/";
        string textureFileExtensions = "*.png";

        private void InitTextures()
        {
            textures = new Dictionary<string, Texture2D>();
            //string[] files = Directory.GetFiles(texturesPath, textureFileExtensions, SearchOption.AllDirectories);
            //
            //foreach (string file in files)
            //{
            //    if (textures.ContainsKey(Path.GetFileNameWithoutExtension(file))) continue;
            //    textures.Add(Path.GetFileNameWithoutExtension(file), LoadTexturePng(file));
            //}
        }

        public static Texture2D LoadTexture(string path)
        {
            if (textures.ContainsKey(path)) return textures[path];
            textures.Add(path, LoadTexturePng(path));
            return textures[path];
        }
        public static async Task<Texture2D> LoadTextureUrl(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                byte[] imageData = await client.GetByteArrayAsync(url);

                using (MemoryStream stream = new MemoryStream(imageData))
                {
                    return Texture2D.FromStream(Global.SpriteBatch.GraphicsDevice, stream);
                }
            }
        }

        public static Texture2D LoadTexturePng(string path)
        {
            FileStream stream;
            if (!File.Exists(path))
            {

                Debug.WriteLine($"Error: Could not find texture at {path}.");
                stream = new FileStream("Content/2D/MissingTexture.png", FileMode.Open);
            }
            else
            {
                stream = new FileStream(path, FileMode.Open);
            }

            Texture2D tex = Texture2D.FromStream(Global.SpriteBatch.GraphicsDevice, stream);
            Debug.WriteLine($"Loaded texture at {path}.");
            stream.Dispose();
            return tex;
        }

        #endregion

        #region SOUND

        static Dictionary<string, SoundEffect> sounds;

        string soundEffectsPath = "Content/Audio/";
        string soundFileExtensions = "*.mp3";

        private void InitSounds()
        {
            sounds = new Dictionary<string, SoundEffect>();
            //string[] files = Directory.GetFiles(soundEffectsPath, soundFileExtensions, SearchOption.AllDirectories);
            //
            //foreach (string file in files)
            //{
            //    if (sounds.ContainsKey(Path.GetFileNameWithoutExtension(file))) continue;
            //    sounds.Add(Path.GetFileNameWithoutExtension(file), LoadSoundMp3(file));
            //}
        }

        public static SoundEffect LoadSound(string path)
        {
            if (sounds.ContainsKey(path)) return sounds[path];
            sounds.Add(path, LoadSoundMp3(path));
            return sounds[path];
        }

        public static SoundEffect LoadSoundMp3(string path)
        {
            FileStream stream;
            if (!File.Exists(path))
            {

                Debug.WriteLine($"Error: Could not find sound at {path}.");
                stream = new FileStream("Content/Audio/Tonevim.mp3", FileMode.Open);
            }
            else
            {
                stream = new FileStream(path, FileMode.Open);
            }
            SoundEffect eff = SoundEffect.FromStream(stream);
            Debug.WriteLine($"Loaded texture at {path}.");
            stream.Dispose();
            return eff;
        }
        #endregion

        #region SAVEFILE

        public static SaveData loadedData;
        private const string SAVE_DATA_PATH = "savedata.json";

        public static void CheckSettings()
        {
            if(loadedData == null)
            {
                if (File.Exists(SAVE_DATA_PATH))
                {
                    LoadSettings();
                }
                else
                {
                    CreateNewSettings();
                }
            }
        }

        public static void LoadSettings()
        {
            string jsonString = File.ReadAllText(SAVE_DATA_PATH);
            loadedData = JsonSerializer.Deserialize<SaveData>(jsonString);
        }

        public static void SaveSettings()
        {
            using (StreamWriter sw = new StreamWriter(SAVE_DATA_PATH))
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(loadedData, options);
                sw.Write(jsonString);
            }
        }

        public static void CreateNewSettings()
        {
            using (StreamWriter sw = new StreamWriter(SAVE_DATA_PATH))
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(new SaveData("Guest"), options);
                sw.Write(jsonString);
            }
        }

        #endregion
    }

    [Serializable]
    public class SaveData
    {
        public string PlayerName { get; set; }

        public SaveData(string playerName)
        {
            PlayerName = playerName;
        }
    }
}
