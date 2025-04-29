using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleManager : MonoBehaviour
{
    #region Fields
    [SerializeField] private List<PuzzleData> puzzleList;

    [SerializeField] private PuzzleData finalPuzzle;

    public UnityEvent OnFinalPuzzleActive;
    #endregion

    #region Validation
    public void ValidatePuzzle(FusionPoint fusionPoint)
    {
        foreach (var puzzle in puzzleList) 
        { 
            if(puzzle.GetFusionPoint() == fusionPoint)
            {
                puzzle.SetFinish(true);
            }
        }
        
        // Save the game
        GameManager.Instance.GetSaveManager().SaveGame();
        Debug.Log("Autosave");
        
        CheckValidation();
    }
    #endregion

    #region CheckValidation

    public void CheckValidation()
    {
        foreach (var puzzle in puzzleList)
        {
            if (!puzzle.GetFinish())
            {
                return;
            }
        }

        AccessToFinalPuzzle();
    }

    #endregion

    #region Final Puzzle
    private void AccessToFinalPuzzle()
    {
        int finishedPuzzle = 0;

        foreach (var puzzle in puzzleList)
        {
            if (puzzle.GetFinish())
            {
                finishedPuzzle++;
            }
        }

        if (finishedPuzzle == puzzleList.Count) 
        {
            OnFinalPuzzleActive?.Invoke();
        }
    }
    #endregion

    #region Data
    public List<PuzzleData> GetData()
    {
        return puzzleList;
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
    #endregion
}
