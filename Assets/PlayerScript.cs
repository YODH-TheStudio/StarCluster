using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour, Controler.IPlayerActions
{
    private CharacterController _controller;
    private Vector3 _direction;

    private bool _isGrabbing = false;
    private GameObject _objectGrabbed;

    [SerializeField]
    private float _speed = 250.0f;

    [SerializeField]
    private float _raycastDistance = 1.25f;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        Controler playerControls = new Controler();
        playerControls.Player.SetCallbacks(this);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 readVector = context.ReadValue<Vector2>();
        Vector3 position = new Vector3(readVector.x, 0, readVector.y);
        Matrix4x4 isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45.0f, 0));

        _direction = isoMatrix.MultiplyPoint3x4(position);

        var relative = (transform.position + _direction) - transform.position;
        var rot = Quaternion.LookRotation(relative, Vector3.up);

        transform.rotation = rot;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (_isGrabbing)
        {
            _isGrabbing = false;
            _objectGrabbed.GetComponent<BoxCollider>().enabled = true;
            _objectGrabbed.GetComponent<Rigidbody>().useGravity = true;
        }
        else
        {
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, _raycastDistance))
            {
                _objectGrabbed = hit.transform.gameObject;

                if (_objectGrabbed.tag == "Movable")
                {
                    _isGrabbing = true;
                    _objectGrabbed.GetComponent<BoxCollider>().enabled = false;
                    _objectGrabbed.GetComponent<Rigidbody>().useGravity = false;
                }
                else
                {
                    _isGrabbing = false;
                }
            }
            else
            {
                _isGrabbing = false;
            }
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _controller.SimpleMove(_direction * _speed * Time.deltaTime);

        if (_isGrabbing)
        {
            _objectGrabbed.transform.position = transform.position + (Vector3.forward * 2);
        }
    }
}
