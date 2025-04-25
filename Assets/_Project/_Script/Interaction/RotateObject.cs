using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class RotateObject : Interactable
{
    #region Fields
    private SoundSystem _soundSystem;
    
    private bool _isOnPedestal;
    private bool _isGrab;
    private Vector3 _pushDirection;

    private const float GrabOffset = 2f;

    [SerializeField] private Transform _grabTransform;

    [SerializeField] private Transform _lookTransform;

    [SerializeField] private float _maxRotation;

    [SerializeField] private float _rotationDuration = 3f;

    private Transform _playerOriginalParent;
    private Rigidbody _rigidbody;

    private float _soundCooldown;
    
    private static readonly int Pushing = Animator.StringToHash("IsPushing");
    private static readonly int PushPull = Animator.StringToHash("IsPushPull");

    #endregion

    #region Main Functions

    private void Awake()
    {
        _soundSystem = GameManager.Instance.GetSoundSystem();
    }

    #endregion

    #region Interact
    public override void Interact()
    {
        base.Interact();
        ToggleGrab();
    }
    #endregion

    #region Push/Pull
    private void ToggleGrab()
    {
        _isGrab = !_isGrab;
        
        if (_isGrab)
        {
            StartCoroutine(Animation(_grabTransform.position));
        }
    }
    #endregion

    #region Attach/Detach
    private void AttachPlayerToObject()
    {
        _playerOriginalParent = UserTransform.parent;
        Vector3 originalWorldPosition = UserTransform.position;

        UserTransform.SetParent(transform);
        UserTransform.position = originalWorldPosition;
        
        UserTransform.GetComponent<PlayerScript>().FreezeRotation();
        
        GameManager.Instance.GetPlayer().GetAnimator().SetBool(Pushing, true);
        GameManager.Instance.GetPlayer().GetAnimator().SetBool(PushPull, true);
    }

    private void DetachObjectFromPlayer()
    {
        UserTransform.SetParent(_playerOriginalParent);
        
        UserTransform.GetComponent<PlayerScript>().UnfreezeRotation();
        
        PlayerScript playerScriptComponent = UserTransform.GetComponent<PlayerScript>();
        playerScriptComponent.MovementLimit = PlayerScript.MovementLimitType.None;
        
        GameManager.Instance.GetPlayer().GetAnimator().SetBool(Pushing, false);
        GameManager.Instance.GetPlayer().GetAnimator().SetBool(PushPull, false);
    }

    #endregion
    
    #region Coroutines

    private IEnumerator Animation(Vector3 start)
    {
        yield return StartCoroutine(MoveAnimation(start));
        AttachPlayerToObject();
        yield return StartCoroutine(RotateAnimation());
        DetachObjectFromPlayer();
        
        _isInteractable = false;
        yield return null;
    }
    private IEnumerator MoveAnimation(Vector3 start)
    {
        PlayerScript playerScript = GameManager.Instance.GetPlayer();

        // Distance between player and start
        float distance = Vector3.Distance(UserTransform.position, start);
        yield return StartCoroutine(playerScript.MoveTo(start, distance / 2f));
        
        // Rotate the player to face the object instantly
        Vector3 direction = _lookTransform.position - UserTransform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        targetRotation.z = 0;
        targetRotation.x = 0;
        UserTransform.rotation = targetRotation;
        
        playerScript.MovementLimit = PlayerScript.MovementLimitType.FullRestriction;
    }

    private IEnumerator RotateAnimation()
    {
        float time = 0;
        Vector3 currentRotation = transform.localRotation.eulerAngles;
        float startYRotation = currentRotation.y;

        
        while (time < _rotationDuration)
        {
            time += Time.deltaTime;
            float t = time / _rotationDuration;

            // Interpolate only the Y-axis rotation
            float currentYRotation = Mathf.Lerp(startYRotation, _maxRotation, t);
            transform.localRotation = Quaternion.Euler(currentRotation.x, currentYRotation, currentRotation.z);

            yield return null;
        }

        // Ensure the final rotation is set
        transform.localRotation = Quaternion.Euler(currentRotation.x, _maxRotation, currentRotation.z);
    }
    #endregion
}
