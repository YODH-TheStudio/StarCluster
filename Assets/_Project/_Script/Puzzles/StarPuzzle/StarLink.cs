using UnityEngine;

public enum StarLinkColor
{
    Black,
    Blue,
    Green,
    Brown,
    Red,
    Purple,
};

public class StarLink : MonoBehaviour
{
    public PuzzleStar StartStar { get; private set; }
    public PuzzleStar EndStar { get; private set; }
    public StarLinkColor LinkColor { get; private set; }

    public StarLink(PuzzleStar start, PuzzleStar end, StarLinkColor linkColor)
    {
        StartStar = start;
        EndStar = end;
        LinkColor = linkColor;
    }
}
