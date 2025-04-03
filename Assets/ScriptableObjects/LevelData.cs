using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level Data", order = 51)]
public class LevelData : ScriptableObject
{
    public List<Vector2> _points = new List<Vector2>();
    public List<(Vector2, Vector2)> _segments = new List<(Vector2, Vector2)>();

    public void AddPoint(Vector2 _point)
    {
        _points.Add(_point);
    }

    public void RemovePoint(Vector2 _point)
    {
        _points.Remove(_point);
    }

    public void AddSegment(Vector2 _pointA, Vector2 _pointB)
    {
        _segments.Add((_pointA, _pointB));
    }

    public void RemoveSegment(Vector2 _pointA, Vector2 _pointB)
    {
        _segments.Remove((_pointA, _pointB));
    }

    public void ClearSegments()
    {
        _segments.Clear();
    }

    public void ResetLevel()
    {
        _points.Clear();
        _segments.Clear();
    }
}
