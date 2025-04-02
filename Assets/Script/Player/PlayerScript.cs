using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour, Controler.IPlayerActions
{
    // Player Controller Variables 
    [SerializeField] private float _speed = 250.0f;
    [SerializeField] private float _raycastDistance = 1.25f;
    [SerializeField] private float _turnSpeed = 360.0f;

    private CharacterController _controller;
    private Vector3 _direction;

    [SerializeField] private InteractionSwitch _interactionSwitch;

    // Grab
    private bool _isGrabbing = false;
    private GameObject _objectGrabbed;

    // Pushing/Pulling
    private bool _isPushingPulling = false; // Set by object on interact
    private Vector3 _lastMoveDirection; // Store normalized player direction each frame

    // Animation Flag
    private bool isAnimating = false;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        Controler playerControls = new Controler();
        playerControls.Player.SetCallbacks(this);
    }

    public void OnMove(Vector2 readVector)
    {
        Vector3 position = new Vector3(readVector.x, 0, readVector.y);
        _direction = Quaternion.Euler(0, 45f, 0) * position;
        _lastMoveDirection = _direction.normalized;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (IsGrabbing())
            {
                SetGrabbing(false);
                if (_objectGrabbed != null)
                {
                    _objectGrabbed.GetComponent<BoxCollider>().enabled = true;
                    _objectGrabbed.GetComponent<Rigidbody>().useGravity = true;
                    _objectGrabbed = null;
                }
                return;
            }

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, _raycastDistance))
            {
                Interactable interactable = hit.transform.GetComponent<Interactable>();
                if (interactable != null)
                {
                    interactable.SetUserTransform(this.transform);
                    interactable.SetUserInteractionNormal(hit.normal);
                    interactable.Interact();
                }
            }
        }
    }

    private void Look()
    {
        if (_direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _turnSpeed * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (!isAnimating)
        {
            if (_isPushingPulling)
            {
                _direction = new Vector3(1, 1, 1) * _direction.z;
            }
            else
            {
                _controller.SimpleMove(_direction * _speed * Time.deltaTime);
            }
        }

        if (!_isPushingPulling) Look();

        if (IsGrabbing() && _objectGrabbed != null)
        {
            _objectGrabbed.transform.position = transform.position + (Vector3.forward * 2);
        }
    }

    public bool IsGrabbing()
    {
        return _isGrabbing;
    }

    public void SetGrabbing(bool isGrabbing)
    {
        _isGrabbing = isGrabbing;
    }

    public GameObject GetObjectGrabbed()
    {
        return _objectGrabbed;
    }

    public void SetObjectGrabbed(GameObject objectGrabbed)
    {
        _objectGrabbed = objectGrabbed;
    }

    public void TogglePushingPulling()
    {
        _isPushingPulling = !_isPushingPulling;
    }

    public Vector3 GetMoveDirection()
    {
        return _lastMoveDirection;
    }

    public void SetMoveDirection(Vector3 direction)
    {
        _lastMoveDirection = direction;
    }

    public void SetIsAnimating(bool animating)
    {
        isAnimating = animating;
    }
}
