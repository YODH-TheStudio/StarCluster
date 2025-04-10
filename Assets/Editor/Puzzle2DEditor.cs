using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

[CustomEditor(typeof(LevelData))]
public class Puzzle2DEditor : Editor
{
    private LevelData _levelData;
    private Canvas _canvas;
    private RectTransform _playArea;
    private GameObject _editorParent;
    private List<GameObject> _pointObjects = new List<GameObject>();
    private List<GameObject> _segmentObjects = new List<GameObject>();
    private int _selectedPointA = -1, _selectedPointB = -1;

    private int _startCircuitPoint = -1, _endCircuitPoint = -1;
    private Color _circuitColor = Color.white;
    private string _circuitSign = "";

    private void OnEnable()
    {
        _levelData = (LevelData)target;
        _canvas = GameObject.FindObjectOfType<Canvas>();

        if (_canvas != null)
        {
            CreateEditorParent();
            CreateBackground();
            CreatePointsUI();
            CreateSegmentsUI();
        }

        EditorApplication.update += Repaint;
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        EditorApplication.update -= Repaint;
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        Event _e = Event.current;
        if (_e.type == EventType.KeyDown && _e.keyCode == KeyCode.P)
        {
            AddRandomPoint();
            _e.Use();
        }
    }

    private void CreateEditorParent()
    {
        _editorParent = new GameObject("Editor");
        _editorParent.transform.SetParent(_canvas.transform);
        RectTransform _rectTransform = _editorParent.AddComponent<RectTransform>();
        _rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
        _rectTransform.anchoredPosition = Vector2.zero;
    }

