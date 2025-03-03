using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionSwitch : MonoBehaviour
{

    public void InteractSwitch(PlayerScript player, GameObject InteractedObject)
    {
        if (InteractedObject.tag == "Interactable")
        {
            if (InteractedObject.layer == 31)
            {
                gameObject.GetComponent<InteractionMove>().OnInteractMove(player, InteractedObject);
            }

            else if (InteractedObject.layer == 30)
            {
                gameObject.GetComponent<InteractionEnigme>().OnInteractEnigme();
            }

            // Add more layer here if needed

            else
            {
                Debug.LogError("No Layer Found for " + InteractedObject.name + " even if it have Interactable tag");
            }
        }
    }
}
