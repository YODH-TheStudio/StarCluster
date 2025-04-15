using System.Threading.Tasks;
using UnityEngine;
using Systems.SceneManagement;

public class MainMenu : Menu
{
    
    [SerializeField] bool hasAlreadyPlayed = true; //serializeField for now to test if the button works
    [SerializeField] GameObject planetMenu;
    
    public async void PlayGame()
    {
        if (hasAlreadyPlayed)
        {
            planetMenu.SetActive(true);
            this.gameObject.SetActive(false);
        }
        else
        {
            await SceneLoader.LoadSceneGroup(1);
        }
    }



}
