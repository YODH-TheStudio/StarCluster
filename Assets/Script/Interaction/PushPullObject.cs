using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PushPullObject : Interactable
{
    private bool _isActive = false;
    private Vector3 _pushDirection;
    [SerializeField] float _pushForce = 3f;

    public override void Interact()
    {
        base.Interact();
        TogglePushPull();
    }

    private void TogglePushPull()
    {
        _isActive = !_isActive;
        PlayerScript playerScriptComponent = _userTransform.GetComponent<PlayerScript>();
        playerScriptComponent.MovementLimit = PlayerScript.MovementLimitType.ForwardBackwardNoLook;

        if (_isActive)
        {
            playerScriptComponent.MovementLimit = PlayerScript.MovementLimitType.ForwardBackwardNoLook;
        }
        else
        {

            playerScriptComponent.MovementLimit = PlayerScript.MovementLimitType.None;
        }
    }

    private void Update()
    {
        if (_isActive)
        {

            Vector3 playerMoveDirection = _userTransform.GetComponent<PlayerScript>().GetMoveDirection();
            float dot = Vector3.Dot(playerMoveDirection, _userInteractionNormal);

            if (dot > 0.5f)  // push
            {
                _pushDirection = _userInteractionNormal; 
                MovePlayerAndObject(_pushDirection);
            }
            else if (dot < -0.5f)  // pull
            {
                _pushDirection = -_userInteractionNormal; 
                MovePlayerAndObject(_pushDirection);
            }
        }
    }

    private void MovePlayerAndObject(Vector3 direction)
    {

        transform.position += direction * _pushForce * Time.deltaTime;
        _userTransform.position += direction * _pushForce * Time.deltaTime;
    }
}
