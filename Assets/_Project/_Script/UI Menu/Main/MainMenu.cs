using Systems.SceneManagement;
using UnityEngine;

public class MainMenu : Menu
{
    #region Fields 
    [SerializeField] private bool hasAlreadyPlayed = true; // TODO : Use the bool from the loaded save
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
    #endregion

    #region PlayGame
    public async void PlayGame()
    {

        if (hasAlreadyPlayed)
        {
            planetMenu.SetActive(true);
            gameObject.SetActive(false);
        }
        else
        {
            _soundSystem.StopMusicSource();
            await SceneLoader.LoadSceneGroup(1);
        }
    }
    #endregion
}
