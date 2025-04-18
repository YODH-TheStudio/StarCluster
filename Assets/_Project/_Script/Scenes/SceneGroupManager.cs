using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Systems.SceneManagement
{
    public class SceneGroupManager
    {
        public event Action<string> OnSceneLoaded = delegate { };
        public event Action<string> OnSceneUnloaded = delegate { };
        public event Action OnSceneGroupLoaded = delegate { };

        private SceneGroup _activeSceneGroup;

        public async Task LoadScenes(SceneGroup group, IProgress<float> progress, bool reloadDupScenes = false)
        {
            _activeSceneGroup = group;
            List<string> loadedScenes = new List<string>();

            await UnloadScenes();

            int sceneCount = SceneManager.sceneCount;

            for (int i = 0; i < sceneCount; i++)
            {
                loadedScenes.Add(SceneManager.GetSceneAt(i).name);
            }

            int totalScenesToLoad = _activeSceneGroup.scenes.Count;

            AsyncOperationGroup operationGroup = new AsyncOperationGroup(totalScenesToLoad);

            for (int i = 0; i < totalScenesToLoad; i++)
            {
                SceneData sceneData = group.scenes[i];
                if (reloadDupScenes == false && loadedScenes.Contains(sceneData.sceneName)) continue;

                var operation = SceneManager.LoadSceneAsync(sceneData.sceneName, LoadSceneMode.Additive);
                await Task.Delay(TimeSpan.FromSeconds(2f)); // Add delay time in loading screen

                operationGroup.Operations.Add(operation);

                OnSceneLoaded.Invoke(sceneData.sceneName);
            }

            // Wait until all AsyncOperations in a group are done
            while (!operationGroup.IsDone)
            {
                progress?.Report(operationGroup.Progress);
                await Task.Delay(100);
            }

            Scene activeScene = SceneManager.GetSceneByName(_activeSceneGroup.FindSceneNameByType(SceneType.ActiveScene));

            if (activeScene.IsValid())
            {
                SceneManager.SetActiveScene(activeScene);
            }

            OnSceneGroupLoaded.Invoke();
        }
        private async Task UnloadScenes()
        {
            List<string> scenes = new List<string>();
            string activeScene = SceneManager.GetActiveScene().name;

            int sceneCount = SceneManager.sceneCount;

            for (int i = 0; i < sceneCount; i++)
            {
                Scene sceneAt = SceneManager.GetSceneAt(i);
                if (!sceneAt.isLoaded) continue;

                string sceneName = sceneAt.name;
                if (sceneName.Equals(activeScene) || sceneName == "Bootstrapper") continue;

                scenes.Add(sceneName);
            }

            // Create an AsyncOperationGroup
            AsyncOperationGroup operationGroup = new AsyncOperationGroup(scenes.Count);

            foreach (string scene in scenes)
            {
                var operation = SceneManager.UnloadSceneAsync(scene);
                if (operation == null) continue;

                operationGroup.Operations.Add(operation);

                OnSceneUnloaded.Invoke(scene);
            }

            // Wait until all AsyncOperations in a group are done
            while (!operationGroup.IsDone)
            {
                await Task.Delay(100);
            }
        }

        private readonly struct AsyncOperationGroup
        {
            public readonly List<AsyncOperation> Operations;

            public float Progress => Operations.Count == 0 ? 0 : Operations.Average(o => o.progress);
            public bool IsDone => Operations.All(o => o.isDone);

            public AsyncOperationGroup(int initialCapacity)
            {
                Operations = new List<AsyncOperation>(initialCapacity);
            }
        }
    }
}