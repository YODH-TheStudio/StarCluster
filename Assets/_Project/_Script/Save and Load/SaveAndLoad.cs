using UnityEngine;


public class SaveAndLoad : MonoBehaviour
{
    [SerializeField] private bool loadOnSceneStart = false;
    private void Start()
    {
        if (loadOnSceneStart)
        {
            Debug.Log("Loading game on scene start");
            LoadGame();
        }
    }
    
    public void SaveGame()
    {
        GameManager.Instance.GetSaveManager().SaveGame();
    }
    public void LoadGame()
    {
        GameManager.Instance.GetSaveManager().LoadGame();
    }

}