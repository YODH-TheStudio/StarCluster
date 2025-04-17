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

public enum PointSize
{
    Petite,
    Moyenne,
    Grosse
}

[System.Serializable]
public class Circuit
{
    public Color circuitColor;
    public Vector2 startPoint;
    public Vector2 endPoint;
    public string sign;
    public string name;  

    public Circuit(Color color, Vector2 start, Vector2 end, string sign = "", string name = "Circuit ")
    {
        this.circuitColor = color;
        this.startPoint = start;
        this.endPoint = end;
        this.sign = sign;
        this.name = name; 
    }
}

public class PointSizeEntry
{
    public Vector2 point;
    public PointSize size;
}


[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level Data", order = 1)]
public class LevelData : ScriptableObject
{
    public List<Vector2> _points = new List<Vector2>();
    public List<Segment> _segments = new List<Segment>();
    public List<Circuit> _circuits = new List<Circuit>();

    // Point size to determine their size in creation time
    public List<PointSizeEntry> pointSizes = new List<PointSizeEntry>();

    public void AddCircuit(Color circuitColor, Vector2 startPoint, Vector2 endPoint, string sign)
    {
        _circuits.Add(new Circuit(circuitColor, startPoint, endPoint, sign));
    }


    public void RemoveCircuit(int index)
    {
        if (index >= 0 && index < _circuits.Count)
        {
            _circuits.RemoveAt(index);
        }
    }
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
