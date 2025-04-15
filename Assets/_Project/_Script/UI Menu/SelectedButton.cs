using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectedButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]private Sprite _baseSprite;
    [SerializeField]private Sprite _pressedSprite;
    private Image _buttonImage;

    private void Awake()
    {
        _buttonImage = GetComponent<Image>();
        _buttonImage.sprite = _baseSprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _buttonImage.sprite = _pressedSprite;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _buttonImage.sprite = _baseSprite;
    }
}
