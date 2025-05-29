using System.Diagnostics;
using System.Numerics;
using TankGame.Networking;
using TankGame_Server;
using TankGame_Server.Command;

namespace TankGame_Server
{
    internal class Program
    {
        private static bool isRunning = false;
        public static CommandHandler commandHandler;

        private static TankServer server;

        static void Main(string[] args)
        {
            Console.Title = "Tank Game Server";
            MyDebugger.OnWrite = Draw.Write;
            new Settings();
            Settings.Init();
            GameLogic.CreateMap();
            commandHandler = new CommandHandler();
            isRunning = true;

            MyDebugger.Write("Please enter desired port: ");
            int desiredPort = Convert.ToInt32(Console.ReadLine());

            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();

            server = new TankServer(Settings.instance.MaxPlayers, desiredPort);
            server.SimulateLatency = true;

            while (isRunning)
            {
                string command = Console.ReadLine();
                commandHandler.TryParseCommand(command);
            }
        }

        private static void MainThread()
        {
            Draw.WriteLine($"Main thread started! Running at {Settings.TICK_RATE} ticks per second.");
            DateTime _nextLoop = DateTime.Now;
            while (isRunning)
            {
                while (_nextLoop < DateTime.Now)
                {
                    GameLogic.Update();

                    _nextLoop = _nextLoop.AddMilliseconds(Settings.MS_PER_TICK);

                    if (_nextLoop > DateTime.Now)
                    {
                        Thread.Sleep(_nextLoop - DateTime.Now);
                    }
                }
            }
        }
    }
}
