    using UnityEngine;

    public class ClickablePointComponent : MonoBehaviour
    {
        #region Fields
        public Puzzle2D puzzle;

        public Vector2 linked2DPoint = new Vector2();

        #endregion

        #region Trigger Functions
        public void TriggerMouseDownByDistance()
        {
            Debug.Log("On mouse down");
            if (puzzle != null)
            {
                puzzle.OnPointClicked3D(linked2DPoint, transform);
            }
        }
        #endregion
    }