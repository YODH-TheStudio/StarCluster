using Systems.SceneManagement;
using UnityEngine;

public class Menu : MonoBehaviour
{
    #region Fields
    protected SceneLoader SceneLoader;
    protected GameManager GameManager;
    private SoundSystem _soundSystem;
    private int _index;
    #endregion

    #region Main Functions
    protected virtual void Awake()
    {
        SceneLoader = PersistentSingleton<SceneLoader>.Instance;
        GameManager = PersistentSingleton<GameManager>.Instance;

        _soundSystem = GameManager.Instance.GetSoundSystem();
    }
    #endregion

    #region Active/Deactivate Menu
    public void ActivateMenuState()
    {
        _soundSystem.PlaySoundFXClipByKey("Ui Clic B", transform.position);
        GameManager.GetStateManager().ChangeState(StateManager.PlayerState.Menu);
    } 
    
    public void DeactivateMenuState()
    {
        _soundSystem.PlaySoundFXClipByKey("Ui Clic C", transform.position);
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
        _soundSystem.PlaySoundFXClipByKey("Ui Clic A", transform.position);
        Application.Quit();
    }
    #endregion
}
