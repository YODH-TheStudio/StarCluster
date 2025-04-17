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
    
    private void Awake()
    {
        SetPlanetLock();
        _rectWidth = rectTransform.rect.width;
        _planetWidth = _rectWidth / PlanetNumber ;
        Debug.Log("_rectWidth" + _rectWidth);
        Debug.Log("_planetWidth" + _planetWidth);
    }

    private void ScrollHorizontalLeft(int increment)
    {
        if (_planetIndex >= 0)
        {
            Vector2 targetPos = new Vector2(rectTransform.anchoredPosition.x - _planetWidth, 
                                            rectTransform.anchoredPosition.y);
            Debug.Log(targetPos);
            StartCoroutine(LerpToPos(targetPos,0.5f));
        }
        
        if (_planetIndex == 0)
        {
            Debug.Log("singe");
            Snap(increment);
        }
    }
    
    private void ScrollHorizontalRight(int increment)
    {
        if (_planetIndex <= 6)
        {
            Vector2 targetPos = new Vector2(rectTransform.anchoredPosition.x + _planetWidth, 
                                            rectTransform.anchoredPosition.y);
            Debug.Log(targetPos);
            StartCoroutine(LerpToPos(targetPos,0.5f));
        }
        
        if (_planetIndex == 6)
        {
            Debug.Log("Chimpanzini Bananini");
            Snap(increment);
        }
    }

    public void MoveSelection(int increment)
    {
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
        rectTransform.position = increment == 1 ? planets[_planetIndex].transform.position : planets[_planetIndex].transform.position;
    }


    private IEnumerator LerpToPos(Vector3 targetPos, float duration)
    {
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
    }
}
