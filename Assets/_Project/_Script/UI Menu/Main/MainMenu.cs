using Systems.SceneManagement;
using UnityEngine;

public class MainMenu : Menu
{
    #region Fields 
    private bool _hasAlreadyPlayed = true;
    [SerializeField] private GameObject planetMenu;
    private GameManager _gameManager;
    private SoundSystem _soundSystem;

    #endregion

    #region Main Functions
    private new void Awake()
    {
        base.Awake();

        _gameManager = GameManager.Instance;
        _soundSystem = _gameManager.GetSoundSystem();
    }
    
    private new void Start()
    {
        LoadPlayerPrefs();
    }
    #endregion

    #region Player Prefs
    private void LoadPlayerPrefs()
    {
        _hasAlreadyPlayed = PlayerPrefs.GetInt("HasAlreadyPlayed", 0) == 1;
    }
    #endregion
    
    #region PlayGame
    public async void PlayGame()
    {

        if (_hasAlreadyPlayed)
        {
            planetMenu.SetActive(true);
            gameObject.SetActive(false);
        }
        else
        {
            PlayerPrefs.SetInt("HasAlreadyPlayed", 1);
            PlayerPrefs.Save();
            _soundSystem.StopMusicSource();
            await SceneLoader.LoadSceneGroup(1);
        }
    }
    #endregion
}
