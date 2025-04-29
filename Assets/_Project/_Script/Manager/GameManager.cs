using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class GameManager : PersistentSingleton<GameManager>
{
    #region Fields
    private PlayerScript _player;
    private CompanionFollow _companion;
    
    private VibrationManager VibrationManager { get; set; }
    private SoundSystem SoundSystem { get; set; }
    private StateManager StateManager { get; set; }
    private DialogueManagerCustom DialogueManager { get; set; }
    private PuzzleManager PuzzleManager { get; set; }
    private SaveManager SaveManager { get; set; }
    
    public event Action<Finger> OnFingerMove;
    public event Action<Finger> OnFingerUp;
    public event Action<Finger> OnFingerDown;
    #endregion

    #region Main Functions
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
        QualitySettings.vSyncCount = 0; // Set vSyncCount to 0 so that using .targetFrameRate is enabled.
        Application.targetFrameRate = 60;
        
        EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += Touch_OnFingerDown;
        ETouch.Touch.onFingerUp += Touch_OnFingerUp;
        ETouch.Touch.onFingerMove += Touch_OnFingerMove;
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

    #region Input
    
    private void Touch_OnFingerMove(Finger touchedFinger)
    {
        OnFingerMove?.Invoke(touchedFinger);
    }
    
    private void Touch_OnFingerUp(Finger touchedFinger)
    {
        OnFingerUp?.Invoke(touchedFinger);
    }
    
    private void Touch_OnFingerDown(Finger touchedFinger)
    {
        OnFingerDown?.Invoke(touchedFinger);
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