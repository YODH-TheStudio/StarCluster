using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private DialogueData _dialogueData;
    public int currentSlot = 1;
    
    // Load and save all
    public void SaveGame(int slot = 0){
        if(slot == 0)
            slot = currentSlot;
        string path = Application.persistentDataPath + "/Slot" + slot.ToString() + "/";
        if (Directory.Exists(path) == false)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            //Debug.Log("Creating save file at " + path);
        }
        Debug.Log("Saving game");
        SavePlayer(slot);
        //SaveDialogueData();
        SavePuzzleData(slot);
        SaveObjects(slot);
    }
    public void LoadGame(int slot = 0){
        if(slot == 0)
            slot = currentSlot;
        Debug.Log("Loading game");
        LoadPlayer(slot);
        //LoadDialogueData();
        LoadPuzzleData(slot);
        LoadObjects(slot);
    }

    public void DeleteSave(int slot)
    {
        string path = Application.persistentDataPath + "/Slot" + slot.ToString();
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
            Debug.Log("Deleted save at " + path);
        }
        else
        {
            Debug.LogWarning("No save at " + path);
        }
    }

    // Dialogues and choices data
    // public void addCompletedDialogue(string dialogueName){
    //     //DialogueData.completedDialogues.Add(dialogueName);
    //     _dialogueData.addCompletedDialogue(dialogueName);
    // }
    // public void addChoiceMade(string choiceName){
    //     //Debug.Log("Saving choice in save manager: " + choiceName);
    //     //DialogueData.choicesMade.Add(choiceName);
    //     _dialogueData.addChoiceMade(choiceName);
    // }
    
    #region Save
    private static void SavePlayer(int slot)
    {
        // BinaryFormatter formatter = new BinaryFormatter();
        // string path = Application.persistentDataPath + "/Slot" + slot.ToString() + "/player.save";
        // FileStream stream = new FileStream(path, FileMode.Create);
        // PlayerData data = new PlayerData();
        //
        // // Insert data into save file as binary
        // formatter.Serialize(stream, data);
        // stream.Close();
        //
        // //Debug.Log("Saved file in " + path);
        PlayerData data = new PlayerData();
        string jsonString = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/Slot" + slot.ToString() + "/player.json", jsonString);
    }
    
    // private static void SaveDialogueData(int slot){
    //     DialogueData data = GameManager.Instance.GetComponent<DialogueData>();
    //     string jsonString = JsonUtility.ToJson(data);
    //     File.WriteAllText(Application.persistentDataPath + "/Slot" + slot.ToString() + "/dialogues.json", jsonString);
    // }
    private static void SavePuzzleData(int slot){
        FusionPoint[] fusionPoints = GameObject.FindObjectsOfType<FusionPoint>();
        Dictionary<string, bool> fusionPointsDic = new Dictionary<string, bool>();
        foreach (FusionPoint fp in fusionPoints)
        {
            fusionPointsDic.Add(fp.name, fp.GetState());
        }
        SerializableDictionary<string, bool> data = new SerializableDictionary<string, bool>(fusionPointsDic);
        File.WriteAllText(Application.persistentDataPath + "/Slot" + slot.ToString() + "/puzzles.json", JsonUtility.ToJson(data));
    }

    private static void SaveObjects(int slot)
    {
        SavePosition[] objects = GameObject.FindObjectsOfType<SavePosition>();
        
        Dictionary<string, Vector3> positionsDic = new Dictionary<string, Vector3>();
        foreach (SavePosition saveable in objects)
        {
            positionsDic.Add(saveable.identifier, saveable.transform.position);
        }
        SerializableDictionary<string, Vector3> positions = new SerializableDictionary<string, Vector3>(positionsDic);
        
        File.WriteAllText(Application.persistentDataPath + "/Slot" + slot.ToString() + "/objects.json", JsonUtility.ToJson(positions));
    }
    #endregion
    
    #region Load
    private static void LoadPlayer(int slot){
        string path = Application.persistentDataPath + "/Slot" + slot.ToString() +"/player.save";
        if(File.Exists(path)){
            //load
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            // Deserialize binary from stream
            PlayerData data = (PlayerData) formatter.Deserialize(stream);

            // Write this down to a file for debug
            string dataString = JsonUtility.ToJson(data);
            File.WriteAllText(Application.persistentDataPath + "/Slot" + slot.ToString() + "/player.json", dataString);

            stream.Close();
            
            Debug.Log("Loading playerPos: " + data.position[0] + ", " + data.position[1] + ", " + data.position[2]);
            GameManager.Instance.GetPlayer().Teleport(data.position[0], data.position[1], data.position[2]);
        } else {
            Debug.LogError("Player save file not found in " + path);
        }
    }
    
    // private static DialogueData LoadDialogueData(int slot){
    //     string path = Application.persistentDataPath + "/Slot" + slot.ToString() + "/dialogues.json";
    //     if(File.Exists(path)){
    //         //load
    //         string jsonString = File.ReadAllText(path);
    //         Debug.Log("Loaded dialogue data: " + jsonString);
    //         DialogueData data = GameManager.Instance.GetComponent<DialogueData>();
    //         JsonUtility.FromJsonOverwrite(jsonString, data);
    //         return data;
    //     } else {
    //         // Create the save
    //         SaveDialogueData(slot);
    //         string jsonString = File.ReadAllText(path);
    //         DialogueData data = JsonUtility.FromJson<DialogueData>(jsonString);
    //         return data;
    //     }
    // }
    
    private static void LoadPuzzleData(int slot){
        string path = Application.persistentDataPath + "/Slot" + slot.ToString() + "/puzzles.json";
        if(File.Exists(path)){
            string jsonString = File.ReadAllText(path);
            Debug.Log("Loaded puzzle data: " + jsonString);
            SerializableDictionary<string, bool> data = JsonUtility.FromJson<SerializableDictionary<string, bool>>(jsonString);
            Dictionary<string, bool> dataDic = data.ToDictionary();

            foreach (KeyValuePair<string, bool> kvp in dataDic)
            {
                if (kvp.Key != null)
                {
                    // Set the state of the fusion point
                    GameObject obj = GameObject.Find(kvp.Key);
                    obj.GetComponent<FusionPoint>().SetState(kvp.Value);
                }
            }
        } else {
            Debug.LogError("Save file not found at " + path);
        }
    }
    
    private static void LoadObjects(int slot){
        string path = Application.persistentDataPath + "/Slot" + slot.ToString() + "/objects.json";
        if(File.Exists(path)){
            string jsonString = File.ReadAllText(path);
            Debug.Log("Loaded Objects data: " + jsonString);
            SerializableDictionary<string, Vector3> data = JsonUtility.FromJson<SerializableDictionary<string, Vector3>>(jsonString);
            Dictionary<string, Vector3> positionsDic = data.ToDictionary();

            foreach (KeyValuePair<string, Vector3> kvp in positionsDic)
            {
                if (kvp.Key != null)
                {
                    // Set the position of the object
                    GameObject obj = GameObject.Find(kvp.Key);
                    obj.transform.position = kvp.Value;
                }
            }
        } else {
            Debug.LogError("Save file not found at " + path);
        }
    }
    #endregion
}
