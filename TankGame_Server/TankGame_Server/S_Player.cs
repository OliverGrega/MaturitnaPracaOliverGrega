using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TankGame;
using TankGame.Networking;
using TankGame.Physics;

namespace TankGame_Server
{
    public class S_Player
    {
        public byte owner;
        public string username;
        public Vector2 position;
        public float rotation;
        public byte hp;
        public bool isDead = false;
        private const float DRIVE_SPEED = 100;
        private const float TURN_SPEED = 2;

        private uint nextReloadTick;
        private bool isReloading = false;

        public Collider collider { get => new Collider(position, new Vector2(16, 16)); }

        private Vector2 Forward
        {
            get
            {
                var dir = new Vector2(MathF.Cos(rotation), MathF.Sin(rotation));
                return dir;
            }
        }

        public S_Player(byte owner, string username, Vector2 position)
        {
            this.owner = owner;
            this.username = username;
            this.position = position;

            hp = (byte)Settings.instance.PlayerHealth;

            serverStateBuffer = new CircularBuffer<StatePayload>(1024);
            serverInputQueue = new Queue<InputPayload>();
            positionHistory = new CircularBuffer<Vector2>(60);
        }

        #region PREDICTION        

        CircularBuffer<StatePayload> serverStateBuffer;
        Queue<InputPayload> serverInputQueue;

        public void HandleServerTick(uint currentTick)
        {
            if (isReloading)
            {
                if (currentTick >= nextReloadTick)
                {
                    isReloading = false;
                    ServerSend.PlayerReloaded(owner);
                }
            }
            TryRespawn();


            uint historyBufferIndex = currentTick % 60;

            positionHistory.Add(position, historyBufferIndex);

            uint bufferIndex = 0;
            while (serverInputQueue.Count > 0)
            {
                InputPayload inputPayload = serverInputQueue.Dequeue();

                bufferIndex = inputPayload.tick % 1024;

                StatePayload statePayload = SimulateMovement(inputPayload);
                serverStateBuffer.Add(statePayload, bufferIndex);
            }           

            if (bufferIndex == 0) return;
            if(!isDead) ServerSend.SendPlayerState(owner, serverStateBuffer.Get(bufferIndex));
        }

        private StatePayload SimulateMovement(InputPayload inputPayload)
        {
            Vector2 velocity = Forward * (Settings.instance.PlayerMoveSpeed * inputPayload.input.Y) * Settings.MIN_TIME_BETWEEN_TICKS;
            Ray moveRay = new Ray(position, velocity);
            
            foreach (var n in GameLogic.players)
            {
                if (n.Value == this) continue;
                RayHit hit = Physics.RayIntersectsCollider(moveRay, Collider.GetSum(n.Value.collider, collider.HalfSize));
                if (hit.IsHit)
                {
                    velocity += hit.Normal * new Vector2(MathF.Abs(velocity.X), MathF.Abs(velocity.Y)) * (1-hit.Time);
                }
            }

            foreach (var n in GameLogic.borders)
            {
                RayHit hit = Physics.RayIntersectsCollider(moveRay, Collider.GetSum(n, collider.HalfSize));
                if (hit.IsHit)
                {
                    velocity += hit.Normal * new Vector2(MathF.Abs(velocity.X), MathF.Abs(velocity.Y)) * (1 - hit.Time);
                }
            }

            position += velocity;
            rotation += inputPayload.input.X * Settings.instance.PlayerTurnSpeed * Settings.MIN_TIME_BETWEEN_TICKS;
            rotation = WrapAngle(rotation);
            return new StatePayload
            {
                tick = inputPayload.tick,
                position = position,
                rotation = rotation,
            };
        }

        public void HandleInput(InputPayload inputPayload)
        {
            serverInputQueue.Enqueue(inputPayload);
        }

        #endregion

        #region DETECT HIT

        private uint maxColliderRollbackTime = 60;

        private CircularBuffer<Vector2> positionHistory;

