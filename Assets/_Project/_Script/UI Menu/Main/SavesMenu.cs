using System;
using System.Collections.Generic;
using UnityEngine;

public class SavesMenu : Menu
{

    #region Fields
    [SerializeField] private List<Texture2D> planetIcons;
    
    private int _previousSavedGroupScene;
    private int _saveIndex = 1;
    
    
    [SerializeField] private List<SaveDataDisplayer> slots;
    #endregion

    #region Main Functions

    private void Start()
    {
        // setup slot text
        if (planetIcons.Count != 7)
        {
            Debug.LogError("Planet icons not set up correctly");
        }
        RefreshSlotsData();
    }

    #endregion

    #region Save Data
    private void RefreshSlotsData()
    {
        // Read save info
        for (int i = 0; i < slots.Count; i++)
        {
            SaveManager.SaveData slotInfo = GameManager.Instance.GetSaveManager().GetSaveData(i+1);
            if(slotInfo == null)
            {
                slots[i].Set("EMPTY",
                    "",
                    null);
            }
            else
            {
                slots[i].Set("PLANET_" + slotInfo.currentPlanet,
                    slotInfo.saveTime,
                    planetIcons[slotInfo.currentPlanet-1]);
            }
        }
    }
    
    public void SetSaveIndex(int newSaveIndex)
    {
        _saveIndex = newSaveIndex;
        GameManager.Instance.GetSaveManager().currentSlot = newSaveIndex;
    }
    
    public void LoadSelectedSave()
    {
        GameManager.Instance.GetSaveManager().LoadGame(_saveIndex);
    }

    public void DeleteSelectedSave()
    {
        GameManager.Instance.GetSaveManager().DeleteSave(_saveIndex);
        RefreshSlotsData();
    }
    #endregion
}
