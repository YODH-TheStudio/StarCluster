using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavesMenu : Menu
{
    private int _previousSavedGroupScene;
    private int _saveIndex = 0;
    
    [SerializeField] private List<SaveDataDisplayer> slots;
    // [SerializeField] private GameObject _saveSlot1;
    // [SerializeField] private GameObject _saveSlot2;
    // [SerializeField] private GameObject _saveSlot3;

    void Start()
    {
        // setup slot text
        if (_planetIcons.Count != 7)
        {
            Debug.LogError("Planet icons not set up correctly");
        }
        RefreshSlotsData();
    }
    

    [SerializeField] private List<Texture2D> _planetIcons;

    public void RefreshSlotsData()
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
                    _planetIcons[slotInfo.currentPlanet]);
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
}
