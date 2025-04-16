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
        
        // Save the game
        GameManager.Instance.GetSaveManager().SaveGame();
        Debug.Log("Autosave");
    }

    private void AccessToFinalPuzzle()
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

    public List<PuzzleData> GetData()
    {
        return _puzzleList;
    }
    // public void SetData(List<PuzzleData> newPuzzleList)
    // {
    //     //Debug.Log("Setting data: " + newPuzzleList.ToString());
    //     // Update positions
    //     foreach (PuzzleData puzzle in newPuzzleList)
    //     {
    //         if (puzzle.GetFinish())
    //         {
    //             puzzle.GetFusionPoint().SetState(true);
    //         }
    //         else
    //         {
    //             puzzle.GetFusionPoint().SetState(false);
    //         }
    //     }
    // }
}
