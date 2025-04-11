using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField]
    private List<PuzzleData> _puzzleList;

    [SerializeField]
    private PuzzleData _finalPuzzle;

    public event Action OnFinalPuzzleActive;

    public void ValidatePuzzle(FusionPoint fusionPoint)
    {
        foreach (var puzzle in _puzzleList) 
        { 
            if(puzzle.GetFusionPoint() == fusionPoint)
            {
                puzzle.SetFinish(true);
            }
        }
    }

    private void AccesToFinalBattle()
    {
        int FinishedPuzzle = 0;

        foreach (var puzzle in _puzzleList)
        {
            if (puzzle.GetFinish())
            {
                FinishedPuzzle++;
            }
        }

        if (FinishedPuzzle == _puzzleList.Count) 
        {
            OnFinalPuzzleActive?.Invoke();
        }
    }
}
