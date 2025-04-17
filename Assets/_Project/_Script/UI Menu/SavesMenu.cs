using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavesMenu : Menu
{
    private int _previousSavedGroupScene;
    private int _saveIndex = 0;

    void Awake()
    {
        // setup slot text
    }

    public void UpdateSlotsData()
    {
        
    }
    
    public void SetSaveIndex(int newSaveIndex)
    {
        _saveIndex = newSaveIndex;
        GameManager.GetSaveManager().currentSlot = newSaveIndex;
    }
    
    public void LoadSelectedSave()
    {
        GameManager.GetSaveManager().LoadGame(_saveIndex);
    }

    public void DeleteSelectedSave()
    {
        GameManager.GetSaveManager().DeleteSave(_saveIndex);
    }
}
