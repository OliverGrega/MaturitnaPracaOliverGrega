using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame_Server.Command.Commands
{
    internal class KillCommand : ICommand
    {
        public string Command => "kill";

        public string Description => "Kills the player";

        public bool Execute(string[] segments, out string response)
        {
            try
            {
                byte userId = Convert.ToByte(segments[1]);
                response = "Killed player " + GameLogic.players[userId].username;
                GameLogic.players[userId].Die(userId);
                return true;
            }
            catch(Exception e)
            {
                response = "Error occured while executing command: " + e.Message;
            }
            response = "Failed";
            return false;
        }
    }
}
