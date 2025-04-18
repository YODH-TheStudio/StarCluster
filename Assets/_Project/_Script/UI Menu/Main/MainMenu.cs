using System.Threading.Tasks;
using UnityEngine;
using Systems.SceneManagement;

public class MainMenu : Menu
{
    
    [SerializeField] bool hasAlreadyPlayed = true; // TODO : Use the bool from the loaded save
    [SerializeField] GameObject planetMenu;
    
    public async void PlayGame()
    {
        if (hasAlreadyPlayed)
        {
            //planetMenu.SetActive(true);
            //this.gameObject.SetActive(false);
            // if (test)
            // {
            //     await SceneLoader.LoadSceneGroup(2);
            //     GameManager.Instance.GetSaveManager().LoadGame(GameManager.Instance.GetSaveManager().currentSlot);
            // }
            // await SceneLoader.LoadSceneGroup(2);
            // GameManager.Instance.GetSaveManager().LoadGame(GameManager.Instance.GetSaveManager().currentSlot);
            planetMenu.SetActive(true);
            this.gameObject.SetActive(false);
        }
        else
        {
            await SceneLoader.LoadSceneGroup(1);
            GameManager.Instance.GetSaveManager().LoadGame(GameManager.Instance.GetSaveManager().currentSlot);
        }
    }
}
