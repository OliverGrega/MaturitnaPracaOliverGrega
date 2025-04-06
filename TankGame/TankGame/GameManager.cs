using Microsoft.Xna.Framework;
using PistaNetworkLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankGame.Particles;
using TankGame.Physics;
using TankGame.PistaNetworkingLibrary.Packets.ServerPackets;
using static TankGame.Global;

namespace TankGame
{
    public class GameManager
    {
        public static PassObject Spawn;
        public static PassObject Destroy;

        public static Tank localPlayer;
        public static Dictionary<byte, Tank> players = new Dictionary<byte, Tank>();

        public static GameManager instance;

        public static bool duringRound = false;

        public static byte topFrag;
        public static int topFragKills;

        public GameManager()
        {
            instance = this;
        }

        public void Draw()
        {
            foreach (var player in players.Values)
            {
                player.Draw();
            }

            if (((TankClient)(TankClient.Active)).tick < TankClient.hideMvpTick)
            {
                Global.SpriteBatch.DrawString(Global.basicFont, $"MVP {players[topFrag].name} [{topFragKills}]", new Vector2(localPlayer.Position.X, localPlayer.Position.Y - 300), Color.White, 0, Global.basicFont.MeasureString($"MVP {players[topFrag].name} [{topFragKills}]") / 2, 1, 0, 0.6f);                
            }
        }

        public void Update()
        {
            foreach (var player in players.Values)
            {
                player.Update();
            }
        }

        public void RemovePlayer(byte id)
        {
            players[id].Destroy();
            players.Remove(id);
        }

        public void RemoveAllPlayers()
        {
            foreach (Tank player in players.Values)
            {
                player.Destroy();
            }
            players.Clear();
        }

        public void SpawnPlayer(byte _id, string _username, Vector2 _pos)
        {
            Tank newPlayer = new Tank(_id, _username, _pos);
            players.Add(_id, newPlayer);
        }

        public void UpdatePlayerState(byte _id, Vector2 _pos, float _rot)
        {
            players[_id].SetPosRot(_pos, _rot);
        }

        public void PlayerShoot(byte _id, Vector2 from, Vector2 to)
        {
            players[_id].Shoot(from, to);
            Spawn?.Invoke(new BulletTrail(from, to));
        }

        public void ChangePlayerHealth(byte _id, byte newHp)
        {
            players[_id].DealDamage(newHp);
            Global.PlaySound("Content/Sounds/hitHurt.mp3");


            StaticEmitter jano = new StaticEmitter(players[_id].Position, players[_id].Rotation);

            ParticleEmitterData pred = new ParticleEmitterData
            {
                particleData = new ParticleData
                {
                    colorEnd = Color.Orange,
                    colorStart = Color.Orange,
                    opacityStart = 1,
                    opacityEnd = 0,
                    rotationEnd = 0,
                    rotationStart = 0,
                    sizeStart = 2,
                    sizeEnd = 2,
                    sourceRect = new Rectangle(16, 0, 4, 4),
                    rotateToVelocity = true,
                    depth = 0.6f
                },
                oneShot = true,
                emitCount = 16,
                lifeSpanMin = 0.4f,
                lifeSpanMax = 0.6f,
                speedMin = 60,
                speedMax = 80,
                angleVariance = 2*MathF.PI,
                time = 0.3f,
                interval = 0.3f,

            };

            ParticleManager.AddParticleEmitter(new ParticleEmitter(jano, pred));
        }
        public void KillPlayer(byte _target, byte _attacker)
        {
            players[_target].Die();
            players[_attacker].kills++;
        }
        public void PlayerReloaded(byte _player)
        {
            players[_player].Reload();
        }

        public void UpdatePlayerState(byte _id, StatePayload statePayload)
        {            
            players[_id].SyncTransform(statePayload);
        }

        public void RespawnPlayer(byte _id, Vector2 _pos)
        {
            players[_id].Respawn(_pos);
        }

        public static Collider[] borders;
        public static void UpdateBounds()
        {
            borders = new Collider[4]
            {
                new Collider(new Vector2(0,((Settings.instance.MapHeight)/2)+16).ToNumerics(), new Vector2(Settings.instance.MapWidth/2,8).ToNumerics()),
                new Collider(new Vector2(0,((-Settings.instance.MapHeight)/2)-16).ToNumerics(), new Vector2(Settings.instance.MapWidth/2,8).ToNumerics()),
                new Collider(new Vector2(((Settings.instance.MapWidth/2)+16),0).ToNumerics(),  new Vector2(8,Settings.instance.MapHeight/2).ToNumerics()),
                new Collider(new Vector2(((-Settings.instance.MapWidth/2)-16),0).ToNumerics(),  new Vector2(8,Settings.instance.MapHeight/2).ToNumerics()),
            };
        }
    }
}
