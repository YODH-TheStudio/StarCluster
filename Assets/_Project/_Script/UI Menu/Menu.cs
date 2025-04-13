using System;
using System.Collections;
using System.Collections.Generic;
using Systems.SceneManagement;
using UnityEngine;

public class Menu : MonoBehaviour
{
    protected SceneLoader SceneLoader;
    protected GameManager GameManager;

    private void Awake()
    {
        SceneLoader = PersistentSingleton<SceneLoader>.Instance;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
