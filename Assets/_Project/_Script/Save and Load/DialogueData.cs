using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueData : MonoBehaviour
{
    public List<string> completedDialogues;
    public List<string> choicesMade;

    public void AddCompletedDialogue(string dialogueName){
        if(completedDialogues.Contains(dialogueName))
            return;
        completedDialogues.Add(dialogueName);
    }
    public void AddChoiceMade(string choiceName){
        if(choicesMade.Contains(choiceName))
            return;
        choicesMade.Add(choiceName);
    }
}
