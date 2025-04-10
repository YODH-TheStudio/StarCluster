using System.Collections;
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

    //
    bool _puzzleSolved = true;

    void Start()
    {
        CreatePlayArea();
        CreateMenu(); 
        InstantiatePoints();
        InstantiateSegments();

        StartCoroutine(RepeatChainCheck());
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

        // Créer un bouton pour chaque circuit défini dans LevelData
        for (int i = 0; i < _levelData._circuits.Count; i++)
        {
            CreateMenuButton(i, _levelData._circuits[i]);
        }
    }

    private void CreateMenuButton(int index, Circuit circuit)
    {
        // Créer un bouton
        GameObject _buttonObj = new GameObject($"CircuitButton_{index}", typeof(Button), typeof(RectTransform), typeof(Text));
        _buttonObj.transform.SetParent(_menuContainer.transform);

        Button _button = _buttonObj.GetComponent<Button>();
        Text _buttonText = _buttonObj.GetComponent<Text>();

        // Créer un conteneur horizontal pour contenir le carré de couleur et le texte
        GameObject _textContainer = new GameObject("TextContainer", typeof(RectTransform));
        _textContainer.transform.SetParent(_buttonObj.transform);
        RectTransform textContainerRect = _textContainer.GetComponent<RectTransform>();
        textContainerRect.sizeDelta = new Vector2(200, 30);  // Ajuste la taille du conteneur selon tes besoins

        // Créer un petit carré de couleur
        GameObject _colorSquare = new GameObject("ColorSquare", typeof(Image));
        _colorSquare.transform.SetParent(_textContainer.transform);
        Image colorSquareImage = _colorSquare.GetComponent<Image>();
        colorSquareImage.color = circuit.circuitColor;  // Utiliser la couleur du circuit
        RectTransform colorSquareRect = _colorSquare.GetComponent<RectTransform>();
        colorSquareRect.sizeDelta = new Vector2(20, 20);  // Taille du carré de couleur

        // Créer un texte avec le numéro et le nom du circuit
        _buttonText.text = $" {circuit.name} {index + 1}"; // Ajouter le numéro devant le nom
        _buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); // Police par défaut
        RectTransform textRect = _buttonText.GetComponent<RectTransform>();
        textRect.anchoredPosition = new Vector2(30, 0);  // Positionner le texte à côté du carré de couleur

        _button.onClick.AddListener(() => OnCircuitButtonClicked(index));
    }

    // Méthode pour gérer le clic sur un bouton de circuit
    private void OnCircuitButtonClicked(int _index)
    {
        _currentCircuitSelected = _index;
        //Debug.Log("Circuit sélectionné : " + _index);  // Affiche l'index dans la console
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
        if (_currentSelected == null)
        {
            _currentSelected = _point;
        }
        else
        {
            if (_currentSelected == _point)
            {
                _currentSelected = null;
                return;
            }

            bool _isSegmentRed = _levelData._segments.Exists(_segment =>
                (_segment.pointA == _currentSelected.Value && _segment.pointB == _point) ||
                (_segment.pointA == _point && _segment.pointB == _currentSelected.Value)
            );

            Color circuitColor = GetCircuitColor();

            if (_isSegmentRed)
            {
                // On vérifie s'il existe déjà une ligne colorée et on la supprime si c'est le cas
                foreach (var colorEntry in _activeColoredLinesByColor)
                {
                    var color = colorEntry.Key;
                    var lines = colorEntry.Value;

                    if (lines.ContainsKey((_currentSelected.Value, _point)) || lines.ContainsKey((_point, _currentSelected.Value)))
                    {
                        RemoveColoredLine(_currentSelected.Value, _point, color); 
                        _currentSelected = null;
                        return;
                    }
                }

                DrawColoredLine(_currentSelected.Value, _point, circuitColor); 
            }

            _currentSelected = null;
        }
    }

    private GameObject DrawColoredLine(Vector2 _a, Vector2 _b, Color color)
    {
        if (!_pointMap.ContainsKey(_a) || !_pointMap.ContainsKey(_b)) return null;
        Vector2 _aExtremity = GetPointExtremity(_a, _b);
        Vector2 _bExtremity = GetPointExtremity(_b, _a);

        GameObject _line = new GameObject("ColoredLine", typeof(Image));
        _line.transform.SetParent(_playArea);
        RectTransform _rt = _line.GetComponent<RectTransform>();

        Vector2 _dir = _bExtremity - _aExtremity;

        _rt.sizeDelta = new Vector2(_dir.magnitude, 5);
        _rt.anchoredPosition = _aExtremity + _dir / 2;
        _rt.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg);

        _line.GetComponent<Image>().color = color;

        if (!_activeColoredLinesByColor.ContainsKey(color))
        {
            _activeColoredLinesByColor[color] = new Dictionary<(Vector2, Vector2), GameObject>();
        }


        _activeColoredLinesByColor[color][(_a, _b)] = _line;

        return _line; 
    }

    private void RemoveColoredLine(Vector2 _a, Vector2 _b, Color color)
    {
        if (_activeColoredLinesByColor.ContainsKey(color))
        {

            if (_activeColoredLinesByColor[color].ContainsKey((_a, _b)))
            {
                GameObject _line = _activeColoredLinesByColor[color][(_a, _b)];
                Destroy(_line);
                _activeColoredLinesByColor[color].Remove((_a, _b));
            }

            if (_activeColoredLinesByColor[color].ContainsKey((_b, _a)))
            {
                GameObject _line = _activeColoredLinesByColor[color][(_b, _a)];
                Destroy(_line); 
                _activeColoredLinesByColor[color].Remove((_b, _a));
            }
        }
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

    private Color GetCircuitColor()
    {
        return _levelData._circuits[_currentCircuitSelected].circuitColor;
    }


























    public bool HasCommonPoint((Vector2, Vector2) segment1, (Vector2, Vector2) segment2)
    {
        // Vérifie si le premier point du premier segment est le même que le premier ou le second point du deuxième segment
        if (segment1.Item1 == segment2.Item1 || segment1.Item1 == segment2.Item2)
        {
            return true;
        }

        // Vérifie si le second point du premier segment est le même que le premier ou le second point du deuxième segment
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

                    // Si la connexion est valide, les segments sont connectés
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
    }

    private IEnumerator RepeatChainCheck()
    {
        




        yield return new WaitForSeconds(0.01f); 
    }


}
