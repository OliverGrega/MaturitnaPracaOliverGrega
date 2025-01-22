using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Scene
{
    public static class SceneManager
    {
        public static IScene CurrentScene { get; private set; }

        public static void LoadScene(IScene scene)
        {
            scene.Load();
            if (CurrentScene != null) UnloadScene();
            CurrentScene = scene;
        }

        public static void UnloadScene()
        {
            CurrentScene.Unload();
            CurrentScene = null;
        }
    }
}
