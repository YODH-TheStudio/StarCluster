using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GroundScript : MonoBehaviour
{
    [SerializeField]
    private GameObject _particleSystem;

    private void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerScript>().SpawnParticle(_particleSystem);
        }
    }
}
