using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PuzzleData
{
    [SerializeField] private FusionPoint fusionPoint;

    [SerializeField] private List<GameObject> puzzleGameObjectList;
    
    private List<Vector3> _savedPositions;

    public void SetFinish(bool finish)
    {
        fusionPoint.SetState(finish);
    }

    public bool GetFinish()
    {
        return fusionPoint.GetState();
    }

    public FusionPoint GetFusionPoint()
    {
        return fusionPoint;
    }
    
    public List<GameObject> GetPuzzleGameObjectList()
    {
        return puzzleGameObjectList;
    }

    public void SavePositions()
    {
        _savedPositions.Clear();
        foreach (GameObject obj in puzzleGameObjectList)
        {
            _savedPositions.Add(obj.transform.position);
        }
    }
}
