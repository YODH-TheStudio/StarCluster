using System;
using UnityEngine;


public class Saveable : MonoBehaviour
{
    public string identifier;

    private void Awake()
    {
        // check if the identifier is empty
        if (string.IsNullOrEmpty(identifier))
        {
            // generate a unique identifier
            
            identifier = Guid.NewGuid().ToString();
        }
        // check if a gameObject has the same name
        while (NameInUse(identifier))
        {
            Debug.LogError("A gameObject with identifier " + identifier + " already exists. Generating a new one.");
            // destroy this gameObject
            identifier = Guid.NewGuid().ToString();
        }
        gameObject.name = identifier;
    }

    private bool NameInUse(string name)
    {
        return GameObject.Find(name) != null;
    }
}
