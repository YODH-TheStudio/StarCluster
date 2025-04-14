using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private DialogueData _dialogueData;
    
    // Load and save all
    public void SaveGame(){
        Debug.Log("Saving game");
        SavePlayer();
        SaveDialogueData();
        SavePuzzleData();
        SaveObjects();
    }
    public void LoadGame(){
        Debug.Log("Loading game");
        LoadPlayer();
        LoadDialogueData();
        LoadPuzzleData();
        LoadObjects();
    }

    // Dialogues and choices data
    public void addCompletedDialogue(string dialogueName){
        //DialogueData.completedDialogues.Add(dialogueName);
        _dialogueData.addCompletedDialogue(dialogueName);
    }
    public void addChoiceMade(string choiceName){
        //Debug.Log("Saving choice in save manager: " + choiceName);
        //DialogueData.choicesMade.Add(choiceName);
        _dialogueData.addChoiceMade(choiceName);
    }
    
    #region Save
    // Player data
    private void SavePlayer(){
        SaveSystem.SavePlayer(GameManager.Instance.GetPlayer());
    }
    
    // Objects data
    private void SaveObjects(){
       SaveSystem.SaveObjects();
    }
    
    private void SaveDialogueData(){
        //Debug.Log("Saving dialogue data");
        SaveSystem.SaveDialogueData();
    }
    private void SavePuzzleData(){
        SaveSystem.SavePuzzleData(GameManager.Instance.GetPuzzleManager());
    }
    #endregion

    #region Load
    private void LoadPlayer(){
        PlayerData data = SaveSystem.LoadPlayer();
        if(data != null){
            // Load position
            Debug.Log("Loading playerPos: " + data.position[0] + ", " + data.position[1] + ", " + data.position[2]);
            //GameManager.Instance.GetPlayer().gameObject.transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
            GameManager.Instance.GetPlayer().Teleport(data.position[0], data.position[1], data.position[2]);
            
            // // Clear Inventory
            // Inventory.Clear();
            // // Load inventory
            // foreach(string objName in data.objects){
            //     
            //     Debug.Log("Loading item: " + objName);
            //
            //     GameObject obj = ObjectsManager.GetObject(objName);
            //     if(obj != null){
            //         Inventory.AddItem(obj);
            //     }
            // }

            // Load day
            //GameManager.Instance.stateMachine.ChangeDay(data.day);
            //GameManager.Instance.stateMachine.ChangeDayState((StateMachine.DayState)data.currentDayState, false);
        }
    }
    public void LoadDialogueData(){
        //_dialogueData = SaveSystem.LoadDialogueData();
        SaveSystem.LoadDialogueData();
    }
    
    private void LoadPuzzleData(){
        List<PuzzleData> data = SaveSystem.LoadPuzzleData();
        if(data != null){
            GameManager.Instance.GetPuzzleManager().SetData(data);
        }
    }

    public void LoadObjects()
    {
        SaveSystem.LoadObjects();
    }
    #endregion
}
