using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankGame;
using TankGame.Networking.Packets.ServerPackets;
using TankGame.Physics;

namespace PistaNetworkLibrary
{
    public class PlayerInfo
    {
        public byte Id {  get; set; }
        public string Username { get; set; }
        public Vector2 Position;
        public float Rotation { get; set; }
        public Vector2 Velocity;
        public Vector2 Forward
        {
            get
            {
                var dir = new Vector2(MathF.Cos(Rotation), MathF.Sin(Rotation));
                return dir;
            }
        }

        private float speed = 0;

        public const float ROLLBACK_DISTANCE = 2;

        public Collider serverCollider => new Collider(Position, new Vector2(8, 8) * 2.5f);

        public void HandleMovementInput(byte input)
        {
            Vector2 prevPos = Position;
            float prevRot = Rotation;

            if (input.Get(7))
            {
                Rotation -= 0.1f;
                MathHelper.WrapAngle(Rotation);
            }
            if (input.Get(6))
            {
                Rotation += 0.1f;
                MathHelper.WrapAngle(Rotation);
            }

            if (input.Get(5))
            {
                speed = MathF.Min(speed + Tank.acceleration, Tank.maxSpeed);
                Velocity = Forward * speed;
            }
            else if (input.Get(4))
            {
                speed = Math.Max(speed - Tank.acceleration, -Tank.maxSpeed);
                Velocity = Forward * speed;
            }
            else
            {
                if (speed < 0)
                {
                    speed = Math.Min(speed + Tank.decceleration, 0);
                }
                else
                {
                    speed = Math.Max(speed - Tank.decceleration, 0);
                }
                Velocity = Forward * speed;
            }

            TankGame.Physics.Ray ray = new TankGame.Physics.Ray(Position, Velocity);
            
            for(int i = 1; i <= Server.clients.Count; i++)
            {
                if (Server.clients[i].player == null) continue;
                if (Server.clients[i].player == this) continue;
                RayHit hit = Physics.RayIntersectsCollider(ray, Server.clients[i].player.serverCollider);
                if (hit.IsHit)
                {
                    Position = hit.Position;
                    if (hit.Normal.X != 0)
                    {
                        //Position.Y += Velocity.Y;
                        Velocity.X = 0;
                    }
                    else if (hit.Normal.Y != 0)
                    {
                        //Position.X += Velocity.X;
                        Velocity.Y = 0;
                    }
                }
            }
            Position += Velocity;
            if (prevPos != Position || prevRot != Rotation)
            {
                Server.Send(new PlayerPosServerPacket(Id, Position, Rotation, false));
            }
        }

        public void MoveRot(Vector2 move, float rot)
        {
            Rotation = rot;
            Position = move;
            Server.Send(new PlayerPosServerPacket(Id, Position, Rotation, true));
        }

        public PlayerInfo(byte _id, string _username)
        {
            Id = _id;
            Username = _username;
        }
    }
}
