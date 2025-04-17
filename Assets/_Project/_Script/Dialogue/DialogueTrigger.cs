using System.Collections;
using System.Collections.Generic;
using MeetAndTalk;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
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
}
