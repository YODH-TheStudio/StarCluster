using System.Collections.Generic;
using System.Linq;
using MeetAndTalk.Localization;
using UnityEngine;
using UnityEngine.UI;



public class SettingsMenu : Menu
{

    #region Enum
    public enum PreviousMenu
    {
        MainMenu,
        PlanetMenu
    }
    #endregion

    #region Fields
    private SoundSystem _settingSoundSystem;
    private VibrationManager _vibrationManager;
    private HandDominanceManager _handDominanceManager;

    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle vibrationToggle;
    [SerializeField] private Toggle handToggle;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject planetMenu;

    private readonly List<string> _languages= new List<string> { "French", "English" };
    private string _currentLanguage;
    private int _currentLanguageIndex;
    

    public PreviousMenu _previousMenu;

    [Range(0,1)]private float _masterVolume;
    [Range(0,1)]private float _musicVolume;
    [Range(0,1)]private float _sfxVolume;
    
    private int _vibration;
    #endregion

    #region Main Functions
    private void Start()
    {
        _settingSoundSystem = GameManager.GetSoundSystem();
        _vibrationManager = GameManager.GetVibrationManager();
        _handDominanceManager = GameManager.GetHandDominanceManager();

        LoadPlayerPrefs();

        vibrationToggle.isOn = _vibrationManager.GetVibrationMode();
        handToggle.isOn = _handDominanceManager.GetHandDominance();
    }
    #endregion

    #region Previous Menu function
    public void OpenFrom(PreviousMenu menu)
    {
        _previousMenu = menu;
        Debug.LogWarning($"Open Settings Menu from {_previousMenu}");
        gameObject.SetActive(true);
    }

    public void OpenFromMainMenu()
    {
        OpenFrom(PreviousMenu.MainMenu);
    }

    public void OpenFromPlanetMenu()
    {
        OpenFrom(PreviousMenu.PlanetMenu);
    }

    public void OnBackButton() 
    {
        gameObject.SetActive(false);
        switch (_previousMenu)
        {
            case PreviousMenu.MainMenu:
                mainMenu.SetActive(true);
                break;
            case PreviousMenu.PlanetMenu:
                planetMenu.SetActive(true);
                break;
        }
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
        _settingSoundSystem.SetMasterVolume(_masterVolume);
    }

    public void MusicVolume()
    {
        _musicVolume =  musicSlider.value;
        _settingSoundSystem.SetMusicVolume(_musicVolume);
        _settingSoundSystem.SetAmbianceVolume(_musicVolume);
    }
    
    public void SfxVolume()
    {
        _sfxVolume = sfxSlider.value;
        _settingSoundSystem.SetSfxVolume(_sfxVolume);
    }

    #endregion

    #region Vibration
    public void SetVibration(bool vibrate)
    {
        _vibrationManager.SwitchVibrationMode(vibrate);
    }
    #endregion

    #region Hand Dominance
    public void SetHandDominance(bool IsLeftHanded)
    {
        _handDominanceManager.SwitchHandDominance(IsLeftHanded);
        Debug.Log(_handDominanceManager.GetHandDominance());
    }
    #endregion
}
