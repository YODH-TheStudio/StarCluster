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
    } 
    
    public void DesactivateMenuState()
    {
        GameManager.GetStateManager().ChangeState(StateManager.PlayerState.Idle);
    }
    
    public virtual int GetIndex()
    {
        return _index;
    }
    public void Quit()
    {
        Application.Quit();
    }
}
