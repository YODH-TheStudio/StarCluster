using System.Collections.Generic;
using UnityEngine;

public class SavesMenu : Menu
{

    #region Fields
    [SerializeField] private List<Texture2D> planetIcons;
    
    private int _previousSavedGroupScene;
    private int _saveIndex = 1;
    
    
    [SerializeField] private List<SaveDataDisplayer> slots;
    // [SerializeField] private GameObject _saveSlot1;
    // [SerializeField] private GameObject _saveSlot2;
    // [SerializeField] private GameObject _saveSlot3;
    #endregion

    #region Main Functions
    private new void Awake()
    {
        // setup slot text
        if (planetIcons.Count != _planetNames.Count)
        {
            Debug.LogError("Planet icons not set up correctly");
        }
        RefreshSlotsData();
    }
    #endregion

    #region Planets Names
    private readonly List<string> _planetNames = new List<string>()
    { 
        "PLANET_JOY",
        "PLANET_EMPATHY",
        "PLANET_SADNESS",
        "PLANET_COMICAL",
        "PLANET_BOREDOM",
        "PLANET_MELANCHOLY",
        "PLANET_SEEDY"
    };
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
                slots[i].Set(_planetNames[slotInfo.currentPlanet],
                    slotInfo.saveTime,
                    planetIcons[slotInfo.currentPlanet]);
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
