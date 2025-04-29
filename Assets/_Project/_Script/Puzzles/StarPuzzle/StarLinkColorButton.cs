    using UnityEngine;

    public class StarLinkColorButton : MonoBehaviour
    {
        [SerializeField] private DrawingColors drawingColor;

        public void SetDrawingColor()
        {
            StarPuzzleManager.Instance.DrawingColor = drawingColor;
            Debug.Log($"color set to : {drawingColor}");
        }
        
        
    }
