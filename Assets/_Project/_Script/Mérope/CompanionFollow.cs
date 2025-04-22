using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CompanionFollow : MonoBehaviour
{
    #region Fields
    private PlayerScript _player;
    private CompanionAnchor _companionAnchor;
    
    [NonSerialized]
    public float catchupSpeed = 0.3f;
    
    [NonSerialized]
    public float bounceAmount = 0.1f;
    
    [NonSerialized]
    public float bounceSpeed = 0.5f;
    
    private NavMeshAgent _navMeshAgent;

    private GameObject _model;

    private Vector3 velocity;

    #endregion

    #region Main Functions
    // Start is called before the first frame update
    void Start()
    {
        _player = GameManager.Instance.GetPlayer();
        
        _companionAnchor = _player.GetComponentInChildren<CompanionAnchor>();
        
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.updatePosition = false; // We will manually update the position
        
        _model = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Follow the player
        if (_player != null)
        {
            Vector3 targetPosition = _companionAnchor.transform.position;
            Vector3 direction = (targetPosition - transform.position).normalized;

            // Rotate towards the direction of movement
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); // Adjust rotation speed as needed
            }

            //transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, catchupSpeed);
            
            _navMeshAgent.destination = targetPosition;
            //_navMeshAgent.speed = (transform.position - targetPosition).magnitude * catchupSpeed;
            Vector3 dest = new Vector3(_navMeshAgent.nextPosition.x, _companionAnchor.transform.position.y, _navMeshAgent.nextPosition.z);
            transform.position = Vector3.SmoothDamp(transform.position, dest, ref velocity, catchupSpeed); //called on FixedUpdate after agent.SetDestination
        }
        else
        {
            Debug.LogWarning("Player not found");
        }

        // Bounce
        if (_model != null)
        {
            float newY = Mathf.Sin(Time.time * bounceSpeed) * bounceAmount;
            _model.transform.localPosition = new Vector3(_model.transform.localPosition.x, Mathf.Lerp(_model.transform.localPosition.y, newY, 0.1f), _model.transform.localPosition.z);
        }
        else
        {
            Debug.LogWarning("Model not found");
        }
    }
    #endregion

    #region Collisions
    void OnCollisionEnter(Collision other)
    {
        //_companionAnchor.NewPos();
    }
    #endregion
}
