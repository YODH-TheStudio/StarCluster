using System.Collections;
using System.Collections.Generic;
using Systems.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePauseMenu : MonoBehaviour
{
    private SceneLoader _sceneLoader;

    private void Awake()
    {
        _sceneLoader = PersistentSingleton<SceneLoader>.Instance;
    }

    public async void GoToMainMenu()
    {
        await _sceneLoader.LoadSceneGroup(0);
    } 

    public void Quit()
    {
        Application.Quit();
    }
}
