using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueData : MonoBehaviour
{
    #region Fields
    public List<string> completedDialogues;
    public List<string> choicesMade;
    #endregion

    #region Choice/Complete Dialogue
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
    #endregion
}
