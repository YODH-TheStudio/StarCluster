using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PuzzleData
{
    [SerializeField]
    private FusionPoint _fusionPoint;

    [SerializeField]
    private List<GameObject> _puzzleGameObjectList;
    
    private List<Vector3> _savedPositions;

    public void SetFinish(bool finish)
    {
        _fusionPoint.SetState(finish);
    }

    public bool GetFinish()
    {
        return _fusionPoint.GetState();
    }

    public FusionPoint GetFusionPoint()
    {
        return _fusionPoint;
    }
    
    public List<GameObject> GetPuzzleGameObjectList()
    {
        return _puzzleGameObjectList;
    }

    public void SavePositions()
    {
        _savedPositions.Clear();
        foreach (GameObject obj in _puzzleGameObjectList)
        {
            _savedPositions.Add(obj.transform.position);
        }
    }

    public void RestorePositions()
    {
        for (int i = 0; i < _puzzleGameObjectList.Count; i++)
        {
            if (i < _savedPositions.Count)
            {
                _puzzleGameObjectList[i].transform.position = _savedPositions[i];
            }
        }
    }
}
