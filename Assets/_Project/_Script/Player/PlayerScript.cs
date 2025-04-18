using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerScript : MonoBehaviour, Controller.IPlayerActions
{
    #region Fields
    private static readonly int Moving = Animator.StringToHash("IsMoving");


    private SoundSystem _soundSystem;
    
    // Player Controller Variable 
    [SerializeField] private float speed = 250.0f;
    [SerializeField] private float turnSpeed = 360.0f;

    [SerializeField] private float raycastDistance = 1.25f;

    private float _footstepTimer = 0f;
    private float _footstepInterval = 0.4f;

    [SerializeField] private Animator playerAnimator;

    [SerializeField] private Vector3 particleStartPos;

    private GameObject _particle;

    // While we dont have ground detection
    [SerializeField] public bool isOnGrass;

    // private CharacterController _controller;
    private Rigidbody _rigidbody;
    private Vector3 _direction;

    private GameObject _objectGrabbed;

    // Interact

    private PlayerInteractionZone _playerInteractionZone;


    private string[] _footstepSfxKeysGround;
    private string[] _footstepSfxKeysGrass;

    #endregion

    #region Classes 
    
    // Limited Movement
    public enum MovementLimitType
    {
        None,             // No restrict
        ForwardBackwardNoLook,  // Only forward backward move / No look possible
        FullRestriction   //
    }

    public MovementLimitType MovementLimit { get; set; } = MovementLimitType.None;

    public GameObject CurrentInteractObject { get; set; }

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

    #endregion

    #region Main Functions
    private void Awake()
    {
        _soundSystem = GameManager.Instance.GetSoundSystem();
        
        _rigidbody = GetComponent<Rigidbody>();
        Controller playerControls = new Controller();
        
        playerControls.Player.SetCallbacks(this);
        _playerInteractionZone = GetComponent<PlayerInteractionZone>();


        _footstepSfxKeysGround = new string[] 
        { 
        "Chemin Pasa", "Chemin Pasb", "Chemin Pasc", "Chemin Pasd",
        "Chemin Pase","Chemin Pasf","Chemin Pasg",
        "Chemin Pash","Chemin Pasi","Chemin Pasj"
        };
        Debug.Log(_footstepSfxKeysGround);
        
        _footstepSfxKeysGrass = new string[] 
        { 
            "Feuillage Fpasa", "Feuillage Fpasb", "Feuillage Fpasc", "Feuillage Fpasd",
            "Feuillage Fpase","Feuillage Fpasf","Feuillage Fpasg",
            "Feuillage Fpash","Feuillage Fpasi","Feuillage Fpasj",
            "Feuillage Fpask", "Feuillage Fpasl"
        };
    }


    private void FixedUpdate()
    {

        if (_direction != Vector3.zero)
        {
            playerAnimator.SetBool(Moving, true);

        }
        else if (_direction == Vector3.zero)
        {
            playerAnimator.SetBool(Moving, false);
        }

        if (_particle != null)
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

        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.1f); // check juste sous les pieds
        bool onGrass = false;

        foreach (var col in colliders)
        {
            GroundScript ground = col.GetComponent<GroundScript>();
            if (ground != null && ground.IsOnGrass)
            {
                onGrass = true;
                break;
            }
        }

        isOnGrass = onGrass;

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

                if (Mathf.Approximately(maxDot, dotNorth))
                {
                    closestDirection = northDirection;
                }
                else if (Mathf.Approximately(maxDot, dotSouth))
                {
                    closestDirection = southDirection;
                }
                else if (Mathf.Approximately(maxDot, dotEast))
                {
                    closestDirection = eastDirection;
                }
                else if (Mathf.Approximately(maxDot, dotWest))
                {
                    closestDirection = westDirection;
                }

                // Limit movement only on object direction and object inverse direction
                float forwardMove = Vector3.Dot(_limitedMoveDirection, closestDirection);
                _limitedMoveDirection = closestDirection * forwardMove;

                _rigidbody.MovePosition(_rigidbody.position + _limitedMoveDirection * (speed * Time.deltaTime));
            }
            else
            {
                _rigidbody.MovePosition(_rigidbody.position + _direction * (speed * Time.deltaTime));
            }

            if (MovementLimit != MovementLimitType.ForwardBackwardNoLook)
            {
                Look();
            }
        }

    }

    #endregion

    #region Input System
    // Allow the player to move, Is called by the character controler component in player
    public void OnMove(Vector2 readVector)
    {
        // Calculated the movement of the player with the 45° change due to isometric view
        Vector3 position = new Vector3(readVector.x, 0, readVector.y);
        Matrix4x4 isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, -45.0f, 0));


        _direction = isoMatrix.MultiplyPoint3x4(position);

        // store last move position to inform component about user movement direction

        _lastMoveDirection = _direction.normalized; 
    }
    
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        
        Interactable interactable = _playerInteractionZone.GetCurrentInteractable();
        
        if (interactable == null) return;
        
        interactable.SetUserTransform(this.transform);
        interactable.Interact();
    

    }

    #endregion

    #region Look
    // Rotate the player in the direction he is walking
    private void Look()
    {
        // Don't go back to the starting rotation when the player don't move
        if (_direction == Vector3.zero) return;
        
        // Calculated the rotation and set it
        var relative = (transform.position + _direction) - transform.position;
        var rot = Quaternion.LookRotation(relative, Vector3.up);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, turnSpeed * Time.deltaTime);
    }
    
    private void LookAt(Vector3 target)
    {
        // Calculated the rotation and set it
        var relative = (target) - transform.position;
        var rot = Quaternion.LookRotation(relative, Vector3.up);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, turnSpeed * Time.deltaTime);
    }
    #endregion

    #region Teleport
    public void Teleport(float x, float y, float z)
    {
        Vector3 newPosition = new Vector3(x, y, z);
        transform.position = newPosition;
    }
    #endregion

    #region SfxPlay
    public void PlayFootstepSound() 
    {
        string[] footstepBank = isOnGrass? _footstepSfxKeysGrass : _footstepSfxKeysGround; 
        
        // Play the footstep sound
        if (footstepBank.Length <= 0) return;
        
        Vector3 spawnPosition = transform.position;
        Debug.Log(footstepBank); 

        _soundSystem.PlayRandomSoundFXClipByKeys(footstepBank, spawnPosition);
    }
    #endregion

    #region Rotation
    public void FreezeRotation()
    {
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }
    
    public void UnfreezeRotation()
    {
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    #endregion

    #region Grab Object
    public GameObject GetObjectGrabbed()
    {
        return _objectGrabbed;
    }

    public void SetObjectGrabbed(GameObject objectGrabbed)
    {
        _objectGrabbed = objectGrabbed;
    }

    #endregion

    #region Animation
    public bool IsMoving()
    {
        return _direction != Vector3.zero;
    }
    #endregion

    #region Particle
    public void SpawnParticle(GameObject particle)
    {
        if (_particle != null)
        {
            _particle.GetComponent<ParticleSystem>().enableEmission = false;
        }
        Destroy(_particle, 2);

        Quaternion particleAngle = gameObject.transform.rotation * Quaternion.AngleAxis(180, new Vector3(0, 1, 0));

        _particle = Instantiate(particle, Vector3.zero, particleAngle);
        _particle.GetComponent<ParticleSystem>().enableEmission = true;
        _particle.transform.SetParent(gameObject.transform);
        _particle.transform.localPosition = particleStartPos;
    }

    public void DeleteParticle(GameObject particle)
    {
        if (_particle == null)
        {
            return;
        }
        Destroy(_particle);
    }

    #endregion

    #region Coroutine
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
            Vector3 newPosition = Vector3.Lerp(initialPosition, targetPosition, t);
            newPosition.y = transform.position.y;

            _rigidbody.MovePosition(newPosition); // Use Rigidbody to move the player

            //LookAt(targetPosition); // Look at the target position
            
            //Vector3 direction = (targetPosition - transform.position).normalized;
            
            //Debug.DrawRay(transform.position, direction * 2, Color.red); // Debug ray to visualize the direction

            yield return null; // Wait until the next frame
        }

        _rigidbody.MovePosition(targetPosition); // Ensure the final position is set
        MovementLimit = MovementLimitType.None;
    }
    #endregion
}
