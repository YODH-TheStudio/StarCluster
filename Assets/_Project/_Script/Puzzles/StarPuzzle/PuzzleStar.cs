using UnityEngine;

public enum StarColor
{
    Blue,
    Green,
    Yellow,
    Red
};

public class PuzzleStar
{
    private bool _isStart;
    private bool _isEnd;
    [SerializeField] private StarColor _starColor;
    
}
