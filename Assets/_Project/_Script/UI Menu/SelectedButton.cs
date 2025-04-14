using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectedButton : PressedButton
{
    [SerializeField]private Sprite selectedSprite;
    [SerializeField]private Sprite emptySprite;
    [SerializeField] private Menu menu;
    private int _index;

    private void Awake()
    {
        _index = menu.GetIndex();
    }
    
    public override void OnPointerDown(PointerEventData eventData)
    {
        ButtonImage.sprite = pressedSprite;
    }
    
    public override void OnPointerUp(PointerEventData eventData)
    {
        ButtonImage.sprite = selectedSprite;
    }
}
