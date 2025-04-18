using UnityEngine;

public class PushPullObject : Interactable
{
    private bool _isActive = false;
    private Vector3 _pushDirection;
    [SerializeField] float _pushForce = 3f;

    private Transform _stoneOriginalParent;
    private Rigidbody _rigidbody;

    private float _soundCooldown = 0f;

    // Utilisation d'une variable pour savoir si l'objet est en collision
    private bool _isColliding = false;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        // Initialement, la pierre est entièrement figée (pas de mouvement ni de rotation)
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
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

        if (_isActive)
        {
            playerScriptComponent.MovementLimit = PlayerScript.MovementLimitType.ForwardBackwardNoLook;
            AttachObjectToPlayer();
        }
        else
        {
            playerScriptComponent.MovementLimit = PlayerScript.MovementLimitType.None;
            DetachObjectFromPlayer();
        }
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
            _soundCooldown -= Time.deltaTime;

            Vector3 playerMoveDirection = _userTransform.GetComponent<PlayerScript>().GetLastMoveDirection();
            float dot = Vector3.Dot(playerMoveDirection, _userInteractionNormal);

            if (dot > 0.5f)  // push
            {
                _pushDirection = _userInteractionNormal;
                MovePlayerAndObject(_pushDirection);
            }
            else if (dot < -0.5f)  // pull
            {
                _pushDirection = -_userInteractionNormal;
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

        if (_soundCooldown <= 0f)
        {
            GameManager.Instance._soundSystem.PlaySoundFXClipByKey("Rock Slide", transform.position);
            _soundCooldown = 1f; // joue un son toutes les 0.4 sec
        }

        
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
