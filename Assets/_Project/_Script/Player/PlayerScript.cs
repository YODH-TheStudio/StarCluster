using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static UnityEngine.ParticleSystem;
using System.Collections;

public class PlayerScript : MonoBehaviour, Controller.IPlayerActions
{
    // Player Controler Variable 
    [SerializeField]
    private float _speed = 250.0f;
    [SerializeField]
    private float _turnSpeed = 360.0f;

    [SerializeField]
    private float _raycastDistance = 1.25f;

    [SerializeField]
    private Animator _playerAnimator;

    [SerializeField]
    private Vector3 _particleStartPos;

    private GameObject _particle;

    // debug pour le temps que on a pas de detecteur de sol
    [SerializeField]
    private bool _isOnGrass;

    private CharacterController _controller;
    private Vector3 _direction;

    private GameObject _objectGrabbed;

    // Interact
    private GameObject _currentInteractObject;
    
    private PlayerInteractionZone _playerInteractionZone;

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
        Controller playerControls = new Controller();
        playerControls.Player.SetCallbacks(this);
        _playerInteractionZone = GetComponent<PlayerInteractionZone>();
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

    // public void OnInteract(InputAction.CallbackContext context)
    // {
    //     if (context.started)
    //     {
    //
    //         if (IsGrabbing())
    //         {
    //             SetGrabbing(false);
    //             _objectGrabbed.GetComponent<BoxCollider>().enabled = true;
    //             _objectGrabbed.GetComponent<Rigidbody>().useGravity = true;
    //
    //             return;
    //         }
    //
    //         else
    //         {
    //             RaycastHit hit;
    //             // Does the ray intersect any objects excluding the player layer
    //             if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, _raycastDistance))
    //             {
    //                 GameManager.Instance._soundSystem.PlaySoundFXClipByKey("Violon");
    //                 GameManager.Instance._soundSystem.ChangeMusicByKey("Doce"); 
    //                 //SoundManager.PlaySound(SoundType.None,1);
    //             }
    //
    //             return;
    //         }
    //     }
    // }
    //
    // Allow the player to interact with objetc, Is called by the character controler component in player
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // // Interact with object
            // RaycastHit hit;
            // if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, _raycastDistance))
            //
            // {
            //     // Is interactable object
            //     Interactable interactable = hit.transform.GetComponent<Interactable>();
            //     if (interactable != null)
            //     {
            //         interactable.SetUserTransform(this.transform);
            //         interactable.SetUserInteractionNormal(hit.normal);
            //         interactable.Interact();
            //     }
            // }
            
            Interactable interactable = _playerInteractionZone.GetCurrentInteractable();
            if (interactable != null)
            {
                interactable.SetUserTransform(this.transform);
                interactable.SetUserInteractionNormal(_playerInteractionZone.GetRaycastHit().normal);
                interactable.Interact();
            }
        }
    }

    // Rotate the player in the direction he is walking
    // private void Look()
    // {
    //     // Calculated the movement of the player with the 45� change due to isometric view
    //     Vector3 position = new Vector3(readVector.x, 0, readVector.y);
    //     Matrix4x4 isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45.0f, 0));
    //
    //
    //      _direction = isoMatrix.MultiplyPoint3x4(position);
    //
    //     // store last move position to inform component about user movement direction
    //
    //     _lastMoveDirection = _direction.normalized; 
    // }
    
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
        
        if (_direction != Vector3.zero)
        {
            _playerAnimator.SetBool("IsMoving", true);
        }
        else if (_direction == Vector3.zero)
        {
            _playerAnimator.SetBool("IsMoving", false);
        }
        
        if(_particle != null)
        {
            if (_direction != Vector3.zero)
            {
                _particle.GetComponent<ParticleSystem>().enableEmission = true;
            }
            else if (_direction == Vector3.zero)
            {
                _particle.GetComponent<ParticleSystem>().enableEmission = false;
            }
        }
        
        if (MovementLimit != MovementLimitType.FullRestriction)
        {
            
            if (MovementLimit == MovementLimitType.ForwardBackwardNoLook)
            {
                
                Vector3 northDirection = Vector3.forward;
                Vector3 southDirection = Vector3.back;
                Vector3 eastDirection = Vector3.right;
                Vector3 westDirection = Vector3.left;

                Vector3 currentForward = transform.forward;

                float dotNorth = Vector3.Dot(currentForward, northDirection);
                float dotSouth = Vector3.Dot(currentForward, southDirection);
                float dotEast = Vector3.Dot(currentForward, eastDirection);
                float dotWest = Vector3.Dot(currentForward, westDirection);

                Vector3 closestDirection = Vector3.zero;
                float maxDot = Mathf.Max(dotNorth, dotSouth, dotEast, dotWest);

                if (maxDot == dotNorth)
                {
                    closestDirection = northDirection;
                }
                else if (maxDot == dotSouth)
                {
                    closestDirection = southDirection;
                }
                else if (maxDot == dotEast)
                {
                    closestDirection = eastDirection;
                }
                else if (maxDot == dotWest)
                {
                    closestDirection = westDirection;
                }
                
                // Limit movement only on object direction and object inverse direction
                float forwardMove = Vector3.Dot(_limitedMoveDirection, closestDirection);  // Calculer la composante "forward"
                _limitedMoveDirection = closestDirection * forwardMove;  // Appliquer la direction en avant uniquement

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

        //     _controller.SimpleMove(_direction * _speed * Time.deltaTime);
        // Look();

    }

    public GameObject GetObjectGrabbed()
    {
        return _objectGrabbed;
    }

    public void SetObjectGrabbed(GameObject objectGrabbed)
    {
        _objectGrabbed = objectGrabbed;
    }

    public bool IsMoving()
    {
        return _direction != Vector3.zero;
    }

    public void SpawnParticle(GameObject particle)
    {
        if (_particle != null)
        {
            _particle.GetComponent<ParticleSystem>().enableEmission = false;
        }
        Destroy(_particle, 2);

        Quaternion _particleAngle = gameObject.transform.rotation * Quaternion.AngleAxis(180, new Vector3(0, 1, 0));

        _particle = Instantiate(particle, Vector3.zero, _particleAngle);
        _particle.GetComponent<ParticleSystem>().enableEmission = true;
        _particle.transform.SetParent(gameObject.transform);
        _particle.transform.localPosition = _particleStartPos;
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
