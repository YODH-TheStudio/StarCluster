using UnityEngine;

public class MainMenu : Menu
{
    [SerializeField] private bool hasAlreadyPlayed = true; // TODO : Use the bool from the loaded save
    [SerializeField] private GameObject planetMenu;
    
    public async void PlayGame()
    {
        //GameManager.Instance._soundSystem.PlaySoundFXClipByKey("UI Clicp", transform.position);
        
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
}
