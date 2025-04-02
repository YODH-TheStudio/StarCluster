using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MéropeFollow : MonoBehaviour
{
    private PlayerScript _player;
    
    [SerializeField] private float catchupSpeed = 0.3f;
    [SerializeField] Vector3 maxOffset;
    
    private NavMeshAgent _agent;
    private Rigidbody _rb;

    private Vector3 velocity;
    
    private List<Vector3> _path;

    private Transform _companionAnchor;
    
    // Start is called before the first frame update
    void Start()
    {
        _player = GameManager.Instance.GetPlayer();
        _agent = GetComponent<NavMeshAgent>();
        
        _companionAnchor = _player.transform.Find("CompanionAnchor");
        
        _path = new List<Vector3>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // follow the player
        if (_player != null)
        {
            transform.position = Vector3.SmoothDamp(transform.position, _companionAnchor.position, ref velocity, catchupSpeed);
        }
        else
        {
            Debug.LogWarning("Player not found");
        }
    }
    
    public void SetSpeed(float speed)
    {
        catchupSpeed = speed;
    }
}
