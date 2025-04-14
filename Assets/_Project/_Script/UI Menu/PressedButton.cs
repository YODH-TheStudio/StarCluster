using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PressedButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]protected Sprite baseSprite;
    [SerializeField]protected Sprite pressedSprite;
    protected Image ButtonImage;

    private void Awake()
    {
        ButtonImage = GetComponent<Image>();
        ButtonImage.sprite = baseSprite;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        ButtonImage.sprite = pressedSprite;
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        ButtonImage.sprite = baseSprite;
    }
}
