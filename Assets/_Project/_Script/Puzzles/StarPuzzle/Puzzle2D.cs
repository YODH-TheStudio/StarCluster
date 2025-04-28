using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;

using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class Puzzle2D : MonoBehaviour
{
    #region Fields
    // Colors
    public Color _pointColor = Color.green;
    public Color _lineColor = Color.red;

    // Level data
    [SerializeField] private Canvas canvas;
    public LevelData _levelData;
    private GameObject _topMenuContainer;

    private Dictionary<(Vector2, Vector2), GameObject> _activeRedLines;
    private Dictionary<Color, Dictionary<(Vector2, Vector2), GameObject>> _activeColoredLinesByColor;

    // Camera
    [SerializeField] private CinemachineVirtualCamera _puzzleCamera;
    private Camera _mainCamera;

    //
    private Vector2? _currentSelected;

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

    [SerializeField] private float _cubeScale = 0.5f;
    [SerializeField] private float _redLineRadius = 0.1f;
    [SerializeField] private float _coloredLineRadius = 0.2f;

    // Circuit / Eraser
    private Dictionary<int, bool> _circuitValidationStatus = new Dictionary<int, bool>();
    private int _currentCircuitSelected = 0;
    private bool _eraserMode = false;

    // Prefab of Stars/ Prefab of Circuit Shape
    [Header("Prefabs")]
    public GameObject _starPrefab; 
    public List<GameObject> _circuitShapePrefabs;
    public GameObject _segmentPrefab;


    private Vector3 _fingerMP = Vector3.zero;

    #endregion

    private void Awake()
    {
        StarPuzzleManager.Instance.PuzzleCanvas = canvas;
        StarPuzzleManager.Instance.PuzzleCamera = _puzzleCamera;
        _mainCamera = Camera.main;
    }
    private void Start()
    {
        _segmentsParent = new GameObject("Segments3D").transform;
        _segmentsParent.SetParent(parentPuzzleGroup);
        
        canvas.gameObject.SetActive(false);
        _puzzleCamera.gameObject.SetActive(false);
        
        CreateTopMenu();

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
        UpdateDragging();
    }

    private void Touch_OnFingerDown(Finger TouchedFinger)
    {
        _fingerMP = TouchedFinger.screenPosition;

        Vector3 hitPosition = GetMouseHitPosition(_mainCamera);
        Debug.Log("HITPOSITION : " + hitPosition);

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

        if (closestObject != null)
        {
            float distanceToClosest = Vector3.Distance(closestObject.transform.position, hitPosition);
            if (distanceToClosest <= maxRange)
            {
                var pointComponent = closestObject.GetComponent<ClickablePointComponent>();
                pointComponent.TriggerMouseDownByDistance();
            }
        }
    }

    private void Touch_OnFingerUp(Finger TouchedFinger)
    {
        Debug.Log("RELACHEMENT");

        if (!_isDragging)
            return;

        _isDragging = false;
        HandleMouseRelease();
    }

    // UI ***
    private void CreateTopMenu()
    {
        _topMenuContainer = new GameObject("TopMenuContainer");
        _topMenuContainer.transform.SetParent(canvas.transform);

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

        // Crée un objet pour le bouton
        GameObject buttonObj = new GameObject("EraserButton", typeof(Button), typeof(RectTransform), typeof(Text));
        buttonObj.transform.SetParent(_topMenuContainer.transform, false);

        Button button = buttonObj.GetComponent<Button>();
        Text buttonText = buttonObj.GetComponent<Text>();

        // Crée un conteneur pour le texte
        GameObject textContainer = new GameObject("TextContainer", typeof(RectTransform));
        textContainer.transform.SetParent(buttonObj.transform);
        RectTransform textContainerRect = textContainer.GetComponent<RectTransform>();
        textContainerRect.sizeDelta = new Vector2(200, 30);

        // Crée un carré de couleur (fond blanc)
        GameObject colorSquare = new GameObject("ColorSquare", typeof(Image));
        colorSquare.transform.SetParent(textContainer.transform);
        Image colorSquareImage = colorSquare.GetComponent<Image>();
        colorSquareImage.color = Color.white; // Fond blanc

        RectTransform colorSquareRect = colorSquare.GetComponent<RectTransform>();
        colorSquareRect.sizeDelta = new Vector2(20, 20);  // Taille du carré (20x20)

        // S'assurer que le carré est bien ancré et visible
        colorSquareRect.anchorMin = new Vector2(0, 0.5f);
        colorSquareRect.anchorMax = new Vector2(0, 0.5f);
        colorSquareRect.anchoredPosition = new Vector2(0, 0); // Centrer le carré dans le conteneur

        // Définir le texte sur le bouton
        buttonText.text = "Gomme";  // Texte du bouton
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");  // Utilisation de LegacyRuntime.ttf

        // Positionner le texte à côté du carré de couleur
        RectTransform textRect = buttonText.GetComponent<RectTransform>();
        textRect.anchoredPosition = new Vector2(30, 0);  // Décalage à droite du carré de couleur

        // Ajouter le gestionnaire de clic pour activer/désactiver le mode gomme
        button.onClick.AddListener(() =>
        {
            _eraserMode = !_eraserMode;
            Debug.Log($"Mode gomme : {_eraserMode}");
        });

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
        _eraserMode = false;
    }

    // UI *

    // 3D Visual *** 

       private void InstantiatePoints3D()
    {
        foreach (Vector2 pos in _levelData._points)
        {
            GameObject cube = Instantiate(_starPrefab, parentPuzzleGroup);
            cube.name = $"Point3D_{pos.x}_{pos.y}";
            cube.transform.position = new Vector3(0f + transform.position.x ,(pos.y / _scaleFactorY) + transform.position.y, (-pos.x / _scaleFactorX) + transform.position.z);
            cube.transform.localScale = Vector3.one * _cubeScale;

            PointSizeEntry pointSizeEntry = _levelData.pointSizes.FirstOrDefault(p => p.pointPosition == pos);
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
        Vector3 aPos = new Vector3(0.1f + transform.position.x, (a.y / _scaleFactorY) + transform.position.y, (-a.x / _scaleFactorX) + transform.position.z);
        Vector3 bPos = new Vector3(0.1f + transform.position.x, (b.y / _scaleFactorY) + transform.position.y, (-b.x / _scaleFactorX) + transform.position.z);

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
        Vector3 aPos = new Vector3(0.1f + transform.position.x, (a.y / _scaleFactorY) + transform.position.y, (-a.x / _scaleFactorX) - transform.position.z);
        Vector3 bPos = new Vector3(0.1f + transform.position.x, (b.y / _scaleFactorY) + transform.position.y, (-b.x / _scaleFactorX) - transform.position.z);

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












    // Logic ***

    private Vector3 mouseWorldPosition;

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
                    //Debug.Log($"☄️ Touché : {h.collider.name} (Tag: {h.collider.tag})");

                    if (h.collider.CompareTag("CTRP"))
                    {
                        mouse3D = h.point;
                        hitPlan = true;
                        break;
                    }
                }

                if (!hitPlan)
                    Debug.Log("⚠️ Aucun objet touché n’a le tag CTRP.");
            }
            else
            {
                Debug.Log("❌ RaycastAll n’a rien touché.");
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
            Gizmos.DrawSphere(mouseWorldPosition, 0.1f);
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
                Debug.Log($"☄️ Touché : {h.collider.name} (Tag: {h.collider.tag})");

                if (h.collider.CompareTag("CTRP"))
                {
                    mouse3D = h.point;
                    hitPlan = true;
                    //Debug.Log($"✅ Mouse 3D Position sur le plan CTRP : {mouse3D}");
                    break; 
                }
            }

            if (!hitPlan)
                Debug.Log("⚠️ Aucun objet touché n’a le tag CTRP.");
        }
        else
        {
            Debug.Log("❌ RaycastAll n’a rien touché.");
        }

        // Check if drag is larger than threshold
        float dragDistance = Vector3.Distance(_currentStartDragPoint.position, mouse3D);
        //Debug.Log($"Drag Distance: {dragDistance} (Threshold: {_dragThreshold})");

        if (dragDistance < _dragThreshold)
        {
            Debug.Log("Threshold too low, no segment drawn.");
            Destroy(_tempCylinder);
            _tempCylinder = null;
            return;
        }

        const float detectionRadius = 20f;
        Transform targetPoint = null;
        Vector2 linked2DPoint = Vector2.zero;

        // shortest point Distance
        float shortestDistance = float.MaxValue;
        Transform shortestPoint = null;

        foreach (var pointTransform in _pointObjects3D)
        {
            float distance = Vector3.Distance(mouse3D, pointTransform.position);
            //Debug.Log($"Checking point: {pointTransform.position}, Distance: {distance}");


            if (_cubePositions3D.TryGetValue(pointTransform.gameObject, out Vector2 point2D) && distance < shortestDistance)
            {
                shortestDistance = distance;
                shortestPoint = pointTransform;
                linked2DPoint = point2D;
            }
        }

        //Debug.Log($"✅ Shortest point found:\n" +
                  //$"- 3D Point: {shortestPoint.position}\n");

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

            bool isSegmentRed = _levelData._segments.Exists(segment =>
                (segment.pointA == startLinked2DPoint && segment.pointB == linked2DPoint) ||
                (segment.pointA == linked2DPoint && segment.pointB == startLinked2DPoint)
            );

            if (isSegmentRed)
            {
                Color circuitColor = GetCircuitColor();
                if (_eraserMode)
                {
                    foreach (var kvp in _activeColoredLinesByColor)
                    {
                        var segmentDict = kvp.Value;
                        var existingColor = kvp.Key;

                        // AB
                        if (segmentDict.TryGetValue((startLinked2DPoint, linked2DPoint), out GameObject existingObj))
                        {
                            Debug.Log("Erase");
                            RemoveColoredLine(startLinked2DPoint, linked2DPoint, existingColor);
                            break;
                        }
                        // BA
                        else if (segmentDict.TryGetValue((linked2DPoint, startLinked2DPoint), out existingObj))
                        {
                            Debug.Log("Erase");
                            RemoveColoredLine(linked2DPoint, startLinked2DPoint, existingColor);
                            break;
                        }
                    }
                }
                else
                {
                    Debug.Log($"✔️ Connexion validée avec le point : {targetPoint.name} (2D: {linked2DPoint})");

                    DrawColored3DSegment(startLinked2DPoint, linked2DPoint, circuitColor);

                }
                _currentSelected = null;
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
                Debug.Log("❌ Aucun point trouvé, cylindre supprimé.");
            }
        }

        _tempCylinder = null;
    }

    public void OnPointClicked3D(Vector2 point, Transform startTransform)
    {
        // Drag and drop 
        _tempCylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        _tempCylinder.GetComponent<MeshRenderer>().enabled = false;
        _tempCylinder.transform.position = startTransform.position;
        _tempCylinder.transform.rotation = Quaternion.identity;
        _currentStartDragPoint = startTransform;
        _isDragging = true; 
        //

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

    private Color GetCircuitColor()
    {
        if (_currentCircuitSelected < _levelData._circuits.Count)
            return _levelData._circuits[_currentCircuitSelected].circuitColor;

        return Color.white;
    }

    // Logic *



















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

    public Vector3 GetMouseHitPosition(Camera camera, string targetTag = "CTRP", float rayLength = 1000f)
    {
        Vector3 mouse3D = Vector3.zero;
        Vector2 mouseScreen = _fingerMP;

        Ray ray = camera.ScreenPointToRay(mouseScreen);
        Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.red, 2f);

        RaycastHit[] hits = Physics.RaycastAll(ray, rayLength);

        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.CompareTag(targetTag))
                {
                    return hit.point;
                }
            }
        }


        return mouse3D;
    }

    private void FixedUpdate()
    {
        CheckPuzzleSolved();
    }

    private void CheckPuzzleSolved()
    {
        _circuitValidationStatus.Clear(); 
        for (int i = 0; i < _levelData._circuits.Count; i++)
        {
            Circuit currentCircuit = _levelData._circuits[i];
            bool isValidConnection = IsChainConnectingPoints(currentCircuit.circuitColor, currentCircuit.startPoint, currentCircuit.endPoint);
            StarPuzzleManager.Instance.Circuits.Add(isValidConnection);
            _circuitValidationStatus[i] = isValidConnection;
            Debug.Log(_circuitValidationStatus);
        }
        Debug.Log(_circuitValidationStatus);
        
        _puzzleSolved =  _circuitValidationStatus.Values.All(v => v);
    }
}