using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PistaNetworkLibrary
{
    public partial class Server
    {
        public static Action<byte> Joined { get; set; }
        public static Action<byte> Joining { get; set; }
        public static Action<byte> Left { get; set; }
    }

    public class PlayerJoiningEventArgs
    {
        public byte PlayerId { get; set; }

        public PlayerJoiningEventArgs(byte playerId)
        {
            PlayerId = playerId;
        }
    }
    public class PlayerJoinedEventArgs
    {
        public byte PlayerId { get; set; }

        public PlayerJoinedEventArgs(byte playerId)
        {
            PlayerId = playerId;
        }
    }
    public class PlayerLeftEventArgs
    {
        public byte PlayerId { get; set; }

        public PlayerLeftEventArgs(byte playerId)
        {
            PlayerId = playerId;
        }
    }
}
