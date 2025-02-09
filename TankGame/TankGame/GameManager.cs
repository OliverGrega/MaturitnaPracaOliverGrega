using Microsoft.Xna.Framework;
using PistaNetworkLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public void UpdatePlayerState(byte _id, StatePayload statePayload)
        {            
            players[_id].SyncTransform(statePayload);
        }
    }
}
