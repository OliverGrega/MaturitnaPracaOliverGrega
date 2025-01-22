using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PistaNetworkLibrary;
using System.Diagnostics;
using TankGame.Input;
using TankGame.Scene;

namespace TankGame
{
    public class Main : Game
    {
        GameManager gameManager;

        public Main()
        {
            Global.GraphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Display.Init(640,640);
            base.Initialize();
            gameManager = new GameManager();
        }

        protected override void LoadContent()
        {
            Global.SpriteBatch = new SpriteBatch(GraphicsDevice);
            Utility.Setup();
            Camera.Setup(Global.SpriteBatch.GraphicsDevice.Viewport);
            Global.basicFont = Content.Load<SpriteFont>("Fonts/pixelfont");

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
            gameManager.FixedUpdate();
        }

        #endregion

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if(previousT == 0)
            {
                previousT = (float)gameTime.TotalGameTime.TotalMilliseconds;
            }

            float now = (float)gameTime.TotalGameTime.TotalMilliseconds;
            float frameTime = now - previousT;
            if(frameTime > maxFrameTime)
            {
                frameTime = maxFrameTime;
            }

            previousT = now;

            accumulator += frameTime;

            while(accumulator >= Global.FIXED_UPDATE_DELTA)
            {
                FixedUpdate();
                accumulator -= Global.FIXED_UPDATE_DELTA;
            }

            alpha = (accumulator / Global.FIXED_UPDATE_DELTA);

            Global.DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Global.TotalTime = (float)gameTime.TotalGameTime.Milliseconds;

            MyMouse.Update();

            ThreadManager.UpdateMain();
            Camera.Update();
            SceneManager.CurrentScene.Update();
            gameManager.Update();

            MyMouse.UpdateOld();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Wheat);

            Global.SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Camera.Transform);

            SceneManager.CurrentScene.Draw();
            gameManager.Draw();

            Global.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
