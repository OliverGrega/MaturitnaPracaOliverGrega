using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PistaNetworkLibrary;
using TankGame.UI;

namespace TankGame.Scene
{
    internal class MenuScene : IScene
    {
        public List<GameObject> objects { get; set; }

        StackedSprite tankIcon;
        float tankIconRot;

        public void Load()
        {
            objects = new List<GameObject>();
            tankIcon = new StackedSprite("Content/Textures/Tank.png",4);
            tankIcon.ChangeStacks(new Rectangle[]
            {
                new Rectangle(16,0,16,16),
                new Rectangle(32,0,16,16),
                new Rectangle(48,0,16,16),
                new Rectangle(16,16,16,16),
                new Rectangle(32,16,16,16),
            });

            objects.Add(new Label("HRA S TANKMI", new Vector2(Display.ScreenWidth / 2, 50), Color.White));
            objects.Add(new Button(() =>
            {
                NetworkManager network = new NetworkManager();
                MyClient client = new MyClient();
                MyClient.instance.TryToConnect("127.0.0.1:7777");
            }, "JOIN", new Vector2(Display.ScreenWidth / 4, 300), Color.White));
        }
        public void Unload()
        {
            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].Destroy();
            }
            objects.Clear();
            objects = null;
        }
        public void Update()
        {
            tankIconRot += Global.DeltaTime;

            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].Update();
            }
        }
        public void Draw()
        {            
            tankIcon.Draw(new Vector2(Display.ScreenWidth * 0.75f, Display.ScreenHeight * 0.75f), tankIconRot, Vector2.One * 4, Color.Red);

            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].Draw();
            }
        }
    }
}
