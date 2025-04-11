using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;

[System.Serializable] // This attribute allows the class to be serialized and saved to a file
public class PlayerData
{
    public float[] position;
    //public List<string> objects;

    public enum DayState
    {
        Morning,
        Noon,
        Afternoon,
        Evening,
        Night
    }
    public int day;
    public DayState currentDayState;

    public PlayerData(PlayerScript player)
    {
        //objects = new List<string>();
        // get inventory slots number
        //int slotsCount = Inventory.slots.Count;
        //Debug.Log("Slots count: " + slotsCount);
       // objects = new string[slotsCount];

        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;

        // // Get items in inventory
        // Debug.Log("Saving inventory");
        // Debug.Log("Inventory items: " + Inventory.items.Count);
        // Debug.Log("objects : " + objects);
        // foreach(Item item in Inventory.items){
        //     Debug.Log("Saving item: " + item.itemName);
        //     objects.Add(item.itemName);
        // }

        // Save current day
        //day = GameManager.Instance.GetComponent<StateMachine>().day;
        //currentDayState = (DayState)GameManager.Instance.stateMachine.currentDayState;
         
        // Write this down to a file for debug
        //string dataString = JsonUtility.ToJson(objects);
        //File.WriteAllText(Application.persistentDataPath + "/objects.json", dataString);
    }
}
