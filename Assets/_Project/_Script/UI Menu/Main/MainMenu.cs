using Systems.SceneManagement;
using UnityEngine;

public class MainMenu : Menu
{
    #region Fields 
    [SerializeField] private bool hasAlreadyPlayed = true; // TODO : Use the bool from the loaded save
    [SerializeField] private GameObject planetMenu;
    private SoundSystem _soundSystem;
    #endregion

    #region Main Function

    protected void Start()
    {
        _soundSystem = GameManager.Instance.GetSoundSystem();
    }


    #endregion

    #region PlayGame
    public async void PlayGame()
    {
        //GameManager.Instance._soundSystem.PlaySoundFXClipByKey("UI Clicp", transform.position);
        
        _soundSystem.PlaySoundFXClipByKey("Ui Clic A", transform.position);

        if (hasAlreadyPlayed)
        {
            planetMenu.SetActive(true);
            gameObject.SetActive(false);
        }
        else
        {
            await SceneLoader.LoadSceneGroup(1);
            GameManager.Instance.GetSaveManager().LoadGame(GameManager.Instance.GetSaveManager().currentSlot);
        }
    }
    #endregion
}
