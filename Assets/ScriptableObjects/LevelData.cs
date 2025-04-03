using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level Data", order = 51)]
public class LevelData : ScriptableObject
{
    // Liste des points du niveau
    public List<Vector2> points = new List<Vector2>();

    // Liste des segments entre les points
    public List<(Vector2, Vector2)> segments = new List<(Vector2, Vector2)>(); // Connexions entre les points

    // M�thode pour ajouter un point
    public void AddPoint(Vector2 point)
    {
        points.Add(point);
    }

    // M�thode pour retirer un point
    public void RemovePoint(Vector2 point)
    {
        points.Remove(point);
    }

    // M�thode pour ajouter un segment entre deux points
    public void AddSegment(Vector2 pointA, Vector2 pointB)
    {
        segments.Add((pointA, pointB));
    }

    // M�thode pour retirer un segment entre deux points
    public void RemoveSegment(Vector2 pointA, Vector2 pointB)
    {
        segments.Remove((pointA, pointB));
    }

    // M�thode pour r�initialiser les segments
    public void ClearSegments()
    {
        segments.Clear();
    }

    // M�thode pour r�initialiser les points et les segments
    public void ResetLevel()
    {
        points.Clear();
        segments.Clear();
    }
}
