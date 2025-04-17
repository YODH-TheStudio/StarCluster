using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnaimationEventRelay : MonoBehaviour
{
    private PlayerScript _playerScript;


    // Start is called before the first frame update
    void Start()
    {
        _playerScript = GetComponentInParent<PlayerScript>();
    }

    // Update is called once per frame
    public void FootStep()
    {
        if (_playerScript != null)
        {
            _playerScript.PlayFootstepSound();
        }
    }
}
