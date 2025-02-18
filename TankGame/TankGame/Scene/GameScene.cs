﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Scene
{
    public class GameScene : IScene
    {
        public List<GameObject> objects { get; set; }


        private GameManager gameManager;
        public void Load(string msg)
        {
            GameManager.Spawn += Spawn;
            GameManager.Destroy += Despawn;
            objects = new List<GameObject>();
            gameManager = new GameManager();
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
            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].Draw();
            }
        }
    }
}
