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
            _playerState = newState;
            OnStateChanged?.Invoke(_playerState); // Déclenche l'événement
        }
    }
    
    public PlayerState GetState()
    {
        return _playerState;
    }
}
