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
        public Color Color { get; set; } = Color.Wheat;

        StackedSprite tankIcon;
        float tankIconRot;

        private TextBox serverIpBox;

        public void Load(string msg)
        {
            objects = new List<GameObject>();
            tankIcon = new StackedSprite("Content/Textures/Tank.png", 6);
            tankIcon.ChangeStacks(new Rectangle[]
            {
                new Rectangle(16,0,16,16),
                new Rectangle(32,0,16,16),
                new Rectangle(48,0,16,16),
                new Rectangle(16,16,16,16),
                new Rectangle(32,16,16,16),
            });

            Camera.Target = null;

            if(msg != null )
            {
                objects.Add(new Label(msg, new Vector2((Display.ScreenWidth / 2), 600), Color.White, 0.5f)
                {
                    Allignment = Allignment.Center
                });
            }

            objects.Add(new Label("HRA S TANKMI", new Vector2((Display.ScreenWidth / 2), 50), Color.White)
            {
                Allignment = Allignment.Center
            });

            objects.Add(new Button(() =>
            {
                Global.Client.TryToConnect(serverIpBox.Content);
            }, "JOIN", new Vector2(Display.ScreenWidth / 5, 300), Color.White));

            objects.Add(new Button(() =>
            {
                Main.instance.Exit();
            }, "QUIT", new Vector2(Display.ScreenWidth / 5, 400), Color.White));

            objects.Add(new Button(() =>
            {
                System.Diagnostics.Process.Start("notepad.exe", "credits.txt");
            }, "CREDITS", new Vector2(Display.ScreenWidth / 5, 350), Color.White));

            objects.Add(new Label("SERVER IP:", new Vector2((Display.ScreenWidth / 2) - (150 * 0.7f), 280 - (25 * 0.7f)), Color.White, 0.5f));
            serverIpBox = new TextBox(21, new Vector2((Display.ScreenWidth / 2) - (150 * 0.7f), 300 - (25 * 0.7f)), new Vector2(550, 50), Color.White, 0.7f)
            {
                Content = "127.0.0.1:7777",
                OnConfirm = x =>
                {
                    Global.Client.TryToConnect(x);
                },
                numericsOnly = true,
                HoverSize = 0.8f
            };
            objects.Add(serverIpBox);

            objects.Add(new TextBox(21, new Vector2((Display.ScreenWidth * 0.75f) - (80 * 0.7f), (Display.ScreenHeight * 0.75f) - (120)), new Vector2(200, 50), Color.White, 0.5f)
            {
                Content = ContentLoader.loadedData.PlayerName,
                HoverSize = 0.6f,
                OnConfirm = x =>
                {
                    ContentLoader.loadedData.PlayerName = x;
                    ContentLoader.SaveSettings();
                }
            });

            //objects.Add(new Building(new Vector2(150, 150)));

            objects.Add(new Label($"Ver. {Global.GameVersion}", new Vector2(0,Display.ScreenHeight-25),Color.White,0.5f));
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
            tankIcon.Draw(new Vector2(Display.ScreenWidth * 0.75f, Display.ScreenHeight * 0.75f), tankIconRot, Vector2.One * 4, Color.White);

            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].Draw();
            }
        }
    }
}
