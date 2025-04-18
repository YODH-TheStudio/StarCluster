using MeetAndTalk;
using UnityEngine;

public class DialogueMiniUIRef : MonoBehaviour
{
    #region Fields
    [SerializeField] private DialogueUIManager dialogueMiniUIManager;
    #endregion

    #region Main Functions
    private void Start()
    {
        GameManager.Instance.GetDialogueManager().SetDialogueMiniUIManager(dialogueMiniUIManager);
    }
    #endregion
}
