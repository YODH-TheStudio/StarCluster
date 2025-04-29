using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueData : MonoBehaviour
{
    public List<string> completedDialogues;
    public List<string> choicesMade;

    public void addCompletedDialogue(string dialogueName){
        if(completedDialogues.Contains(dialogueName))
            return;
        completedDialogues.Add(dialogueName);
    }
    public void addChoiceMade(string choiceName){
        if(choicesMade.Contains(choiceName))
            return;
        choicesMade.Add(choiceName);
    }
}
