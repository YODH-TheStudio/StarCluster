using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    
    public PlayerScript _player {get; private set;}
    
    // Singleton
    private static GameManager _instance;
    
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();

                if (_instance == null)
                {
                    GameObject singleton = new GameObject(typeof(GameManager).ToString());
                    _instance = singleton.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        if (_player == null)
        {
            _player = FindObjectOfType<PlayerScript>();
        }
    }

    void FindPlayer()
    {
        _player = FindObjectOfType<PlayerScript>();
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
    
    public PlayerScript GetPlayer()
    {
        if (_player == null)
        {
            FindPlayer();
        }
        return _player;
    }
}