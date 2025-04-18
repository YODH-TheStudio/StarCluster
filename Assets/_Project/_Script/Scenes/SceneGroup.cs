using System;
using System.Collections.Generic;
using System.Linq;

namespace Systems.SceneManagement
{
    [Serializable]
    public class SceneGroup
    {
        public string groupName = "New Scene Group";
        public List<SceneData> scenes;

        public string FindSceneNameByType(SceneType sceneType)
        {
            return scenes.FirstOrDefault(scene => scene.sceneType == sceneType)?.sceneName; 
        }
    }


    [Serializable]
    public class SceneData 
    {
        public string sceneName;
        public SceneType sceneType;
    }

    public enum SceneType { ActiveScene, MainMenu, UserInterface, HUD, Cinematic, Enironment, Tooling }

}