using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionZone : MonoBehaviour
{
    [SerializeField]
    private float _raycastDistance = 1.25f;

    private GameObject _interactionButton;

    private PlayerScript _player = null;

    private VibrationManager _vibrationManager = null;
    
    private Interactable _currentInteractable = null;
    
    private RaycastHit _raycastHit;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameManager.Instance.GetPlayer();
        _vibrationManager = GameManager.Instance.GetVibrationManager();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_interactionButton == null)
        {
            return;
        }
        
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, _raycastDistance) ||
            Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(0.25f, 0, 1)), out hit, _raycastDistance) ||
            Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(-0.25f, 0, 1)), out hit, _raycastDistance) ||
            Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(0.5f, 0, 1)), out hit, _raycastDistance) ||
            Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(-0.5f, 0, 1)), out hit, _raycastDistance) 
            )
        {
            print(hit.collider.transform.name);
            if(hit.collider.transform.GetComponent<Interactable>())
            {
                _vibrationManager.Vibrate(100f, 0.2f);
                _interactionButton.SetActive(true);
                _currentInteractable = hit.collider.transform.GetComponent<Interactable>();
                _raycastHit = hit;
            }
            else
            {
                _interactionButton.SetActive(false);
                _currentInteractable = null;
            }
            return;
        }
        
        _interactionButton.SetActive(false);
    }
    
    public void SetInteractionButton(GameObject button)
    {
        _interactionButton = button;
    }
    
    public Interactable GetCurrentInteractable()
    {
        return _currentInteractable;
    }
    
    public RaycastHit GetRaycastHit()
    {
        return _raycastHit;
    }
}
