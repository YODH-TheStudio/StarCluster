using UnityEngine;


[CreateAssetMenu(fileName = "NewPlatformSettings", menuName = "Game/PlatformSettings")]
public class PlatformSettings : ScriptableObject
{
    public int ActivatorId; 
    public Color DefaultColor = Color.white;
    public Color ActiveColor = Color.green;
}
