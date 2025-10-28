using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    #region Field

    private DialogueData _dialogueData;
    private SaveData _saveData;
    public int currentSlot = 1;

    #endregion
    
    public SaveData GetSaveData()
    {
        return _saveData;
    }
    
    #region Main Functions
    
    // Load and save all
    public void SaveGame(int slot = 0){
        if(slot == 0)
            slot = currentSlot;
        string path = Application.persistentDataPath + "/Saves/Slot" + slot.ToString() + "/";
        if (Directory.Exists(path) == false)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            //Debug.Log("Creating save file at " + path);
        }
        //Debug.Log("Saving game");
        SavePlayer(slot);
        //SaveDialogueData();
        SavePuzzleData(slot);
        SaveObjects(slot);
        SaveInfo(slot); // Time played, planets unlocked, etc.
    }
    public void LoadGame(int slot = 0){
        if(slot == 0)
            slot = currentSlot;
        
        string path = Application.persistentDataPath + "/Saves/Slot" + slot.ToString() + "/";
        if (Directory.Exists(path) == false)
        {
            //SaveGame(slot);
            //Directory.CreateDirectory(Path.GetDirectoryName(path));
            //Debug.Log("Creating save file at " + path);
        }
        Debug.Log("Loading save at: " + path);
        LoadPlayer(slot);
        LoadPuzzleData(slot);
        LoadObjects(slot);
    }

    public void DeleteSave(int slot)
    {
        string path = Application.persistentDataPath + "/Saves/Slot" + slot.ToString();
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
            //Debug.Log("Deleted save at " + path);
        }
        else
        {
            Debug.LogWarning("No save at " + path);
        }
    }
    
    #endregion

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
    [System.Serializable]
    public class SaveData
    {
        public int unlockedPlanets = 1;
        public int currentPlanet = 1;
        public int unlockedPleiads = 0;
        public string saveTime = DateTime.Now.ToString("g");
        public string playtime = "0:00:00";
    }
    [System.Serializable]
    public class SaveableObjectsData
    {
        public SerializableDictionary<string, Vector3> positions;
        public SerializableDictionary<string, Quaternion> rotations;
        public SerializableDictionary<string, bool> enabled;
    }
    
    private void SaveInfo(int slot)
    {
        _saveData = new SaveData();
        string jsonString = JsonUtility.ToJson(_saveData);
        File.WriteAllText(Application.persistentDataPath + "/Saves/Slot" + slot.ToString() + "/info.json", jsonString);
    }
    private static void SavePlayer(int slot)
    {
        //Debug.LogWarning("SAVING PLAYER");
        PlayerData data = new PlayerData();
        string jsonString = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/Saves/Slot" + slot.ToString() + "/player.json", jsonString);
    }
    
    // private static void SaveDialogueData(int slot){
    //     DialogueData data = GameManager.Instance.GetComponent<DialogueData>();
    //     string jsonString = JsonUtility.ToJson(data);
    //     File.WriteAllText(Application.persistentDataPath + "/Saves/Slot" + slot.ToString() + "/dialogues.json", jsonString);
    // }
    private static void SavePuzzleData(int slot){
        FusionPoint[] fusionPoints = GameObject.FindObjectsOfType<FusionPoint>();
        Dictionary<string, bool> fusionPointsDic = new Dictionary<string, bool>();
        foreach (FusionPoint fp in fusionPoints)
        {
            fusionPointsDic.Add(fp.name, fp.GetState());
        }
        SerializableDictionary<string, bool> data = new SerializableDictionary<string, bool>(fusionPointsDic);
        File.WriteAllText(Application.persistentDataPath + "/Saves/Slot" + slot.ToString() + "/puzzles.json", JsonUtility.ToJson(data));
    }

    private static void SaveObjects(int slot)
    {
        SaveableObject[] objects = Resources.FindObjectsOfTypeAll<SaveableObject>();
        
        // Save positions
        Dictionary<string, Vector3> positionsDic = new Dictionary<string, Vector3>();
        Dictionary<string, Quaternion> rotationsDic = new Dictionary<string, Quaternion>();
        Dictionary<string, bool> enabledDic = new Dictionary<string, bool>();
        
        Dictionary<string, SaveableObjectsData> saveableObjectsDic = new Dictionary<string, SaveableObjectsData>();
        
        foreach (SaveableObject saveable in objects)
        {
            if(saveable.SavePos)
                positionsDic.Add(saveable.identifier, saveable.transform.position);
            if(saveable.SaveRot)
                rotationsDic.Add(saveable.identifier, saveable.transform.rotation);
            if(saveable.SaveEnabled)
                enabledDic.Add(saveable.identifier, saveable.gameObject.activeSelf);
        }
        SaveableObjectsData saveableObjectsData = new SaveableObjectsData();
        saveableObjectsData.positions = new SerializableDictionary<string, Vector3>(positionsDic);
        saveableObjectsData.rotations = new SerializableDictionary<string, Quaternion>(rotationsDic);
        saveableObjectsData.enabled = new SerializableDictionary<string, bool>(enabledDic);
        
        File.WriteAllText(Application.persistentDataPath + "/Saves/Slot" + slot.ToString() + "/objects.json", JsonUtility.ToJson(saveableObjectsData));
    }
    #endregion
    
    #region Load

    public SaveData GetSaveData(int slot)
    {
        string path = Application.persistentDataPath + "/Saves/Slot" + slot.ToString() + "/info.json";
        if(File.Exists(path)){
            string jsonString = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(jsonString);
            return data;
        } else {
            Debug.Log("Save file not found at " + path);
            return null;
        }
    }
    
    private static void LoadPlayer(int slot){
        print("aaaaaaaaaaaaaaaaa");
        print(Application.persistentDataPath);
        string path = Application.persistentDataPath + "/Saves/Slot" + slot.ToString() + "/player.json";
        if(File.Exists(path)){
            string jsonString = File.ReadAllText(path);
            // Debug.Log("Loaded Player data: " + jsonString);
            PlayerData data = JsonUtility.FromJson<PlayerData>(jsonString);
            GameManager.Instance.GetPlayer().Teleport(data.position[0], data.position[1], data.position[2]);
        } else {
            Debug.Log("Save file not found at " + path);
        }
    }
    
    // private static DialogueData LoadDialogueData(int slot){
    //     string path = Application.persistentDataPath + "/Saves/Slot" + slot.ToString() + "/dialogues.json";
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
        string path = Application.persistentDataPath + "/Saves/Slot" + slot.ToString() + "/puzzles.json";
        if(File.Exists(path)){
            string jsonString = File.ReadAllText(path);
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
            Debug.Log("Save file not found at " + path);
        }
    }
    
    private static void LoadObjects(int slot){
        string path = Application.persistentDataPath + "/Saves/Slot" + slot.ToString() + "/objects.json";
        if(File.Exists(path)){
            string jsonString = File.ReadAllText(path);
            SaveableObjectsData data = JsonUtility.FromJson<SaveableObjectsData>(jsonString);
            Dictionary<string, Vector3> positionsDic = data.positions.ToDictionary();
            Dictionary<string, Quaternion> rotationsDic = data.rotations.ToDictionary();
            Dictionary<string, bool> enabledDic = data.enabled.ToDictionary();

            // Get all saveable objects
            SaveableObject[] objects = Resources.FindObjectsOfTypeAll<SaveableObject>();
            foreach (SaveableObject saveable in objects)
            {
                if (saveable.SavePos)
                {
                    // load pos
                    Debug.Log("Loading position for " + saveable.identifier);
                    if (positionsDic.ContainsKey(saveable.identifier))
                    {
                        saveable.transform.position = positionsDic[saveable.identifier];
                    }
                }
                if (saveable.SaveRot)
                {
                    // load pos
                    Debug.Log("Loading rotation for " + saveable.identifier);
                    if (rotationsDic.ContainsKey(saveable.identifier))
                    {
                        saveable.transform.rotation = rotationsDic[saveable.identifier];
                    }
                }
                if (saveable.SaveEnabled)
                {
                    // load pos
                    Debug.Log("Loading enabled for " + saveable.identifier);
                    if (enabledDic.ContainsKey(saveable.identifier))
                    {
                        saveable.gameObject.SetActive(enabledDic[saveable.identifier]);
                    }
                }
            }
            
            // // Load positions
            // foreach (KeyValuePair<string, Vector3> kvp in positionsDic)
            // {
            //     if (kvp.Key != null)
            //     {
            //         // Set the position of the object
            //         GameObject obj = GameObject.Find(kvp.Key);
            //         obj.transform.position = kvp.Value;
            //     }
            // }
            // // Load rotations
            // foreach (KeyValuePair<string, Quaternion> kvp in rotationsDic)
            // {
            //     if (kvp.Key != null)
            //     {
            //         // Set the position of the object
            //         GameObject obj = GameObject.Find(kvp.Key);
            //         obj.transform.rotation = kvp.Value;
            //     }
            // }
            // // Load active
            // foreach (KeyValuePair<string, bool> kvp in enabledDic)
            // {
            //     if (kvp.Key != null)
            //     {
            //         // Set the position of the object
            //         GameObject obj = GameObject.Find(kvp.Key);
            //         obj.gameObject.SetActive(kvp.Value);
            //     }
            // }
        } else {
            Debug.Log("Save file not found at " + path);
        }
    }
    #endregion
}