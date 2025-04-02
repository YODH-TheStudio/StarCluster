using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class CompanionAnchor : MonoBehaviour
{
    [SerializeField] private Vector3 runPosition = new Vector3(0, 1, -1.5f);
    
    [SerializeField] private float bounceSpeed = 0.5f;
    [SerializeField] private float floatBounce = 0.1f;
    [SerializeField] private float companionRunTime = 0.3f;
    [SerializeField] private float companionOrbitTime = 1.0f;
    [SerializeField] private float orbitOffset = 0.5f;
    [SerializeField] private Vector2 orbitRadius = new Vector2(2.5f, 3.5f);
    
    private MéropeFollow _companion;
    private PlayerScript _player;
    
    // Start is called before the first frame update
    void Start()
    {
        _player = GameManager.Instance.GetPlayer();
        _companion = GameObject.Find("Mérope").GetComponent<MéropeFollow>();

        if (_player)
        {
            Debug.LogError("Player not found");
        }
        if(_companion == null)
        {
            Debug.LogError("Companion not found");
        }
        
        Debug.Log("CompanionAnchor Start: " + runPosition);
        transform.position = runPosition;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player is running
        if (_player.IsMoving())
        {
            _companion.SetSpeed(companionRunTime);
            transform.localPosition = runPosition;
        }
        else if((transform.position - _companion.transform.position).magnitude < 0.5f)
        {
            _companion.SetSpeed(companionOrbitTime);
            //Debug.Log("Moving to another position");
            // move to another position
            int angle = Random.Range(0, 360);
            float distance = Random.Range(orbitRadius.x, orbitRadius.y);
            Vector3 newPosition = new Vector3(Mathf.Cos(angle) * distance, 0, Mathf.Sin(angle) * distance);
            transform.position = _player.transform.position + newPosition + new Vector3(0,orbitOffset,0);
        }
        
        // Move up and down
        float variance = (float)Math.Sin(Time.time * bounceSpeed);
        Vector3 currentOffset = new Vector3(0, variance * floatBounce * 0.001f, 0);
        transform.position += currentOffset;
    }
}
