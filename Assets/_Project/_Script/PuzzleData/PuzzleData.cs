using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PuzzleData
{
    #region Fields
    [SerializeField] private FusionPoint fusionPoint;

    [SerializeField] private List<GameObject> puzzleGameObjectList;
    
    private List<Vector3> _savedPositions;
    #endregion

    #region Finish
    public void SetFinish(bool finish)
    {
        fusionPoint.SetState(finish);
    }

    public bool GetFinish()
    {
        return fusionPoint.GetState();
    }
    #endregion

    #region Fusion Point
    public FusionPoint GetFusionPoint()
    {
        return fusionPoint;
    }
    #endregion

    #region Puzzle GameObject
    public List<GameObject> GetPuzzleGameObjectList()
    {
        return puzzleGameObjectList;
    }
    #endregion

    #region SavePosition
    public void SavePositions()
    {
        _savedPositions.Clear();
        foreach (GameObject obj in puzzleGameObjectList)
        {
            _savedPositions.Add(obj.transform.position);
        }
    }
    #endregion
}
