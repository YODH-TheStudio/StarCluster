using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandDominanceManager : MonoBehaviour
{
    #region Fields
    private bool _isLeftHandDominant;

    public event Action onUpdate;

    #endregion

    #region Main Functions
    private void Start()
    {
        LoadPlayerPrefs();
    }
    #endregion

    #region Vibration
    private void LoadPlayerPrefs()
    {
        _isLeftHandDominant = PlayerPrefs.GetInt("isLeftHandDominant", 0) == 1;
    }

    public void SwitchHandDominance(bool isLeftHanded)
    {
        PlayerPrefs.SetInt("isLeftHandDominant", isLeftHanded ? 1 : 0);
        PlayerPrefs.Save();
        _isLeftHandDominant = PlayerPrefs.GetInt("isLeftHandDominant", 0) == 1;
        onUpdate?.Invoke();
    }

    public bool GetHandDominance()
    {
        return _isLeftHandDominant;
    }
    #endregion
}
