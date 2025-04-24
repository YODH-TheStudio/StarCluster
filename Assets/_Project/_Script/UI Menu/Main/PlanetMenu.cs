using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanetMenu : Menu
{
    private SaveManager _saveManager;
    
    [SerializeField] private Image[] planets;
    [SerializeField] private RectTransform rectTransform;
    private float _rectWidth;
    private float _planetWidth;
    private const int PlanetNumber = 7;
    
    private int _planetIndex;
    private int _unlockedPlanets;

    private bool _isMoving = false;

    protected override void Awake()
    {
        base.Awake();
        
        SetPlanetLock();
        _rectWidth = rectTransform.rect.width;
        _planetWidth = _rectWidth / PlanetNumber;
    }

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
            
            if (i <= _unlockedPlanets)
            {
                planet.SetIsUnlocked(true);
            }
            else
            {
                planet.SetIsUnlocked(false);
            }
        }
    }

    private void Snap(int increment)
    {
        if (increment > 0)
        {
            rectTransform.anchoredPosition = new Vector2(increment * _planetWidth * (PlanetNumber - 5), rectTransform.anchoredPosition.y); 
        }
        else
        {
            rectTransform.anchoredPosition = new Vector2(increment * _planetWidth * (PlanetNumber - 3), rectTransform.anchoredPosition.y); 
        }
    }


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
    
    public async void LoadSavedPlanet()
    {
        int slot = GameManager.GetSaveManager().currentSlot;

        SaveManager.SaveData data = GameManager.GetSaveManager().GetSaveData(slot);
        int planetIndex;
        if (data == null)
        {
            planetIndex = 1;
        }
        else
        {
            planetIndex = GameManager.GetSaveManager().GetSaveData(slot).currentPlanet;
        }
        
        Debug.Log(planetIndex);
        await SceneLoader.LoadSceneGroup(planetIndex);
        GameManager.GetSaveManager().LoadGame(slot);
    }
}
