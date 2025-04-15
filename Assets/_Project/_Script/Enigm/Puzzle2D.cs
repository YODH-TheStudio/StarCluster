using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Puzzle2D : MonoBehaviour
{
    // Colors
    public Color _pointColor = Color.green;
    public Color _lineColor = Color.red;

    // Level data
    [SerializeField] Canvas _canvas;
    public LevelData _levelData;
    private GameObject _topMenuContainer;

    private Dictionary<(Vector2, Vector2), GameObject> _activeRedLines = new Dictionary<(Vector2, Vector2), GameObject>();
    private Dictionary<Color, Dictionary<(Vector2, Vector2), GameObject>> _activeColoredLinesByColor = new Dictionary<Color, Dictionary<(Vector2, Vector2), GameObject>>();

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

    // Resolve
    private int _currentCircuitSelected = 0;
    private bool _puzzleSolved = true;

    // Drag and drop
    private bool isDragging = false;
    private Transform currentStartDragPoint;
    private GameObject tempCylinder;
    private Vector3 mousePos;

    void Start()
    {
        CreateTopMenu();
        _segmentsParent = new GameObject("Segments3D").transform;
        InstantiatePoints3D();
        InstantiateSegments();
    }

    private void CreateTopMenu()
    {
        _topMenuContainer = new GameObject("TopMenuContainer");
        _topMenuContainer.transform.SetParent(_canvas.transform);

        RectTransform topMenuRect = _topMenuContainer.AddComponent<RectTransform>();
        topMenuRect.anchorMin = new Vector2(0.5f, 1);
        topMenuRect.anchorMax = new Vector2(0.5f, 1);
        topMenuRect.pivot = new Vector2(0.5f, 1);
        topMenuRect.anchoredPosition = new Vector2(0, -20);
        topMenuRect.sizeDelta = new Vector2(600, 50);

        HorizontalLayoutGroup layoutGroup = _topMenuContainer.AddComponent<HorizontalLayoutGroup>();
        layoutGroup.spacing = 10;
        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
        layoutGroup.childForceExpandWidth = false;

        for (int i = 0; i < _levelData._circuits.Count; i++)
        {
            CreateCircuitButton(i, _levelData._circuits[i]);
        }
    }

    private void CreateCircuitButton(int index, Circuit circuit)
    {
        GameObject buttonObj = new GameObject($"CircuitButton_{index}", typeof(Button), typeof(RectTransform), typeof(Text));
        buttonObj.transform.SetParent(_topMenuContainer.transform);

        Button button = buttonObj.GetComponent<Button>();
        Text buttonText = buttonObj.GetComponent<Text>();

        GameObject textContainer = new GameObject("TextContainer", typeof(RectTransform));
        textContainer.transform.SetParent(buttonObj.transform);
        RectTransform textContainerRect = textContainer.GetComponent<RectTransform>();
        textContainerRect.sizeDelta = new Vector2(200, 30);

        GameObject colorSquare = new GameObject("ColorSquare", typeof(Image));
        colorSquare.transform.SetParent(textContainer.transform);
        Image colorSquareImage = colorSquare.GetComponent<Image>();
        colorSquareImage.color = circuit.circuitColor;
        RectTransform colorSquareRect = colorSquare.GetComponent<RectTransform>();
        colorSquareRect.sizeDelta = new Vector2(20, 20);

        buttonText.text = $" {circuit.name} {index + 1}";
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        RectTransform textRect = buttonText.GetComponent<RectTransform>();
        textRect.anchoredPosition = new Vector2(30, 0);

        button.onClick.AddListener(() => OnCircuitButtonClicked(index));
    }

    private void OnCircuitButtonClicked(int index)
    {
        _currentCircuitSelected = index;
    }

    private void InstantiatePoints3D()
    {
        if (_mainCamera == null || _cinemachineMainCamera == null || _puzzleCamera == null)
            return;

        // Start/End point of circuit
        Dictionary<Vector2, Color> circuitPoints = new Dictionary<Vector2, Color>();
        foreach (var circuit in _levelData._circuits)
        {
            circuitPoints[circuit.startPoint] = circuit.circuitColor;
            circuitPoints[circuit.endPoint] = circuit.circuitColor;
        }

        _cinemachineMainCamera.gameObject.SetActive(false);
        _mainCamera.gameObject.SetActive(false);
        _puzzleCamera.gameObject.SetActive(true);

        foreach (Vector2 pos in _levelData._points)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = $"Point3D_{pos.x}_{pos.y}";
            cube.transform.position = new Vector3(0f, pos.y / 30f + 10f, -pos.x / 30f - 54f);
            cube.transform.localScale = Vector3.one * 0.5f;

            var rend = cube.GetComponent<Renderer>();

            if (rend) rend.material.color = _pointColor;

            // Change shape (color) depend on if start point or end point exist for this point
            if (circuitPoints.TryGetValue(pos, out Color circuitColor))
            {
                rend.material.color = circuitColor;
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
        foreach (var segment in _levelData._segments)
        {
            Vector2 a = segment.pointA;
            Vector2 b = segment.pointB;
            Draw3DLine(a, b);
        }
    }

    private Vector3 mouseWorldPosition;

    private void UpdateDragging()
    {

        if (isDragging)
        {
            Vector3 mouseScreen = Input.mousePosition;
            mouseScreen.z = Vector3.Distance(_puzzleCamera.transform.position, currentStartDragPoint.position);

            mouseWorldPosition = _puzzleCamera.ScreenToWorldPoint(mouseScreen);

            Vector3 start = currentStartDragPoint.position;
            Vector3 end = mouseWorldPosition;

            // Dir
            Vector3 dir = end - start;
            float distance = dir.magnitude;
            Vector3 dirNormalized = dir.normalized;

            // 
            Vector3 midPoint = start + dirNormalized * (distance / 2f);

            // Cylinder
            tempCylinder.transform.position = midPoint;
            tempCylinder.transform.up = dirNormalized; // Oriente le cylindre selon Y (axe principal du mesh)
            tempCylinder.transform.localScale = new Vector3(0.1f, distance / 2f, 0.1f); // Y = demi-distance
        }
    }

    private void OnDrawGizmos()
    {
        if (isDragging)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(mouseWorldPosition, 0.1f);
        }
    }

    void HandleMouseRelease()
    {
        if (!isDragging || !Input.GetMouseButtonUp(0))
            return;

        isDragging = false;

        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = Vector3.Distance(_puzzleCamera.transform.position, currentStartDragPoint.position);

        Vector3 mouse3D = _puzzleCamera.ScreenToWorldPoint(mouseScreen);
        mouse3D.x = 0;

        const float detectionRadius = 5f;
        Transform targetPoint = null;
        Vector2 linked2DPoint = Vector2.zero;

        // shortest point Distance
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

        if (targetPoint)
        {
            var startPoint = currentStartDragPoint.gameObject.GetComponent<ClickablePointComponent>();
            Vector2 startLinked2DPoint = startPoint != null ? startPoint.linked2DPoint : Vector2.zero;

            bool isSegmentRed = _levelData._segments.Exists(segment =>
                (segment.pointA == startLinked2DPoint && segment.pointB == linked2DPoint) ||
                (segment.pointA == linked2DPoint && segment.pointB == startLinked2DPoint)
            );

            if (isSegmentRed)
            {
                Debug.Log($"✔️ Connexion validée avec le point : {targetPoint.name} (2D: {linked2DPoint})");
                Color circuitColor = GetCircuitColor();
                DrawColored3DSegment(startLinked2DPoint, linked2DPoint, circuitColor);
                _currentSelected = null;
                Destroy(tempCylinder);

            }
            else
            {
                Destroy(tempCylinder);
            }
        }
        else
        {
            if (tempCylinder != null)
            {
                Destroy(tempCylinder);
                //Debug.Log("❌ Aucun point trouvé, cylindre supprimé.");
            }
        }

        tempCylinder = null;
    }

    public void OnPointClicked3D(Vector2 point, Transform startTransform)
    {
        // Drag and drop 
        tempCylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        tempCylinder.transform.position = startTransform.position;
        tempCylinder.transform.rotation = Quaternion.identity;
        currentStartDragPoint = startTransform;
        isDragging = true; 
        //



        if (!_currentSelected.HasValue)
        {
            _currentSelected = point;
        }
        else
        {
            if (_currentSelected == point)
            {
                _currentSelected = null;
                return;
            }

            bool isSegmentRed = _levelData._segments.Exists(segment =>
                (segment.pointA == _currentSelected.Value && segment.pointB == point) ||
                (segment.pointA == point && segment.pointB == _currentSelected.Value)
            );

            Color circuitColor = GetCircuitColor();

            if (isSegmentRed)
            {
                foreach (var colorEntry in _activeColoredLinesByColor)
                {
                    var color = colorEntry.Key;
                    var lines = colorEntry.Value;

                    if (lines.ContainsKey((_currentSelected.Value, point)) || lines.ContainsKey((point, _currentSelected.Value)))
                    {
                        RemoveColoredLine(_currentSelected.Value, point, color);
                        _currentSelected = null;
                        return;
                    }
                }
            }

            _currentSelected = null;
        }
    }

    private GameObject DrawColored3DSegment(Vector2 a, Vector2 b, Color color)
    {
        // Check any color already exist
        foreach (var kvp in _activeColoredLinesByColor)
        {
            var segmentDict = kvp.Value;
            var existingColor = kvp.Key;

            // AB
            if (segmentDict.TryGetValue((a, b), out GameObject existingObj))
            {
                // 
                if (existingColor == color)
                {
                    Debug.Log($"⚠️ Segment déjà existant entre {a} et {b} avec la même couleur.");
                    return null;
                }

                // 
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

        // 
        Vector3 aPos = new Vector3(0f, (a.y / 30f) + 10f, (-a.x / 30f) - 54f);
        Vector3 bPos = new Vector3(0f, (b.y / 30f) + 10f, (-b.x / 30f) - 54f);

        Vector3 dir = bPos - aPos;
        float distance = dir.magnitude;

        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.name = "ColoredSegmentCylinder";
        cylinder.transform.SetParent(_segmentsParent);

        cylinder.transform.position = aPos + dir / 2f;
        cylinder.transform.up = dir.normalized;
        cylinder.transform.localScale = new Vector3(0.2f, distance / 2f, 0.2f);

        Renderer cylinderRenderer = cylinder.GetComponent<Renderer>();
        cylinderRenderer.material.color = color;

        // 
        if (!_activeColoredLinesByColor.ContainsKey(color))
            _activeColoredLinesByColor[color] = new Dictionary<(Vector2, Vector2), GameObject>();

        _activeColoredLinesByColor[color][(a, b)] = cylinder;

        return cylinder;
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

    private void Draw3DLine(Vector2 a, Vector2 b)
    {
        Vector3 aPos = new Vector3(0f, (a.y / 30f) + 10f, (-a.x / 30f) - 54f);
        Vector3 bPos = new Vector3(0f, (b.y / 30f) + 10f, (-b.x / 30f) - 54f);

        Vector3 dir = bPos - aPos;
        float distance = dir.magnitude;

        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.name = "SegmentCylinder";
        cylinder.transform.SetParent(_segmentsParent);

        cylinder.transform.position = aPos + dir / 2f;
        cylinder.transform.up = dir.normalized;

        cylinder.transform.localScale = new Vector3(0.1f, distance / 2f, 0.1f);
        cylinder.GetComponent<Renderer>().material.color = Color.red;

        _activeRedLines[(a, b)] = cylinder;
    }

    private Color GetCircuitColor()
    {
        if (_currentCircuitSelected < _levelData._circuits.Count)
            return _levelData._circuits[_currentCircuitSelected].circuitColor;

        return Color.white;
    }

    public bool HasCommonPoint((Vector2, Vector2) segment1, (Vector2, Vector2) segment2)
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

        if (!_activeColoredLinesByColor.ContainsKey(color))
        {
            return chains;
        }

        var coloredSegments = _activeColoredLinesByColor[color];

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
            if (!visitedSegments.Contains(segment.Key))
            {
                List<(Vector2, Vector2)> chain = new List<(Vector2, Vector2)>();
                FollowChain(segment.Key, chain);
                chains.Add(chain);
            }
        }

        return chains;
    }


    public bool IsChainConnectingPoints(Color searchColor, Vector2 startPoint, Vector2 endPoint)
    {
        List<List<(Vector2, Vector2)>> chains = FindSegmentChainsByColor(searchColor);

        foreach (var chain in chains)
        {
            if (chain.Count > 1)
            {
                Vector2 firstSegmentStart = chain[0].Item1;
                Vector2 firstSegmentEnd = chain[0].Item2;
                Vector2 lastSegmentStart = chain[chain.Count - 1].Item1; 
                Vector2 lastSegmentEnd = chain[chain.Count - 1].Item2; 

                bool isFirstSegmentConnected = (firstSegmentStart == startPoint || firstSegmentStart == endPoint) || (firstSegmentEnd == startPoint || firstSegmentEnd == endPoint);
                bool isLastSegmentConnected = (lastSegmentStart == startPoint || lastSegmentStart == endPoint) || (lastSegmentEnd == startPoint || lastSegmentEnd == endPoint);

                if (isFirstSegmentConnected && isLastSegmentConnected)
                {
                    bool firstSegmentHasStartPoint = (firstSegmentStart == startPoint || firstSegmentEnd == startPoint);
                    bool lastSegmentHasEndPoint = (lastSegmentStart == endPoint || lastSegmentEnd == endPoint);
                    bool firstSegmentHasEndPoint = (firstSegmentStart == endPoint || firstSegmentEnd == endPoint);
                    bool lastSegmentHasStartPoint = (lastSegmentStart == startPoint || lastSegmentEnd == startPoint);

                    bool isValidConnection = (firstSegmentHasStartPoint && lastSegmentHasEndPoint) || (firstSegmentHasEndPoint && lastSegmentHasStartPoint);

                    if (isValidConnection)
                    {
                        Debug.Log("Les segments sont connectés");
                        return true;
                    }
                    else
                    {
                        Debug.Log("Les segments ne sont pas connectés");
                    }
                }
            }
        }

        return false;
    }

    private void Update()
    {
        UpdateDragging();

        HandleMouseRelease();

        // Puzzle solved Logic
        _puzzleSolved = true;
        for (int i = 0; i < _levelData._circuits.Count; i++)
        {
            Circuit currentCircuit = _levelData._circuits[i];
            bool isValidConnection = IsChainConnectingPoints(currentCircuit.circuitColor, currentCircuit.startPoint, currentCircuit.endPoint);

            if (!isValidConnection)
            {
                _puzzleSolved = false;
                break;
            }
        }

        if (_puzzleSolved)
        {
            Debug.Log("Le puzzle est réussi !");
        }

        if (isClicked) return;  
        isClicked = true;
        // Try raycast logic
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _puzzleCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.CompareTag("CasseTete"))
                {
                    ClickablePointComponent clickableComponent = hit.transform.GetComponent<ClickablePointComponent>();
                    if (clickableComponent != null)
                    {
                        clickableComponent.OnMouseDown();
                    }
                }
            }
        }
           
    }

    private bool isClicked = false;

    private IEnumerator ResetClickLock()
    {
        yield return new WaitForSeconds(0.2f); 
        isClicked = false;
    }


}
