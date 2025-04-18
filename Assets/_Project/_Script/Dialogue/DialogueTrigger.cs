using MeetAndTalk;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    #region Start Dialogue 
    public void StartDialogue(DialogueContainerSO dialogue)
    {
        if (dialogue != null)
        {
            GameManager.Instance.GetDialogueManager().StartDialogue(dialogue);
        }
    }
    
    public void StartMiniDialogue(DialogueContainerSO dialogue)
    {
        if (dialogue != null)
        {
            GameManager.Instance.GetDialogueManager().StartMiniDialogue(dialogue);
        }
    }
    #endregion
}
