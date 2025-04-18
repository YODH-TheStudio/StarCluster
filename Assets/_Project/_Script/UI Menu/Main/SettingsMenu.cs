using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : Menu
{
    #region Fields
    private SoundSystem _soundSystem;
    private VibrationManager _vibrationManager;
    
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle vibrationToggle;

    private readonly List<string> _languages= new List<string> { "English", "French" };
    private string _currentLanguage;
    private int _currentLanguageIndex;
    
    [Range(0,1)]private float _masterVolume;
    [Range(0,1)]private float _musicVolume;
    [Range(0,1)]private float _sfxVolume;
    
    private bool _canVibrate;
    private int _vibration;
    #endregion

    #region Main Functions
    private void Start()
    {
        _soundSystem = GameManager.GetSoundSystem();
        _vibrationManager = GameManager.GetVibrationManager();

        LoadPlayerPrefs();
    }
    #endregion

    #region Player Prefs
    private void LoadPlayerPrefs()
    {
        _currentLanguage = PlayerPrefs.GetString("Language", "en");
        _currentLanguageIndex = PlayerPrefs.GetInt("Language", 0);
        
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");

        _vibration = PlayerPrefs.GetInt("CanVibrate", 1);
        vibrationToggle.isOn = _vibration == 1;
    }
    #endregion

    #region Language
    public void SwitchLanguage()
    {
        _currentLanguageIndex = (_currentLanguageIndex + 1) % _languages.Count();
        _currentLanguage = _languages[_currentLanguageIndex];
        PlayerPrefs.SetString("Language", _currentLanguage);
        PlayerPrefs.SetInt("LanguageIndex", _currentLanguageIndex);
        PlayerPrefs.Save();
    }
    #endregion

    #region Volume
    public void MasterVolume()
    {
        _masterVolume = masterSlider.value;
        _soundSystem.SetMasterVolume(_masterVolume);
    }

    public void MusicVolume()
    {
        _musicVolume =  musicSlider.value;
        _soundSystem.SetMusicVolume(_musicVolume);
        _soundSystem.SetAmbianceVolume(_musicVolume);
    }
    
    public void SfxVolume()
    {
        _sfxVolume = sfxSlider.value;
        _soundSystem.SetSfxVolume(_sfxVolume);
    }

    #endregion

    #region Vibration
    public void SetVibration()
    {
        _vibrationManager.SwitchVibrationMode();
        _canVibrate = _vibrationManager.GetVibrationMode();
    }
    #endregion
}
