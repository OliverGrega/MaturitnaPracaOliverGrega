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
using TankGame.PistaNetworkingLibrary.Packets.ServerPackets;
using TankGame.Physics;
using TankGame.Networking;
using TankGame.Input;

namespace TankGame
{
    public class Tank : Entity
    {
        public float driveSpeed = 100;
        public float turnSpeed = 2;
        public static readonly float acceleration = 0.05f*4;
        public static readonly float decceleration = 0.05f;

        public byte id;
        public string name;

        byte inputData;
        //left,right,forward,backward

        public bool IsOwner { get; }

        public Collider tankCollider { get => new Collider(lastServerState.position, new Vector2(16, 16).ToNumerics()); }

        public Tank(byte id, string username, Vector2 position) : base("Content/Textures/Tank.png", position,4)
        {
            IsOwner = (id == TankClient.Active.myId);

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

            if (IsOwner)
            {
                TankClient.OnTick += HandleClientTick;

                clientInputBuffer = new CircularBuffer<InputPayload>(1024);
            }
            else
            {
                smooothing = true;
            }
            clientStateBuffer = new CircularBuffer<StatePayload>(1024);
        }

        float timer = 0;
        public override void Update()
        {
            if (IsOwner)
            {                
                HandleInput();
            }
            else
            {
                timer += Global.DeltaTime;
                timer = Math.Clamp(timer / 10, 0, 1);
                Position = Vector2.Lerp(lastProcessedState.position, lastServerState.position, Global.DeltaTime * 15);
            }
        }

        #region INPUT

        private Vector2 input;

        private void HandleInput()
        {
            input = new Vector2();            
            if (Keyboard.GetState().IsKeyDown(Keys.W)) input.Y = 1;
            else if (Keyboard.GetState().IsKeyDown(Keys.S)) input.Y = -1;
            if (Keyboard.GetState().IsKeyDown(Keys.A)) input.X = -1;
            else if (Keyboard.GetState().IsKeyDown(Keys.D)) input.X = 1;

            if (Keyboard.GetState().IsKeyDown(Keys.Q)) Position += Forward * 5;

            if (MyKeyboard.IsKeyClicked(Keys.Space))
            {
                ((TankClient)TankClient.Active).SendShoot();
            }
        }

        private byte ConvertInput(Vector2 input)
        {
            byte fin = 0;
            if (input.Y == 1) fin.Set(7, true);
            else if (input.Y == -1) fin.Set(6, true);
            if (input.X == 1) fin.Set(5, true);
            else if (input.X == -1) fin.Set(4, true);
            return fin;
        }

        #endregion

        #region TRANSFORM
        public void ChangeRotation(float amount)
        {
            Rotation += amount;
            Rotation = MathHelper.WrapAngle(Rotation);
        }              

        public override void SetPosRot(Vector2 pos, float rot)
        {
            base.SetPosRot(pos, rot);
        }

        #endregion

        #region NETWORKING

        CircularBuffer<StatePayload> clientStateBuffer;
        CircularBuffer<InputPayload> clientInputBuffer;
        StatePayload lastServerState;
        StatePayload lastProcessedState;

        public void HandleClientTick(uint currentTick)
        {
            if (input == Vector2.Zero) return;
            uint bufferIndex = currentTick % 1024;

            InputPayload inputPayload = new InputPayload
            {
                tick = currentTick,
                input = this.input.ToNumerics()
            };
            clientInputBuffer.Add(inputPayload, bufferIndex);

            byte compressedInput = ConvertInput(input);
            if (compressedInput != 0) ((TankClient)TankClient.Active).SendInput(compressedInput);

            StatePayload statePayload = ProcessMovement(inputPayload);
            clientStateBuffer.Add(statePayload, bufferIndex);

            HandleServerReconciliation();
        }

        private StatePayload ProcessMovement(InputPayload inputPayload)
        {
            Position += Forward * (driveSpeed * inputPayload.input.Y) * Global.TIME_BETWEEN_TICKS;
            Rotation += inputPayload.input.X * turnSpeed * Global.TIME_BETWEEN_TICKS;
            Rotation = MathHelper.WrapAngle(Rotation);
            return new StatePayload
            {
                tick = inputPayload.tick,
                position = Position.ToNumerics(),
                rotation = Rotation,
            };
        }
        public void SyncTransform(StatePayload state)
        {
            if (!IsOwner)
            {
                lastProcessedState = lastServerState;
                Rotation = MathHelper.WrapAngle(state.rotation);
            }
            lastServerState = state;
        }

        #region RECONCILIATION
        private bool ShouldReconcile()
        {
            bool isNewServerState = !lastServerState.Equals(default);
            bool isLastStateUndefined = lastProcessedState.Equals(default) || !lastProcessedState.Equals(lastServerState);

            return isNewServerState && isLastStateUndefined;
        }
        private void HandleServerReconciliation()
        {
            if (!ShouldReconcile()) return;

            float positionError = 0;
            uint bufferIndex;
            StatePayload rewindState = default;

            bufferIndex = lastServerState.tick % 1024;           

            rewindState = lastServerState;
            positionError = Vector2.Distance(rewindState.position, clientStateBuffer.Get(bufferIndex).position);
            if (positionError > 10)
            {
                ReconcileState(rewindState);
            }

            lastProcessedState = lastServerState;
        }

        private void ReconcileState(StatePayload rewindState)
        {
            Position = rewindState.position;
            Rotation = rewindState.rotation;
            
            if (!rewindState.Equals(lastServerState)) return;

            clientStateBuffer.Add(rewindState, rewindState.tick);

            uint tickToReplay = lastServerState.tick;

            while(tickToReplay < ((TankClient)TankClient.Active).tick)
            {
                uint bufferIndex = tickToReplay % 1024;
                StatePayload statePayload = ProcessMovement(clientInputBuffer.Get(bufferIndex));
                clientStateBuffer.Add(statePayload, bufferIndex);
                tickToReplay++;
            }
        }

        #endregion

        #endregion

        #region DRAWING

        public void Shoot(Vector2 from, Vector2 to)
        {
            Physics.Ray ray = new Physics.Ray(from.ToNumerics(), to.ToNumerics());
            foreach(var n in GameManager.players.Values)
            {
                if (n == this) continue;
                RayHit hit = Physics.Physics.RayIntersectsCollider(ray, n.tankCollider);
            }
        }

        public override void Draw()
        {
            if (!IsOwner)
            {
                Utility.DrawCollider(tankCollider, Color.Red, 1);
                Sprite.Draw(Position, Rotation - defaultRot, Scale, Color.Red);
                Global.SpriteBatch.DrawString(Global.basicFont,
                    $"{name}",
                    Position - new Vector2(0, 50),
                    Color.White,
                    0,
                    Global.basicFont.MeasureString(name) / 2,
                    0.5f,
                    0,
                    0);
            }
            else
            {
                Sprite.Draw(Position, Rotation - defaultRot, Scale, Color.White);
                Utility.DrawCollider(tankCollider, Color.Green, 1);
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
            if (!IsOwner) return;

            Global.SpriteBatch.DrawString(Global.basicFont,
            $"{((TankClient)TankClient.Active).tick}",
            new Vector2(50, 50),
            Color.Red,
            0,
            Vector2.Zero,
            3,
            0,
            0);
        }

        #endregion

        public override void OnDestroyed()
        {
            if(IsOwner) TankClient.OnTick -= HandleClientTick;
        }        
    }
}
