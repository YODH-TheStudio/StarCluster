using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{

    private PlayerScript _player;
    
    private VibrationManager _vibrationManager { get; set; }
    public SoundSystem _soundSystem { get; private set; }
    public StateManager _stateManager { get; private set; }

    // Singleton
    private static GameManager _instance;


    public KeyCode _key = KeyCode.Space;
    public KeyCode _keytoo = KeyCode.P;

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

        if (_soundSystem == null)
        {
            _soundSystem = FindObjectOfType<SoundSystem>();
        }
        
        if (_stateManager == null)
        {
            FindStateManager();
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Instance._soundSystem.ChangeMusicByKey("Main");
    }

    void FindPlayer()
    {
        _player = FindObjectOfType<PlayerScript>();
    }

    void FindVibrationManager()
    {
        _vibrationManager = FindObjectOfType<VibrationManager>();
    }

    void FindStateManager()
    {
        _stateManager = FindObjectOfType<StateManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(_key))
        {
            PlayTestSFX();
        }
        else if (Input.GetKeyUp(_keytoo))
        {
            PlayTestSFXToo();
        }
    }

    void PlayTestSFX()
    {
        Vector3 spawnPosition = transform.position;
        Instance._soundSystem.PlaySoundFXClipByKey("Violon", spawnPosition);
        Debug.Log("SFX 'Violon' lancé à la position : " + spawnPosition);
    }   
    void PlayTestSFXToo()
    {
        Vector3 spawnPosition = transform.position;
        Instance._soundSystem.PlaySoundFXClipByKey("Tung", spawnPosition);
        Debug.Log("SFX 'Violon' lancé à la position : " + spawnPosition);
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

    public StateManager GetStateManager()
    {
        if (_stateManager == null)
        {
            FindStateManager();
        }
        return _stateManager;
    }
}