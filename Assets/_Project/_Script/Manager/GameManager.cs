using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class GameManager : PersistentSingleton<GameManager>
{
    #region Fields
    private PlayerScript _player;
    private CompanionFollow _companion;
    
    private VibrationManager VibrationManager { get; set; }
    private HandDominanceManager HandDominanceManager { get; set; }
    private SoundSystem SoundSystem { get; set; }
    private StateManager StateManager { get; set; }
    private DialogueManagerCustom DialogueManager { get; set; }
    private PuzzleManager PuzzleManager { get; set; }
    private SaveManager SaveManager { get; set; }
    #endregion

    #region Main Functions
    private new void Awake()
    {
        if (VibrationManager == null)
        {
            FindVibrationManager();
        }

        if (HandDominanceManager == null)
        {
            FindHandDominanceManager();
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
        QualitySettings.vSyncCount = 0; // Set vSyncCount to 0 so that using .targetFrameRate is enabled.
        Application.targetFrameRate = 60;
        
        EnhancedTouchSupport.Enable();
        Instance.SoundSystem.ChangeMusicByKey("Menu");
    }
    #endregion

    #region Finders
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

    private void FindHandDominanceManager()
    {
        HandDominanceManager = FindObjectOfType<HandDominanceManager>();
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
    #endregion

    #region Getters
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

    public HandDominanceManager GetHandDominanceManager()
    {
        if (HandDominanceManager == null)
        {
            FindHandDominanceManager();
        }
        return HandDominanceManager;
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

    #endregion
    
    #region Save/Load   
    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
        // Check if in the main menu, in that case don't save the game
        //Debug.Log("Application Quit: PlayerState is" + GetStateManager().GetState());
        if (GetStateManager().GetState() != StateManager.PlayerState.Menu)
        {
            Debug.LogWarning("Saving game on quit");
            Instance.GetSaveManager().SaveGame();
        }
    }
    #endregion
}