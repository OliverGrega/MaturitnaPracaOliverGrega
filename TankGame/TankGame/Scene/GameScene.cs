using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankGame.Physics;

namespace TankGame.Scene
{
    public class GameScene : IScene
    {
        public List<GameObject> objects { get; set; }
        public Color Color { get; set; } = Color.DarkGreen;

        private GameManager gameManager;

        private Texture2D backgroundTexture;

        public void Load(string msg)
        {
            GameManager.Spawn += Spawn;
            GameManager.Destroy += Despawn;
            objects = new List<GameObject>();
            gameManager = new GameManager();
            backgroundTexture = ContentLoader.LoadTexture("Content/Textures/Background.png");
        }

        public void Spawn(object obj)
        {
            objects.Add(obj as GameObject);
        }

        public void Despawn(object obj)
        {
            objects.Remove(obj as GameObject);
        }

        public void Unload()
        {
            GameManager.Spawn -= Spawn;
            GameManager.Destroy -= Despawn;
            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].Destroy();
            }
            objects.Clear();
            objects = null;
        }

        public void Update()
        {
            gameManager.Update();
            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].Update();
            }
        }

        public void Draw()
        {
            gameManager.Draw(); 
            TankClient.DrawDebug();
            Global.SpriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White);

            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].Draw();
            }

            if (GameManager.borders == null) return;
            foreach(var n in GameManager.borders)
            {
                Utility.DrawCollider(n, Color.Black);
            }
        }
    }
}
