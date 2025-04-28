using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class StarPuzzleManager : Singleton<StarPuzzleManager>
{
    [SerializeField] private FusionPoint fusionPoint;
    public CinemachineVirtualCamera PuzzleCamera {get; set;}
    public Canvas PuzzleCanvas { get; set; }
    public Canvas DiodesCanvas { get; set; }

    public List<bool> Circuits { get; set; }
    public bool isPuzzleActive;
    
    private new void Awake()
    {
        base.Awake();
    }
    
    public void SwitchCamera()
    {
        if (isPuzzleActive)
        {
            GameManager.Instance.GetStateManager().ChangeState(StateManager.PlayerState.Idle);
        }
        else
        {
            GameManager.Instance.GetStateManager().ChangeState(StateManager.PlayerState.Puzzle);
        }
        isPuzzleActive = !PuzzleCamera.gameObject.activeSelf;
        PuzzleCamera.gameObject.SetActive(!PuzzleCamera.gameObject.activeSelf);
        PuzzleCanvas.gameObject.SetActive(!PuzzleCanvas.gameObject.activeSelf);
        DiodesCanvas.gameObject.SetActive(!DiodesCanvas.gameObject.activeSelf);
    }

    public void PuzzleComplete()
    {
        fusionPoint.SetState(true);
        SwitchCamera();
    }
    private IEnumerator FadeToBlack(bool puzzleActive)
    {
        // Fade In or Out of camera to open / close the puzzle
        yield return new WaitForSecondsRealtime(0.1f);
    }
}
