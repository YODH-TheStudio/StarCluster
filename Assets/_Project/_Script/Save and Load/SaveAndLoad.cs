using UnityEngine;

public class SaveAndLoad : MonoBehaviour
{
    public void SaveGame()
    {
        GameManager.Instance.GetSaveManager().SaveGame();
    }
    public void LoadGame()
    {
        GameManager.Instance.GetSaveManager().LoadGame();
    }
}