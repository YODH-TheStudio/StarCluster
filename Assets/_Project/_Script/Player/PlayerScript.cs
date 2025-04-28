using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerScript : MonoBehaviour
{
    #region Fields
    private static readonly int Moving = Animator.StringToHash("IsMoving");


    private SoundSystem _soundSystem;
    
    // Player Controller Variable 
    [SerializeField] private float speed = 7.0f;
    [SerializeField] private float pushSpeed = 3.0f;
    [SerializeField] private float turnSpeed = 360.0f;

    [SerializeField] private float raycastDistance = 1.25f;

    [SerializeField] private LayerMask ground;
    
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

    private Vector3[] _checkDirections;

    private Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, -45.0f, 0));

    private Vector3 _initialPosition;
    
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
        _direction = newDirection;
    }

    #endregion

    #region Main Functions
    private void Awake()
    {
        _soundSystem = GameManager.Instance.GetSoundSystem();
        
        _rigidbody = GetComponent<Rigidbody>();
        Controller playerControls = new Controller();
        
        _playerInteractionZone = GetComponent<PlayerInteractionZone>();


        _footstepSfxKeysGround = new string[] 
        { 
        "Chemin Pas A", "Chemin Pas B", "Chemin Pas C", "Chemin Pas D",
        "Chemin Pas E","Chemin Pas F","Chemin Pas G",
        "Chemin Pas H","Chemin Pas I","Chemin Pas J"
        };
        
        _footstepSfxKeysGrass = new string[] 
        { 
            "Feuillage Fpas A", "Feuillage Fpas B", "Feuillage Fpas C", "Feuillage Fpas D",
            "Feuillage Fpas E","Feuillage Fpas F","Feuillage Fpas G",
            "Feuillage Fpas H","Feuillage Fpas I","Feuillage Fpas J",
            "Feuillage Fpas K", "Feuillage Fpas L"
        };
        
        _checkDirections = new Vector3[]
        {
            Vector3.forward,
            Vector3.back,
            Vector3.left,
            Vector3.right,
            (Vector3.forward + Vector3.left).normalized,
            (Vector3.forward + Vector3.right).normalized,
            (Vector3.back + Vector3.left).normalized,
            (Vector3.back + Vector3.right).normalized
        };
        
        _initialPosition = transform.position;
    }


    private void FixedUpdate()
    {
        if (transform.position.y < 0)
        {
            transform.position = _initialPosition;
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
        
        if (!IsMoveDirectionSafe(_direction))
        {
            _direction = Vector3.zero;
            playerAnimator.SetBool(Moving, false);
        }
        
        if (!IsGroundedBelowPlayer())
        {
            Vector3 currentVelocity = _rigidbody.velocity;
            _rigidbody.velocity = new Vector3(0f, currentVelocity.y, 0f);
        }

        if (MovementLimit != MovementLimitType.FullRestriction)
        {
            if (MovementLimit == MovementLimitType.ForwardBackwardNoLook)
            {
                playerAnimator.SetBool(Moving, false);

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
                float forwardMove = Vector3.Dot(_direction, closestDirection);
                _direction = closestDirection * forwardMove;

                _rigidbody.MovePosition(_rigidbody.position + _direction * (pushSpeed * Time.deltaTime));
            }
            else
            {
                if (_direction != Vector3.zero)
                {
                    playerAnimator.SetBool(Moving, true);

                }
                else if (_direction == Vector3.zero)
                {
                    playerAnimator.SetBool(Moving, false);
                }

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
    
    public void OnInteract()
    {
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
    
    #region Ground Detection
    private bool IsGroundInDirection(Vector3 worldDir)
    {
        Vector3 origin = transform.position + Vector3.down * 0.5f + worldDir * 1f;
        return Physics.Raycast(origin, Vector3.down, 0.5f, ground);
    }
    
    private bool IsMoveDirectionSafe(Vector3 moveDir)
    {
        // if (moveDir == Vector3.zero) return true;
        //
        // Vector3 moveDirIso = _isoMatrix.MultiplyPoint3x4(moveDir.normalized);
        //
        // foreach (var dir in _checkDirections)
        // {
        //     Vector3 dirIso = _isoMatrix.MultiplyPoint3x4(dir);
        //     if (Vector3.Dot(dirIso, moveDirIso) > 0.9f)
        //     {
        //         if (!IsGroundInDirection(dirIso))
        //             return false;
        //     }
        // }
        // return true;

        RaycastHit hit;
        if(Physics.Linecast(transform.position, new Vector3(transform.position.x + moveDir.x, transform.position.y - 1.3f, transform.position.z + moveDir.z), out hit, ground))
        {
            Debug.DrawLine(transform.position, new Vector3(transform.position.x + moveDir.x, transform.position.y - 1.3f, transform.position.z + moveDir.z), Color.green);
            return true;
        }
        Debug.DrawLine(transform.position, new Vector3(transform.position.x + moveDir.x, transform.position.y - 1.3f, transform.position.z + moveDir.z), Color.red);
        return false;
    }
    
    
    private void OnDrawGizmos()
    {
        if (_checkDirections == null) return;

        Gizmos.color = Color.yellow;

        foreach (var dir in _checkDirections)
        {
            Vector3 worldDir = _isoMatrix.MultiplyPoint3x4(dir);
            Vector3 origin = transform.position + Vector3.down * 0.5f + worldDir * 1f;
            Gizmos.DrawRay(origin, Vector3.down *0.5f);
            Gizmos.DrawSphere(origin, 0.05f);
        }
    }
    
    private bool IsGroundedBelowPlayer()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        return Physics.Raycast(origin, Vector3.down, 0.6f, ground);
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
        playerAnimator.SetBool(Moving, true);
        float elapsedTime = 0f;
        Vector3 initialPosition = transform.position;

        MovementLimit = MovementLimitType.FullRestriction;
        
        
        Vector3 target = new Vector3(targetPosition.x, initialPosition.y, targetPosition.z);
        Vector3 initial = new Vector3(initialPosition.x, initialPosition.y, initialPosition.z);
        Vector3 direction = target - initial;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            Vector3 newPosition = Vector3.Lerp(initial, target, t);
            newPosition.y = transform.position.y;

            _rigidbody.MovePosition(newPosition); // Use Rigidbody to move the player

            LookAt(transform.position + direction); // Look at the target position
            
            yield return null; // Wait until the next frame
        }

        _rigidbody.MovePosition(targetPosition); // Ensure the final position is set
        MovementLimit = MovementLimitType.None;
        playerAnimator.SetBool(Moving, false);
    }
    #endregion

    #region Getters
    
    public Animator GetAnimator()
    {
        return playerAnimator;
    } 
    
    public bool IsMoving()
    {
        return _direction != Vector3.zero;
    }

    #endregion
}