        private bool TryGetColliderAtTick(uint tick, out Collider rollback)
        {
            rollback = default;
            uint tickDiff = GameLogic.currentTick - tick;
            if (tickDiff >= maxColliderRollbackTime) return false;
        
            var bufferIndex = tick % 60;
            var pos = positionHistory.Get(bufferIndex);
            Console.WriteLine($"Tick: {tick}, PosAtTick: {pos}");
            rollback = new Collider(pos, new Vector2(16, 16));
        
            return true;
        }

        public void Shoot(uint attackTick, byte attacker)
        {
            if (isReloading) return;
            Vector2 from = position;
            Vector2 to = (Forward * Settings.instance.PlayerShootRange);
            MyDebugger.WriteLine($"Trying to shoot");
            Vector2 hitPoint = Vector2.Zero;
            foreach (var n in GameLogic.players.Values)
            {
                if (n == this || n.isDead) continue;
                if (n.DetectHit(from, to, attackTick, out Vector2 tmpHitPoint, out string msg))
                {
                    hitPoint = tmpHitPoint;
                    MyDebugger.WriteLine($"Player {username} hit player {n.username}!");
                    n.DealDamage(attacker);
                    break;
                }            
            }
            to += from;
            if(hitPoint != Vector2.Zero) to = hitPoint;
            ServerSend.SendPlayerShoot(owner, from, to);
            isReloading = true;
            nextReloadTick = GameLogic.currentTick + (uint)(Settings.instance.PlayerReloadTime * Settings.TICK_RATE);
        }

        public bool DetectHit(Vector2 from, Vector2 to, uint tick,out Vector2 hitPoint, out string msg)
        {
            Ray ray = new Ray(from, to);
            if(TryGetColliderAtTick(tick, out Collider rollback))
            {
                RayHit hit = Physics.RayIntersectsCollider(ray, rollback);
                Console.WriteLine($"FROM: {from}, TO: {to}, COLL_POS: {rollback.Position}");
                if(hit.IsHit)
                {
                    msg = "HIT";
                    hitPoint = hit.Position;
                    return true;
                }
                else
                {
                    msg = "MISS";
                    hitPoint = hit.Position;
                    return false;
                }
            }
            msg = "Tick is old";
            rollback = default;
            hitPoint = Vector2.Zero;
            return false;
        }

        #endregion

        #region HEALTH

        public bool DealDamage(byte attacker)
        {
            hp--;
            ServerSend.SendPlayerDamage(hp, owner);
            if (hp <= 0)
            {
                Die(attacker);
                return true;
            }
            return false;
        }

        public void Die(byte attacker)
        {
            Deaths++;
            GameLogic.players[attacker].Kills++;
            ServerSend.PlayerKilled(owner, attacker);
            nextRespawnTick = GameLogic.currentTick + (uint)(Settings.instance.PlayerRespawnTime * Settings.TICK_RATE);
            isDead = true;
        }

        private uint nextRespawnTick;
        public void TryRespawn()
        {
            if (isDead)
            {
                if(GameLogic.currentTick >= nextRespawnTick)
                {
                    isDead = false;
                    Respawn();
                }
            }
        }

        public void Respawn()
        {
            position = new Vector2(Random.Shared.Next(-Settings.instance.MapWidth/2, Settings.instance.MapWidth / 2), Random.Shared.Next(-Settings.instance.MapHeight / 2, Settings.instance.MapHeight / 2));
            ServerSend.RespawnPlayer(owner, position);
        }

        #endregion

        #region STATS

        public byte Kills { get; set; } = 0;
        public byte Deaths { get; set; } = 0;

        #endregion

        public static float WrapAngle(float angle)
        {
            if (angle > -MathF.PI && angle <= MathF.PI)
            {
                return angle;
            }

            angle %= MathF.PI * 2f;
            if (angle <= -MathF.PI)
            {
                return angle + MathF.PI * 2f;
            }

            if (angle > MathF.PI)
            {
                return angle - MathF.PI * 2f;
            }

            return angle;
        }
    }
}
