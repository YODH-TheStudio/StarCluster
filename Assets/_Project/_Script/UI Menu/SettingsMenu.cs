using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Systems.SceneManagement;

public class SettingsMenu : Menu
{
    [SerializeField] private VibrationManager vibrationManager;
    [SerializeField] private SoundManager soundManager;
    
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
        await SceneLoader.LoadSceneGroup(0);
    }
}
