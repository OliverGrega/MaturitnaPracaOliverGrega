using GameServer.Command;
using System.Diagnostics;
using System.Numerics;
using TankGame.Networking.Packets.ServerPackets;
using PistaNetworkLibrary;

namespace GameServer
{
    internal class Program
    {
        private static bool isRunning = false;
        public static CommandHandler commandHandler;
        static void Main(string[] args)
        {
            Console.Title = "Tank Game Server";
            MyDebugger.OnWrite = Draw.WriteLine;
            Settings.Init();
            commandHandler = new CommandHandler();
            isRunning = true;

            Server.Joining += OnJoining;

            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();

            Server.Start(Settings.instance.MaxPlayers, Settings.instance.Port);
            while (isRunning)
            {
                string command = Console.ReadLine();
                commandHandler.TryParseCommand(command);
            }
        }        

        static void OnJoining(byte id)
        {
            Server.Send(new WelcomeServerPacket(id));
        }

        private static void MainThread()
        {
            Draw.WriteLine($"Main thread started! Running at {Settings.instance.ServerTickRate} ticks per second.");
            DateTime _nextLoop = DateTime.Now;
            while (isRunning)
            {
                while (_nextLoop < DateTime.Now)
                {

                    GameLogic.Update();

                    _nextLoop = _nextLoop.AddMilliseconds(Settings.instance.MsPerTick);

                    if (_nextLoop > DateTime.Now)
                    {
                        Thread.Sleep(_nextLoop - DateTime.Now);
                    }
                }
            }
        }
    }
}
