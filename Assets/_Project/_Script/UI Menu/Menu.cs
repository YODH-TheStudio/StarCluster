using System;
using System.Collections;
using System.Collections.Generic;
using Systems.SceneManagement;
using UnityEngine;

public class Menu : MonoBehaviour
{
    protected SceneLoader SceneLoader;
    protected GameManager GameManager;
    private int _index;

    private void Awake()
    {
        SceneLoader = PersistentSingleton<SceneLoader>.Instance;
        GameManager = PersistentSingleton<GameManager>.Instance;
    }

    public void ActivateMenuState()
    {
        GameManager.GetStateManager().ChangeState(StateManager.PlayerState.Menu);
        GameManager.Instance._soundSystem.PlaySoundFXClipByKey("UI Clicb", transform.position);
    } 
    
    public void DeactivateMenuState()
    {
        GameManager.GetStateManager().ChangeState(StateManager.PlayerState.Idle);
    }

    public virtual async void LoadGroupScene(int index)
    {
        await SceneLoader.LoadSceneGroup(index);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
