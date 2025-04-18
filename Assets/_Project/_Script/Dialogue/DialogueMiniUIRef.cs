using MeetAndTalk;
using UnityEngine;

public class DialogueMiniUIRef : MonoBehaviour
{
    [SerializeField] private DialogueUIManager dialogueMiniUIManager;
    
    private void Start()
    {
        GameManager.Instance.GetDialogueManager().SetDialogueMiniUIManager(dialogueMiniUIManager);
    }
}
