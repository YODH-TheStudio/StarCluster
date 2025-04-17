using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavesMenu : Menu
{
    private int _previousSavedGroupScene;
    private int _saveIndex = 0;
    [SerializeField] private GameObject _saveSlot1;
    [SerializeField] private GameObject _saveSlot2;
    [SerializeField] private GameObject _saveSlot3;

    void Awake()
    {
        // setup slot text
    }
    
    Dictionary<int, string> planetNames = new Dictionary<int, string>()
    {
        {1, "PLANET_JOY"},
        {2, "PLANET_EMPATHY"},
        {3, "PLANET_SADNESS"},
        {4, "PLANET_COMICAL"},
        {5, "PLANET_BOREDOM"},
        {6, "PLANET_MELANCHOLY"},
        {7, "PLANET_SEEDY"}
    };

    [SerializeField] private Dictionary<int, Texture2D> planetIcons = new Dictionary<int, Texture2D>();

    public void RefreshSlotsData()
    {
        // Read save info
        int[] slot1Info = GameManager.Instance.GetSaveManager().GetInfo(1);
        string planetName1 = planetNames[slot1Info[2]];
        _saveSlot1.GetComponent<SaveDataDisplayer>().Set(planetName1, DateTime(slot1Info[0]), planetIcons[slot1Info[2]]);
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
