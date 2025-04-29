using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;


public class StarPuzzleManager : Singleton<StarPuzzleManager>
{
    [SerializeField] private FusionPoint fusionPoint;
    public CinemachineVirtualCamera PuzzleCamera {get; set;}
    public Canvas PuzzleCanvas { get; set; }

    public List<bool> Circuits { get; set; }
    public bool isPuzzleActive;
    private bool _isFinishedPuzzle;
    public DrawingColors DrawingColor { get; set; }
    
    public event Action OnPuzzleEnter;
    public event Action OnPuzzleExit;
    
    private new void Awake()
    {
        base.Awake();
    }
    
    public void SwitchCamera()
    {
        if (_isFinishedPuzzle) return;
        if (isPuzzleActive)
        {
            OnPuzzleExit?.Invoke();
        }
        else
        {
            OnPuzzleEnter?.Invoke();
        }
        isPuzzleActive = !PuzzleCamera.gameObject.activeSelf;
        PuzzleCanvas.gameObject.SetActive(!PuzzleCanvas.gameObject.activeSelf);
    }

    public void PuzzleComplete()
    {
        SwitchCamera();
        fusionPoint.SetState(true);
        _isFinishedPuzzle = true;
    }
    private IEnumerator FadeToBlack(bool puzzleActive)
    {
        // Fade In or Out of camera to open / close the puzzle
        yield return new WaitForSecondsRealtime(0.1f);
    }
}
