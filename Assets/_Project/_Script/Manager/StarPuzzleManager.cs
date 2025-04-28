using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class StarPuzzleManager : Singleton<StarPuzzleManager>
{
    public CinemachineVirtualCamera PuzzleCamera {get; set;}
    public Canvas PuzzleCanvas { get; set; }
    public Canvas DiodesCanvas { get; set; }

    public List<bool> Circuits { get; set; }
    private bool _isFinished;
    public bool isPuzzleActive;
    

    private new void Awake()
    {
        base.Awake();
    }
    
    public void SwitchCamera()
    {
        isPuzzleActive = !PuzzleCamera.gameObject.activeSelf;
        PuzzleCamera.gameObject.SetActive(!PuzzleCamera.gameObject.activeSelf);
        PuzzleCanvas.gameObject.SetActive(!PuzzleCanvas.gameObject.activeSelf);
        DiodesCanvas.gameObject.SetActive(!DiodesCanvas.gameObject.activeSelf);
    }
    
    private IEnumerator FadeToBlack(bool puzzleActive)
    {
        // Fade In or Out of camera to open / close the puzzle
        yield return new WaitForSecondsRealtime(0.1f);
    }
}
