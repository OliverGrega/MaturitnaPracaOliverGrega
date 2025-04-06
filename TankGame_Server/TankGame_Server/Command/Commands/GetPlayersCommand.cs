using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankGame_Server;

namespace TankGame_Server.Command.Commands
{
    public class GetPlayersCommand : ICommand
    {
        public string Command { get; } = "list";

        public string Description { get; } = "Returns all players";

        public bool Execute(string[] segments, out string response)
        {
            string finishedResponse = "";
            if (TankServer.Active.ConnectedPlayers == 0)
            {
                response = "No players connected!";
                return false;
            }
            finishedResponse += $"Players [{TankServer.Active.ConnectedPlayers}/{Settings.instance.MaxPlayers}]";
            foreach (var n in GameLogic.players)
            {
                finishedResponse += $"\n[{n.Key}] {n.Value.username}";
            }
            response = finishedResponse;
            return true;
        }
    }
}
