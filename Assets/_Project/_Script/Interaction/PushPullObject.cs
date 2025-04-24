using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PushPullObject : Interactable
{
    #region Fields
    private SoundSystem _soundSystem;
    
    private bool _isOnPedestal;
    private bool _isGrab;
    private Vector3 _pushDirection;

    private const float GrabOffset = 2f;

    private List<Vector3> _offsetPosition;

    private Transform _stoneOriginalParent;
    private Rigidbody _rigidbody;

    private VibrationManager _vibrationManager = null;

    private float _soundCooldown;
    
    private PlayerScript _playerScript;
    
    [SerializeField] MeshRenderer _meshRenderer;
    
    [SerializeField] private Material _glowMaterial;

    private static readonly int Pushing = Animator.StringToHash("IsPushing");

    private static readonly int Pulling = Animator.StringToHash("IsPulling");

    private static readonly int IsPushPull = Animator.StringToHash("IsPushPull");
    
    private AudioSource _pushAudioSource;
    [SerializeField] private AudioClip pushClip;

    private bool _isAudioPlaying;

    #endregion

    #region Main Functions

    private void Awake()
    {
        _soundSystem = GameManager.Instance.GetSoundSystem();
    }

    void Start()
    {
        _offsetPosition = new List<Vector3>();
        _offsetPosition.Add(new Vector3(0, 0, GrabOffset));
        _offsetPosition.Add(new Vector3(0, 0, -GrabOffset));
        _offsetPosition.Add(new Vector3(GrabOffset, 0, 0));
        _offsetPosition.Add(new Vector3(-GrabOffset, 0, 0));
        _playerScript = GameManager.Instance.GetPlayer();

        _pushAudioSource = gameObject.AddComponent<AudioSource>();
        _pushAudioSource.clip = pushClip;
        _pushAudioSource.loop = true;
        _pushAudioSource.playOnAwake = false;

        _pushAudioSource.outputAudioMixerGroup = _soundSystem.GetSFXMixerGroup();

        _vibrationManager = FindObjectOfType<VibrationManager>();

    }
    private void FixedUpdate()
    {
        if (!_isGrab) return;
        
        _soundCooldown -= Time.deltaTime;

        Vector3 playerMoveDirection = _playerScript.GetLastMoveDirection();
        Vector3 direction = transform.position - UserTransform.position;
        direction.Normalize();
        float dot = Vector3.Dot(playerMoveDirection, direction);
        
        if (dot > 0.5f)  // push
        {
            _pushDirection = direction;
        
            _playerScript.GetAnimator().SetBool(Pushing, true);
            _playerScript.GetAnimator().SetBool(Pulling, false);
            
            MovePlayerAndObject(_pushDirection);
            
        }
        else if (dot < -0.5f)  // pull
        {
            _pushDirection = -direction;
            
            _playerScript.GetAnimator().SetBool(Pulling, true);
            _playerScript.GetAnimator().SetBool(Pushing, false);
        
            MovePlayerAndObject(_pushDirection);
            
        }
        else
        {
            _playerScript.SetMoveDirection(Vector3.zero);
            _playerScript.GetAnimator().SetBool(Pushing, false);
            _playerScript.GetAnimator().SetBool(Pulling, false);
            UserTransform.GetComponent<PlayerScript>().SetMoveDirection(Vector3.zero);

            if (_pushAudioSource.isPlaying)
            {
                _pushAudioSource.Stop();
                _isAudioPlaying = false;
            }
        }
    }

    #endregion

    #region Interact
    public override void Interact()
    {
        base.Interact();
        TogglePushPull();
    }
    #endregion

    #region Push/Pull
    private void TogglePushPull()
    {
        _isGrab = !_isGrab;
        
        Vector3 playerTransformPosition = UserTransform.gameObject.transform.position;
        
        if (!_isGrab)
        {
            _playerScript.MovementLimit = PlayerScript.MovementLimitType.None;
            DetachObjectFromPlayer();
        }
        else
        {
            // Find nearest position
            Vector3 closestPosition = GetClosestPosition(playerTransformPosition);
            
            StartCoroutine(MoveAnimation(closestPosition));
        }
    }

    private bool PossibleToGrab(Vector3 destinationPosition)
    {
        // Shoot a raycast to check if the object is in the way
        RaycastHit hit;
        if (!Physics.Raycast(UserTransform.position, destinationPosition - UserTransform.position, out hit, GrabOffset)) return true;
        
        return hit.collider.gameObject == null;
    }
    
    public void SetIsOnPedestal(bool isOnPedestal)
    {
        _isOnPedestal = isOnPedestal;
        
        GlowSymbol();
        
        _soundSystem.PlaySoundFXClipByKey("Rock Socle", transform.position);
        
        if (_isGrab)
        {
            _playerScript.MovementLimit = PlayerScript.MovementLimitType.None;
            DetachObjectFromPlayer();
        }

        _isInteractable = false;
    }
    private Vector3 GetClosestPosition(Vector3 playerPosition)
    {
        Vector3 closestPosition = _offsetPosition[0] + transform.position;
        closestPosition.y = playerPosition.y;
        float closestDistance = Vector3.Distance(playerPosition, closestPosition);
        
        foreach (var offset in _offsetPosition)
        {
            Vector3 offsetPosition = offset + transform.position;
            offsetPosition.y = playerPosition.y;
            float distance = Vector3.Distance(playerPosition, offsetPosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPosition = offsetPosition;
            }
        }

        return closestPosition;
    }
    #endregion

    #region Glow
    private void GlowSymbol()
    {
        if (_isOnPedestal)
        {
            if (_glowMaterial && _meshRenderer)
            {
                Material[] materials = _meshRenderer.materials;
                materials[0] = _glowMaterial;
                _meshRenderer.materials = materials;
                StartCoroutine(AtlassiumAnimation(3));
            }
        }
    }
    
    public void DeactivateAtlassium()
    {
        if (_meshRenderer)
        {
            Material[] materials = _meshRenderer.materials;
            materials = new Material[1] { materials[0] };
            _meshRenderer.materials = materials;
        }
    }

    public void SetAtlassiumAlpha(float alpha)
    {
        if (_meshRenderer)
        {
            Material[] materials = _meshRenderer.materials;
            materials[1].SetFloat("_Alpha", alpha);
            _meshRenderer.materials = materials;
        }
    }
    
    #endregion

    #region Attach/Detach
    private void AttachObjectToPlayer()
    {
        _stoneOriginalParent = transform.parent;
        Vector3 originalWorldPosition = transform.position;

        transform.SetParent(UserTransform);
        transform.position = originalWorldPosition;
        
        _playerScript.GetAnimator().SetBool(IsPushPull, true);
        
        GameManager.Instance.GetPlayer().FreezeRotation();
    }

    private void DetachObjectFromPlayer()
    {
        transform.SetParent(_stoneOriginalParent);
        
        _playerScript.GetAnimator().SetBool(IsPushPull, false);
        _playerScript.GetAnimator().SetBool(Pulling, false);
        _playerScript.GetAnimator().SetBool(Pushing, false);
        
        GameManager.Instance.GetPlayer().UnfreezeRotation();
        UserTransform.GetComponent<PlayerScript>().UnfreezeRotation();

        if (_isAudioPlaying)
        {
            _pushAudioSource.Stop();
            _isAudioPlaying = false;
        }

    }

    #endregion

    #region Movement
    private void MovePlayerAndObject(Vector3 direction)
    {
        _playerScript.SetMoveDirection(direction);


        if (!_pushAudioSource.isPlaying)
        {
            _pushAudioSource.Play();
            _isAudioPlaying = true;
        }

        if (_vibrationManager != null)
        {
            _vibrationManager.Vibrate(0.3f, 0.1f); 
        }

    }
    #endregion

    #region Coroutines
    private IEnumerator MoveAnimation(Vector3 start)
    {
        // Distance between player and start
        float distance = Vector3.Distance(UserTransform.position, start);
        yield return StartCoroutine(_playerScript.MoveTo(start, distance / 2f));
        
        // Rotate the player to face the object instantly
        Vector3 direction = transform.position - UserTransform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        targetRotation.z = 0;
        targetRotation.x = 0;
        UserTransform.rotation = targetRotation;
        
        _playerScript.MovementLimit = PlayerScript.MovementLimitType.ForwardBackwardNoLook;
        AttachObjectToPlayer();
    }
    
    private IEnumerator AtlassiumAnimation(float duration)
    {
        float elapsedTime = 0f;
        Material[] materials = _meshRenderer.materials;
        _meshRenderer.materials = materials;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            float colorValue = Mathf.Lerp(0f, 1f, t);

            materials[1].SetFloat("_ColorSlider", colorValue);
            _meshRenderer.materials = materials;

            yield return null; // Wait for the next frame
        }

        // Ensure the final value is set
        materials[1].SetFloat("_ColorSlider", 1f);
        _meshRenderer.materials = materials;
    }
    
    #endregion
}
