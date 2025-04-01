using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]private bool _hasAlreadyPlayed; //serializeField for now to test if the button works
    private string _previousSavedScene;
    private int _saveIndex;

    public void PlayGame()
    {
        //En attendant d'avoir la cinématique
        SceneManager.LoadScene("BlockOutDeplacement");
    }

    public void ContinueGame()
    {
        if (_previousSavedScene != null)
            SceneManager.LoadScene(_previousSavedScene);

        //En attendant d'avoir la sauvegarde
        SceneManager.LoadScene("BlockOutDeplacement");
    }

    public void SetSelectedSaveIndex()
    {
        string name = this.name;
        Debug.Log(name);
        _saveIndex = int.Parse(name);
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
