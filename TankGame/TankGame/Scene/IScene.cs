﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Scene
{
    public interface IScene
    {
        public List<GameObject> objects { get; }
        public void Load();
        public void Unload();
        public void Update();
        public void Draw();
    }
}
