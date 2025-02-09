using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
    public class NetworkedEntity : Entity
    {
        Dictionary<string,object> syncedData = new Dictionary<string,object>();
        
        public NetworkedEntity(string texturePath, Vector2 pos, float stackOffset) : base(texturePath, pos, stackOffset)
        {
        }

        public void SetValue<T>(string key, object value) where T : class
        {
            if (syncedData.ContainsKey(key)) syncedData[key] = value;
            else syncedData.Add(key, value);
        }

        public bool TryGetValue<T>(string key, out object value) where T : class
        {
            if(syncedData.ContainsKey(key))
            {
                value = syncedData[key];
                return true;
            }
            value = null;
            return false;
        }
    }

}
