using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame_Server
{
    public enum DrawFlags
    {
        None,
        FromClient,
        ToClient,
        Error,
        Important
    }

    public static class Draw
    {
        public static void WriteLine(string _message, DrawFlags _flag = DrawFlags.None)
        {
            switch (_flag)
            {
                case DrawFlags.None:
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"[{DateTime.Now}] {_message}");
                    break;
                case DrawFlags.ToClient:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"[{DateTime.Now}] {_message}");
                    break;
                case DrawFlags.FromClient:
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine($"[{DateTime.Now}] {_message}");
                    break;
                case DrawFlags.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{DateTime.Now}] {_message}");
                    break;
                case DrawFlags.Important:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[{DateTime.Now}] {_message}");
                    break;
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void Write(string _message)
        {
           Console.Write($"[{DateTime.Now}] "+_message);
        }
        public static void Write(string _message, DrawFlags _flag = DrawFlags.None)
        {
            switch (_flag)
            {
                case DrawFlags.None:
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write($"[{DateTime.Now}] {_message}");
                    break;
                case DrawFlags.ToClient:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($"[{DateTime.Now}] {_message}");
                    break;
                case DrawFlags.FromClient:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"[{DateTime.Now}] {_message}");
                    break;
                case DrawFlags.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"[{DateTime.Now}] {_message}");
                    break;
                case DrawFlags.Important:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"[{DateTime.Now}] {_message}");
                    break;
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
