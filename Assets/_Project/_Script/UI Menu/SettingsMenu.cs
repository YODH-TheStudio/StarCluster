using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SettingsMenu : Menu
{
    private SoundSystem _soundSystem;
    private VibrationManager _vibrationManager;

    private readonly List<string> _languages= new List<string> { "English", "French" };
    private string _currentLanguage;
    private int _currentLanguageIndex;
    
    [Range(0,1)]private float _masterVolume;
    [Range(0,1)]private float _musicVolume;
    [Range(0,1)]private float _sfxVolume;
    
    private bool _canVibrate;
    private int _vibration;

    private void Start()
    {
        _soundSystem = GameManager.GetSoundSystem();
        _vibrationManager = GameManager.GetVibrationManager();
        
        LoadPlayerPrefs();
    }

    private void LoadPlayerPrefs()
    {
        _currentLanguage = PlayerPrefs.GetString("Language", "en");
        _currentLanguageIndex = PlayerPrefs.GetInt("Language", 0);
        _masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        _musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        _sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        _vibration = PlayerPrefs.GetInt("CanVibrate", 1);
        _canVibrate = _vibration == 1;
    }

    public void SwitchLanguage()
    {
        _currentLanguageIndex = (_currentLanguageIndex++) % _languages.Count();
        _currentLanguage = _languages[_currentLanguageIndex];
        PlayerPrefs.SetString("Language", _currentLanguage);
        PlayerPrefs.SetInt("LanguageIndex", _currentLanguageIndex);
    }
    
    public void MasterVolume(float volume)
    {
        _masterVolume = volume;
        PlayerPrefs.SetFloat("MasterVolume", _masterVolume);
    }

    public void MusicVolume(float volume)
    {
        _musicVolume = volume;
        PlayerPrefs.SetFloat("MusicVolume", _musicVolume);
    }
    
    public void SfxVolume(float volume)
    {
        _sfxVolume = volume;
        PlayerPrefs.SetFloat("SFXVolume", _sfxVolume);
    }

    public void SetVibration()
    {
        _vibrationManager.SwitchVibrationMode();
        _canVibrate = _vibrationManager.GetVibrationMode();
        
        _vibration = _canVibrate ? 1 : 0;
        PlayerPrefs.SetInt("CanVibrate", _vibration);
    }
    
    public async void GoToMainMenu()
    {
        await SceneLoader.LoadSceneGroup(0);
    }
}
