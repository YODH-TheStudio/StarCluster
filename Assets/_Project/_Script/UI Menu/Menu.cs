using Systems.SceneManagement;
using UnityEngine;

public class Menu : MonoBehaviour
{
    #region Fields
    protected SceneLoader SceneLoader;
    protected GameManager GameManager;
    private int _index;
    #endregion

    #region Main Functions
    protected virtual void Awake()
    {
        SceneLoader = PersistentSingleton<SceneLoader>.Instance;
        GameManager = PersistentSingleton<GameManager>.Instance;
    }
    #endregion

    #region Active/Deactivate Menu
    public void ActivateMenuState()
    {
        GameManager.GetStateManager().ChangeState(StateManager.PlayerState.Menu);
    } 
    
    public void DeactivateMenuState()
    {
        GameManager.GetStateManager().ChangeState(StateManager.PlayerState.Idle);
    }
    #endregion

    #region Load Scene
    public virtual async void LoadGroupScene(int index)
    {
        await SceneLoader.LoadSceneGroup(index);
    }
    #endregion

    #region Quit
    public void Quit()
    {
        Application.Quit();
    }
    #endregion
}
