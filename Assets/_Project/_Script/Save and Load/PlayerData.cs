[System.Serializable]
public class PlayerData
{
    public float[] position;
    //public List<string> objects;
    
    public PlayerData()
    {
        //objects = new List<string>();
        // get inventory slots number
        //int slotsCount = Inventory.slots.Count;
        //Debug.Log("Slots count: " + slotsCount);
       // objects = new string[slotsCount];

       PlayerScript player = GameManager.Instance.GetPlayer();
       
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
    }
}
