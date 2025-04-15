using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Systems.SceneManagement;

public class SettingsMenu : Menu
{
    private SoundSystem _soundSystem;
    private VibrationManager _vibrationManager;

    private void Start()
    {
        _soundSystem = GameManager.GetSoundSystem();
        _vibrationManager = GameManager.GetVibrationManager();
    }

    public void SwitchLanguage()
    {
        
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

    public void SetVibration()
    {
        _vibrationManager.SwitchVibrationMode();
    }
    
    public async void GoToMainMenu()
    {
        await SceneLoader.LoadSceneGroup(0);
    }
}
