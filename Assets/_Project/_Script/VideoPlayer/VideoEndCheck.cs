using System;
using System.Collections;
using Systems.SceneManagement;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class VideoEndCheck : MonoBehaviour
{
    #region Fields
    [SerializeField] private VideoPlayer _videoPlayer;
    private SceneLoader _sceneLoader;
    private GameManager _gameManager;
    private SoundSystem _soundSystem;

    [SerializeField] private GameObject skipButton;
    private Coroutine _hideButtonCoroutine;
    #endregion

    #region Main Functions
    private void Start()
    {
        _sceneLoader = PersistentSingleton<SceneLoader>.Instance;
        _gameManager = GameManager.Instance;
        _soundSystem = _gameManager.GetSoundSystem();
        skipButton.SetActive(false); // Ensure the button is initially hidden
    }

    private void OnEnable()
    {
        ETouch.Touch.onFingerDown += Touch_OnFingerDown;

        _videoPlayer.loopPointReached += OnVideoEnd;
    }

    private void OnDisable()
    {
        ETouch.Touch.onFingerDown -= Touch_OnFingerDown;

        _videoPlayer.loopPointReached -= OnVideoEnd;
    }

    #endregion

    #region Touch
    private void Touch_OnFingerDown(Finger touchedFinger)
    {
        // Show the skip button
        skipButton.SetActive(true);

        // Cancel any existing coroutine to reset the timer
        if (_hideButtonCoroutine != null)
        {
            StopCoroutine(_hideButtonCoroutine);
        }

        // Start a coroutine to hide the button after 2 seconds
        _hideButtonCoroutine = StartCoroutine(HideSkipButtonAfterDelay(2f));
    }
    #endregion

    #region Coroutines
    private IEnumerator HideSkipButtonAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        skipButton.SetActive(false);
    }
    #endregion

    #region Main Functions
    private async void OnVideoEnd(VideoPlayer vp)
    {
        Debug.Log("Video Ended");
        _soundSystem.ChangeMusicByKey("Hope");
        await _sceneLoader.LoadSceneGroup(2);
    }

    public async void OnEnd()
    {
        Debug.Log("Video Ended");
        _soundSystem.ChangeMusicByKey("Hope");
        await _sceneLoader.LoadSceneGroup(2);
    }
    #endregion
}