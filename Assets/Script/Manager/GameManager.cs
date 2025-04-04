using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{

    private PlayerScript _player;
    
    
    private VibrationManager _vibrationManager { get; set; }

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

    void FindPlayer()
    {
        _player = FindObjectOfType<PlayerScript>();
    }

    void FindVibrationManager()
    {
        _vibrationManager = FindObjectOfType<VibrationManager>();
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (_player == null)
        {
            FindPlayer();
        }

        if (_vibrationManager == null)
        {
            FindVibrationManager();
        }
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

    public VibrationManager GetVibrationManager()
    {
        if (_vibrationManager == null)
        {
            FindVibrationManager();
        }
        return _vibrationManager;
    }
}