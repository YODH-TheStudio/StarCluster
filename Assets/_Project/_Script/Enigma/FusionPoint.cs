using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FusionPoint : Interactable
{
    [SerializeField]
    private bool _isFinished;

    [SerializeField]
    private UnityEvent _onInteractIfPuzzleNotFinish;

    [SerializeField]
    private UnityEvent _onInteractIfPuzzleFinish;

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

    public bool GetState()
    {
        return _isFinished;
    }

    public void SetState(bool finish)
    {
        _isFinished = finish;
    }
}
