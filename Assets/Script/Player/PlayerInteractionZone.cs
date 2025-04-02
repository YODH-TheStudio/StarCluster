using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionZone : MonoBehaviour
{
    [SerializeField]
    private float _raycastDistance = 1.25f;

    [SerializeField]
    private GameObject _interactionButton;

    private PlayerScript _player = null;

    private VibrationManager _vibrationManager = null;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameManager.Instance.GetPlayer();
        _vibrationManager = GameManager.Instance.GetVibrationManager();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, _raycastDistance) ||
            Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(0.25f, 0, 1)), out hit, _raycastDistance) ||
            Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(-0.25f, 0, 1)), out hit, _raycastDistance) ||
            Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(0.5f, 0, 1)), out hit, _raycastDistance) ||
            Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(-0.5f, 0, 1)), out hit, _raycastDistance) 
            )
        {
            if(hit.transform.gameObject.tag == "Interactable")
            {
                _vibrationManager.Vibrate(100f, 0.2f); 
                _interactionButton.SetActive(true);
            }
            else
            {
                _interactionButton.SetActive(false);
            }
        }
        else if(_player.IsGrabbing())
        {
            _interactionButton.SetActive(true);
        }
        else
        {
            _interactionButton.SetActive(false);
        }
    }
}
