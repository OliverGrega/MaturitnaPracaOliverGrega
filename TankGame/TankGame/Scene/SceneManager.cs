using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankGame.Particles;

namespace TankGame.Scene
{
    public static class SceneManager
    {
        public static IScene CurrentScene { get; private set; }
        public static Action OnSceneChanged;

        public static void LoadScene(IScene scene)
        {
            scene.Load(null);
            ParticleManager.ClearAll();
            if (CurrentScene != null) UnloadScene();
            CurrentScene = scene;
        }
        public static void LoadScene(IScene scene, string options)
        {
            scene.Load(options);
            ParticleManager.ClearAll();
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
