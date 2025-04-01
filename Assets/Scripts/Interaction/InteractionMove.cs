using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class InteractionMove : MonoBehaviour
{
    public void OnInteractMove(PlayerScript player, GameObject InteractedObject)
    {
        // if the player is not grabbing with an object, grab the object
        if (!player.IsGrabbing())
        {
            player.SetGrabbing(true);
            player.SetObjectGrabbed(InteractedObject);
            player.GetObjectGrabbed().GetComponent<BoxCollider>().enabled = false;
            player.GetObjectGrabbed().GetComponent<Rigidbody>().useGravity = false;
        }
    }
}
