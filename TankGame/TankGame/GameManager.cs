using Microsoft.Xna.Framework;
using PistaNetworkLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankGame.Networking.Packets.ServerPackets;
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
        }

        public void Update()
        {
            foreach (var player in players.Values)
            {
                player.Update();
            }
        }

        public void FixedUpdate()
        {
            foreach (var player in players.Values)
            {
                player.FixedUpdate();
            }
        }

        public void RemovePlayer(byte id)
        {
            Destroy(players[id]);
            players.Remove(id);
        }
        public void RemoveAllPlayers()
        {
            foreach (Tank player in players.Values)
            {
                Destroy(player);
            }
            players.Clear();
        }
        public void SpawnPlayer(byte _id, string _username, Vector2 _pos)
        {
            Tank newPlayer = new Tank(_id, _username, _pos);
            players.Add(_id, newPlayer);
            //Spawn(newPlayer);
        }
    }
}
