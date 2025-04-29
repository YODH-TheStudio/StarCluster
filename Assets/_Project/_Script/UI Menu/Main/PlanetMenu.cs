using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlanetMenu : Menu
{
    #region Fields
    private SaveManager _saveManager;
    
    [SerializeField] private Image[] planets;
    [SerializeField] private Image[] clonePlanets;
    [SerializeField] private RectTransform rectTransform;
    private float _rectWidth;
    private float _planetWidth;
    
    private const int PlanetNumber = 7;
    private int _planetIndex;
    private int _unlockedPlanets;

    private bool _isMoving = false;

    #endregion

    #region Main Functions
    protected override void Awake()
    {
        base.Awake();

        _unlockedPlanets = 1; 
        
        SetPlanetLock();
        _rectWidth = rectTransform.rect.width;
        _planetWidth = _rectWidth / PlanetNumber;
    }
    #endregion

    #region Scroll

    private void ScrollHorizontalLeft(int increment)
    {
        if (_planetIndex < 0) return;
        
        Vector2 targetPos = new Vector2(rectTransform.anchoredPosition.x - _planetWidth, 
            rectTransform.anchoredPosition.y);
            
        StartCoroutine(_planetIndex == 0
            ? LerpToPos(targetPos, 0.5f, () => Snap(increment))
            : LerpToPos(targetPos, 0.5f));
    }
    
    private void ScrollHorizontalRight(int increment)
    {
        if (_planetIndex > 6) return;
        
        Vector2 targetPos = new Vector2(rectTransform.anchoredPosition.x + _planetWidth, 
            rectTransform.anchoredPosition.y);

        StartCoroutine(_planetIndex == 6
            ? LerpToPos(targetPos, 0.5f, () => Snap(increment))
            : LerpToPos(targetPos, 0.5f));
    }
    #endregion

    #region Planet Selection
    public void MoveSelection(int increment)
    {
        if (_isMoving) return;
        
        _planetIndex = (_planetIndex + increment + PlanetNumber) % PlanetNumber; 
        Debug.Log(_planetIndex);
        if (increment >= 1)
        {
            ScrollHorizontalLeft(increment);
        }
        else
        {
            ScrollHorizontalRight(increment);
        }
    }
    private void SetPlanetLock()
    {
        for (int i = 0; i < planets.Length; i++)
        {
            var planet = planets[i].GetComponent<UnlockedSprite>();

            planet.SetIsUnlocked(i < _unlockedPlanets);
        }
    }

    private void Snap(int increment)
    {
        rectTransform.anchoredPosition = increment > 0 ? new Vector2(increment * _planetWidth * (PlanetNumber - 5), rectTransform.anchoredPosition.y) : new Vector2(increment * _planetWidth * (PlanetNumber - 3), rectTransform.anchoredPosition.y);
    }
    
    public override async void LoadGroupScene(int index)
    {
        if (_planetIndex != 0)
        {
            Debug.LogWarning("Impossible de charger : cette planète est verrouillée !");
            return;
        }
        base.LoadGroupScene(index);
    }
#endregion

    #region Coroutine
    private IEnumerator LerpToPos(Vector3 targetPos, float duration,System.Action onComplete = null)
    {
        _isMoving = true;
        Vector3 startPos = rectTransform.anchoredPosition;
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            rectTransform.anchoredPosition = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
        rectTransform.anchoredPosition = targetPos;
        _isMoving = false;
        onComplete?.Invoke();
    }
    #endregion
}
