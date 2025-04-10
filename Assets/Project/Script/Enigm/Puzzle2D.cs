using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class Puzzle2D : MonoBehaviour
{
    public Canvas _canvas;
    public LevelData _levelData;
    public Color _pointColor = Color.green;
    public Color _lineColor = Color.red;

    private RectTransform _playArea;
    private List<RectTransform> _pointObjects = new List<RectTransform>();
    private Dictionary<Vector2, RectTransform> _pointMap = new Dictionary<Vector2, RectTransform>();
    private Dictionary<(Vector2, Vector2), GameObject> _activeRedLines = new Dictionary<(Vector2, Vector2), GameObject>();  // Lignes rouges existantes

    private Dictionary<Color, Dictionary<(Vector2, Vector2), GameObject>> _activeColoredLinesByColor = new Dictionary<Color, Dictionary<(Vector2, Vector2), GameObject>>();

    private Vector2? _currentSelected = null;

    // Tableaux locaux pour gérer les segments et points actuels
    private List<Vector2> _currentPoints = new List<Vector2>();
    private List<(Vector2, Vector2)> _currentSegments = new List<(Vector2, Vector2)>();

    private float _pointRadius = 7.5f;
    private GameObject _menuContainer;

    // Circuit
    private int _currentCircuitSelected = 0;

    void Start()
    {
        //Debug.Log($"[Puzzle2D] Points trouvés dans LevelData: {_levelData._points.Count}");
        foreach (var _p in _levelData._points)
        {
            //Debug.Log($"Point: {_p}");
        }

        //Debug.Log($"[Puzzle2D] Segments trouvés dans LevelData: {_levelData._segments.Count}");
        foreach (var _seg in _levelData._segments)
        {
            //Debug.Log($"Segment: ({_seg.pointA}, {_seg.pointB})");
        }

        CreatePlayArea();
        CreateMenu(); 
        InstantiatePoints();
        InstantiateSegments();
    }

    private void CreatePlayArea()
    {
        GameObject _bg = new GameObject("PlayArea", typeof(Image));
        _bg.transform.SetParent(_canvas.transform);
        RectTransform _rect = _bg.GetComponent<RectTransform>();
        _rect.anchorMin = _rect.anchorMax = new Vector2(0.5f, 0.5f);
        _rect.pivot = new Vector2(0.5f, 0.5f);
        _rect.anchoredPosition = Vector2.zero;
        _rect.sizeDelta = new Vector2(600, 600); 

        Image _img = _bg.GetComponent<Image>();
        _img.color = new Color(0, 0, 0, 0.8f);

        _playArea = _rect;
    }

    // Méthode pour créer le menu horizontal
    private void CreateMenu()
    {
        // Créer un container pour le menu
        _menuContainer = new GameObject("MenuContainer");
        _menuContainer.transform.SetParent(_canvas.transform);
        RectTransform _menuRect = _menuContainer.AddComponent<RectTransform>();
        _menuRect.anchorMin = _menuRect.anchorMax = new Vector2(0.5f, 1);
        _menuRect.pivot = new Vector2(0.5f, 1);
        _menuRect.anchoredPosition = new Vector2(0, -50); // Placer le menu un peu plus bas
        _menuRect.sizeDelta = new Vector2(600, 50); // Taille du menu

        // Ajouter un layout horizontal pour organiser les boutons
        HorizontalLayoutGroup _layoutGroup = _menuContainer.AddComponent<HorizontalLayoutGroup>();
        _layoutGroup.spacing = 10; // Espacement entre les boutons
        _layoutGroup.childAlignment = TextAnchor.MiddleCenter;
        _layoutGroup.childForceExpandWidth = false;

        // Ajouter 5 boutons
        for (int _i = 0; _i < 5; _i++)
        {
            CreateMenuButton(_i);
        }
    }

    // Méthode pour créer un bouton du menu
    private void CreateMenuButton(int _index)
    {
        // Créer un bouton
        GameObject _buttonObj = new GameObject($"CircuitButton_{_index}", typeof(Button), typeof(RectTransform), typeof(Text));
        _buttonObj.transform.SetParent(_menuContainer.transform);

        Button _button = _buttonObj.GetComponent<Button>();
        Text _buttonText = _buttonObj.GetComponent<Text>();
        _buttonText.text = $"Circuit {_index + 1}";
        _buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); // Police par défaut

        _button.onClick.AddListener(() => OnCircuitButtonClicked(_index));
    }

    // Méthode pour gérer le clic sur un bouton de circuit
    private void OnCircuitButtonClicked(int _index)
    {
        //Debug.Log($"Circuit {_index + 1} sélectionné");
        _currentCircuitSelected = _index;
    }

    private void InstantiatePoints()
    {
        foreach (Vector2 _pos in _levelData._points)
        {
            GameObject _pointGO = new GameObject("Point", typeof(Image), typeof(Button));
            _pointGO.transform.SetParent(_playArea);
            RectTransform _rt = _pointGO.GetComponent<RectTransform>();
            _rt.anchorMin = _rt.anchorMax = new Vector2(0.5f, 0.5f);
            _rt.sizeDelta = new Vector2(15, 15);
            _rt.anchoredPosition = _pos;

            _pointGO.GetComponent<Image>().color = _pointColor;

            Button _btn = _pointGO.GetComponent<Button>();
            Vector2 _capturedPos = _pos;
            _btn.onClick.AddListener(() => OnPointClicked(_capturedPos));

            _pointObjects.Add(_rt);
            _pointMap[_pos] = _rt;
        }

        // Ajouter les points au tableau local
        _currentPoints.AddRange(_levelData._points);
    }

    private void InstantiateSegments()
    {
        foreach (var _segment in _levelData._segments)
        {
            Vector2 _a = _segment.pointA;
            Vector2 _b = _segment.pointB;

            if (!_pointMap.ContainsKey(_a) || !_pointMap.ContainsKey(_b))
            {
                //Debug.LogWarning($"Un des points du segment ({_a}, {_b}) n'existe pas dans _pointMap.");
                continue;
            }

            DrawLine(_a, _b);
            _currentSegments.Add((_a, _b));
        }
    }

    private void OnPointClicked(Vector2 _point)
    {
        //Debug.Log($"[OnPointClicked] Point cliqué : {_point}");

        if (_currentSelected == null)
        {
            //Debug.Log("[OnPointClicked] Aucun point sélectionné, sélection du point actuel.");
            _currentSelected = _point;
        }
        else
        {
            if (_currentSelected == _point)
            {
                //Debug.Log("[OnPointClicked] Le point cliqué est le même que le point sélectionné, désélection.");
                _currentSelected = null;
                return;
            }

            //Debug.Log($"[OnPointClicked] Sélection d'un segment entre {_currentSelected.Value} et {_point}");

            // Vérifier si le segment existe dans _levelData._segments (lignes rouges)
            bool _isSegmentRed = _levelData._segments.Exists(_segment =>
                (_segment.pointA == _currentSelected.Value && _segment.pointB == _point) ||
                (_segment.pointA == _point && _segment.pointB == _currentSelected.Value)
            );

            //Debug.Log($"[OnPointClicked] Segment rouge trouvé ? {_isSegmentRed}");

            Color circuitColor = GetCircuitColor();
            if (_isSegmentRed)
            {
                // Vérifier si la ligne existe déjà dans le dictionnaire de la couleur spécifique
                if (_activeColoredLinesByColor.ContainsKey(circuitColor) &&
                    (_activeColoredLinesByColor[circuitColor].ContainsKey((_currentSelected.Value, _point)) ||
                     _activeColoredLinesByColor[circuitColor].ContainsKey((_point, _currentSelected.Value))))
                {
                    //Debug.Log($"[OnPointClicked] Le segment entre {_currentSelected.Value} et {_point} existe déjà. Suppression de la ligne colorée.");
                    // Si la ligne colorée existe déjà, on la supprime
                    RemoveLine(_currentSelected.Value, _point, circuitColor);
                    _currentSegments.Remove((_currentSelected.Value, _point));
                }
                else
                {
                    //Debug.Log($"[OnPointClicked] Le segment entre {_currentSelected.Value} et {_point} n'est pas encore dessiné. Ajout de la ligne colorée.");
                    // Si le segment existe mais n'est pas encore dessiné, on l'ajoute
                    DrawColoredLine(_currentSelected.Value, _point, circuitColor);
                    _currentSegments.Add((_currentSelected.Value, _point));
                }
            }

            // Réinitialiser la sélection après l'ajout ou la suppression de ligne
            _currentSelected = null;
        }
    }

    private void DrawColoredLine(Vector2 _a, Vector2 _b, Color color)
    {
        if (!_pointMap.ContainsKey(_a) || !_pointMap.ContainsKey(_b)) return;

        // Calculer l'extrémité des points en prenant en compte le rayon
        Vector2 _aExtremity = GetPointExtremity(_a, _b);
        Vector2 _bExtremity = GetPointExtremity(_b, _a);

        GameObject _line = new GameObject("ColoredLine", typeof(Image));
        _line.transform.SetParent(_playArea);
        RectTransform _rt = _line.GetComponent<RectTransform>();

        Vector2 _dir = _bExtremity - _aExtremity;

        _rt.sizeDelta = new Vector2(_dir.magnitude, 5);
        _rt.anchoredPosition = _aExtremity + _dir / 2;
        _rt.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg);

        // Appliquer la couleur spécifique
        _line.GetComponent<Image>().color = color;

        // Vérifier si le dictionnaire pour la couleur existe, sinon le créer
        if (!_activeColoredLinesByColor.ContainsKey(color))
        {
            _activeColoredLinesByColor[color] = new Dictionary<(Vector2, Vector2), GameObject>();
        }

        // Ajouter la ligne au dictionnaire de la couleur spécifique
        _activeColoredLinesByColor[color][(_a, _b)] = _line;
        _activeColoredLinesByColor[color][(_b, _a)] = _line;
    }

    private void DrawLine(Vector2 _a, Vector2 _b)
    {
        if (!_pointMap.ContainsKey(_a) || !_pointMap.ContainsKey(_b)) return;

        // Calculer l'extrémité des points en prenant en compte le rayon
        Vector2 _aExtremity = GetPointExtremity(_a, _b);
        Vector2 _bExtremity = GetPointExtremity(_b, _a);

        GameObject _line = new GameObject("Line", typeof(Image));
        _line.transform.SetParent(_playArea);
        RectTransform _rt = _line.GetComponent<RectTransform>();

        Vector2 _dir = _bExtremity - _aExtremity;

        _rt.sizeDelta = new Vector2(_dir.magnitude, 5);
        _rt.anchoredPosition = _aExtremity + _dir / 2;
        _rt.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg);

        _line.GetComponent<Image>().color = _lineColor;

        _activeRedLines[(_a, _b)] = _line;
        _activeRedLines[(_b, _a)] = _line;
    }

    private Vector2 GetPointExtremity(Vector2 _point, Vector2 _direction)
    {
        // Trouver le vecteur direction normalisé
        Vector2 _dir = _direction - _point;
        _dir.Normalize();

        // Appliquer le rayon pour obtenir l'extrémité du point
        return _point + _dir * _pointRadius;
    }

    private void RemoveLine(Vector2 _a, Vector2 _b, Color color)
    {
        if (_activeColoredLinesByColor.ContainsKey(color) &&
            _activeColoredLinesByColor[color].TryGetValue((_a, _b), out GameObject _line))
        {
            Destroy(_line);
            _activeColoredLinesByColor[color].Remove((_a, _b));
            _activeColoredLinesByColor[color].Remove((_b, _a));
        }
    }

    private Color GetCircuitColor()
    {
        switch (_currentCircuitSelected)
        {
            case 0: return Color.green;  // Circuit 1 - vert
            case 1: return Color.blue;   // Circuit 2 - bleu
            case 2: return Color.yellow; // Circuit 3 - jaune
            case 3: return Color.cyan;   // Circuit 4 - cyan
            case 4: return Color.magenta; // Circuit 5 - magenta
            default: return Color.white; // Par défaut
        }
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
                if (segment.Key.Item1 == currentEndPoint && !visitedSegments.Contains(segment.Key))
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
            if (chain.Count > 0)
            {
                Vector2 firstPoint = chain[0].Item1;
                Vector2 lastPoint = chain[chain.Count - 1].Item2;

                if (firstPoint == startPoint && lastPoint == endPoint)
                {
                    Debug.Log("Relié");
                    return true;
                }
            }
        }

        return false;
    }

    private void Update()
    {
        IsChainConnectingPoints(Color.green, _levelData._points[1], _levelData._points[2]);
    }




}

// Segment dans l'inverse sa marche pas