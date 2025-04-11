using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Systems.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    private SceneLoader _sceneLoader;

    private void Awake()
    {
        _sceneLoader = PersistentSingleton<SceneLoader>.Instance;
    }

    public void MasterVolume(float volume)
    {

    }

    public void MusicVolume(float volume)
    {

    }
    public void SFXVolume(float volume)
    {

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
