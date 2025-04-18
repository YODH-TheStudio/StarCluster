using UnityEngine;

public class ClickablePointComponent : MonoBehaviour
{
    public Puzzle2D puzzle;

    public Vector2 linked2DPoint;

    public void TriggerMouseDownByDistance()
    {
        Debug.Log("On mouse down");
        if (puzzle != null)
        {
            puzzle.OnPointClicked3D(linked2DPoint, transform);
        }
    }
}