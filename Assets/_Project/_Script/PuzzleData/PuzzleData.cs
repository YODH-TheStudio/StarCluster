using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleData : MonoBehaviour
{
    [SerializeField]
    private FusionPoint _fusionPoint;

    [SerializeField]
    private List<GameObject> _puzzleGameObjectList;

    [SerializeField]
    private bool _isFinish;

    public void SetFinish(bool finish)
    {
        _isFinish = finish;
        _fusionPoint.SetState(finish);
    }

    public bool GetFinish()
    {
        return _isFinish;
    }

    public FusionPoint GetFusionPoint()
    {
        return _fusionPoint;
    }
}
