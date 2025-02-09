using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame_Server.Command.Commands
{
    internal class GetPlayerPositionHistory : ICommand
    {
        public string Command { get; } = "poshistory";

        public string Description { get; } = "Lists all commands";

        public bool Execute(string[] segments, out string response)
        {
            byte plyId = Convert.ToByte(segments[1]);
            response = "History: \n";
            
            return true;
        }
    }
}
