using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankGame.Networking;

namespace TankGame
{
    public class NetworkedEntity : Entity
    {
        public ushort NetworkId { get; set; }
        public object[] syncedData = new object[]
        {
        };
        
        public NetworkedEntity(string texturePath, Vector2 pos, float stackOffset) : base(texturePath, pos, stackOffset)
        {

        }

        public void ChangeData(byte dataId,PacketBuilder packet)
        {            
            switch (Type.GetTypeCode(syncedData[dataId].GetType()))
            {
                case TypeCode.Byte:
                    syncedData[dataId] = packet.ReadByte();
                    break;
                    case TypeCode.Int16:
                    syncedData[dataId] = packet.ReadShort();
                    break;
                case TypeCode.Int32:
                    syncedData[dataId] = packet.ReadInt();
                    break;
                case TypeCode.String:
                    syncedData[dataId] = packet.ReadString();
                    break;
                case TypeCode.Boolean:
                    syncedData[dataId] = packet.ReadBool();
                    break;
                case TypeCode.Single:
                    syncedData[dataId] = packet.ReadFloat();
                    break;
            }
        }

        public void Spawn(ushort id, Vector2 pos)
        {
            NetworkId = id;
            Position = pos;
        }

        public void Despawn()
        {

        }
    }
}
