using UnityEngine;

public class ClickablePointComponent : MonoBehaviour
{
    public Puzzle2D puzzle;

    public Vector2 linked2DPoint;

    public void OnMouseDown()
    {
        if (puzzle != null)
        {
            puzzle.OnPointClicked3D(linked2DPoint, transform);
        }
    }
}