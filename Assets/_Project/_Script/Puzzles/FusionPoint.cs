using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class FusionPoint : Interactable
{
    #region Fields

    [SerializeField]
    private bool _isFinished;

    [SerializeField]
    private UnityEvent _onInteractIfPuzzleNotFinish;

    [SerializeField]
    private UnityEvent _onInteractIfPuzzleFinish;

    #endregion

    #region Interactable Functions
    override public void Interact()
    {
        if (_isFinished)
        {
            _onInteractIfPuzzleFinish.Invoke();
        }
        else
        {
            _onInteractIfPuzzleNotFinish.Invoke();
        }
    }
    #endregion

    #region Getteur
    public bool GetState()
    {
        return _isFinished;
    }
    #endregion

    #region Setteur
    public void SetState(bool finish)
    {
        _isFinished = finish;
    }
    #endregion
}
