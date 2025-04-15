using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavesMenu : Menu
{
    private int _previousSavedGroupScene;
    private int _saveIndex = 1;



    public void SetSaveIndex(int newSaveIndex)
    {
        _saveIndex = newSaveIndex;
    }
    
    public void LoadSelectedSave()
    {
        //load the save at the _saveIndex
        GameManager.GetSaveManager().LoadGame(_saveIndex);
    }

    public void DeleteSelectedSave()
    {
        //delete the save at the _saveIndex
        GameManager.GetSaveManager().DeleteSave(_saveIndex);
    }
}
