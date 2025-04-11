using System.Collections;
using System.Collections.Generic;
using MeetAndTalk;
using UnityEngine;

public class DialogueMiniUIRef : MonoBehaviour
{
    [SerializeField]
    private DialogueUIManager _dialogueMiniUIManager;
    
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.GetDialogueManager().SetDialogueMiniUIManager(_dialogueMiniUIManager);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
