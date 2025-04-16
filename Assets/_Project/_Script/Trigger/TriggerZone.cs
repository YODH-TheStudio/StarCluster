using System.Collections;
using System.Collections.Generic;
using MeetAndTalk;
using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : MonoBehaviour
{

    [SerializeField] private DialogueContainerSO _dialogue;
    [SerializeField] private UnityEvent _onTriggerEnterEvent;
    [SerializeField] private UnityEvent _onDialogueEndEvent;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnEndDialogue()
    {
        // Unsubscribe from the event
        GameManager.Instance.GetDialogueManager().ProcessEndDialogue -= OnEndDialogue;
        _onDialogueEndEvent.Invoke();
        // Add your logic here for when the dialogue ends
    }

    public void StartDialogue()
    {
        GameManager.Instance.GetDialogueManager().StartDialogue(_dialogue);
            
        GameManager.Instance.GetDialogueManager().ProcessEndDialogue += OnEndDialogue;
    }
    
    public void StartDialogue()
    {
        if (_dialogue != null)
        {
            GameManager.Instance.GetDialogueManager().StartDialogue(_dialogue);
            
            GameManager.Instance.GetDialogueManager().ProcessEndDialogue += OnEndDialogue;
        }
        else
        {
            Debug.LogError("DialogueContainer is null for trigger zone");
        }
    }
    
    public void StartMiniDialogue()
    {
        if (_dialogue != null)
        {
            GameManager.Instance.GetDialogueManager().StartMiniDialogue(_dialogue);
        }
        else
        {
            Debug.LogError("DialogueContainer is null for trigger zone");
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerScript>() != null)
        {
            if (_dialogue != null)
            {
                _onTriggerEnterEvent?.Invoke();
            }
            else
            {
                Debug.LogError("DialogueContainer is null for trigger zone");
            }
        }
    }
}
