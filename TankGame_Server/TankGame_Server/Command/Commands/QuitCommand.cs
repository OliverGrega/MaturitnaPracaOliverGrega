using PistaNetworkLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame_Server.Command.Commands
{
    public class QuitCommand : ICommand
    {
        public string Command { get; } = "quit";

        public string Description { get; } = "Shutsdown the server";

        public bool Execute(string[] segments, out string response)
        {
            Environment.Exit(0);
            response = "Bye bye! :)";
            return true;
        }
    }
}
