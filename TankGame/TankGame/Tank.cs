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
using TankGame.Particles;

namespace TankGame
{
    public class Tank : Entity
    {
        public float driveSpeed = 100;
        public float turnSpeed = 2;
        public static readonly float acceleration = 0.05f*4;
        public static readonly float decceleration = 0.05f;

        public byte id;
        public byte hp;
        public bool isDead = false;
        public bool isReloading = false;
        public string name;

        public byte kills, deaths;

        byte inputData;
        //left,right,forward,backward

        public bool IsOwner { get; }
        DynamicEmitter tireEmitter;
        ParticleEmitter tierParticleEmitter;
        public Collider tankCollider { get => new Collider(lastServerState.position, new Vector2(16, 16).ToNumerics()); }

        public Tank(byte id, string username, Vector2 position) : base("Content/Textures/Tank.png", position,6)
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

            hp = (byte)Settings.instance.PlayerHealth;

            if (IsOwner)
            {
                TankClient.OnTick += HandleClientTick;
                GameManager.localPlayer = this;

                clientInputBuffer = new CircularBuffer<InputPayload>(1024);
                Camera.Target = this;
            }
            else
            {
                smooothing = true;
            }
            clientStateBuffer = new CircularBuffer<StatePayload>(1024);


            tireEmitter = new DynamicEmitter(this);

            ParticleEmitterData pred = new ParticleEmitterData
            {
                particleData = new ParticleData
                {
                    colorEnd = Color.White,
                    colorStart = Color.Gray,
                    opacityStart = 1,
                    opacityEnd = 0,
                    rotationEnd = 0,
                    rotationStart = 0,
                    sizeStart = 1,
                    sizeEnd = 3,
                    sourceRect = new Rectangle(12, 0, 4, 4),
                    rotateToVelocity = true
                },
                oneShot = false,
                loop = true,
                emitCount = 3,
                lifeSpanMin = 0.4f,
                lifeSpanMax = 0.6f,
                speedMin = 40,
                speedMax = 60,
                angleVariance = 0.8f,
                time = 0.3f,
                interval = 0.1f,
                
            };
            tierParticleEmitter = new ParticleEmitter(tireEmitter, pred);
            ParticleManager.AddParticleEmitter(tierParticleEmitter);

            LoadReload();
            Respawn(position);
        }

        float timer = 0;
        public override void Update()
        {
            if (isDead) return;
            if (IsOwner)
            {                
                HandleInput();
            }
            else
            {
                if (Position != lastServerState.position)
                {
                    Position = Vector2.Lerp(Position, lastServerState.position, Global.DeltaTime * 10);
                    tireEmitter.CanEmit = true;
                }
                else tireEmitter.CanEmit = false;
            }
            UpdateReload();
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

            if (MyKeyboard.IsKeyClicked(Keys.Space))
            {
                ((TankClient)TankClient.Active).SendShoot();
            }
        }

