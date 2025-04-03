using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private bool _hasAlreadyPlayed; //serializeField for now to test if the button works
    private string _previousSavedScene;
    private int _saveIndex = 1;
    private int _planetIndex = 1;
    private int _unlockedPlanets = 0;
    private int _unlockedPleiades = 0;

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

    public void PlayGame()
    {
        //En attendant d'avoir la cinématique
        SceneManager.LoadScene("BlockOutDeplacement");
        SceneManager.LoadScene("GameplayUI");
    }

    public void ContinueGame()
    {
        if (_previousSavedScene != null)
            SceneManager.LoadScene(_previousSavedScene);

        //En attendant d'avoir la sauvegarde
        SceneManager.LoadScene("BlockOutDeplacement");
        SceneManager.LoadScene("GameplayUI");
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
