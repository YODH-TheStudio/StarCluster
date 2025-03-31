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

    private bool _isGrabbing = false;
    private GameObject _objectGrabbed;

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
    }

    // Allow the player to interact with objetc, Is called by the character controler component in player
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (IsGrabbing())
            {
                SetGrabbing(false);
                _objectGrabbed.GetComponent<BoxCollider>().enabled = true;
                _objectGrabbed.GetComponent<Rigidbody>().useGravity = true;

                return;
            }

            else
            {
                RaycastHit hit;
                // Does the ray intersect any objects excluding the player layer
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, _raycastDistance))

                {
                    //_interactionSwitch.InteractSwitch(this, hit.transform.gameObject);

                    // Is interactable object
                    Interactable interactable = hit.transform.GetComponent<Interactable>();


                    if (interactable != null)
                    {
                        interactable.SetUserTransform(this.transform);
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
            _controller.SimpleMove(_direction * _speed * Time.deltaTime);
        }
        Look();

        if (IsGrabbing())
        {
            _objectGrabbed.transform.position = transform.position + (Vector3.forward * 2);
        }
    }

    // Code to interact with object
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
}
