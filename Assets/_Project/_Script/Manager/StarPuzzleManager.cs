using System.Collections;
using Cinemachine;
using UnityEngine;

public class StarPuzzleManager : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private CinemachineVirtualCamera puzzleCamera;

    private void Awake()
    {
        // _virtualCamera = ;
        puzzleCamera = FindObjectOfType<CinemachineVirtualCamera>();
    }

    
    
    
    
    private IEnumerator FadeToBlack(bool isPuzzleActive)
    {
        // Fade In or Out of camera to open / close the puzzle
        yield return new WaitForSecondsRealtime(0.1f);
        _virtualCamera.gameObject.SetActive(!isPuzzleActive);
        puzzleCamera.gameObject.SetActive(isPuzzleActive);
        //set active Canvas from HUD Scene
    }
}
