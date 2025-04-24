using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class Puzzle2D : MonoBehaviour
{
    #region Fields
    // Level data
    public LevelData _levelData;

    private Dictionary<Color, Dictionary<(Vector2, Vector2), GameObject>> _activeColoredLinesByColor;

    // Camera
    [SerializeField] private Camera _puzzleCamera;
    [SerializeField] private Cinemachine.CinemachineVirtualCamera _cinemachineMainCamera;
    [SerializeField] private Camera _mainCamera;

    //
    private Vector2? _currentSelected = null;

    // 3D part
    private Dictionary<GameObject, Vector2> _cubePositions3D = new Dictionary<GameObject, Vector2>();
    private List<Transform> _pointObjects3D = new List<Transform>();
    private Transform _segmentsParent;

    public Transform parentPuzzleGroup;

    // Resolve
    private bool _puzzleSolved = true;

    // Drag and drop
    private bool _isDragging = false;
    private Transform _currentStartDragPoint;
    private GameObject _tempCylinder;
    private float _dragThreshold = 1f;

    // Visual size and offset
    [Header("3D Display Settings")]
    [SerializeField] private float _scaleFactorX = 30f;
    [SerializeField] private float _scaleFactorY = 30f;
    [SerializeField] private float _yOffset = 10f;
    [SerializeField] private float _zOffset = 54f;

    [SerializeField] private float _cubeScale = 0.5f;
    [SerializeField] private float _redLineRadius = 0.1f;
    [SerializeField] private float _coloredLineRadius = 0.2f;

    // Circuit / Eraser
    private Dictionary<int, bool> _circuitValidationStatus = new Dictionary<int, bool>();

    // Prefab of Stars/ Prefab of Circuit Shape
    [Header("Prefabs")]
    public GameObject _starPrefab; 
    public List<GameObject> _circuitShapePrefabs;
    public GameObject _segmentPrefab;


    private Vector3 _fingerMP = Vector3.zero;

    #endregion

    void Start()
    {
        _segmentsParent = new GameObject("Segments3D").transform;
        _segmentsParent.SetParent(parentPuzzleGroup);
        
        InstantiatePoints3D();
        InstantiateSegments();
        
        ETouch.Touch.onFingerMove += Touch_OnFingerMove;
        ETouch.Touch.onFingerDown += Touch_OnFingerDown;
        ETouch.Touch.onFingerUp += Touch_OnFingerUp;
    }

    private void Touch_OnFingerMove(Finger TouchedFinger)
    {
        _fingerMP = TouchedFinger.screenPosition;

        // On mouse move
    }

    private void Touch_OnFingerDown(Finger TouchedFinger)
    {
        _fingerMP = TouchedFinger.screenPosition;


        float minDistance = Mathf.Infinity;
        GameObject closestObject = null;

        foreach (var kvp in _cubePositions3D)
        {
            GameObject obj = kvp.Key;


        }

        float maxRange = 3f;

    }

    private void Touch_OnFingerUp(Finger TouchedFinger)
    {
        Debug.Log("RELACHEMENT");

        if (!_isDragging)
            return;

        _isDragging = false;
    }

    private void SwitchCamera(bool activatePuzzleCamera)
    {
        if (_mainCamera == null || _cinemachineMainCamera == null || _puzzleCamera == null) return;

        _cinemachineMainCamera.gameObject.SetActive(!activatePuzzleCamera);
        _mainCamera.gameObject.SetActive(!activatePuzzleCamera);
        _puzzleCamera.gameObject.SetActive(activatePuzzleCamera);
    }

    // 3D Visual *** 

    private void InstantiatePoints3D()
    {
        foreach (Vector2 pos in _levelData._points)
        {
            GameObject cube = Instantiate(_starPrefab, parentPuzzleGroup);
            cube.name = $"Point3D_{pos.x}_{pos.y}";
            cube.transform.position = new Vector3(0f, pos.y / _scaleFactorY + _yOffset, -pos.x / _scaleFactorX - _zOffset);
            cube.transform.localScale = Vector3.one * _cubeScale;

            PointSizeEntry pointSizeEntry = _levelData.pointSizes.FirstOrDefault(p => p.pointPosition == pos);
            if (pointSizeEntry != null)
            {
                switch (pointSizeEntry.size)
                {
                    case PointSize.Petite:
                        cube.transform.localScale = Vector3.one * 30.0f; 
                        break;
                    case PointSize.Moyenne:
                        cube.transform.localScale = Vector3.one * 43.0f;    
                        break;
                    case PointSize.Grosse:
                        cube.transform.localScale = Vector3.one * 65.0f;    
                        break;
                    default:
                        cube.transform.localScale = Vector3.one * 30.0f; // Petite par défaut
                        break;
                }
            }
            else
            {
                cube.transform.localScale = Vector3.one * 30.0f;  
            }

            var rend = cube.GetComponent<Renderer>();

            // Change shape depend on if start point or end point exist for this point
            foreach (var circuit in _levelData._circuits)
            {
                if (int.TryParse(circuit.sign, out int prefabIndex) && prefabIndex > 0 && prefabIndex <= _circuitShapePrefabs.Count)
                {
                    if (circuit.startPoint == pos)
                    {
                        GameObject startPrefab = Instantiate(_circuitShapePrefabs[prefabIndex - 1], parentPuzzleGroup);
                        startPrefab.transform.position = cube.transform.position + circuit.startPointOffset;

                    }

                    if (circuit.endPoint == pos)
                    {
                        GameObject endPrefab = Instantiate(_circuitShapePrefabs[prefabIndex - 1], parentPuzzleGroup);
                        endPrefab.transform.position = cube.transform.position + circuit.endPointOffset;
                    }
                }
                else
                {
                    Debug.LogWarning($"⚠️ Circuit avec sign '{circuit.sign}' : Sign non valide ou hors limites.");
                }
            }

            cube.tag = "CasseTete";

            _pointObjects3D.Add(cube.transform);
            _cubePositions3D[cube] = pos;

            // var clickScript = cube.AddComponent<ClickablePointComponent>();
            // clickScript.puzzle = this;
            // clickScript.linked2DPoint = pos;
        }
    }

    private void InstantiateSegments()
    {
        foreach (var segment in _levelData._segments)
        {
            Vector2 a = segment.pointA;
            Vector2 b = segment.pointB;
            Draw3DLine(a, b);
        }
    }

    private void Draw3DLine(Vector2 a, Vector2 b)
    {
        Vector3 aPos = new Vector3(0.1f, (a.y / _scaleFactorY) + _yOffset, (-a.x / _scaleFactorX) - _zOffset);
        Vector3 bPos = new Vector3(0.1f, (b.y / _scaleFactorY) + _yOffset, (-b.x / _scaleFactorX) - _zOffset);

        Vector3 dir = bPos - aPos;
        float distance = dir.magnitude;

        if (_segmentPrefab == null || parentPuzzleGroup == null)
        {
            Debug.LogWarning("segmentPrefab ou parentPuzzleGroup n’est pas assigné !");
            return;
        }

        GameObject segment = Instantiate(_segmentPrefab, parentPuzzleGroup);
        segment.name = "SegmentCylinder";

        segment.transform.position = aPos + dir / 2f;
        segment.transform.up = dir.normalized;

        segment.transform.localScale = new Vector3(_redLineRadius, distance / 2f, _redLineRadius);
    }


    private GameObject DrawColored3DSegment(Vector2 a, Vector2 b, Color color)
    {
        // Vérification d'existence du segment avec n'importe quelle couleur
        foreach (var kvp in _activeColoredLinesByColor)
        {
            var segmentDict = kvp.Value;
            var existingColor = kvp.Key;

            // AB
            if (segmentDict.TryGetValue((a, b), out GameObject existingObj))
            {
                if (existingColor == color)
                {
                    Debug.Log($"⚠️ Segment déjà existant entre {a} et {b} avec la même couleur.");
                    return null;
                }

                Debug.Log($"🗑️ Segment entre {a} et {b} existant avec une autre couleur ({existingColor}). Suppression...");
                Destroy(existingObj);
                segmentDict.Remove((a, b));
                break;
            }

            // BA
            if (segmentDict.TryGetValue((b, a), out existingObj))
            {
                if (existingColor == color)
                {
                    Debug.Log($"⚠️ Segment déjà existant entre {b} et {a} avec la même couleur.");
                    return null;
                }

                Debug.Log($"🗑️ Segment entre {b} et {a} existant avec une autre couleur ({existingColor}). Suppression...");
                Destroy(existingObj);
                segmentDict.Remove((b, a));
                break;
            }
        }

        // Conversion des points en 3D
        Vector3 aPos = new Vector3(0.1f, (a.y / _scaleFactorY) + _yOffset, (-a.x / _scaleFactorX) - _zOffset);
        Vector3 bPos = new Vector3(0.1f, (b.y / _scaleFactorY) + _yOffset, (-b.x / _scaleFactorX) - _zOffset);

        Vector3 dir = bPos - aPos;
        float distance = dir.magnitude;

        if (_segmentPrefab == null || parentPuzzleGroup == null)
        {
            Debug.LogWarning("segmentPrefab ou parentPuzzleGroup n’est pas assigné !");
            return null;
        }

        // Création du segment depuis le prefab
        GameObject segment = Instantiate(_segmentPrefab, parentPuzzleGroup);
        segment.name = "ColoredSegmentCylinder";

        segment.transform.position = aPos + dir / 2f;
        segment.transform.up = dir.normalized;
        segment.transform.localScale = new Vector3(_coloredLineRadius, distance / 2f, _coloredLineRadius);

        Renderer rend = segment.GetComponent<Renderer>();
        if (rend != null)
        {
            Material mat = rend.material;
            mat.EnableKeyword("_EMISSION"); // Active l’émission
            Color emissionColor = color * 1.0f ; // Exemple : intensity = 1.5f
            mat.SetColor("_EmissionColor", emissionColor);
        }

        if (!_activeColoredLinesByColor.ContainsKey(color))
            _activeColoredLinesByColor[color] = new Dictionary<(Vector2, Vector2), GameObject>();

        _activeColoredLinesByColor[color][(a, b)] = segment;

        return segment;
    }
    
    // 3D Visual *

}
