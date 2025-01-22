using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Command
{
    public class CommandHandler
    {

        public Dictionary<string, ICommand> commandList;


        public CommandHandler()
        {
            commandList = new Dictionary<string, ICommand>();

            Type[] types = Assembly.GetCallingAssembly().GetTypes();
            foreach (Type type in types)
            {
                if (type.GetInterface("ICommand") == typeof(ICommand))
                {
                    RegisterCommand((ICommand)Activator.CreateInstance(type));
                }
            }

        }

        private void RegisterCommand(ICommand _command)
        {
            commandList.Add(_command.Command, _command);
        }

        public void TryParseCommand(string _command)
        {
            string[] properties = _command.Split(' ');

            commandList.TryGetValue(properties[0], out ICommand command);
            if (command == null) return;
            command.Execute(properties, out string response);
            Draw.WriteLine(response);
        }

        
    }
}
