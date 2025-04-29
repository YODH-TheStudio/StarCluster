using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public enum AxisOptions { Both, Horizontal, Vertical }

public class PlayerJoystick : MonoBehaviour
{
    #region Fields
    [SerializeField] protected RectTransform background = null;
    [SerializeField] private RectTransform handle = null;
    [SerializeField] private float movementRange;
    [SerializeField] private StateManager.PlayerState allowedStates;

    private PlayerScript _player = null;
    private Finger _movementFinger;
    private Vector2 _movementAmount;
    private RectTransform _baseRect = null;
    private Canvas _canvas;
    private Camera _cam;
    private Vector2 _handleStartPosition;

    #endregion

    #region Main Functions
    protected virtual void Start()
    {
        _baseRect = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        if (_canvas == null)
            Debug.LogError("The Joystick is not placed inside a canvas");

        Vector2 center = new Vector2(0.5f, 0.5f);
        background.pivot = center;
        handle.anchorMin = center;
        handle.anchorMax = center;
        handle.pivot = center;
        handle.anchoredPosition = Vector2.zero;
        background.gameObject.SetActive(false);

        _player = GameManager.Instance.GetPlayer();
    }
    private void Update()
    {
        _player.OnMove(_movementAmount);
    }

    private void OnEnable()
    {
        GameManager.Instance.OnFingerDown += Touch_OnFingerDown;
        GameManager.Instance.OnFingerUp += Touch_OnFingerUp;
        GameManager.Instance.OnFingerMove += Touch_OnFingerMove;
        
        GameManager.Instance.GetStateManager().OnStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnFingerDown -= Touch_OnFingerDown;
        GameManager.Instance.OnFingerUp -= Touch_OnFingerUp;
        GameManager.Instance.OnFingerMove -= Touch_OnFingerMove;
        
        StateManager stateManager = GameManager.Instance.GetStateManager();
        if (stateManager != null)
        {
            stateManager.OnStateChanged -= HandleStateChanged;
        }
    }

    #endregion

    #region StateChange

    private void HandleStateChanged(StateManager.PlayerState newState)
    {
        //check if the new state is in the allowed states
        if (allowedStates.HasFlag(newState))
        {
            GameManager.Instance.OnFingerDown += Touch_OnFingerDown;
            GameManager.Instance.OnFingerUp += Touch_OnFingerUp;
            GameManager.Instance.OnFingerMove += Touch_OnFingerMove;
        }else
        {
            GameManager.Instance.OnFingerDown -= Touch_OnFingerDown;
            GameManager.Instance.OnFingerUp -= Touch_OnFingerUp;
            GameManager.Instance.OnFingerMove -= Touch_OnFingerMove;
            CancelJoystick();
        }
    }
    
    #endregion

    #region Touch
    private void Touch_OnFingerDown(Finger touchedFinger)
    {
        if(_movementFinger == null && touchedFinger.screenPosition.x <= Screen.width / 2f)
        {
            _movementFinger = touchedFinger;
            _movementAmount = Vector2.zero;
            background.gameObject.SetActive(true);
            background.anchoredPosition = ScreenPointToAnchoredPosition(touchedFinger.screenPosition);
            _handleStartPosition = Vector2.zero;
            handle.anchoredPosition = _handleStartPosition;
        }
    }
    private void Touch_OnFingerUp(Finger touchedFinger)
    {
        if(touchedFinger == _movementFinger)
        {
            CancelJoystick();
        }
    }
    
    private void CancelJoystick()
    {
        _movementFinger = null;
        _movementAmount = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
        background.gameObject.SetActive(false);
    }

    private void Touch_OnFingerMove(Finger touchedFinger)
    {
        if (touchedFinger == _movementFinger)
        {
            Vector2 knobPosition;
            float movementRadius = background.sizeDelta.x / 2f;
            ETouch.Touch currentTouche = touchedFinger.currentTouch;
            Vector2 backgroundPosition = new Vector2(background.position.x, background.position.y);
            knobPosition = (currentTouche.screenPosition - backgroundPosition).normalized * movementRadius;

            var delta = currentTouche.screenPosition - backgroundPosition;

            delta = Vector2.ClampMagnitude(delta, movementRange);

            handle.anchoredPosition = _handleStartPosition + delta;

            _movementAmount = knobPosition / movementRadius;
        }
    }

    protected Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition)
    {
        Vector2 localPoint;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_baseRect, screenPosition, _cam, out localPoint)) return Vector2.zero;
        
        Vector2 pivotOffset = _baseRect.pivot * _baseRect.sizeDelta;
        return localPoint - (background.anchorMax * _baseRect.sizeDelta) + pivotOffset;
    }
    #endregion
}
