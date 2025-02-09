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
        public static void Update()
        {
            ThreadManager.UpdateMain();
            currentTick++;
            foreach(var player in players.Values)
            {
                player.HandleServerTick(currentTick);
            }
        }


        public static void HandleInput(byte _playerId, InputPayload inputPayload)
        {
            if (!players.ContainsKey(_playerId)) return;
            players[_playerId].HandleInput(inputPayload);
        }

        public static void PlayerShoot(byte attacker, uint attackTick)
        {
            players[attacker].Shoot(attackTick);
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
