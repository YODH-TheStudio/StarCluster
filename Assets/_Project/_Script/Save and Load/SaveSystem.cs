using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public static class SaveSystem
{
    public static void SavePlayer (PlayerScript player){
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.save";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(player);

        // Insert data into save file as binary
        formatter.Serialize(stream, data);
        stream.Close();

        Debug.Log("Saved file in " + path);
    }

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

    public static void SaveDialogueData(){
        DialogueData data = GameManager.Instance.GetComponent<DialogueData>();
        string jsonString = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/dialogueData.json", jsonString);
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
}
