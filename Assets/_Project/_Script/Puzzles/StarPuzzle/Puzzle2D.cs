using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class Puzzle2D : MonoBehaviour
{
    private static readonly int s_emissionColor = Shader.PropertyToID("_EmissionColor");

    #region Fields

    private SoundSystem _soundSystem;

    private string[] _validSFXkey;

    public LevelData levelData;

    [SerializeField] private CinemachineVirtualCamera puzzleCamera;
    private Camera _mainCamera;
    
    private readonly Dictionary<Color, Dictionary<(Vector2, Vector2), GameObject>> _activeColoredLinesByColor = new Dictionary<Color, Dictionary<(Vector2, Vector2), GameObject>>();
    
    private readonly Dictionary<GameObject, Vector2> _cubePositions3D = new Dictionary<GameObject, Vector2>();
    private readonly List<Transform> _pointObjects3D = new List<Transform>();
    
    private Transform _segmentsParent;
    public Transform parentPuzzleGroup;

    private bool _isDragging = false;
    private Transform _currentStartDragPoint;
    private GameObject _tempCylinder;
    private const float DragThreshold = 1f;

    [Header("3D Display Settings")]
    [SerializeField] private float scaleFactorX = 30f;
    [SerializeField] private float scaleFactorY = 30f;

    [SerializeField] private float cubeScale = 0.5f;
    [SerializeField] private float redLineRadius = 0.1f;
    [SerializeField] private float coloredLineRadius = 0.2f;

    private readonly Dictionary<int, bool> _circuitValidationStatus = new Dictionary<int, bool>();
    List<bool> _checks = new List<bool>();

    [Header("Prefabs")]
    public GameObject starPrefab; 
    public List<GameObject> circuitShapePrefabs;
    public GameObject segmentPrefab;
    
    private Vector3 _fingerMP = Vector3.zero;
    private Vector3 _mouseWorldPosition;

    private DrawingColors _drawingColor;
    #endregion

    private void Awake()
    {
        StarPuzzleManager.Instance.PuzzleCamera = puzzleCamera;
        _drawingColor = StarPuzzleManager.Instance.DrawingColor;
        _mainCamera = Camera.main;

        _soundSystem = GameManager.Instance.GetSoundSystem();

        _validSFXkey = new string[]
        {
        "Chimes Chime B", "Chimes Chime C", "Chimes Chime D", "Chimes Chime E"
        };

    }
    
    private void Start()
    {
        StarPuzzleManager.Instance.Circuits = new List<bool>(new bool[levelData._circuits.Count]);
    
        _segmentsParent = new GameObject("Segments3D").transform;
        _segmentsParent.SetParent(parentPuzzleGroup);
        
        puzzleCamera.gameObject.SetActive(false);

        InstantiatePoints3D();
        InstantiateSegments();
        
        Vector3 rotation = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + 180, transform.rotation.eulerAngles.z);
        RotatePuzzle(rotation);
    }
    
    private void OnEnable()
    {
        if (StarPuzzleManager.HasInstance)
        {
            StarPuzzleManager.Instance.OnPuzzleEnter += HandlePuzzleEnter;
            StarPuzzleManager.Instance.OnPuzzleExit += HandlePuzzleExit;
        }
    }

    private void OnDisable()
    {
        if (StarPuzzleManager.HasInstance)
        {
            StarPuzzleManager.Instance.OnPuzzleEnter -= HandlePuzzleEnter;
            StarPuzzleManager.Instance.OnPuzzleExit -= HandlePuzzleExit;
        }
    }

    private void HandlePuzzleEnter()
    {
        GameManager.Instance.GetStateManager().ChangeState(StateManager.PlayerState.Puzzle);
        ETouch.Touch.onFingerMove += Touch_OnFingerMove;
        ETouch.Touch.onFingerDown += Touch_OnFingerDown;
        ETouch.Touch.onFingerUp += Touch_OnFingerUp;
    }
    
    private void HandlePuzzleExit()
    {
        GameManager.Instance.GetStateManager().ChangeState(StateManager.PlayerState.Idle);
        ETouch.Touch.onFingerMove -= Touch_OnFingerMove;
        ETouch.Touch.onFingerDown -= Touch_OnFingerDown;
        ETouch.Touch.onFingerUp -= Touch_OnFingerUp;
    }
    
    private void Touch_OnFingerMove(Finger touchedFinger)
    {
        _fingerMP = touchedFinger.screenPosition;

        UpdateDragging();
    }

    private void Touch_OnFingerDown(Finger touchedFinger)
    {
        _fingerMP = touchedFinger.screenPosition;

        Vector3 hitPosition = GetMouseHitPosition(_mainCamera);

        float minDistance = Mathf.Infinity;
        GameObject closestObject = null;

        foreach (var kvp in _cubePositions3D)
        {
            GameObject obj = kvp.Key;
            float dist = Vector3.Distance(obj.transform.position, hitPosition);

            if (dist < minDistance)
            {
                minDistance = dist;
                closestObject = obj;
            }
        }

        float maxRange = 3f;

        if (closestObject == null) return;
        
        float distanceToClosest = Vector3.Distance(closestObject.transform.position, hitPosition);
        
        if (distanceToClosest > maxRange) return;
        
        var pointComponent = closestObject.GetComponent<ClickablePointComponent>();
        pointComponent.TriggerMouseDownByDistance();
    }

    private void Touch_OnFingerUp(Finger touchedFinger)
    {
        if (!_isDragging)
            return;

        _isDragging = false;
        HandleMouseRelease();
    }
    
    private void InstantiatePoints3D()
    {
        foreach (Vector2 pos in levelData._points)
        {
            GameObject cube = Instantiate(starPrefab, parentPuzzleGroup);
            cube.name = $"Point3D_{pos.x}_{pos.y}";
            cube.transform.position = new Vector3(0f + transform.position.x ,(pos.y / scaleFactorY) + transform.position.y, (-pos.x / scaleFactorX) + transform.position.z);
            cube.transform.localScale = Vector3.one * cubeScale;

            PointSizeEntry pointSizeEntry = levelData.pointSizes.FirstOrDefault(p => p.pointPosition == pos);
            if (pointSizeEntry != null)
            {
                switch (pointSizeEntry.size)
                {
                    case PointSize.Moyenne:
                        cube.transform.localScale = Vector3.one * 43.0f;    
                        break;
                    case PointSize.Grosse:
                        cube.transform.localScale = Vector3.one * 65.0f;    
                        break;
                    default:
                        cube.transform.localScale = Vector3.one * 30.0f;
                        break;
                }
            }
            else   
            {
                cube.transform.localScale = Vector3.one * 30.0f;  
            }
            
            foreach (var circuit in levelData._circuits)
            {
                if (int.TryParse(circuit.sign, out int prefabIndex) && prefabIndex > 0 && prefabIndex <= circuitShapePrefabs.Count)
                {
                    if (circuit.startPoint == pos)
                    {
                        GameObject startPrefab = Instantiate(circuitShapePrefabs[prefabIndex - 1], parentPuzzleGroup);
                        startPrefab.transform.position = cube.transform.position + circuit.startPointOffset;

                    }

                    if (circuit.endPoint == pos)
                    {
                        GameObject endPrefab = Instantiate(circuitShapePrefabs[prefabIndex - 1], parentPuzzleGroup);
                        endPrefab.transform.position = cube.transform.position + circuit.endPointOffset;
                    }
                }
                else
                {
                    Debug.LogWarning($"Circuit with sign '{circuit.sign}' : sign invalid or out of bound");
                }
            }

            cube.tag = "CasseTete";

            _pointObjects3D.Add(cube.transform);
            _cubePositions3D[cube] = pos;
            
            var clickScript = cube.AddComponent<ClickablePointComponent>();
            clickScript.puzzle = this;
            clickScript.linked2DPoint = pos;
        }
    }

    private void InstantiateSegments()
    {
        foreach (var segment in levelData._segments)
        {
            Vector2 a = segment.pointA;
            Vector2 b = segment.pointB;
            Draw3DLine(a, b);
        }
    }

    private void Draw3DLine(Vector2 a, Vector2 b)
    {
        Vector3 aPos = new Vector3(0.1f + transform.position.x, (a.y / scaleFactorY) + transform.position.y, (-a.x / scaleFactorX) + transform.position.z);
        Vector3 bPos = new Vector3(0.1f + transform.position.x, (b.y / scaleFactorY) + transform.position.y, (-b.x / scaleFactorX) + transform.position.z);

        Vector3 dir = bPos - aPos;
        float distance = dir.magnitude;

        if (segmentPrefab == null || parentPuzzleGroup == null)
        {
            Debug.LogWarning("segmentPrefab or parentPuzzleGroup isn't assigned");
            return;
        }

        GameObject segment = Instantiate(segmentPrefab, parentPuzzleGroup);
        segment.name = "SegmentCylinder";

        segment.transform.position = aPos + dir / 2f;
        segment.transform.up = dir.normalized;

        segment.transform.localScale = new Vector3(redLineRadius, distance / 2f, redLineRadius);
    }
    
    private void DrawColored3DSegment(Vector2 a, Vector2 b, Color color)
    {
        foreach (var (existingColor, segmentDict) in _activeColoredLinesByColor)
        {
            // AB
            if (segmentDict.TryGetValue((a, b), out GameObject existingObj))
            {
                if (existingColor == color)
                {
                    Debug.Log($"Segment already existing between {a} and {b} with the same color.");
                }

                Debug.Log($"Segment between {a} and {b} already exist with another color ({existingColor}). Suppression...");
                Destroy(existingObj);
                segmentDict.Remove((a, b));
                break;
            }

            // BA
            if (segmentDict.TryGetValue((b, a), out existingObj))
            {
                if (existingColor == color)
                {
                    Debug.Log($"Segment already existing between {a} and {b} with the same color.");
                }
            
                Debug.Log($"Segment between {a} and {b} already exist with another color ({existingColor}). Suppression...");
                Destroy(existingObj);
                segmentDict.Remove((b, a));
                break;
            }
        }
        
        Vector3 aPos = new Vector3(0.1f, a.y / scaleFactorY, -a.x / scaleFactorX);
        Vector3 bPos = new Vector3(0.1f, b.y / scaleFactorY, -b.x / scaleFactorX);

        Vector3 dir = bPos - aPos;
        float distance = dir.magnitude;

        GameObject segment = Instantiate(segmentPrefab, parentPuzzleGroup);
        segment.name = "ColoredSegmentCylinder";
        
        segment.transform.localPosition = aPos + dir / 2f;
        segment.transform.up = dir.normalized;
        segment.transform.rotation = Quaternion.Euler(segment.transform.rotation.eulerAngles.x, segment.transform.rotation.eulerAngles.y + 225f, segment.transform.rotation.eulerAngles.z);
        segment.transform.localScale = new Vector3(coloredLineRadius, distance / 2f, coloredLineRadius);

        Renderer rend = segment.GetComponent<Renderer>();
        if (rend != null)
        {
            Material mat = rend.material;
            mat.EnableKeyword("_EMISSION");
            Color emissionColor = color * 1.0f ;
            mat.SetColor(s_emissionColor, emissionColor);
        }

        if (!_activeColoredLinesByColor.ContainsKey(color))
            _activeColoredLinesByColor[color] = new Dictionary<(Vector2, Vector2), GameObject>();

        _activeColoredLinesByColor[color][(a, b)] = segment;

    }
    
    private void RotatePuzzle(Vector3 rotation)
    {
        parentPuzzleGroup.localEulerAngles = rotation;
    }
    
    private void UpdateDragging()
    {

        if (_isDragging)
        {
            Vector3 mouse3D = Vector3.zero;
            Vector3 mouseScreen = _fingerMP;
            Ray ray = _mainCamera.ScreenPointToRay(mouseScreen);
            Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red, 20f);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            if (hits.Length > 0)
            {
                bool hitPlan = false;

                foreach (RaycastHit h in hits)
                {
                    if (h.collider.CompareTag("CTRP"))
                    {
                        mouse3D = h.point;
                        hitPlan = true;
                        break;
                    }
                }

                if (!hitPlan)
                    Debug.Log("No object hit has CTRP tag.");
            }
            else
            {
                Debug.Log("RaycastAll hit nothing");
            }

            Vector3 start = _currentStartDragPoint.position;
            Vector3 end = mouse3D;

            // Dir
            Vector3 dir = end - start;
            float distance = dir.magnitude;
            Vector3 dirNormalized = dir.normalized;

            // 
            Vector3 midPoint = start + dirNormalized * (distance / 2f);

            // Cylinder
            _tempCylinder.transform.position = midPoint;
            _tempCylinder.transform.up = dirNormalized; 
            _tempCylinder.transform.localScale = new Vector3(0.1f, distance / 2f, 0.1f);
            _tempCylinder.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    private void OnDrawGizmos()
    {
        if (_isDragging)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_mouseWorldPosition, 0.1f);
        }
    }


    void HandleMouseRelease()
    {

        Vector3 mouse3D = Vector3.zero;

        Vector3 mouseScreen = _fingerMP;

        Ray ray = _mainCamera.ScreenPointToRay(mouseScreen);

        Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red, 20f);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        if (hits.Length > 0)
        {
            bool hitPlan = false;

            foreach (RaycastHit h in hits)
            {
                if (h.collider.CompareTag("CTRP"))
                {
                    mouse3D = h.point;
                    hitPlan = true;
                    break; 
                }
            }

            if (!hitPlan)
                Debug.Log("No object hit has CTRP tag.");
        }
        else
        {
            Debug.Log("RaycastAll hit nothing");
        }
        
        float dragDistance = Vector3.Distance(_currentStartDragPoint.position, mouse3D);

        if (dragDistance < DragThreshold)
        {
            Debug.Log("Threshold too low, no segment drawn.");
            Destroy(_tempCylinder);
            _tempCylinder = null;
            return;
        }

        const float detectionRadius = 20f;
        Transform targetPoint = null;
        Vector2 linked2DPoint = Vector2.zero;

        float shortestDistance = float.MaxValue;
        Transform shortestPoint = null;

        foreach (var pointTransform in _pointObjects3D)
        {
            float distance = Vector3.Distance(mouse3D, pointTransform.position);

            if (_cubePositions3D.TryGetValue(pointTransform.gameObject, out Vector2 point2D) && distance < shortestDistance)
            {
                shortestDistance = distance;
                shortestPoint = pointTransform;
                linked2DPoint = point2D;
            }
        }

        if (shortestPoint != null && Vector3.Distance(mouse3D, shortestPoint.position) < detectionRadius)
        {
            targetPoint = shortestPoint;
        }
        else
        {
            if (shortestPoint == null)
            {
                Debug.Log("shortestPoint is null.");
            }
            else
            {
                Debug.Log($"Distance too large: {Vector3.Distance(mouse3D, shortestPoint.position)} > {detectionRadius}");
            }
        }

        if (targetPoint)
        {
            var startPoint = _currentStartDragPoint.gameObject.GetComponent<ClickablePointComponent>();
            Vector2 startLinked2DPoint = startPoint != null ? startPoint.linked2DPoint : Vector2.zero;

            bool isSegmentRed = levelData._segments.Exists(segment =>
                (segment.pointA == startLinked2DPoint && segment.pointB == linked2DPoint) ||
                (segment.pointA == linked2DPoint && segment.pointB == startLinked2DPoint)
            );

            if (isSegmentRed)
            {
                Color circuitColor = GetColorFromDrawingColor();

                if (_drawingColor == DrawingColors.Eraser)
                {
                    foreach (var kvp in _activeColoredLinesByColor)
                    {
                        var segmentDict = kvp.Value;
                        var existingColor = kvp.Key;

                        // AB
                        if (segmentDict.TryGetValue((startLinked2DPoint, linked2DPoint), out GameObject existingObj))
                        {
                            RemoveColoredLine(startLinked2DPoint, linked2DPoint, existingColor);
                            break;
                        }
                        // BA
                        else if (segmentDict.TryGetValue((linked2DPoint, startLinked2DPoint), out existingObj))
                        {
                            RemoveColoredLine(linked2DPoint, startLinked2DPoint, existingColor);
                            break;
                        }
                    }
                }
                else
                {
                    Debug.Log($"Connexion validated with point : {targetPoint.name} (2D: {linked2DPoint})");

                    DrawColored3DSegment(startLinked2DPoint, linked2DPoint, circuitColor);
                    _soundSystem.PlayRandomSoundFXClipByKeys(_validSFXkey, transform.position);

                }
                Destroy(_tempCylinder);

            }
            else
            {
                Debug.Log("No valid segment found, cylinder destroyed.");
                Destroy(_tempCylinder);
            }

        }
        else
        {
            if (_tempCylinder != null)
            {
                Destroy(_tempCylinder);
                Debug.Log("No valid segment found, cylinder destroyed.");
            }
        }

        _tempCylinder = null;
        CheckPuzzleSolved();
    }

    private Color GetColorFromDrawingColor()
    {
        Color circuitColor;
        _drawingColor = StarPuzzleManager.Instance.DrawingColor;
        switch (_drawingColor)
        {
            case DrawingColors.Red:
                circuitColor = Color.red;
                break;
            case DrawingColors.Blue:
                circuitColor = Color.blue;
                break;
            case DrawingColors.Brown:
                circuitColor = Color.yellow;
                break;
            case DrawingColors.Purple:
                circuitColor = Color.magenta;
                break;
            default:
                circuitColor = Color.green;
                break;
        }

        return circuitColor;
    }

    public void OnPointClicked3D(Vector2 point, Transform startTransform)
    {
        _tempCylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        _tempCylinder.GetComponent<MeshRenderer>().enabled = false;
        _tempCylinder.transform.position = startTransform.position;
        _tempCylinder.transform.rotation = Quaternion.identity;
        _currentStartDragPoint = startTransform;
        _isDragging = true; 
    }

    private void RemoveColoredLine(Vector2 a, Vector2 b, Color color)
    {
        if (_activeColoredLinesByColor.ContainsKey(color))
        {
            if (_activeColoredLinesByColor[color].ContainsKey((a, b)))
            {
                Destroy(_activeColoredLinesByColor[color][(a, b)]);
                _activeColoredLinesByColor[color].Remove((a, b));
            }

            if (_activeColoredLinesByColor[color].ContainsKey((b, a)))
            {
                Destroy(_activeColoredLinesByColor[color][(b, a)]);
                _activeColoredLinesByColor[color].Remove((b, a));
            }
        }
    }

    private DrawingColors GetCircuitColor()
    {
        return _drawingColor;
    }
    
    private bool HasCommonPoint((Vector2, Vector2) segment1, (Vector2, Vector2) segment2)
    {
        if (segment1.Item1 == segment2.Item1 || segment1.Item1 == segment2.Item2)
        {
            return true;
        }
        if (segment1.Item2 == segment2.Item1 || segment1.Item2 == segment2.Item2)
        {
            return true;
        }

        return false;
    }
    public List<List<(Vector2, Vector2)>> FindSegmentChainsByColor(Color color)
    {
        List<List<(Vector2, Vector2)>> chains = new List<List<(Vector2, Vector2)>>();

        if (!_activeColoredLinesByColor.TryGetValue(color, value: out var coloredSegments))
        {
            return chains;
        }

        HashSet<(Vector2, Vector2)> visitedSegments = new HashSet<(Vector2, Vector2)>();

        void FollowChain((Vector2, Vector2) startSegment, List<(Vector2, Vector2)> chain)
        {
            if (visitedSegments.Contains(startSegment)) return;

            chain.Add(startSegment);
            visitedSegments.Add(startSegment);

            var currentEndPoint = startSegment.Item2;

            foreach (var segment in coloredSegments)
            {
                if (HasCommonPoint(startSegment, segment.Key) && !visitedSegments.Contains(segment.Key))
                {
                    FollowChain(segment.Key, chain);
                }
            }
        }

        foreach (var segment in coloredSegments)
        {
            if (visitedSegments.Contains(segment.Key)) continue;
            
            List<(Vector2, Vector2)> chain = new List<(Vector2, Vector2)>();
            FollowChain(segment.Key, chain);
            chains.Add(chain);
        }

        return chains;
    }


    public bool IsChainConnectingPoints(Color searchColor, Vector2 startPoint, Vector2 endPoint)
    {
        List<List<(Vector2, Vector2)>> chains = FindSegmentChainsByColor(searchColor);

        foreach (var chain in chains)
        {
            if (chain.Count <= 1) continue;
            
            Vector2 firstSegmentStart = chain[0].Item1;
            Vector2 firstSegmentEnd = chain[0].Item2;
            Vector2 lastSegmentStart = chain[chain.Count - 1].Item1; 
            Vector2 lastSegmentEnd = chain[chain.Count - 1].Item2; 

            bool isFirstSegmentConnected = (firstSegmentStart == startPoint || firstSegmentStart == endPoint) || (firstSegmentEnd == startPoint || firstSegmentEnd == endPoint);
            bool isLastSegmentConnected = (lastSegmentStart == startPoint || lastSegmentStart == endPoint) || (lastSegmentEnd == startPoint || lastSegmentEnd == endPoint);

            if (!isFirstSegmentConnected || !isLastSegmentConnected) continue;
                
            bool firstSegmentHasStartPoint = (firstSegmentStart == startPoint || firstSegmentEnd == startPoint);
            bool lastSegmentHasEndPoint = (lastSegmentStart == endPoint || lastSegmentEnd == endPoint);
            bool firstSegmentHasEndPoint = (firstSegmentStart == endPoint || firstSegmentEnd == endPoint);
            bool lastSegmentHasStartPoint = (lastSegmentStart == startPoint || lastSegmentEnd == startPoint);

            bool isValidConnection = (firstSegmentHasStartPoint && lastSegmentHasEndPoint) || (firstSegmentHasEndPoint && lastSegmentHasStartPoint);

            if (isValidConnection)
            {
                Debug.Log("Segments are connected");
                return true;
            }
            Debug.Log("Segments aren't connected");
        }

        return false;
    }

    private Vector3 GetMouseHitPosition(Camera currentCamera, string targetTag = "CTRP", float rayLength = 1000f)
    {
        Vector3 mouse3D = Vector3.zero;
        Vector2 mouseScreen = _fingerMP;

        Ray ray = currentCamera.ScreenPointToRay(mouseScreen);
        Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.red, 2f);

        RaycastHit[] hits = Physics.RaycastAll(ray, rayLength);

        if (hits.Length <= 0) return mouse3D;
        
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag(targetTag))
            {
                return hit.point;
            }
        }
        return mouse3D;
    }
    
    private void CheckPuzzleSolved()
    {
        _checks.Clear();
        for (int i = 0; i < levelData._circuits.Count; i++)
        {
            Circuit currentCircuit = levelData._circuits[i];
            bool isValidConnection = IsChainConnectingPoints(currentCircuit.circuitColor, currentCircuit.startPoint, currentCircuit.endPoint);
            _checks.Add(isValidConnection);
            StarPuzzleManager.Instance.Circuits[i] = isValidConnection;
            _circuitValidationStatus[i] = isValidConnection;
        }
        
        if (_circuitValidationStatus.Values.All(v => v))
        {
            StarPuzzleManager.Instance.PuzzleComplete();
        }
    }
}