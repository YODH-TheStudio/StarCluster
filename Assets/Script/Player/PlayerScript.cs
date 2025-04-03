using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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

    // Interact
    private GameObject _currentInteractObject;

    // Limited Movement
    public enum MovementLimitType
    {
        None,             // No restrict
        ForwardBackwardNoLook,  // Only forward backward move / No look possible
        FullRestriction   //
    }
    private MovementLimitType _movementLimit = MovementLimitType.None;
    public MovementLimitType MovementLimit
    {
        get { return _movementLimit; }
        set{ _movementLimit = value; }
    }
    public GameObject CurrentInteractObject
    {
        get => _currentInteractObject;
        set => _currentInteractObject = value;
    }

    private Vector3 _lastMoveDirection; // Get normalized player direction each frame

    private Vector3 _limitedMoveDirection; // Get normalized player direction each frame
    public Vector3 GetLastMoveDirection()
    {
        return _lastMoveDirection;
    }

    public void SetMoveDirection(Vector3 newDirection)
    {
        _limitedMoveDirection = newDirection;
    }

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
        if (MovementLimit != MovementLimitType.FullRestriction)
        {
            if (MovementLimit == MovementLimitType.ForwardBackwardNoLook)
            {
                // Limit movement only on object direction and object inverse direction
                float forwardMove = Vector3.Dot(_limitedMoveDirection, transform.forward);  // Calculer la composante "forward"
                _limitedMoveDirection = transform.forward * forwardMove;  // Appliquer la direction en avant uniquement

                _controller.SimpleMove(_limitedMoveDirection * _speed * Time.deltaTime);
            }
            else
            {
                _controller.SimpleMove(_direction * _speed * Time.deltaTime);
            }

            if (MovementLimit != MovementLimitType.ForwardBackwardNoLook)
            {
                Look();
            }
        }
    }


    // Allow the player to interact with objetc, Is called by the character controler component in player
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // Interact with object
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, _raycastDistance))

            {
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

    // Move the player to the target position in a given duration, with movement restriction
    public IEnumerator MoveTo(Vector3 targetPosition, float duration)
    {
        float elapsedTime = 0f;
        Vector3 initialPosition = transform.position;

        MovementLimit = MovementLimitType.FullRestriction;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            transform.position = Vector3.Lerp(initialPosition, targetPosition, t);

            yield return null; // Attendre jusqu'au prochain frame
        }

        transform.position = targetPosition;
        MovementLimit = MovementLimitType.None;

        Debug.Log("Movement complete!");
    }
}