    private void CreateBackground()
    {
        GameObject _background = new GameObject("Background", typeof(Image));
        _background.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.8f);
        _playArea = _background.GetComponent<RectTransform>();
        _playArea.SetParent(_editorParent.transform);
        _playArea.sizeDelta = new Vector2(Screen.width * 0.6f, Screen.height * 0.8f);
        _playArea.anchorMin = _playArea.anchorMax = new Vector2(0.5f, 0.5f);
        _playArea.anchoredPosition = Vector2.zero;
    }

    private void AddPointToUI(Vector2 _point, int _index)
    {
        GameObject _pointObject = new GameObject("Point" + _index, typeof(Image));
        _pointObject.transform.SetParent(_playArea.transform);
        _pointObject.GetComponent<Image>().color = Color.green;

        RectTransform _rectTransform = _pointObject.GetComponent<RectTransform>();
        _rectTransform.sizeDelta = new Vector2(10, 10);
        _rectTransform.anchoredPosition = _point;

        GameObject _labelObject = new GameObject("Label" + _index, typeof(TextMeshProUGUI));
        _labelObject.transform.SetParent(_pointObject.transform);
        TextMeshProUGUI _labelText = _labelObject.GetComponent<TextMeshProUGUI>();
        _labelText.text = "P" + _index;
        _labelText.fontSize = 10;
        _labelText.alignment = TextAlignmentOptions.Center;

        RectTransform _labelRect = _labelObject.GetComponent<RectTransform>();
        _labelRect.sizeDelta = new Vector2(50, 20);
        _labelRect.anchoredPosition = new Vector2(0, 15);

        _pointObjects.Add(_pointObject);
        Canvas.ForceUpdateCanvases();
    }

    private GameObject CreateSimpleLine(Vector2 _pointA, Vector2 _pointB)
    {
        GameObject _lineObject = new GameObject("Line", typeof(Image));
        _lineObject.transform.SetParent(_playArea.transform);

        Image _lineImage = _lineObject.GetComponent<Image>();
        _lineImage.color = Color.red;

        RectTransform _rectTransform = _lineObject.GetComponent<RectTransform>();
        float _distance = Vector2.Distance(_pointA, _pointB);
        float _angle = Mathf.Atan2(_pointB.y - _pointA.y, _pointB.x - _pointA.x) * Mathf.Rad2Deg;

        _rectTransform.sizeDelta = new Vector2(_distance, 10f);
        _rectTransform.rotation = Quaternion.Euler(0, 0, _angle);
        _rectTransform.anchoredPosition = (_pointA + _pointB) / 2f;

        GameObject _labelObject = new GameObject("SegmentLabel", typeof(TextMeshProUGUI));
        _labelObject.transform.SetParent(_lineObject.transform);
        TextMeshProUGUI _labelText = _labelObject.GetComponent<TextMeshProUGUI>();

        int segmentIndex = _segmentObjects.Count;
        _labelText.text = "S" + segmentIndex;
        _labelText.fontSize = 10;
        _labelText.alignment = TextAlignmentOptions.Center;
        _labelText.color = Color.white;

        RectTransform _labelRect = _labelObject.GetComponent<RectTransform>();
        _labelRect.sizeDelta = new Vector2(30, 20);
        _labelRect.anchoredPosition = new Vector2(0, -10);

        _segmentObjects.Add(_lineObject);
        return _lineObject;
    }

    private void CreateSegmentsUI()
    {
        foreach (var _segment in _levelData._segments)
        {
            CreateSimpleLine(_segment.pointA, _segment.pointB);
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        List<string> pointOptions = new List<string>();
        for (int i = 0; i < _levelData._points.Count; i++)
        {
            pointOptions.Add("Point " + i);
        }

        if (_levelData._points.Count == 0)
        {
            EditorGUILayout.LabelField("Aucun point disponible pour le circuit.");
            return;
        }

        _startCircuitPoint = EditorGUILayout.Popup("Start Point", _startCircuitPoint, pointOptions.ToArray());
        _endCircuitPoint = EditorGUILayout.Popup("End Point", _endCircuitPoint, pointOptions.ToArray());
        _circuitColor = EditorGUILayout.ColorField("Couleur Circuit", _circuitColor);
        _circuitSign = EditorGUILayout.TextField("Signe", _circuitSign);

        if (GUILayout.Button("Ajouter Circuit"))
        {
            if (_startCircuitPoint == _endCircuitPoint)
            {
                EditorGUILayout.HelpBox("Le point de départ et le point d'arrivée ne peuvent pas être identiques.", MessageType.Error);
            }
            else
            {
                _levelData.AddCircuit(_circuitColor, _levelData._points[_startCircuitPoint], _levelData._points[_endCircuitPoint], _circuitSign);
            }
        }

        if (GUILayout.Button("Ajouter un Point"))
        {
            AddRandomPoint();
        }

        EditorGUILayout.LabelField("Points dans le Niveau:");
        for (int _i = 0; _i < _levelData._points.Count; _i++)
        {
            EditorGUILayout.BeginHorizontal();
            _levelData._points[_i] = EditorGUILayout.Vector2Field("Point " + _i, _levelData._points[_i]);

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                RemovePoint(_i);
            }

            EditorGUILayout.EndHorizontal();
            UpdatePointPositionInUI(_i);
        }

        EditorGUILayout.LabelField("Créer un Segment entre deux points:");
        _selectedPointA = EditorGUILayout.Popup("Point A", _selectedPointA, GetPointNames());
        _selectedPointB = EditorGUILayout.Popup("Point B", _selectedPointB, GetPointNames());

        if (GUILayout.Button("Ajouter Segment"))
        {
            if (_selectedPointA != _selectedPointB && _selectedPointA >= 0 && _selectedPointB >= 0)
            {
                Vector2 _pointA = _levelData._points[_selectedPointA];
                Vector2 _pointB = _levelData._points[_selectedPointB];

                _levelData.AddSegment(_pointA, _pointB);
                EditorUtility.SetDirty(_levelData);
                CreateSimpleLine(_pointA, _pointB);
            }
            else
            {
                Debug.LogWarning("Sélectionner deux points différents.");
            }
        }

        EditorGUILayout.LabelField("Segments du Niveau:");
        for (int _i = 0; _i < _levelData._segments.Count; _i++)
        {
            var _segment = _levelData._segments[_i];
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Segment {_i}: {_segment.pointA} -> {_segment.pointB}");

            if (GUILayout.Button("Supprimer"))
            {
                RemoveSegment(_segment.pointA, _segment.pointB);
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Réinitialiser les Segments"))
        {
            ResetSegments();
        }
    }

    private void RemoveCircuit(int index)
    {
        _levelData._circuits.RemoveAt(index);
        EditorUtility.SetDirty(_levelData);
    }

    private void CreatePointsUI()
    {
        for (int _i = 0; _i < _levelData._points.Count; _i++)
        {
            AddPointToUI(_levelData._points[_i], _i);
        }
    }

    private void AddRandomPoint()
    {
        Vector2 _newPoint = GenerateRandomPointInsidePlayArea();
        _levelData.AddPoint(_newPoint);
        EditorUtility.SetDirty(_levelData);
        AddPointToUI(_newPoint, _levelData._points.Count - 1);
    }

    private Vector2 GenerateRandomPointInsidePlayArea()
    {
        return new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

    private void UpdatePointPositionInUI(int _index)
    {
        if (_index < _pointObjects.Count)
        {
            GameObject _pointObject = _pointObjects[_index];
            _pointObject.GetComponent<RectTransform>().anchoredPosition = _levelData._points[_index];
        }
    }

    private void RemovePoint(int _index)
    {
        Vector2 _point = _levelData._points[_index];

        _levelData.RemovePoint(_point);
        DestroyImmediate(_pointObjects[_index]);
        _pointObjects.RemoveAt(_index);
        EditorUtility.SetDirty(_levelData);
    }

    private void ResetSegments()
    {
    }

    private void RemoveSegment(Vector2 _pointA, Vector2 _pointB)
    {
        int _index = _levelData._segments.FindIndex(seg => seg.pointA == _pointA && seg.pointB == _pointB);
        if (_index >= 0)
        {
            _levelData.RemoveSegment(_pointA, _pointB);
            DestroyImmediate(_segmentObjects[_index]);
            _segmentObjects.RemoveAt(_index);
            EditorUtility.SetDirty(_levelData);
        }
    }

    private string[] GetPointNames()
    {
        string[] _pointNames = new string[_levelData._points.Count];
        for (int _i = 0; _i < _levelData._points.Count; _i++)
            _pointNames[_i] = "Point " + _i;
        return _pointNames;
    }
}
