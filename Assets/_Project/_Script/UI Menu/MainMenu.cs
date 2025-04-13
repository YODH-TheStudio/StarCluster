using System.Threading.Tasks;
using UnityEngine;
using Systems.SceneManagement;

public class MainMenu : Menu
{
    
    private bool _hasAlreadyPlayed = true; //serializeField for now to test if the button works
    
    public async void PlayGame()
    {
        if (_hasAlreadyPlayed)
        {
            await SceneLoader.LoadSceneGroup(2);
        }
        else
        {
            await SceneLoader.LoadSceneGroup(1);
        }
    }



}
