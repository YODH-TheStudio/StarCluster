using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{

    private PlayerScript _player;
    private DialogueData _dialogueData;
    void Start()
    {
        _player = GameManager.Instance.GetPlayer();
        if(_player == null)
            Debug.LogError("Player not found in SaveManager");
        //_dialogueData = GameManager.Instance.GetDialogueManager() GetComponent<DialogueData>();
    }

    
    // Load and save all
    public void SaveGame(){
        SavePlayer();
        SaveDialogueData();
    }
    public void LoadGame(){
        LoadPlayer();
        LoadDialogueData();
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
        SaveSystem.SavePlayer(_player);
    }

    public void SaveDialogueData(){
        Debug.Log("Saving dialogue data");
        SaveSystem.SaveDialogueData();
    }
    #endregion

    #region Load
    private void LoadPlayer(){
        PlayerData data = SaveSystem.LoadPlayer();
        if(data != null){
            // Load position
            _player.transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);

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
    #endregion
}
