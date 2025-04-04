using MeetAndTalk;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine;
using UnityEngine.Events;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public delegate void Notify();

public class DialogueManagerCustom : MonoBehaviour
{

    private DialogueContainerSO _dialogueContainer;
    public UnityEvent endEvent;
    public UnityEvent startEvent;

    public event Notify ProcessEndDialogue;
    public event Notify ProcessSkipDialogue;

    public DialogueContainerSO test;

    private bool _isDialogue;
    
    [Header("Script References")]
    [SerializeField]
    private DialogueUIManager dialogueUIManager;
    [SerializeField]
    private DialogueManager dialogueManager;
    private MeetAndTalk.Localization.LocalizationManager localizationManager;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += Touch_OnFingerDown;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartDialogue(test);
    }

    public void OnStartDialogue()
    {
        // stateMachine.currentState = StateMachine.State.InDialogue;
        _isDialogue = true;
    }

    public void OnEndDialogue()
    {   
        // get dialogue name

        // dialogueManager.audioSource.Stop();
        // stateMachine.currentState = StateMachine.State.Walking;
        _isDialogue = false;
        StartProcessEnd();
    }
    
    public void StartProcessEnd()
    {
        OnProcessEndDialogue();
        // ProcessSkipDialogue -= StartProcessSkip;
    }

    public void StartProcessSkip()
    {
        OnProcessSkipDialogue();
    }

    protected virtual void OnProcessEndDialogue() //protected virtual method
    {
        //if ProcessCompleted is not null then call delegate
        ProcessEndDialogue?.Invoke();
    }

    protected virtual void OnProcessSkipDialogue() //protected virtual method
    {
        dialogueManager.SkipDialogue();
    }
    
    public void StartDialogue(DialogueContainerSO dialogueContainerParam)
    {
        //run the event when the dialogue start
        startEvent.Invoke();

        // //start the dialogue
        // GameManager.Instance.dialogueManager.StartDialogue(dialogueContainerParam);
        
        
        // if (GameManager.instance.stateMachine.currentState != StateMachine.State.Walking)
        // {
        //     return;
        // }
        //
        // Move move = GameObject.FindGameObjectWithTag("Player").GetComponent<Move>();
        // move.StopMovement();
        
        dialogueManager.StartDialogue(dialogueContainerParam);
        
        //subscribe to the event
        // ProcessSkipDialogue += StartProcessSkip;
        
        
        // //subscribe to the event
        ProcessEndDialogue += bl_ProcessEndDialogue;

        //subscribe to the event
        // ProcessSkipDialogue += bl_ProcessSkipDialogue;
    }

    // event handler
    public void bl_ProcessEndDialogue()
    {
        //run the event when the dialogue end
        endEvent.Invoke();
        //// GameManager.instance.ProcessSkipDialogue -= bl_ProcessSkipDialogue;
        // GameManager.Instance.ProcessEndDialogue -= bl_ProcessEndDialogue;

    }

    // event handler
    public void bl_ProcessSkipDialogue()
    {
        // GameManager.instance.dialogueManager.SkipDialogue();
    }
    
    private void Touch_OnFingerDown(Finger TouchedFinger)
    {
        if (_isDialogue && dialogueManager.isSkippeable)
        {
            //if dialogueUIManager.lastTypingTime is less than Time.time + 0.1s then skip the dialogue
            if (dialogueUIManager.lastTypingTime < Time.time - 0.1f)
            {
                StartProcessSkip();
            }
            else
            {
                dialogueUIManager.SetFullText(dialogueUIManager.fullText);
                dialogueUIManager.characterIndex = dialogueUIManager.fullText.Length;
            }
        }
    }
}