using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
    public class GameObject
    {
        public virtual void Update() { }
        public virtual void Draw() { }
        public void Destroy()
        {
            OnDestroyed();
        }

        public virtual bool Hover()
        {
            return false;
        }

        public virtual void OnDestroyed() { }
    }
}
