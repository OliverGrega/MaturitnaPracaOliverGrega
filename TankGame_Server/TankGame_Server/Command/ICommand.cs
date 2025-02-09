using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame_Server.Command
{
    public interface ICommand
    {
        string Command { get; }
        string Description { get; }

        bool Execute(string[] segments, out string response);
    }
}
