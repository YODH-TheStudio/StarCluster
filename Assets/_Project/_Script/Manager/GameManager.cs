using UnityEngine;

public class GameManager : PersistentSingleton<GameManager>
{

    private PlayerScript _player;
    private CompanionFollow _companion;
    
    private VibrationManager VibrationManager { get; set; }
    private SoundSystem SoundSystem { get; set; }
    private StateManager StateManager { get; set; }
    private DialogueManagerCustom DialogueManager { get; set; }
    private PuzzleManager PuzzleManager { get; set; }
    private SaveManager SaveManager { get; set; }

    private new void Awake()
    {
        if (VibrationManager == null)
        {
            FindVibrationManager();
        }

        if (SoundSystem == null)
        {
            FindSoundManager();
        }
        
        if (StateManager == null)
        {
            FindStateManager();
        }

        if (DialogueManager == null)
        {
            FindDialogueManager();
        }
    }
    
    private void Start()
    {
        Instance.SoundSystem.ChangeMusicByKey("Main");
    }

    private void FindPlayer()
    {
        _player = FindObjectOfType<PlayerScript>();
    }
    
    private void FindCompanion()
    {
        _companion = FindObjectOfType<CompanionFollow>();
    }
    private void FindPuzzleManager()
    {
        PuzzleManager = FindObjectOfType<PuzzleManager>();
    }
    private void FindSaveManager()
    {
        SaveManager = FindObjectOfType<SaveManager>();
    }
    private void FindVibrationManager()
    {
        VibrationManager = FindObjectOfType<VibrationManager>();
    }

    private void FindSoundManager()
    {
        SoundSystem = FindObjectOfType<SoundSystem>();
    }

    private void FindStateManager()
    {
        StateManager = FindObjectOfType<StateManager>();
    }

    private void FindDialogueManager()
    {
        DialogueManager = FindObjectOfType<DialogueManagerCustom>();
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
        if (VibrationManager == null)
        {
            FindVibrationManager();
        }
        return VibrationManager;
    }

    public StateManager GetStateManager()
    {
        if (StateManager == null)
        {
            FindStateManager();
        }
        return StateManager;
    }

    public PuzzleManager GetPuzzleManager()
    {
        if (PuzzleManager == null)
        {
            FindPuzzleManager();
        }
        return PuzzleManager;
    }
    public SaveManager GetSaveManager()
    {
        if (SaveManager == null)
        {
            FindSaveManager();
        }
        return SaveManager;
    }
    
    public DialogueManagerCustom GetDialogueManager()
    {
        if (DialogueManager == null)
        {
            FindDialogueManager();
        }
        return DialogueManager;
    }
    
    public SoundSystem GetSoundSystem()
    {
        if (SoundSystem == null)
        {
            FindSoundManager();
        }
        return SoundSystem;
    }


    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
        Instance.GetSaveManager().SaveGame();
    }
}