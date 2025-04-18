using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetMenu : Menu
{
    private int _planetIndex;
    private int _unlockedPlanets;
    private int _planetNumber = 7;
    
    public void MoveSelection(int increment)
    {
        GameManager.Instance._soundSystem.PlaySoundFXClipByKey("Ui Clica", transform.position); 
        _planetIndex = (_planetIndex + increment + _planetNumber) % _planetNumber;
        Debug.Log(_planetIndex);
    }
}
