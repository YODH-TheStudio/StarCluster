using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionZone : MonoBehaviour
{
    [SerializeField]
    private float _raycastDistance = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, _raycastDistance))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward), Color.yellow);
        }
    }
}
