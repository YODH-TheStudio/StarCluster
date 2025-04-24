using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class StarPuzzleInput : MonoBehaviour
{
    [SerializeField] private StarPuzzleBoard puzzleBoard;
    [SerializeField] private Camera puzzleCamera;
    
    private PuzzleStar _startStar;
    private PuzzleStar _currentHover;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        Touch.onFingerDown += HandleFingerDown;
        Touch.onFingerMove += HandleFingerMove;
        Touch.onFingerUp += HandleFingerUp;
    }

    private void OnDisable()
    {
        Touch.onFingerDown -= HandleFingerDown;
        Touch.onFingerMove -= HandleFingerMove;
        Touch.onFingerUp -= HandleFingerUp;
        EnhancedTouchSupport.Disable();
    }

    private void HandleFingerDown(Finger finger)
    {
        PuzzleStar touched = RaycastToStar(finger.screenPosition);
        if (touched != null && touched.IsStartStar())
        {
            _startStar = touched;
        }
    }

    private void HandleFingerMove(Finger finger)
    {
        if (_startStar == null) return;

        _currentHover = RaycastToStar(finger.screenPosition);
    }

    private void HandleFingerUp(Finger finger)
    {
        if (_startStar == null) return;

        PuzzleStar endStar = RaycastToStar(finger.screenPosition);
        if (endStar != null && endStar.IsEndStar())
        {
            // PuzzleBoard.TryCreateLink(_startStar, endStar);
        }

        _startStar = null;
        _currentHover = null;
    }

    private PuzzleStar RaycastToStar(Vector2 screenPos)
    {
        Ray ray = puzzleCamera.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.collider.GetComponent<PuzzleStar>();
        }

        return null;
    }
}