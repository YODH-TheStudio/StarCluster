using MeetAndTalk;
using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    #region Field

    [SerializeField] private UnityEvent onEndEvent;

    #endregion
    
    #region Start Dialogue 
    public void StartDialogue(DialogueContainerSO dialogue)
    {
        if (dialogue != null)
        {
            GameManager.Instance.GetDialogueManager().StartDialogue(dialogue);
            GameManager.Instance.GetDialogueManager().ProcessEndDialogue += OnEndDialogue;
        }
    }
    
    public void StartMiniDialogue(DialogueContainerSO dialogue)
    {
        if (dialogue != null)
        {
            GameManager.Instance.GetDialogueManager().StartMiniDialogue(dialogue);
        }
    }

    private void OnEndDialogue()
    {
        onEndEvent?.Invoke();
        GameManager.Instance.GetDialogueManager().ProcessEndDialogue -= OnEndDialogue;
    }
    #endregion
}
