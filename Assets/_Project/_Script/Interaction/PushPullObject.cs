using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushPullObject : Interactable
{
    private bool _isActive = false;
    private Vector3 _pushDirection;
    [SerializeField] float _pushForce = 3f;

    private float _grabOffset = 1.5f;

    private List<Vector3> _offsetPosition;

    private Transform _stoneOriginalParent;
    private Rigidbody _rigidbody;

    // Utilisation d'une variable pour savoir si l'objet est en collision
    private bool _isColliding = false;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        // Initialement, la pierre est entièrement figée (pas de mouvement ni de rotation)
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
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
        _isActive = !_isActive;
        
        PlayerScript playerScriptComponent = _userTransform.GetComponent<PlayerScript>();
        Vector3 playerTransformPosition = _userTransform.gameObject.transform.position;
        
        if (!_isActive)
        {
            playerScriptComponent.MovementLimit = PlayerScript.MovementLimitType.None;
            DetachObjectFromPlayer();
        }
        else
        {
            Debug.Log("PushPullObject : " + _isActive);
            DetachObjectFromPlayer();
            
            // Trouver la position la plus proche
            Vector3 closestPosition = _offsetPosition[0] + transform.position;
            closestPosition.y = playerTransformPosition.y;
            float closestDistance = Vector3.Distance(playerTransformPosition, closestPosition);
        
            foreach (var offset in _offsetPosition)
            {
                Vector3 offsetPosition = offset + transform.position;
                offsetPosition.y = playerTransformPosition.y;
                float distance = Vector3.Distance(playerTransformPosition, offsetPosition);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPosition = offsetPosition;
                }
            }
        
            // Lancer la coroutine pour la position la plus proche
            StartCoroutine(PhaseAnimation(closestPosition));
        }
    
        // if (_isActive)
        // {
        //     playerScriptComponent.MovementLimit = PlayerScript.MovementLimitType.ForwardBackwardNoLook;
        //     AttachObjectToPlayer();
        // }
        // else
        // {
        //     playerScriptComponent.MovementLimit = PlayerScript.MovementLimitType.None;
        //     DetachObjectFromPlayer();
        // }
    }

    private void AttachObjectToPlayer()
    {
        _stoneOriginalParent = transform.parent;
        Vector3 originalWorldPosition = transform.position;

        transform.SetParent(_userTransform);
        transform.position = originalWorldPosition;

        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    private void DetachObjectFromPlayer()
    {
        transform.SetParent(_stoneOriginalParent);
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll; // La pierre est complètement figée lorsqu'elle est détachée
    }

    private void Update()
    {
        if (_isActive)
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
                if (!_isColliding)  // Empêche la poussée si l'objet est en collision
                {
                    MovePlayerAndObject(_pushDirection);
                }
                else
                {
                    Debug.Log("collision");
                    _userTransform.GetComponent<PlayerScript>().SetMoveDirection(Vector3.zero);
                }
            }
            else
            {
                _userTransform.GetComponent<PlayerScript>().SetMoveDirection(Vector3.zero);
            }
        }
    }

    private void MovePlayerAndObject(Vector3 direction)
    {
        //_userTransform.position += direction * _pushForce * Time.deltaTime;
        _userTransform.GetComponent<PlayerScript>().SetMoveDirection(direction);
    }

    private IEnumerator PhaseAnimation(Vector3 start)
    {
        PlayerScript playerScript = GameManager.Instance.GetPlayer();

        //distance beetween player and start
        float distance = Vector3.Distance(_userTransform.position, start);
        yield return StartCoroutine(playerScript.MoveTo(start, distance));
        
        //rotate the player to face the object instantly
        Vector3 direction = transform.position - _userTransform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        targetRotation.z = 0;
        targetRotation.x = 0;
        _userTransform.rotation = targetRotation;
        
        playerScript.MovementLimit = PlayerScript.MovementLimitType.ForwardBackwardNoLook;
        AttachObjectToPlayer();
    }
    
private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PushPullObject"))
        {
            _isColliding = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("PushPullObject"))
        {
            _isColliding = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("PushPullObject"))
        {
            _isColliding = false;
        }
    }
}
