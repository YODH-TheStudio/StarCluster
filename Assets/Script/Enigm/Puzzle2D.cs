using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Puzzle2D : MonoBehaviour
{
    public Canvas canvas;
    public Color pointColor = Color.green;
    public Color lineColor = Color.red;
    public int numberOfPoints = 100;

    private RectTransform playArea;
    private List<RectTransform> points = new List<RectTransform>();
    private RectTransform currentPoint;
    private Dictionary<(RectTransform, RectTransform), GameObject> connections = new Dictionary<(RectTransform, RectTransform), GameObject>();

    private void Start()
    {
        CreateBackground();
        GeneratePoints();
    }

    private void CreateBackground()
    {
        GameObject background = new GameObject("Background", typeof(Image));
        Image backgroundImage = background.GetComponent<Image>();
        backgroundImage.color = new Color(0f, 0f, 0f, 0.8f);

        playArea = background.GetComponent<RectTransform>();
        playArea.SetParent(canvas.transform);
        playArea.sizeDelta = new Vector2(Screen.width * 0.6f, Screen.height * 0.6f);
        playArea.anchorMin = new Vector2(0.5f, 0.5f);
        playArea.anchorMax = new Vector2(0.5f, 0.5f);
        playArea.anchoredPosition = Vector2.zero;
    }

    private void GeneratePoints()
    {
        int rows = Mathf.CeilToInt(Mathf.Sqrt(numberOfPoints));
        int cols = Mathf.CeilToInt(Mathf.Sqrt(numberOfPoints));
        float spacingX = playArea.sizeDelta.x / cols;
        float spacingY = playArea.sizeDelta.y / rows;

        for (int i = 0; i < numberOfPoints; i++)
        {
            int row = i / cols;
            int col = i % cols;
            float x = (col + 0.5f) * spacingX - (playArea.sizeDelta.x / 2);
            float y = (row + 0.5f) * spacingY - (playArea.sizeDelta.y / 2);
            CreatePoint(x, y);
        }
    }

    private void CreatePoint(float x, float y)
    {
        GameObject newPoint = new GameObject("Point", typeof(Image));
        Image pointImage = newPoint.GetComponent<Image>();
        pointImage.color = pointColor;

        RectTransform pointRect = newPoint.GetComponent<RectTransform>();
        pointRect.SetParent(playArea);
        pointRect.sizeDelta = new Vector2(15, 15);
        pointRect.anchoredPosition = new Vector2(x, y);
        points.Add(pointRect);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Input.mousePosition;
            foreach (var point in points)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(point, mousePosition))
                {
                    OnPointClicked(point);
                    return;
                }
            }
        }

        CheckLineIntersections();
    }

    private void OnPointClicked(RectTransform clickedPoint)
    {
        if (currentPoint == null)
        {
            currentPoint = clickedPoint;
        }
        else
        {
            if (ArePointsConnected(currentPoint, clickedPoint))
            {
                UnlinkPoints(currentPoint, clickedPoint);
            }
            else
            {
                LinkPoints(currentPoint, clickedPoint);
            }
            currentPoint = null;
        }
    }

    private void LinkPoints(RectTransform start, RectTransform end)
    {
        GameObject newLine = new GameObject("Line", typeof(Image));
        Image lineImage = newLine.GetComponent<Image>();
        lineImage.color = lineColor;
        RectTransform lineRect = newLine.GetComponent<RectTransform>();

        Vector2 startPos = start.anchoredPosition;
        Vector2 endPos = end.anchoredPosition;
        Vector2 direction = endPos - startPos;
        float distance = direction.magnitude;

        lineRect.SetParent(playArea);
        lineRect.anchoredPosition = startPos + direction / 2;
        lineRect.sizeDelta = new Vector2(distance, 5);
        lineRect.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

        connections[(start, end)] = newLine;
        connections[(end, start)] = newLine;
    }

    private void UnlinkPoints(RectTransform start, RectTransform end)
    {
        if (connections.TryGetValue((start, end), out GameObject line))
        {
            Destroy(line);
            connections.Remove((start, end));
            connections.Remove((end, start));
        }
    }

    private bool ArePointsConnected(RectTransform start, RectTransform end)
    {
        return connections.ContainsKey((start, end));
    }


    private void CheckLineIntersections()
    {
        List<(RectTransform, RectTransform)> linePairs = new List<(RectTransform, RectTransform)>(connections.Keys);
        for (int i = 0; i < linePairs.Count; i++)
        {
            for (int j = i + 1; j < linePairs.Count; j++)
            {
                if (DoLinesIntersect(linePairs[i], linePairs[j]))
                {
                    Debug.Log("Intersection détectée entre " + linePairs[i] + " et " + linePairs[j]);
                }
            }
        }
    }

    private bool DoLinesIntersect((RectTransform, RectTransform) line1, (RectTransform, RectTransform) line2)
    {
        Vector2 A = line1.Item1.anchoredPosition;
        Vector2 B = line1.Item2.anchoredPosition;
        Vector2 C = line2.Item1.anchoredPosition;
        Vector2 D = line2.Item2.anchoredPosition;

        // Appel de la fonction pour vérifier les intersections, en excluant les cas où les segments partagent un point.
        return LineSegmentsIntersect(A, B, C, D) && !SegmentsSharePoint(A, B, C, D);
    }

    private bool LineSegmentsIntersect(Vector2 A, Vector2 B, Vector2 C, Vector2 D)
    {
        float denominator = (B.x - A.x) * (D.y - C.y) - (B.y - A.y) * (D.x - C.x);
        if (denominator == 0) return false; // Les segments sont parallèles

        float numerator1 = (A.y - C.y) * (D.x - C.x) - (A.x - C.x) * (D.y - C.y);
        float numerator2 = (A.y - C.y) * (B.x - A.x) - (A.x - C.x) * (B.y - A.y);

        float r = numerator1 / denominator;
        float s = numerator2 / denominator;

        return (r >= 0 && r <= 1) && (s >= 0 && s <= 1);
    }

    private bool SegmentsSharePoint(Vector2 A, Vector2 B, Vector2 C, Vector2 D)
    {
        return (A == C || A == D || B == C || B == D);
    }
}
