using System.Collections;
using System.Collections.Generic;
using MeetAndTalk;
using UnityEngine;

public class TriggerZone : MonoBehaviour
{

    [SerializeField] private DialogueContainerSO _dialogue;
    
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
        Debug.Log("aaaaa");
        // Add your logic here for when the dialogue ends
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerScript>() != null)
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
    }
}
