using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : PersistentSingleton<GameManager>
{

    private PlayerScript _player;
    private CompanionFollow _companion;
    
    private VibrationManager _vibrationManager { get; set; }
    public SoundSystem _soundSystem { get; private set; }
    public StateManager _stateManager { get; private set; }
    public DialogueManagerCustom _dialogueManager { get; private set; }
    
    public KeyCode _key = KeyCode.Space;
    public KeyCode _keytoo = KeyCode.P;

    // Start is called before the first frame update
    void Awake()
    {
        // if (_player == null)
        // {
        //     FindPlayer();
        // }
        // if (_companion == null)
        // {
        //     FindCompanion();
        // }

        if (_vibrationManager == null)
        {
            FindVibrationManager();
        }

        if (_soundSystem == null)
        {
            FindSoundManager();
        }
        
        if (_stateManager == null)
        {
            FindStateManager();
        }

        if (_dialogueManager == null)
        {
            FindDialogueManager();
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
    
    void FindCompanion()
    {
        _companion = FindObjectOfType<CompanionFollow>();
    }

    void FindVibrationManager()
    {
        _vibrationManager = FindObjectOfType<VibrationManager>();
    }

    void FindSoundManager()
    {
        _soundSystem = FindObjectOfType<SoundSystem>();
    }

    void FindStateManager()
    {
        _stateManager = FindObjectOfType<StateManager>();
    }

    void FindDialogueManager()
    {
        _dialogueManager = FindObjectOfType<DialogueManagerCustom>();
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
        Debug.Log("SFX 'Violon' lanc� � la position : " + spawnPosition);
    }   
    void PlayTestSFXToo()
    {
        Vector3 spawnPosition = transform.position;
        Instance._soundSystem.PlaySoundFXClipByKey("Tung", spawnPosition);
        Debug.Log("SFX 'Violon' lanc� � la position : " + spawnPosition);
    }


    public PlayerScript GetPlayer()
    {
        if (_player == null)
        {
            FindPlayer();
        }
        return _player;
    }
    
    public CompanionFollow GetCompanion()
    {
        if (_companion == null)
        {
            FindCompanion();
        }
        return _companion;
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

    public DialogueManagerCustom GetDialogueManager()
    {
        if (_dialogueManager == null)
        {
            FindDialogueManager();
        }
        return _dialogueManager;
    }
    
    public SoundSystem GetSoundSystem()
    {
        if (_soundSystem == null)
        {
            FindSoundManager();
        }
        return _soundSystem;
    }
}