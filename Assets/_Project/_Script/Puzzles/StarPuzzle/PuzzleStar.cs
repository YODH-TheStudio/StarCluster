using UnityEngine;

public class PuzzleStar : MonoBehaviour
{
    private bool _isStart;
    private bool _isEnd;

    private void Start()
    {
        // Set the stars data based on Level Data
    }

    public bool IsStartStar()
    {
        return _isStart;
    } 
    public bool IsEndStar()
    {
        return _isEnd;
    }
}
