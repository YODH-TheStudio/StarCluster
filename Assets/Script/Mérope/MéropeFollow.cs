using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class MéropeFollow : MonoBehaviour
{
    private PlayerScript _player;
    
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float maxDistance = 5.5f;
    [SerializeField] private float minDistance = 2.5f;
    [SerializeField] private float floatHeight = 1.5f;
    [SerializeField] private float floatCatchupSpeed = 0.3f;
    [SerializeField] Vector3 maxOffset;
    
    private NavMeshAgent _agent;
    private Rigidbody _rb;

    private Vector3 velocity;
    
    private List<Vector3> _path;

    private Transform companionAnchor;
    
    // Start is called before the first frame update
    void Start()
    {
        _player = GameManager.Instance.GetPlayer();
        _agent = GetComponent<NavMeshAgent>();
        
        companionAnchor = _player.transform.Find("CompanionAnchor");
        Debug.Log("CompanionAnchor: " + companionAnchor);
        
        _path = new List<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        
        // Update the offset
        float variance = (float)Math.Sin(Time.time);
        Vector3 currentOffset = new Vector3(variance * maxOffset.x, variance * maxOffset.y, variance * maxOffset.z);
        
        // follow the player
        if (_player != null)
        {
            transform.position = Vector3.SmoothDamp(transform.position, companionAnchor.position /*+ currentOffset*/, ref velocity, floatCatchupSpeed);
 
            // if ((transform.position - _player.transform.position).magnitude < minDistance)
            // {
            //     // If too close to the player, move away
            //     transform.position -= (transform.position - _player.transform.position).normalized * speed * Time.deltaTime;
            // }
        }
        else
        {
            Debug.LogWarning("Player not found");
        }

        // // Move up and down
        // Vector3 newPosition = transform.position;
        // newPosition.y = Mathf.Sin(Time.time * speed) * floatSpeed + floatHeight;
        // transform.position = newPosition;
    }
}
