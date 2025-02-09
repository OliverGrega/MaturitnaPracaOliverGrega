using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Networking
{
    public static class MyDebugger
    {
        public delegate void DebugWrite(string message);
        public static DebugWrite OnWrite;
        public static void WriteLine(string _msg)
        {
            Debug.WriteLine(_msg);
            OnWrite?.Invoke(_msg+"\n");
        }
        public static void Write(string _msg)
        {
            Debug.Write(_msg);
            OnWrite?.Invoke(_msg);
        }
    }
}
