using System;
using System.Collections;
using System.Collections.Generic;
using Systems.SceneManagement;
using UnityEngine;

public class EndAnimation : MonoBehaviour
{
    #region Field

    private SceneLoader _sceneLoader;

    #endregion

    #region Main Functions
    
    private void Awake()
    {
        _sceneLoader = PersistentSingleton<SceneLoader>.Instance;
    }

    #endregion

    #region Change Scene

    public void GoToMenuCredit()
    {
        _sceneLoader.LoadSceneGroup(0);
    }

    #endregion
}
