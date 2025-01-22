using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PistaNetworkLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankGame;
using TankGame.Networking.Packets.ClientPackets;
using TankGame.Physics;

namespace TankGame
{
    public class Tank : Entity
    {
        public float speed;
        public static readonly float maxSpeed = 6;
        public static readonly float acceleration = 0.05f*4;
        public static readonly float decceleration = 0.05f;

        public byte id;
        public string name;

        public bool IsOwner { get => id == MyClient.instance.myId; }

        byte inputData;
        //left,right,forward,backward

        public Collider tankCollider { get => new Collider(Position, new Vector2(8, 8) * Scale); }

        public Tank(byte id, string username, Vector2 position) : base("Content/Textures/Tank.png", position,4)
        {
            Sprite.ChangeStacks(new Rectangle[]
            {
                new Rectangle(16,0,16,16),
                new Rectangle(32,0,16,16),
                new Rectangle(48,0,16,16),  
                new Rectangle(16,16,16,16),
                new Rectangle(32,16,16,16),
            });
            Scale = new Vector2(2.5f, 2.5f);
            defaultRot = -MathF.PI / 2;
            this.id = id;
            name = username;
            smooothing = true;
        }

        public override void Update()
        {
            base.Update();
            if (!IsOwner)
            {

                return;
            }
            HandleInput();

        }

        private void HandleInput()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                inputData.Set(7, true);
                //ChangeRotation(-0.1f);
            }
            else
            {
                inputData.Set(7, false);   
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                inputData.Set(6, true);
                //ChangeRotation(0.1f);
            }
            else
            {
                inputData.Set(6, false);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                Sprite.StackOffset -= 0.1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.E))
            {
                Sprite.StackOffset += 0.1f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                inputData.Set(5, true);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                inputData.Set(5, false);
                inputData.Set(4, true);
            }
            else
            {
                inputData.Set(5, false);
                inputData.Set(4, false);
            }

            MyClient.Send(new PlayerInputClientPacket(inputData));
            
        }

        public void ChangeRotation(float amount)
        {
            Rotation += amount;
            Rotation = MathHelper.WrapAngle(Rotation);
        }
        
        void MoveForward()
        {
            speed = Math.Min(speed + acceleration, maxSpeed);
            Velocity = Forward * speed;
            Move();
        }
        void MoveBackwards()
        {
            speed = Math.Max(speed - acceleration, -maxSpeed);
            Velocity = Forward * speed;
            Move();
        }
        void ReduceSpeed()
        {
            if(speed < 0)
            {
                speed = Math.Min(speed + decceleration, 0);
            }
            else
            {
                speed = Math.Max(speed - decceleration, 0);
            }
            Velocity = Forward * speed;
            Move();
        }

        public override void Draw()
        {
            base.Draw();
            if (smooothing)
            {
                Global.SpriteBatch.DrawString(Global.basicFont,
                    $"{name}",
                    smoothPos - new Vector2(0, 50),
                    Color.White,
                    0,
                    Global.basicFont.MeasureString(name) / 2,
                    0.5f,
                    0,
                    0);
            }
            else
            {
                Global.SpriteBatch.DrawString(Global.basicFont,
                    $"{name}",
                    Position - new Vector2(0,50),
                    Color.White,
                    0,
                    Global.basicFont.MeasureString(name)/2,
                    0.5f,
                    0,
                    0);
            }
            Utility.DrawRectangle(tankCollider.ToRect(), Color.Red, 1);
            Utility.DrawLineSegment(Position, Position + Velocity * 10, Color.Green, 4);
            Utility.DrawLineSegment(Position, Position + Right * speed * 10, Color.Red, 4);
        }

        float lastRot;
        public void FixedUpdate()
        {
            if (!IsOwner) return;

            //MyClient.Send(new PlayerInputClientPacket(inputData));
            //if(Velocity != Vector2.Zero || lastRot!=Rotation)
            //{
            //    MyClient.Send(new PlayerPosClientPacket(Position, Rotation));
            //    //MyClient.Send(new PlayerInputClientPacket());
            //    Debug.WriteLine(Convert.ToString(inputData,2).PadLeft(8, '0'));
            //    lastRot = Rotation;
            //}

        }
    }
}
