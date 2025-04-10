using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    [Flags] public enum PlayerState
    {
        Idle = 2,
        Menu = 4,
        Dialogue = 8,
        Phasing = 16,
        PushPull = 32,
    }
    
    private PlayerState _playerState;
    private PlayerState _previousState;
    
    // Événement déclenché lorsqu'un état change
    public event Action<PlayerState> OnStateChanged;
    
    // Start is called before the first frame update
    void Start()
    {
        _playerState = PlayerState.Idle;
    }

    // Méthode pour changer l'état
    public void ChangeState(PlayerState newState)
    {
        if (_playerState != newState)
        {
            _previousState = _playerState; // Enregistre l'état précédent
            _playerState = newState;
            
            OnStateChanged?.Invoke(_playerState); // Déclenche l'événement
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
}
