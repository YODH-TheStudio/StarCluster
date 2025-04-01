using MeetAndTalk;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public delegate void Notify();

public class DialogueManagerCustom : MonoBehaviour
{

    private DialogueContainerSO _dialogueContainer;
    public UnityEvent endEvent;
    public UnityEvent startEvent;

    public event Notify ProcessEndDialogue;
    public event Notify ProcessSkipDialogue;

    public DialogueContainerSO test;
    
    [Header("Script References")]
    public DialogueUIManager dialogueUIManager;
    public DialogueManager dialogueManager;
    public MeetAndTalk.Localization.LocalizationManager localizationManager;
    
    // Start is called before the first frame update
    void Start()
    {
        StartDialogue(test);
    }

    // Update is called once per frame


    public void OnStartDialogue()
    {
        // stateMachine.currentState = StateMachine.State.InDialogue;
        // isDialogue = true;
    }

    public void OnEndDialogue()
    {   
        // get dialogue name

        // dialogueManager.audioSource.Stop();
        // stateMachine.currentState = StateMachine.State.Walking;
        // isDialogue = false;
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
}