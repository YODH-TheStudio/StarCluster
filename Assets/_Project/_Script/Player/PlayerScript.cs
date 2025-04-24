using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static UnityEngine.ParticleSystem;
using System.Collections;
using UnityEditor.Localization.Plugins.XLIFF.V12;

public class PlayerScript : MonoBehaviour, Controller.IPlayerActions
{
    // Player Controler Variable 
    [SerializeField]
    private float _speed = 250.0f;
    [SerializeField]
    private float _turnSpeed = 360.0f;

    [SerializeField]
    private float _raycastDistance = 1.25f;

    private float _footstepTimer = 0f;
    private float _footstepInterval = 0.4f;

    [SerializeField]
    private Animator _playerAnimator;

    [SerializeField]
    private Vector3 _particleStartPos;

    private GameObject _particle;

    // debug pour le temps que on a pas de detecteur de sol
    [SerializeField]
    public bool _isOnGrass;

    private CharacterController _controller;
    private Vector3 _direction;

    private bool _isGrabbing = false;
    private GameObject _objectGrabbed;

    // Interact
    private GameObject _currentInteractObject;


    private string[] _footstepSFXKeys_ground;
    private string[] _footstepSFXKeys_Grass;


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


        _footstepSFXKeys_ground = new string[] 
        { 
        "Chemin Pasa", "Chemin Pasb", "Chemin Pasc", "Chemin Pasd",
        "Chemin Pase","Chemin Pasf","Chemin Pasg",
        "Chemin Pash","Chemin Pasi","Chemin Pasj"
        };
        Debug.Log(_footstepSFXKeys_ground);
        
        _footstepSFXKeys_Grass = new string[] 
        { 
            "Feuillage Fpasa", "Feuillage Fpasb", "Feuillage Fpasc", "Feuillage Fpasd",
            "Feuillage Fpase","Feuillage Fpasf","Feuillage Fpasg",
            "Feuillage Fpash","Feuillage Fpasi","Feuillage Fpasj",
            "Feuillage Fpask", "Feuillage Fpasl"
        };
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

    #region  OnInteract

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

    #endregion
   
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

            _footstepTimer -= Time.deltaTime;
            if(_footstepTimer < 0f)
            {
                PlayFootstepSound();
                _footstepTimer = _footstepInterval; 
            }
                
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

        _isOnGrass = onGrass;

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

    public void Teleport(float x, float y, float z)
    {
        Debug.Log("Teleporting to: " + x + ", " + y + ", " + z);
        Vector3 newPosition = new Vector3(x, y, z);
        _controller.enabled = false;
        transform.position = newPosition;
        _controller.enabled = true;
    }

    public void PlayFootstepSound() 
    {
        string[] footstepBank = _isOnGrass? _footstepSFXKeys_Grass : _footstepSFXKeys_ground; 
        
        // Play the footstep sound
        if (footstepBank.Length > 0)
        {
            Vector3 spawnPosition = transform.position;
            Debug.Log(footstepBank); 

            GameManager.Instance._soundSystem.PlayRandomSoundFXClipByKeys(footstepBank, spawnPosition);
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

    public void DeleteParticle(GameObject particle)
    {
        if (_particle == null)
        {
            return;
        }
        Destroy(_particle);
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
