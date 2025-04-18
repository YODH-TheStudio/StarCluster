using System;
using System.Collections;
using System.Collections.Generic;
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

    private float _soundCooldown;

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
    }
    private void FixedUpdate()
    {
        if (!_isGrab) return;
        
        _soundCooldown -= Time.deltaTime;

        Vector3 playerMoveDirection = UserTransform.GetComponent<PlayerScript>().GetLastMoveDirection();
        Vector3 direction = transform.position - UserTransform.position;
        direction.Normalize();
        float dot = Vector3.Dot(playerMoveDirection, direction);
        
        if (dot > 0.5f)  // push
        {
            _pushDirection = direction;
            MovePlayerAndObject(_pushDirection);
        }
        else if (dot < -0.5f)  // pull
        {
            _pushDirection = -direction;
            MovePlayerAndObject(_pushDirection);
        }
        else
        {
            UserTransform.GetComponent<PlayerScript>().SetMoveDirection(Vector3.zero);
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
        
        PlayerScript playerScriptComponent = UserTransform.GetComponent<PlayerScript>();
        Vector3 playerTransformPosition = UserTransform.gameObject.transform.position;
        
        if (!_isGrab)
        {
            playerScriptComponent.MovementLimit = PlayerScript.MovementLimitType.None;
            DetachObjectFromPlayer();
        }
        else
        {
            // Find nearest position
            Vector3 closestPosition = GetClosestPosition(playerTransformPosition);
            
            StartCoroutine(PhaseAnimation(closestPosition));
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

    #region ???
    private void GlowSymbol()
    {
        if (_isOnPedestal)
        {
            // Glow the symbol
        }
        else
        {
            // Unglow the symbol
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
        
        UserTransform.GetComponent<PlayerScript>().FreezeRotation();
    }

    private void DetachObjectFromPlayer()
    {
        transform.SetParent(_stoneOriginalParent);
        
        UserTransform.GetComponent<PlayerScript>().UnfreezeRotation();
    }

    #endregion

    #region Movement
    private void MovePlayerAndObject(Vector3 direction)
    {
        UserTransform.GetComponent<PlayerScript>().SetMoveDirection(direction);

        if (_soundCooldown <= 0f)
        {
            _soundSystem.PlaySoundFXClipByKey("Rock Slide", transform.position);
            _soundCooldown = 1f;
        }

        
    }
    #endregion

    #region Coroutines
    private IEnumerator PhaseAnimation(Vector3 start)
    {
        PlayerScript playerScript = GameManager.Instance.GetPlayer();

        // Distance between player and start
        float distance = Vector3.Distance(UserTransform.position, start);
        yield return StartCoroutine(playerScript.MoveTo(start, distance / 2f));
        
        // Rotate the player to face the object instantly
        Vector3 direction = transform.position - UserTransform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        targetRotation.z = 0;
        targetRotation.x = 0;
        UserTransform.rotation = targetRotation;
        
        playerScript.MovementLimit = PlayerScript.MovementLimitType.ForwardBackwardNoLook;
        AttachObjectToPlayer();
    }
    #endregion
}
