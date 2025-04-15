using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Linq;

public static class SaveSystem
{
    #region Save
    public static void SavePlayer (PlayerScript player){
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.save";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData();

        // Insert data into save file as binary
        formatter.Serialize(stream, data);
        stream.Close();

        Debug.Log("Saved file in " + path);
    }
    
    public static void SaveDialogueData(){
        DialogueData data = GameManager.Instance.GetComponent<DialogueData>();
        string jsonString = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/dialogueData.json", jsonString);
    }
    public static void SavePuzzleData(PuzzleManager puzzleManager){
        // SerializableList<PuzzleData> puzzleData = new SerializableList<PuzzleData>(puzzleManager.GetData());
        // string jsonString = JsonUtility.ToJson(puzzleData);
        // Debug.Log("Puzzle data: "+ puzzleData);
        // File.WriteAllText(Application.persistentDataPath + "/puzzleData.json", jsonString);
        
        FusionPoint[] fusionPoints = GameObject.FindObjectsOfType<FusionPoint>();
        Dictionary<string, bool> fusionPointsDic = new Dictionary<string, bool>();
        foreach (FusionPoint fp in fusionPoints)
        {
            fusionPointsDic.Add(fp.name, fp.GetState());
        }
        SerializableDictionary<string, bool> data = new SerializableDictionary<string, bool>(fusionPointsDic);
        File.WriteAllText(Application.persistentDataPath + "/puzzleData.json", JsonUtility.ToJson(data));
    }

    public static void SaveObjects()
    {
        Saveable[] objects = GameObject.FindObjectsOfType<Saveable>();
        //Debug.Log("Found " + objects.Length + " objects to save");
        
        Dictionary<string, Vector3> positionsDic = new Dictionary<string, Vector3>();
        foreach (Saveable saveable in objects)
        {
            //Debug.Log("Dic entry: " + saveable.identifier + ", " + saveable.transform.position);
            positionsDic.Add(saveable.identifier, saveable.transform.position);
        }
        SerializableDictionary<string, Vector3> positions = new SerializableDictionary<string, Vector3>(positionsDic);
        
        //Debug.Log("Json: " + JsonUtility.ToJson(positions));
        File.WriteAllText(Application.persistentDataPath + "/objects.json", JsonUtility.ToJson(positions));
        //Debug.Log("Positions of objects saved: " + positions);
        //Debug.Log("Saved objects to " + Application.persistentDataPath + "/objects.save");
    }
    #endregion
    
    #region Load
    public static PlayerData LoadPlayer(){
        string path = Application.persistentDataPath + "/player.save";
        if(File.Exists(path)){
            //load
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            // Deserialize binary from stream
            PlayerData data = (PlayerData) formatter.Deserialize(stream);

            // Write this down to a file for debug
            string dataString = JsonUtility.ToJson(data);
            File.WriteAllText(Application.persistentDataPath + "/player.json", dataString);

            stream.Close();

            return data;
        } else {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
    
    public static DialogueData LoadDialogueData(){
        string path = Application.persistentDataPath + "/dialogueData.json";
        if(File.Exists(path)){
            //load
            string jsonString = File.ReadAllText(path);
            Debug.Log("Loaded dialogue data: " + jsonString);
            DialogueData data = GameManager.Instance.GetComponent<DialogueData>();
            JsonUtility.FromJsonOverwrite(jsonString, data);
            return data;
        } else {
            // Create the save
            SaveDialogueData();
            string jsonString = File.ReadAllText(path);
            DialogueData data = JsonUtility.FromJson<DialogueData>(jsonString);
            return data;
        }
    }
    
    public static void LoadPuzzleData(){
        // string path = Application.persistentDataPath + "/puzzleData.json";
        // if(File.Exists(path)){
        //     //load
        //     string jsonString = File.ReadAllText(path);
        //     //Debug.Log("Loaded puzzle data: " + jsonString);
        //     SerializableList<PuzzleData> data = JsonUtility.FromJson<SerializableList<PuzzleData>>(jsonString);
        //     List<PuzzleData> list = data.ToList();
        //     return list;
        // } else {
        //     // Create the save
        //     SavePuzzleData(GameManager.Instance.GetPuzzleManager());
        //     string jsonString = File.ReadAllText(path);
        //     List<PuzzleData> data = JsonUtility.FromJson<PuzzleData[]>(jsonString).ToList();
        //     return data;
        // }
        string path = Application.persistentDataPath + "/puzzleData.json";
        if(File.Exists(path)){
            string jsonString = File.ReadAllText(path);
            Debug.Log("Loaded puzzle data: " + jsonString);
            SerializableDictionary<string, bool> data = JsonUtility.FromJson<SerializableDictionary<string, bool>>(jsonString);
            //Dictionary<string, Vector3> data = JsonUtility.FromJson<Dictionary<string, Vector3>>(jsonString);
            Dictionary<string, bool> dataDic = data.ToDictionary();

            // Deserialize binary from stream
            //Dictionary<GameObject, Vector3> data = (Dictionary<GameObject, Vector3>) formatter.Deserialize(stream);
            foreach (KeyValuePair<string, bool> kvp in dataDic)
            {
                if (kvp.Key != null)
                {
                    // Set the position of the object
                    GameObject obj = GameObject.Find(kvp.Key);
                    obj.GetComponent<FusionPoint>().SetState(kvp.Value);
                }
            }
            //return data;
        } else {
            Debug.LogError("Save file not found at " + path);
            //return null;
        }
    }
    
    public static void LoadObjects(){
        string path = Application.persistentDataPath + "/objects.json";
        if(File.Exists(path)){
            string jsonString = File.ReadAllText(path);
            Debug.Log("Loaded Objects data: " + jsonString);
            SerializableDictionary<string, Vector3> data = JsonUtility.FromJson<SerializableDictionary<string, Vector3>>(jsonString);
            //Dictionary<string, Vector3> data = JsonUtility.FromJson<Dictionary<string, Vector3>>(jsonString);
            Dictionary<string, Vector3> positionsDic = data.ToDictionary();

            // Deserialize binary from stream
            //Dictionary<GameObject, Vector3> data = (Dictionary<GameObject, Vector3>) formatter.Deserialize(stream);
            foreach (KeyValuePair<string, Vector3> kvp in positionsDic)
            {
                if (kvp.Key != null)
                {
                    // Set the position of the object
                    GameObject obj = GameObject.Find(kvp.Key);
                    obj.transform.position = kvp.Value;
                }
            }
            //return data;
        } else {
            Debug.LogError("Save file not found at " + path);
            //return null;
        }
    }
    #endregion
}
