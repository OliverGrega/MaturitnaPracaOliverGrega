﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PistaNetworkLibrary;

namespace GameServer.Command.Commands
{
    public class GetPlayersCommand : ICommand
    {
        public string Command { get; } = "list";

        public string Description { get; } = "Returns all players";

        public bool Execute(string[] segments, out string response)
        {
            string finishedResponse = "";
            if (Server.ConnectedPlayers == 0)
            {
                response = "No players connected!";
                return false;
            }
            finishedResponse += $"Players [{Server.ConnectedPlayers}/{Settings.instance.MaxPlayers}]";
            foreach (var n in Server.clients)
            {
                if (n.Value.tcp.socket == null) continue;
                finishedResponse += $"\n[{n.Key}] {n.Value.player.Username}";
            }
            response = finishedResponse;
            return true;
        }
    }
}
