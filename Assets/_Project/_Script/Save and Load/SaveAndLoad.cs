using UnityEngine;

public class SaveAndLoad : MonoBehaviour
{
    #region Save/Load
    public void SaveGame()
    {
        GameManager.Instance.GetSaveManager().SaveGame();
    }
    public void LoadGame()
    {
        GameManager.Instance.GetSaveManager().LoadGame();
    }
    #endregion
}