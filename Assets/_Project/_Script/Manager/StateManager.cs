using System;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    #region Fields
    [Flags] public enum PlayerState
    {
        Idle = 2,
        Menu = 4,
        Dialogue = 8,
        Phasing = 16,
        Loading = 32,
        Puzzle = 64
    }
    
    private PlayerState _playerState;
    private PlayerState _previousState;
    
    public event Action<PlayerState> OnStateChanged;

    #endregion

    #region Main Functions
    private void Start()
    {
        _playerState = PlayerState.Idle;
    }
    #endregion

    #region Sates
    public void ChangeState(PlayerState newState)
    {
        if (_playerState != newState)
        {
            _previousState = _playerState;
            _playerState = newState;
            
            OnStateChanged?.Invoke(_playerState);
        }
    }
    
    public void GoBackToPreviousState()
    {
        if (_previousState != 0)
        {
            _playerState = _previousState;
            OnStateChanged?.Invoke(_playerState);
        }
    }
    
    public PlayerState GetState()
    {
        return _playerState;
    }
    #endregion
}
