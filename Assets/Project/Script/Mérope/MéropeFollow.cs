using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MéropeFollow : MonoBehaviour
{
    private PlayerScript _player;
    
    public float catchupSpeed = 0.3f;
    
    public float bounceAmount = 0.1f;
    public float bounceSpeed = 0.5f;
    
    private NavMeshAgent _agent;
    private Rigidbody _rb;

    private Vector3 velocity;
    
    private List<Vector3> _path;

    private Transform _companionAnchor;
    private GameObject _model;
    
    // Start is called before the first frame update
    void Start()
    {
        _player = GameManager.Instance.GetPlayer();
        _agent = GetComponent<NavMeshAgent>();
        
        _companionAnchor = _player.transform.Find("CompanionAnchor");
        
        _path = new List<Vector3>();
        _model = transform.GetChild(0).gameObject;
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
        
        // bounce
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
}
