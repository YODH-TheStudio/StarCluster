using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PuzzleMaker : MonoBehaviour
{
    #region Fields
    public LevelData _levelData;

    private Dictionary<Color, Dictionary<(Vector2, Vector2), GameObject>> _activeColoredLinesByColor;

    private Vector2? _currentSelected = null;

    private Dictionary<GameObject, Vector2> _cubePositions3D = new Dictionary<GameObject, Vector2>();
    private List<Transform> _pointObjects3D = new List<Transform>();
    private Transform _segmentsParent;

    private Transform _parentPuzzleGroup;

    // Visual size and offset
    [Header("3D Display Settings")]
    [SerializeField] private float _scaleFactorX = 30f;
    [SerializeField] private float _scaleFactorY = 30f;
    [SerializeField] private float _yOffset = 10f;
    [SerializeField] private float _zOffset = 54f;

    [SerializeField] private float _cubeScale = 0.5f;
    [SerializeField] private float _redLineRadius = 0.1f;
    [SerializeField] private float _coloredLineRadius = 0.2f;

    // Prefab of Stars/ Prefab of Circuit Shape
    [Header("Prefabs")]
    public GameObject _starPrefab; 
    public List<GameObject> _circuitShapePrefabs;
    public GameObject _segmentPrefab;
    
    #endregion

    void Start()
    {
        _segmentsParent = this.transform;
        _segmentsParent.SetParent(_parentPuzzleGroup);
        
        InstantiatePoints3D();
        InstantiateSegments();
    }
    
    private void InstantiatePoints3D()
    {
        foreach (Vector2 pos in _levelData._points)
        {
            GameObject cube = Instantiate(_starPrefab, _parentPuzzleGroup);
            cube.name = $"Point3D_{pos.x}_{pos.y}";
            cube.transform.position = new Vector3(0f, pos.y / _scaleFactorY + _yOffset, -pos.x / _scaleFactorX - _zOffset);
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
                        GameObject startPrefab = Instantiate(_circuitShapePrefabs[prefabIndex - 1], _parentPuzzleGroup);
                        startPrefab.transform.position = cube.transform.position + circuit.startPointOffset;

                    }

                    if (circuit.endPoint == pos)
                    {
                        GameObject endPrefab = Instantiate(_circuitShapePrefabs[prefabIndex - 1], _parentPuzzleGroup);
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
        Vector3 aPos = new Vector3(0.1f, (a.y / _scaleFactorY) + _yOffset, (-a.x / _scaleFactorX) - _zOffset);
        Vector3 bPos = new Vector3(0.1f, (b.y / _scaleFactorY) + _yOffset, (-b.x / _scaleFactorX) - _zOffset);

        Vector3 dir = bPos - aPos;
        float distance = dir.magnitude;

        if (_segmentPrefab == null || _parentPuzzleGroup == null)
        {
            Debug.LogWarning("segmentPrefab ou parentPuzzleGroup n’est pas assigné !");
            return;
        }

        GameObject segment = Instantiate(_segmentPrefab, _parentPuzzleGroup);
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

        if (_segmentPrefab == null || _parentPuzzleGroup == null)
        {
            Debug.LogWarning("segmentPrefab ou parentPuzzleGroup n’est pas assigné !");
            return null;
        }

        // Création du segment depuis le prefab
        GameObject segment = Instantiate(_segmentPrefab, _parentPuzzleGroup);
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

}
