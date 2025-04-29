using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public enum DrawingColors
{
    Green,
    Blue,
    Red,
    Brown,
    Purple,
    Eraser
}
public class StarPuzzleManager : Singleton<StarPuzzleManager>
{
    [SerializeField] private FusionPoint fusionPoint;
    public CinemachineVirtualCamera PuzzleCamera {get; set;}
    public Canvas DiodesCanvas { get; set; }

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
        DiodesCanvas.gameObject.SetActive(!DiodesCanvas.gameObject.activeSelf);
    }

    public void PuzzleComplete()
    {
        Debug.Log("PuzzleComplete");
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
