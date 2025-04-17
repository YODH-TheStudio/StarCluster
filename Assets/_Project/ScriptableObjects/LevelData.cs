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
    public Vector3 startPointOffset; 
    public Vector3 endPointOffset;  

    public Circuit(Color color, Vector2 start, Vector2 end, string sign = "", string name = "Circuit ", Vector3 startOffset = default, Vector3 endOffset = default)
    {
        this.circuitColor = color;
        this.startPoint = start;
        this.endPoint = end;
        this.sign = sign;
        this.name = name;
        this.startPointOffset = startOffset == default ? new Vector3(0f, 0f, 0f) : startOffset; 
        this.endPointOffset = endOffset == default ? new Vector3(0f, 0f, 0f) : endOffset;    
    }
}

[System.Serializable]
public class PointSizeEntry
{
    public Vector2 pointPosition;
    public PointSize size;
    public string name;
    public PointSizeEntry(Vector2 position, PointSize size = PointSize.Petite, string name = "")
    {
        this.pointPosition = position;
        this.size = size;
        this.name = name;  
    }
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
    public void AddPoint(Vector2 _point, string name)
    {
        _points.Add(_point);

        pointSizes.Add(new PointSizeEntry(_point, PointSize.Petite, name));
    }

    public void RemovePoint(Vector2 _point)
    {
        _points.Remove(_point);
        pointSizes.RemoveAll(p => p.pointPosition == _point);
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

    //public void SetAllPointsToSmall()
    //{
    //    foreach (var point in _points)
    //    {
    //        pointSizes.Add(new PointSizeEntry(point, PointSize.Petite));
    //    }
    //}

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
