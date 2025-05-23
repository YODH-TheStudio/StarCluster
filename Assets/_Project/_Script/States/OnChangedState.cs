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
        
        if (allowedStates.HasFlag(GameManager.Instance.GetStateManager().GetState()))
        {
            onAllowedState?.Invoke();
        }
        else
        {
            onNotAllowedState?.Invoke();
        }
    }
    
    private void OnDisable()
    {
        StateManager stateManager = GameManager.Instance.GetStateManager();
        
        if (stateManager != null)
        {
            stateManager.OnStateChanged -= HandleStateChanged;
        }
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
