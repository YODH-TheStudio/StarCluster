using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushPullObject : Interactable
{
    private bool _isOnPedestal = false;
    private bool _isGrab = false;
    private Vector3 _pushDirection;

    private float _grabOffset = 2f;

    private List<Vector3> _offsetPosition;

    private Transform _stoneOriginalParent;

    void Start()
    {
        _offsetPosition = new List<Vector3>();
        _offsetPosition.Add(new Vector3(0, 0, _grabOffset));
        _offsetPosition.Add(new Vector3(0, 0, -_grabOffset));
        _offsetPosition.Add(new Vector3(_grabOffset, 0, 0));
        _offsetPosition.Add(new Vector3(-_grabOffset, 0, 0));
    }

    public override void Interact()
    {
        base.Interact();
        TogglePushPull();
    }

    private void TogglePushPull()
    {
        _isGrab = !_isGrab;
        
        PlayerScript playerScriptComponent = _userTransform.GetComponent<PlayerScript>();
        Vector3 playerTransformPosition = _userTransform.gameObject.transform.position;
        
        if (!_isGrab)
        {
            playerScriptComponent.MovementLimit = PlayerScript.MovementLimitType.None;
            DetachObjectFromPlayer();
        }
        else
        {
            // Trouver la position la plus proche
            Vector3 closestPosition = GetClosestPosition(playerTransformPosition);
            
            StartCoroutine(PhaseAnimation(closestPosition));

            // if (PossibleToGrab(closestPosition))
            // {
            //     // Lancer la coroutine pour la position la plus proche
            //     StartCoroutine(PhaseAnimation(closestPosition));
            // }
        }
    }

    private bool PossibleToGrab(Vector3 DestinationPosition)
    {
        //Shoot a raycast to check if the object is in the way
        RaycastHit hit;
        if (Physics.Raycast(_userTransform.position, DestinationPosition - _userTransform.position, out hit, _grabOffset))
        {
            if (hit.collider.gameObject != null)
            {
                return false;
            }
        }

        return true;
    }
    
    public void SetIsOnPedestal(bool isOnPedestal)
    {
        _isOnPedestal = isOnPedestal;
        
        GlowSymbol();
    }

    private void GlowSymbol()
    {
        if (_isOnPedestal)
        {
            //Glow the symbol
        }
        else
        {
            //Unglow the symbol
        }
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

    private void AttachObjectToPlayer()
    {
        _stoneOriginalParent = transform.parent;
        Vector3 originalWorldPosition = transform.position;

        transform.SetParent(_userTransform);
        transform.position = originalWorldPosition;
        
        _userTransform.GetComponent<PlayerScript>().FreezeRotation();

        // _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    private void DetachObjectFromPlayer()
    {
        transform.SetParent(_stoneOriginalParent);
        
        _userTransform.GetComponent<PlayerScript>().UnfreezeRotation();

        // _rigidbody.constraints = RigidbodyConstraints.FreezeAll; // La pierre est complètement figée lorsqu'elle est détachée
    }

    private void FixedUpdate()
    {
        if (_isGrab)
        {
            Vector3 playerMoveDirection = _userTransform.GetComponent<PlayerScript>().GetLastMoveDirection();
            Vector3 direction = transform.position - _userTransform.position;
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
                _userTransform.GetComponent<PlayerScript>().SetMoveDirection(Vector3.zero);
            }
        }
    }

    private void MovePlayerAndObject(Vector3 direction)
    {
        _userTransform.GetComponent<PlayerScript>().SetMoveDirection(direction);
        
    }

    private IEnumerator PhaseAnimation(Vector3 start)
    {
        PlayerScript playerScript = GameManager.Instance.GetPlayer();

        //distance beetween player and start
        float distance = Vector3.Distance(_userTransform.position, start);
        yield return StartCoroutine(playerScript.MoveTo(start, distance / 2f));
        
        //rotate the player to face the object instantly
        Vector3 direction = transform.position - _userTransform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        targetRotation.z = 0;
        targetRotation.x = 0;
        _userTransform.rotation = targetRotation;
        
        playerScript.MovementLimit = PlayerScript.MovementLimitType.ForwardBackwardNoLook;
        AttachObjectToPlayer();
    }
}
