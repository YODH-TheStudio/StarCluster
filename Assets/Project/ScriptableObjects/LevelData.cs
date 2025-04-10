using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[System.Serializable]
public class Segment
{
    public Vector2 pointA;
    public Vector2 pointB;

    public Segment(Vector2 pointA, Vector2 pointB)
    {
        this.pointA = pointA;
        this.pointB = pointB;
    }
}

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level Data", order = 1)]
public class LevelData : ScriptableObject
{
    public List<Vector2> _points = new List<Vector2>();
    public List<Segment> _segments = new List<Segment>();

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
        _segments.Add(new Segment(_pointA, _pointB));
    }

    public void RemoveSegment(Vector2 _pointA, Vector2 _pointB)
    {
        Segment segmentToRemove = _segments.Find(s => s.pointA == _pointA && s.pointB == _pointB);
        if (segmentToRemove != null)
        {
            _segments.Remove(segmentToRemove);
        }
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
