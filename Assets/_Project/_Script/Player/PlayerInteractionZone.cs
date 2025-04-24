using UnityEngine;

public class PlayerInteractionZone : MonoBehaviour
{
    #region Fields
    [SerializeField] private float raycastDistance = 1.25f;

    private GameObject _interactionButton;

    private PlayerScript _player = null;

    private VibrationManager _vibrationManager = null;
    
    private Interactable _currentInteractable = null;

    private Vector3 _ray1;
    private Vector3 _ray2;
    private Vector3 _ray3;
    private Vector3 _ray4;

    #endregion

    #region Main Functions
    private void Start()
    {
        _player = GameManager.Instance.GetPlayer();
        _vibrationManager = GameManager.Instance.GetVibrationManager();
        

        _ray1 = new Vector3(1f, 0, 1);
        _ray1.Normalize();
        _ray2 = new Vector3(-1f, 0, 1);
        _ray2.Normalize();
        _ray3 = new Vector3(1f, 0, -1);
        _ray3.Normalize();
        _ray4 = new Vector3(-1f, 0, -1);
        _ray4.Normalize();
    }

    private void FixedUpdate()
    {
        if (_interactionButton == null)
        {
            return;
        }
        
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, raycastDistance) ||
            Physics.Raycast(transform.position, transform.TransformDirection(_ray1), out hit, raycastDistance) ||
            Physics.Raycast(transform.position, transform.TransformDirection(_ray2), out hit, raycastDistance) ||
            Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), out hit, raycastDistance) ||
            Physics.Raycast(transform.position, transform.TransformDirection(_ray3), out hit, raycastDistance) ||
            Physics.Raycast(transform.position, transform.TransformDirection(_ray4), out hit, raycastDistance) ||
            Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hit, raycastDistance) ||
            Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, raycastDistance)
           )
        {
            if(hit.collider.transform.GetComponent<Interactable>() && hit.collider.transform.GetComponent<Interactable>().IsInteractable())
            {
                _vibrationManager.Vibrate(100f, 0.2f);
                _interactionButton.SetActive(true);
                _currentInteractable = hit.collider.transform.GetComponent<Interactable>();
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
    #endregion

    #region Interactions

    public void SetInteractionButton(GameObject button)
    {
        _interactionButton = button;
    }
    
    public Interactable GetCurrentInteractable()
    {
        return _currentInteractable;
    }
    #endregion
}
