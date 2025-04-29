using System;
using UnityEngine;

public class CanvasInput : MonoBehaviour
{
    #region Fields
    [SerializeField] private GameObject interactionButton;
    private Canvas _canvas;
    #endregion

    #region Main Functions

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.worldCamera = Camera.main;
    }

    private void Start()
    {
        GameManager.Instance.GetPlayer().gameObject.GetComponent<PlayerInteractionZone>().SetInteractionButton(interactionButton);
    }
    #endregion
}
