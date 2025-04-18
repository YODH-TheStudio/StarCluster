using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CompanionAnchor : MonoBehaviour
{
    [SerializeField] private Vector3 runPosition = new Vector3(0, 1, -1.5f);
    
    /* Companion catchup speed, while running and orbiting */
    [SerializeField] private Vector2 catchupSpeed =  new Vector2(0.3f, 1.0f);

    [SerializeField] private float orbitOffset = 0.5f;
    [SerializeField] private Vector2 orbitRadius = new Vector2(2.5f, 3.5f);

    /* Bounce magnitude, while running and orbiting */
    [SerializeField] private Vector2 bounceAmount = new Vector2(0.1f, 0.1f);
    /* Bounce speed, while running and orbiting */
    [SerializeField] private Vector2 bounceSpeed = new Vector2(0.5f, 0.1f);
    
    private CompanionFollow _companion;
    private PlayerScript _player;
    
    // Start is called before the first frame update
    void Start()
    {
        _player = GameManager.Instance.GetPlayer();
        _companion = GameManager.Instance.GetCompanion();

        if (_player == null)
        {
            Debug.LogError("Player not found");
        }
        if(_companion == null)
        {
            Debug.LogError("Companion not found");
        }
        
        transform.position = runPosition;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player is running
        if (_player.IsMoving())
        {
            _companion.catchupSpeed = catchupSpeed.x;
            _companion.bounceAmount = bounceAmount.x;
            _companion.bounceSpeed = bounceSpeed.x;
            transform.localPosition = runPosition;
        }
        else if((transform.position - _companion.transform.position).magnitude < 0.5f)
        {
            _companion.catchupSpeed = catchupSpeed.y;
            _companion.bounceAmount = bounceAmount.y;
            _companion.bounceSpeed = bounceSpeed.y;
            
            // move to another position
            NewPos();
        }
    }

    public void NewPos()
    {
        int angle = Random.Range(0, 360);
        float distance = Random.Range(orbitRadius.x, orbitRadius.y);
        Vector3 newPosition = new Vector3(Mathf.Cos(angle) * distance, 0, Mathf.Sin(angle) * distance);
        transform.position = _player.transform.position + newPosition + new Vector3(0,orbitOffset,0);
    }
}
