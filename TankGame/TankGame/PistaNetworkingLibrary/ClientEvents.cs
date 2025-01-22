using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PistaNetworkLibrary
{
    public partial class MyClient
    {
        public static Action OnJoined { get; set; }
        public static Action<string> OnLeft { get; set; }
        public static Action OnFailedToConnect { get; set; }
    }
}
