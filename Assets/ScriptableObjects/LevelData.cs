using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level Data", order = 51)]
public class LevelData : ScriptableObject
{
    // Liste des points du niveau
    public List<Vector2> points = new List<Vector2>();

    // Liste des segments entre les points
    public List<(Vector2, Vector2)> segments = new List<(Vector2, Vector2)>(); // Connexions entre les points

    // Méthode pour ajouter un point
    public void AddPoint(Vector2 point)
    {
        points.Add(point);
    }

    // Méthode pour retirer un point
    public void RemovePoint(Vector2 point)
    {
        points.Remove(point);
    }

    // Méthode pour ajouter un segment entre deux points
    public void AddSegment(Vector2 pointA, Vector2 pointB)
    {
        segments.Add((pointA, pointB));
    }

    // Méthode pour retirer un segment entre deux points
    public void RemoveSegment(Vector2 pointA, Vector2 pointB)
    {
        segments.Remove((pointA, pointB));
    }

    // Méthode pour réinitialiser les segments
    public void ClearSegments()
    {
        segments.Clear();
    }

    // Méthode pour réinitialiser les points et les segments
    public void ResetLevel()
    {
        points.Clear();
        segments.Clear();
    }
}
