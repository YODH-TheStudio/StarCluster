using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

[CustomEditor(typeof(LevelData))]
public class Puzzle2DEditor : Editor
{
    private LevelData levelData;
    private Canvas canvas;
    private RectTransform playArea;
    private GameObject editorParent;
    private List<GameObject> pointObjects = new List<GameObject>();
    private List<GameObject> segmentObjects = new List<GameObject>(); // Stocke les objets segments
    private int selectedPointA = -1, selectedPointB = -1;

    private void OnEnable()
    {
        levelData = (LevelData)target;
        canvas = GameObject.FindObjectOfType<Canvas>();

        if (canvas != null)
        {
            CreateEditorParent();
            CreateBackground();
            CreatePointsUI();
            CreateSegmentsUI();
        }

        EditorApplication.update += Repaint;
    }

    private void OnDisable()
    {
        EditorApplication.update -= Repaint;
    }

    private void CreateEditorParent()
    {
        editorParent = new GameObject("Editor");
        editorParent.transform.SetParent(canvas.transform);
        RectTransform rectTransform = editorParent.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
        rectTransform.anchoredPosition = Vector2.zero;
    }

    private void CreateBackground()
    {
        GameObject background = new GameObject("Background", typeof(Image));
        background.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.8f);
        playArea = background.GetComponent<RectTransform>();
        playArea.SetParent(editorParent.transform);
        playArea.sizeDelta = new Vector2(Screen.width * 0.6f, Screen.height * 0.8f);
        playArea.anchorMin = playArea.anchorMax = new Vector2(0.5f, 0.5f);
        playArea.anchoredPosition = Vector2.zero;
    }

    private void AddPointToUI(Vector2 point, int index)
    {
        GameObject pointObject = new GameObject("Point" + index, typeof(Image));
        pointObject.transform.SetParent(playArea.transform);
        pointObject.GetComponent<Image>().color = Color.green;

        RectTransform rectTransform = pointObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(10, 10);
        rectTransform.anchoredPosition = point;

        // Ajouter un label avec TextMeshPro
        GameObject labelObject = new GameObject("Label" + index, typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(pointObject.transform);
        TextMeshProUGUI labelText = labelObject.GetComponent<TextMeshProUGUI>();
        labelText.text = "P" + index;
        labelText.fontSize = 10; // Ajustez la taille de la police selon vos besoins
        labelText.alignment = TextAlignmentOptions.Center;

        // Positionner le label au-dessus du point
        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.sizeDelta = new Vector2(50, 20); // Ajuster la taille du label
        labelRect.anchoredPosition = new Vector2(0, 15); // Positionner au-dessus du point

        pointObjects.Add(pointObject);
        Canvas.ForceUpdateCanvases();
    }

    private GameObject CreateSimpleLine(Vector2 pointA, Vector2 pointB)
    {
        GameObject lineObject = new GameObject("Line", typeof(Image));
        lineObject.transform.SetParent(playArea.transform);

        Image lineImage = lineObject.GetComponent<Image>();
        lineImage.color = Color.red;

        RectTransform rectTransform = lineObject.GetComponent<RectTransform>();
        float distance = Vector2.Distance(pointA, pointB);
        float angle = Mathf.Atan2(pointB.y - pointA.y, pointB.x - pointA.x) * Mathf.Rad2Deg;

        rectTransform.sizeDelta = new Vector2(distance, 10f);
        rectTransform.rotation = Quaternion.Euler(0, 0, angle);
        rectTransform.anchoredPosition = (pointA + pointB) / 2f;

        GameObject labelObject = new GameObject("SegmentLabel", typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(lineObject.transform);
        TextMeshProUGUI labelText = labelObject.GetComponent<TextMeshProUGUI>();
        labelText.text = "S" + segmentObjects.Count; // Affiche le numéro du segment
        labelText.fontSize = 14; // Taille de la police
        labelText.alignment = TextAlignmentOptions.Center;

        // Positionner le label au-dessus de la ligne
        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.sizeDelta = new Vector2(60, 20); // Ajuster la taille du label
        labelRect.anchoredPosition = new Vector2(0, 20); // Positionner le label au-dessus de la ligne

        segmentObjects.Add(lineObject);
        return lineObject;
    }

    private void CreateSegmentsUI()
    {
        foreach (var segment in levelData.segments)
        {
            CreateSimpleLine(segment.Item1, segment.Item2);
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Ajouter un Point"))
        {
            Vector2 newPoint = GenerateRandomPointInsidePlayArea();
            levelData.AddPoint(newPoint);
            EditorUtility.SetDirty(levelData);
            AddPointToUI(newPoint, levelData.points.Count - 1);
        }

        EditorGUILayout.LabelField("Points dans le Niveau:");
        for (int i = 0; i < levelData.points.Count; i++)
        {
            levelData.points[i] = EditorGUILayout.Vector2Field("Point " + i, levelData.points[i]);
            UpdatePointPositionInUI(i);
        }

        EditorGUILayout.LabelField("Créer un Segment entre deux points:");
        selectedPointA = EditorGUILayout.Popup("Point A", selectedPointA, GetPointNames());
        selectedPointB = EditorGUILayout.Popup("Point B", selectedPointB, GetPointNames());

        if (GUILayout.Button("Ajouter Segment"))
        {
            if (selectedPointA != selectedPointB && selectedPointA >= 0 && selectedPointB >= 0)
            {
                Vector2 pointA = levelData.points[selectedPointA];
                Vector2 pointB = levelData.points[selectedPointB];

                levelData.AddSegment(pointA, pointB);
                EditorUtility.SetDirty(levelData);
                CreateSimpleLine(pointA, pointB);
            }
            else
            {
                Debug.LogWarning("Sélectionner deux points différents.");
            }
        }

        EditorGUILayout.LabelField("Segments du Niveau:");
        for (int i = 0; i < levelData.segments.Count; i++)
        {
            var segment = levelData.segments[i];
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Segment {i}: {segment.Item1} -> {segment.Item2}");

            if (GUILayout.Button("Supprimer"))
            {
                RemoveSegment(segment.Item1, segment.Item2);
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Réinitialiser les Segments"))
        {
            ResetSegments();
        }
    }

    private void CreatePointsUI()
    {
        for (int i = 0; i < levelData.points.Count; i++)
        {
            AddPointToUI(levelData.points[i], i);
        }
    }

    private Vector2 GenerateRandomPointInsidePlayArea()
    {
        return new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

    private void UpdatePointPositionInUI(int index)
    {
        if (index < pointObjects.Count)
        {
            GameObject pointObject = pointObjects[index];
            pointObject.GetComponent<RectTransform>().anchoredPosition = levelData.points[index];
        }
    }

    private void ResetSegments()
    {
        levelData.ClearSegments();

        foreach (var segmentObject in segmentObjects)
        {
            DestroyImmediate(segmentObject);
        }

        segmentObjects.Clear();
        EditorUtility.SetDirty(levelData);
    }

    private void RemoveSegment(Vector2 pointA, Vector2 pointB)
    {
        int index = levelData.segments.FindIndex(seg => seg.Item1 == pointA && seg.Item2 == pointB);
        if (index >= 0)
        {
            levelData.RemoveSegment(pointA, pointB);
            DestroyImmediate(segmentObjects[index]);
            segmentObjects.RemoveAt(index);
            EditorUtility.SetDirty(levelData);
        }
    }

    private string[] GetPointNames()
    {
        string[] pointNames = new string[levelData.points.Count];
        for (int i = 0; i < levelData.points.Count; i++)
            pointNames[i] = "Point " + i;

        return pointNames;
    }
}
