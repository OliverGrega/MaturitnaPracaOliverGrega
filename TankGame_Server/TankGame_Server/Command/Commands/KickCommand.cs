using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame_Server.Command.Commands
{
    internal class KickCommand : ICommand
    {
        public string Command => "kick";

        public string Description => "Kicks player from server";

        public bool Execute(string[] segments, out string response)
        {
            try
            {
                byte userId = Convert.ToByte(segments[1]);
                response = "Kicked player " + GameLogic.players[userId].username;
                TankServer.Active.clients[userId].tryKick = true;
                TankServer.Active.clients[userId].Disconnect("Kicked!");
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
