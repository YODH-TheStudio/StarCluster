using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChangeState : MonoBehaviour
{

    [SerializeField] private bool SetStateOnStart = true;
    public StateManager.PlayerState StateToSet;
    void Start()
    {
        if(SetStateOnStart)
            GameManager.Instance.GetStateManager().ChangeState(StateToSet);
    }
    
    public void SetState(/*StateManager.PlayerState state*/)
    {
        GameManager.Instance.GetStateManager().ChangeState(StateToSet);
    }
}
