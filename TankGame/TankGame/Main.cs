using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using TankGame.Input;
using TankGame.Scene;
using TankGame.Networking;

namespace TankGame
{
    public class Main : Game
    {
        public static Main instance;
        public Main()
        {
            instance = this;
            Global.GraphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            new ContentLoader();
            Display.Init(640,640);
            base.Initialize();            
        }

        protected override void LoadContent()
        {
            Global.SpriteBatch = new SpriteBatch(GraphicsDevice);
            Utility.Setup();
            Camera.Setup(Global.SpriteBatch.GraphicsDevice.Viewport);
            Global.basicFont = Content.Load<SpriteFont>("Fonts/pixelfont");
            Global.Client = new TankClient();

            SceneManager.LoadScene(new MenuScene());
            

            //NetworkManager network = new NetworkManager();
            //MyClient client = new MyClient();
            //MyClient.instance.TryToConnect("127.0.0.1:7777");
        }

        #region FIXEDUPDATE

        private float previousT = 0;
        private float accumulator = 0;
        private float maxFrameTime = 250;
        private float alpha = 0;

        private void FixedUpdate()
        {

        }

        #endregion

        float packetTimer;
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Global.DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Global.TotalTime = (float)gameTime.TotalGameTime.Milliseconds;

            MyKeyboard.Update();
            MyMouse.Update();

            packetTimer += Global.DeltaTime;
            if(packetTimer >=1)
            {
                NetworkingUtil.ResetTimer();
                packetTimer = 0;
            }

            ThreadManager.UpdateMain();
            Camera.Update();
            SceneManager.CurrentScene.Update();

            if (TankClient.Active.isConnected) ((TankClient)TankClient.Active).Update();
            MyMouse.UpdateOld();
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Wheat);

            Global.SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Camera.Transform);

            SceneManager.CurrentScene.Draw();

            Global.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
