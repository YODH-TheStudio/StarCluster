using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GroundScript : MonoBehaviour
{
    #region Fields

    [SerializeField]
    private GameObject _particleSystem;

    private bool _isOnGrass;

    public bool IsOnGrass => _isOnGrass;

    #endregion

    #region Triggers
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            _isOnGrass = true;
            other.gameObject.GetComponent<PlayerScript>().SpawnParticle(_particleSystem);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            _isOnGrass = false;
            other.gameObject.GetComponent<PlayerScript>().DeleteParticle(_particleSystem);
        }
    }
    #endregion
}
