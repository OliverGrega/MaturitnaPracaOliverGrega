using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TankGame;
using TankGame.Networking;
using TankGame.Physics;

namespace TankGame_Server
{
    class GameLogic
    {
        public static Dictionary<byte, S_Player> players = new Dictionary<byte, S_Player>();
        public static uint currentTick;
        public static Collider[] borders;

        public static void CreateMap()
        {
            borders = new Collider[4]
            {
                new Collider(new Vector2(0,((Settings.instance.MapHeight)/2)+16), new Vector2(Settings.instance.MapWidth/2,8)),
                new Collider(new Vector2(0,((-Settings.instance.MapHeight)/2)-16), new Vector2(Settings.instance.MapWidth/2,8)),
                new Collider(new Vector2(((Settings.instance.MapWidth/2)+16),0),  new Vector2(8,Settings.instance.MapHeight/2)),
                new Collider(new Vector2(((-Settings.instance.MapWidth/2)-16),0),  new Vector2(8,Settings.instance.MapHeight/2)),
            };

            nextRoundTick = currentTick + (uint)Settings.instance.RoundDurationInTicks;
        }

        private static uint nextRoundTick;

        public static void Update()
        {
            ThreadManager.UpdateMain();
            currentTick++;
            if(currentTick >= nextRoundTick)
            {
                EndRound();
            }

            foreach(var player in players.Values)
            {
                player.HandleServerTick(currentTick);
            }
        }

        public static void EndRound()
        {
            nextRoundTick = currentTick + (uint)Settings.instance.RoundDurationInTicks;
            if (players.Count != 0)
            {
                var plysByKills = players.Values.OrderBy(x => x.Kills).ToArray()[players.Values.Count - 1];
                ServerSend.RoundEnded(plysByKills.owner, plysByKills.Kills);
            }
            else
            {
                ServerSend.RoundEnded(0, 0);
            }

            foreach(var n in players.Values)
            {
                n.Respawn();
            }
        }

        public static void HandleInput(byte _playerId, InputPayload inputPayload)
        {
            if (!players.ContainsKey(_playerId)) return;
            players[_playerId].HandleInput(inputPayload);
        }

        public static void PlayerShoot(byte attacker, uint attackTick)
        {
            players[attacker].Shoot(attackTick,attacker);
        }

        public static void SpawnPlayer(byte ownerId, string username)
        {
            if (players.ContainsKey(ownerId)) return;
            players.Add(ownerId, new S_Player(ownerId, username, new System.Numerics.Vector2(100,100)));
        }



        public static void DespawnPlayer(byte ownerId)
        {
            if (!players.ContainsKey(ownerId)) return;
            players.Remove(ownerId);
            ServerSend.DespawnPlayer(ownerId);
        }

        public static S_Player Get(byte id)
        {
            return players.FirstOrDefault(x=>x.Key == id).Value;
        }
        public static S_Player Get(string username)
        {
            return players.FirstOrDefault(x => x.Value.username == username).Value;
        }
    }
}
