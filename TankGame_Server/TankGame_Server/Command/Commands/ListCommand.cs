using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame_Server.Command.Commands
{
    internal class ListCommand : ICommand
    {
        public string Command { get; } = "help";

        public string Description { get; } = "Lists all commands";

        public bool Execute(string[] segments, out string response)
        {
            response = "Commands: \n";
            foreach(var n in Program.commandHandler.commandList)
            {
                response += $"{n.Value.Command} - {n.Value.Description}\n";
            }
            return true;
        }
    }
}
