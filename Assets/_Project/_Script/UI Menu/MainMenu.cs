using System.Threading.Tasks;
using UnityEngine;
using Systems.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private bool _hasAlreadyPlayed = true; //serializeField for now to test if the button works
    //Game Manager to get all the infos
    SceneLoader _sceneLoader;
    private string _previousSavedScene;
    private int _saveIndex = 1;
    private int _planetIndex = 1;
    private int _unlockedPlanets = 0;
    private int _unlockedPleiades = 0;

    private void Awake()
    {
        _sceneLoader = PersistentSingleton<SceneLoader>.Instance;
    }
    public void nextPlanet()
    {
        Debug.Log(_planetIndex);
        if (_planetIndex == 8) return;

        _planetIndex++;
    }

    public void previousPlanet()
    {
        Debug.Log(_planetIndex);
        if (_planetIndex == 1) return;

        _planetIndex--;
    }

    public async void PlayGame()
    {
        if (_hasAlreadyPlayed)
        {
            await _sceneLoader.LoadSceneGroup(2);
        }
        else
        {
            await _sceneLoader.LoadSceneGroup(1);
        }
    }

    public void SetSelectedSaveIndex()
    {
        string name = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;
        Debug.Log(name);
        if (int.TryParse(name, out int index))
        {
            _saveIndex = index;
        }
        else
        {
            Debug.LogError("Button name is not valid: " + name);
        }
    }

    public void LoadSelectedSave()
    {
        //load the save at the _saveIndex
    }

    public void DeleteSelectedSave()
    {
        //delete the save at the _saveIndex
    }

    public void Quit()
    {
        Application.Quit();
    }
}