        public void Respawn(Vector2 spawnPoint)
        {
            Position = spawnPoint;
            isDead = false;
            isReloading = false;
            ParticleManager.AddParticleEmitter(tierParticleEmitter);
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

        public void ResetStats()
        {
            deaths = 0;
            kills = 0;
        }

        #region NETWORKING

        CircularBuffer<StatePayload> clientStateBuffer;
        CircularBuffer<InputPayload> clientInputBuffer;
        StatePayload lastServerState;
        StatePayload lastProcessedState;

        public void HandleClientTick(uint currentTick)
        {
            if (input == Vector2.Zero)
            {
                tireEmitter.CanEmit = false;
                return;
            }
            tireEmitter.CanEmit = true;
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
            Vector2 velocity = Forward * (Settings.instance.PlayerMoveSpeed * inputPayload.input.Y) * Global.TIME_BETWEEN_TICKS;

            Physics.Ray moveRay = new Physics.Ray(Position.ToNumerics(), velocity.ToNumerics());
            foreach (var n in GameManager.players)
            {
                if (n.Value == this) continue;
                RayHit hit = Physics.Physics.RayIntersectsCollider(moveRay, Collider.GetSum(n.Value.tankCollider, tankCollider.HalfSize));
                if (hit.IsHit)
                {
                    velocity += hit.Normal * new Vector2(MathF.Abs(velocity.X), MathF.Abs(velocity.Y)) * (1 - hit.Time);
                }
            }

            if (GameManager.borders != null)
            {
                foreach (var n in GameManager.borders)
                {
                    RayHit hit = Physics.Physics.RayIntersectsCollider(moveRay, Collider.GetSum(n, tankCollider.HalfSize));
                    if (hit.IsHit)
                    {
                        velocity += hit.Normal * new Vector2(MathF.Abs(velocity.X), MathF.Abs(velocity.Y)) * (1 - hit.Time);
                    }
                }
            }


            Position += velocity;
            Rotation += inputPayload.input.X * Settings.instance.PlayerTurnSpeed * Global.TIME_BETWEEN_TICKS;
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

        #region HEALTH
        
        public void DealDamage(byte newHp)
        {
            hp = newHp;
        }

        public void Die()
        {
            isDead = true;
            deaths++;
            ParticleManager.RemoveParticleEmitter(tierParticleEmitter);
        }

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
            isReloading = true;
        }

        public override void Draw()
        {
            if (isDead)
            {
                if(IsOwner) DeadHUD();
                return;
            }
            DrawReload();

            if (!IsOwner)
            {
                Utility.DrawCollider(tankCollider, Color.Red, 1);
                Sprite.Draw(Position, Rotation - defaultRot, Scale, Color.Red);
            }
            else
            {
                Sprite.Draw(Position, Rotation - defaultRot, Scale, Color.White);
                Utility.DrawCollider(tankCollider, Color.LightGreen, 1);
            }

            Global.SpriteBatch.DrawString(Global.basicFont,
                $"{name}",
                Position - new Vector2(0, 25),
                Color.White,
                0,
                Global.basicFont.MeasureString(name) / 2,
                0.5f,
                0,
                0.8f);

            if (!IsOwner) return;

           //Global.SpriteBatch.DrawString(Global.basicFont,
           //$"{((TankClient)TankClient.Active).tick}",
           //new Vector2(50, 50),
           //Color.Red,
           //0,
           //Vector2.Zero,
           //3,
           //0,
           //0);
        }

        void DeadHUD()
        {
            Global.SpriteBatch.DrawString(Global.basicFont, "YOU ARE DEAD", new Vector2(Position.X, Position.Y-300), Color.White, 0, Global.basicFont.MeasureString("YOU ARE DEAD") / 2, 1, 0, 0.6f);
            Global.SpriteBatch.DrawString(Global.basicFont, "WAIT FOR RESPAWN", new Vector2(Position.X, Position.Y - 250), Color.White, 0, Global.basicFont.MeasureString("WAIT FOR RESPAWN") / 2, 1, 0, 0.6f);
        }

        #endregion

        #region RELOAD

        private const float reloadAnimSpeed = 10;
        private float reloadAnimState = 0;
        private const string reloadIconPath = "Content/Textures/ReloadIcon.png";
        private Texture2D reloadIconTexture;
        private void LoadReload()
        {
            reloadIconTexture = ContentLoader.LoadTexture(reloadIconPath);
        }

        private void UpdateReload()
        {
            if (!isReloading) return;
            reloadAnimState += reloadAnimSpeed * Global.DeltaTime;
            reloadAnimState = MathHelper.WrapAngle(reloadAnimState);
        }

        private void DrawReload()
        {
            if (isReloading)
            {
                Global.SpriteBatch.Draw(reloadIconTexture, Position - new Vector2(0, 50), null, Color.Orange, reloadAnimState, new Vector2(2,5.5f), 2, 0, 0.61f);
            }
        }

        public void Reload()
        {
            isReloading = false;
        }

        #endregion

        public override void OnDestroyed()
        {
            if(IsOwner) TankClient.OnTick -= HandleClientTick;

            ParticleManager.RemoveParticleEmitter(tierParticleEmitter);
        }        
    }
}
