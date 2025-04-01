using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activator : MonoBehaviour
{
    public PlatformSettings Settings;

    public int GetId()
    {
        return Settings != null ? Settings.ActivatorId : -1;
    }
}
