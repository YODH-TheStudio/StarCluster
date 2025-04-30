using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractionEvent : Interactable
{
    #region Field

    [SerializeField] private UnityEvent _onInteract;

    #endregion

    #region Interact

    public override void Interact()  
    {
        _onInteract?.Invoke();
    }
    
    #endregion
}
