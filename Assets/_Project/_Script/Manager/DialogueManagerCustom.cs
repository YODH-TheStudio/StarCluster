using MeetAndTalk;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine;
using UnityEngine.Events;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public delegate void Notify();

public class DialogueManagerCustom : MonoBehaviour
{
    #region Fields
    private DialogueContainerSO _dialogueContainer;
    public UnityEvent endEvent;
    public UnityEvent startEvent;

    public event Notify ProcessEndDialogue;
    public event Notify ProcessSkipDialogue;

    private bool _isDialogue;
    private bool _isMiniDialogue;
    
    [Header("Script References")]
    private DialogueUIManager _dialogueUIManager;
    private DialogueUIManager _dialogueMiniUIManager;
    private DialogueManager _dialogueManager;
    private MeetAndTalk.Localization.LocalizationManager _localizationManager;

    #endregion

    #region Main Functions
    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += Touch_OnFingerDown;
    }
    
    public void SetDialogueManager(DialogueManager dialogueManager)
    {
        _dialogueManager = dialogueManager;
        _dialogueUIManager = _dialogueManager.MainUI;
    }
    
    public void SetDialogueMiniUIManager(DialogueUIManager dialogueMiniUIManager)
    {
        _dialogueMiniUIManager = dialogueMiniUIManager;
    }

    #endregion

    #region OnStart/End Dialogue
    public void OnStartDialogue()
    {
        if (_dialogueManager.MainUI == _dialogueUIManager)
        {
            _isDialogue = true;
            GameManager.Instance.GetStateManager().ChangeState(StateManager.PlayerState.Dialogue);
        }else
        {
            _isMiniDialogue = true;
        }
    }

    public void OnEndDialogue()
    {   
        if (_dialogueManager.MainUI == _dialogueUIManager)
        {
            _isDialogue = false;
            GameManager.Instance.GetStateManager().ChangeState(StateManager.PlayerState.Idle);
        }else
        {
            _isMiniDialogue = false;
            _dialogueManager.ChangeUI(_dialogueUIManager);
        }
        StartProcessEnd();
    }

    #endregion

    #region Preocess End/Skip Dialogue
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
        _dialogueManager.SkipDialogue();
    }
    #endregion

    #region Start Dialogue
    public void StartDialogue(DialogueContainerSO dialogueContainerParam)
    {
        //run the event when the dialogue start
        startEvent.Invoke();
        if (_dialogueManager != null)
        {
            if (_isMiniDialogue)
            {
                _dialogueManager.ForceEndDialog();
            }
            _dialogueManager.ChangeUI(_dialogueUIManager);
            _dialogueManager.StartDialogue(dialogueContainerParam);
        }
        else
        {
            Debug.LogError("DialogueManager is not found");
        }
        
        // //subscribe to the event
        ProcessEndDialogue += bl_ProcessEndDialogue;
    }
    
    public void StartMiniDialogue(DialogueContainerSO dialogueContainerParam)
    {
        //run the event when the dialogue start
        startEvent.Invoke();

        if (_dialogueMiniUIManager != null)
        {
            _dialogueManager.ChangeUI(_dialogueMiniUIManager);
            _dialogueManager.StartDialogue(dialogueContainerParam);
        }
        else
        {
            Debug.LogError("DialogueManager is not found");
        }
    }

    #endregion

    #region event handler
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
   

    private void Touch_OnFingerDown(Finger touchedFinger)
    {
        if (_isDialogue && _dialogueManager.isSkippeable)
        {
            //if dialogueUIManager.lastTypingTime is less than Time.time + 0.1s then skip the dialogue
            if (_dialogueUIManager.lastTypingTime < Time.time - 0.1f)
            {
                StartProcessSkip();
            }
            else
            {
                _dialogueUIManager.SetFullText(_dialogueUIManager.fullText);
                _dialogueUIManager.characterIndex = _dialogueUIManager.fullText.Length;
            }
        }
    }
    #endregion
}