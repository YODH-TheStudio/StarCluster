using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour, Controler.IPlayerActions
{
    // Player Controler Variable 
    [SerializeField]
    private float _speed = 250.0f;

    [SerializeField]
    private float _raycastDistance = 1.25f;

    [SerializeField]
    private float _turnSpeed = 360.0f;

    private CharacterController _controller;
    private Vector3 _direction;

    [SerializeField]
    private InteractionSwitch _interactionSwitch;

    // Grab
    private bool _isGrabbing = false;
    private GameObject _objectGrabbed;

    // Pushing Pulling
    private bool _isPushingPulling = false; // set by object on interact
    private Vector3 _lastMoveDirection; // Get normalized player direction each frame

    public void TogglePushingPulling(){ _isPushingPulling = !_isPushingPulling; }
    public Vector3 GetMoveDirection()
    {
        return _lastMoveDirection;
    }

    private bool isAnimating = false;
    public void SetIsAnimating(bool animating)
    {
        isAnimating = animating;
    }

    // Set the variable
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        Controler playerControls = new Controler();
        playerControls.Player.SetCallbacks(this);
    }

    // Allow the player to move, Is called by the character controler component in player
    public void OnMove(Vector2 readVector)
    {
        // Calculated the movement of the player with the 45° change due to isometric view
        Vector3 position = new Vector3(readVector.x, 0, readVector.y);
        Matrix4x4 isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45.0f, 0));

        _direction = isoMatrix.MultiplyPoint3x4(position);

        // store last move position to inform component about user movement direction
        _lastMoveDirection = _direction.normalized; 
    }

    // Allow the player to interact with objetc, Is called by the character controler component in player
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // Hold object
            if (IsGrabbing())
            {
                SetGrabbing(false);
                _objectGrabbed.GetComponent<BoxCollider>().enabled = true;
                _objectGrabbed.GetComponent<Rigidbody>().useGravity = true;

                return;
            }


            // Interact with object
            else
            {
                RaycastHit hit;
                // Does the ray intersect any objects excluding the player layer
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, _raycastDistance))

                {
                    // Old
                    //_interactionSwitch.InteractSwitch(this, hit.transform.gameObject);

                    // Is interactable object
                    Interactable interactable = hit.transform.GetComponent<Interactable>();
                    if (interactable != null)
                    {
                        interactable.SetUserTransform(this.transform);
                        interactable.SetUserInteractionNormal(hit.normal);
                        interactable.Interact();
                    }
                }

                return;
            }
        }
    }

    // Rotate the player in the direction he is walking
    void Look()
    {
        // Don't go back to the starting rotation when the player don't move
        if (_direction != Vector3.zero)
        {
            // Calculated the rotation and set it
            var relative = (transform.position + _direction) - transform.position;
            var rot = Quaternion.LookRotation(relative, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, _turnSpeed * Time.deltaTime);
        }  
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isAnimating)
        {
            if (_isPushingPulling)
            {
                // Limit movement only on object direction and object inverse direction
                float forwardMove = _direction.z;
                _direction = new Vector3(1, 1, 1) * forwardMove;
            }
            else
            {
                _controller.SimpleMove(_direction * _speed * Time.deltaTime); 
            }
        }

        if (!_isPushingPulling)
        {
            Look();
        }


        if (IsGrabbing())
        {
            _objectGrabbed.transform.position = transform.position + (Vector3.forward * 2);
        }
    }


    // Grabbed
    public bool IsGrabbing(){ return _isGrabbing; }
    public void SetGrabbing(bool isGrabbing){ _isGrabbing = isGrabbing; }
    public GameObject GetObjectGrabbed(){ return _objectGrabbed; }
    public void SetObjectGrabbed(GameObject objectGrabbed){ _objectGrabbed = objectGrabbed; }
}
