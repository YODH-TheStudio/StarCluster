using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnChangedState : MonoBehaviour
{
    [SerializeField] private StateManager.PlayerState allowedStates;
    
    [SerializeField] private UnityEvent onAllowedState;
    [SerializeField] private UnityEvent onNotAllowedState;
    
    private void OnEnable()
    {
        GameManager.Instance.GetStateManager().OnStateChanged += HandleStateChanged;
    }
    
    private void OnDisable()
    {
        GameManager.Instance.GetStateManager().OnStateChanged -= HandleStateChanged;
    }
    
    private void HandleStateChanged(StateManager.PlayerState state)
    {
        if (allowedStates.HasFlag(state))
        {
            onAllowedState?.Invoke();
        }
        else
        {
            onNotAllowedState?.Invoke();
        }
    }
}
