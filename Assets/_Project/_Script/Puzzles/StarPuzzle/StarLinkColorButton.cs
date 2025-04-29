    using System;
    using UnityEngine;

    public enum DrawingColors
    {
        Green,
        Blue,
        Red,
        Brown,
        Purple,
        Eraser
        
    }
    public class StarLinkColorButton : MonoBehaviour
    {
        [SerializeField] private DrawingColors drawingColor;
        public Action OnColorChange;

        public void SetDrawingColor()
        {
            StarPuzzleManager.Instance.DrawingColor = drawingColor;
            Debug.Log($"color set to : {drawingColor}");
        }
    }
