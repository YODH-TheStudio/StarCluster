using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavesMenu : Menu
{
    private int _previousSavedGroupScene;
    private int _saveIndex;

    public enum SaveState {Full, Empty};

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

    public override int GetIndex()
    {
        return _saveIndex;
    }

}
