using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.OnScreen;
using static UnityEngine.InputSystem.OnScreen.OnScreenStick;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public enum AxisOptions { Both, Horizontal, Vertical }

public class PlayerJoystick : MonoBehaviour
{
    [SerializeField] protected RectTransform _background = null;
    [SerializeField] private RectTransform _handle = null;
    [SerializeField] private float _movementRange;
    [SerializeField] private StateManager.PlayerState _allowedStates;

    private PlayerScript _player = null;
    private Finger _MovementFinger;
    private Vector2 _MovementAmount;
    private RectTransform _baseRect = null;
    private Canvas _canvas;
    private Camera _cam;
    private Vector2 _handleStartPosition;

    protected virtual void Start()
    {
        _baseRect = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        if (_canvas == null)
            Debug.LogError("The Joystick is not placed inside a canvas");

        Vector2 center = new Vector2(0.5f, 0.5f);
        _background.pivot = center;
        _handle.anchorMin = center;
        _handle.anchorMax = center;
        _handle.pivot = center;
        _handle.anchoredPosition = Vector2.zero;
        _background.gameObject.SetActive(false);

        _player = GameManager.Instance.GetPlayer();
    }

    private void OnEnable()
    {
        GameManager.Instance.GetStateManager().OnStateChanged += HandleStateChanged;
        EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += Touch_OnFingerDown;
        ETouch.Touch.onFingerUp += Touch_OnFingerUp;
        ETouch.Touch.onFingerMove += Touch_OnFingerMove;
    }

    private void OnDisable()
    {
        ETouch.Touch.onFingerDown -= Touch_OnFingerDown;
        ETouch.Touch.onFingerUp -= Touch_OnFingerUp;
        ETouch.Touch.onFingerMove -= Touch_OnFingerMove;
        EnhancedTouchSupport.Disable();
    }

    private void HandleStateChanged(StateManager.PlayerState newState)
    {
        //check if the new state is in the allowed states
        if (_allowedStates.HasFlag(newState))
        {
            ETouch.Touch.onFingerDown += Touch_OnFingerDown;
            ETouch.Touch.onFingerUp += Touch_OnFingerUp;
            ETouch.Touch.onFingerMove += Touch_OnFingerMove;
        }else
        {
            ETouch.Touch.onFingerDown -= Touch_OnFingerDown;
            ETouch.Touch.onFingerUp -= Touch_OnFingerUp;
            ETouch.Touch.onFingerMove -= Touch_OnFingerMove;
        }
    }
    
    private void Touch_OnFingerDown(Finger TouchedFinger)
    {
        if(_MovementFinger == null && TouchedFinger.screenPosition.x <= Screen.width / 2f)
        {
            _MovementFinger = TouchedFinger;
            _MovementAmount = Vector2.zero;
            _background.gameObject.SetActive(true);
            _background.anchoredPosition = ScreenPointToAnchoredPosition(TouchedFinger.screenPosition);
            _handleStartPosition = Vector2.zero;
            _handle.anchoredPosition = _handleStartPosition;
        }
    }
    private void Touch_OnFingerUp(Finger TouchedFinger)
    {
        if(TouchedFinger == _MovementFinger)
        {
            _MovementFinger = null;
            _MovementAmount = Vector2.zero;
            _handle.anchoredPosition = Vector2.zero;
            _background.gameObject.SetActive(false);
        }
    }

    private void Touch_OnFingerMove(Finger TouchedFinger)
    {
        if (TouchedFinger == _MovementFinger)
        {
            Vector2 knobPosition;
            float movementRadius = _background.sizeDelta.x / 2f;
            ETouch.Touch currentTouche = TouchedFinger.currentTouch;
            Vector2 backgroundPosition = new Vector2(_background.position.x, _background.position.y);
            knobPosition = (currentTouche.screenPosition - backgroundPosition).normalized * movementRadius;

            var delta = currentTouche.screenPosition - backgroundPosition;

            delta = Vector2.ClampMagnitude(delta, _movementRange);

            _handle.anchoredPosition = _handleStartPosition + delta;

            _MovementAmount = knobPosition / movementRadius;
        }
    }

    protected Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition)
    {
        Vector2 localPoint = Vector2.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_baseRect, screenPosition, _cam, out localPoint))
        {
            Vector2 pivotOffset = _baseRect.pivot * _baseRect.sizeDelta;
            return localPoint - (_background.anchorMax * _baseRect.sizeDelta) + pivotOffset;
        }
        return Vector2.zero;
    }

    private void Update()
    {
        _player.OnMove(_MovementAmount);
    }
}
