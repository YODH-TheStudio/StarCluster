using MeetAndTalk;
using System;
using System.Collections;
using System.Collections.Generic;
using Systems.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Video;

public class VideoEndCheck : MonoBehaviour
{
    #region Fields
    [SerializeField]private VideoPlayer _videoPlayer;
    private SceneLoader _sceneLoader;
    private GameManager _gameManager;
    private SoundSystem _soundSystem;
    #endregion

    #region Main Functions
    private void Start()
    {
        _sceneLoader = PersistentSingleton<SceneLoader>.Instance;
        _gameManager = GameManager.Instance;
        _soundSystem = _gameManager.GetSoundSystem();

        _videoPlayer.loopPointReached += OnVideoEnd;
    }
    #endregion

    #region Main Functions
    private async void OnVideoEnd(VideoPlayer vp)
    {
        Debug.Log("Video Ended");
        _soundSystem.ChangeMusicByKey("Hope");
        await _sceneLoader.LoadSceneGroup(2);
    }
    #endregion

}
