using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetMenu : Menu
{
    private int _planetIndex = 1;
    private int _unlockedPlanets = 0;
    private int _unlockedPleiades = 0;
    private int _planetNumber = 7;
    
    public void NextPlanet()
    {
        Debug.Log(_planetIndex);
        if (_planetIndex == 8) return;

        _planetIndex++;
    }

    public void PreviousPlanet()
    {
        if (_planetIndex == 1) return;

        _planetIndex--;
    }

    private void MoveSelection(int increment)
    {
        _planetIndex = (_planetIndex + increment + _planetNumber) % _planetNumber; 
        Debug.Log(_planetIndex);
    }
}
