using MeetAndTalk;
using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : MonoBehaviour
{
    #region Fields
    [SerializeField] private DialogueContainerSO dialogue;
    [SerializeField] private UnityEvent onTriggerEnterEvent;
    [SerializeField] private UnityEvent onDialogueEndEvent;

    #endregion

    #region End Dialogue event
    private void OnEndDialogue()
    {
        // Unsubscribe from the event
        GameManager.Instance.GetDialogueManager().ProcessEndDialogue -= OnEndDialogue;
        onDialogueEndEvent.Invoke();
        // Add your logic here for when the dialogue ends
    }
    #endregion

    #region Start Dialogue
    public void StartDialogue()
    {
        if (dialogue != null)
        {
            GameManager.Instance.GetDialogueManager().StartDialogue(dialogue);
            
            GameManager.Instance.GetDialogueManager().ProcessEndDialogue += OnEndDialogue;
        }
        else
        {
            Debug.LogError("DialogueContainer is null for trigger zone");
        }
    }
    
    public void StartMiniDialogue()
    {
        if (dialogue != null)
        {
            GameManager.Instance.GetDialogueManager().StartMiniDialogue(dialogue);
        }
        else
        {
            Debug.LogError("DialogueContainer is null for trigger zone");
        }
    }
    #endregion

    #region Trigger Functions
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerScript>() == null) return;
        
        if (dialogue != null)
        {
            onTriggerEnterEvent?.Invoke();
        }
        else
        {
            Debug.LogError("DialogueContainer is null for trigger zone");
        }
    }
    #endregion
}
