using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class CompanionAnchor : MonoBehaviour
{
    [SerializeField] private Vector3 runPosition = new Vector3(0, 1, -1.5f);
    
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
            transform.localPosition = runPosition;
        }
        else if((transform.position - _companion.transform.position).magnitude < 0.5f)
        {
            Debug.Log("Moving to another position");
            // move to another position
            int angle = Random.Range(0, 360);
            float distance = Random.Range(1.5f, 2.5f);
            Vector3 newPosition = new Vector3(Mathf.Cos(angle) * distance, 0, Mathf.Sin(angle) * distance);
            transform.position = _player.transform.position + newPosition;
        }
    }
}
