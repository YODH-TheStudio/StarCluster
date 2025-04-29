using UnityEngine;

public class Diodes : MonoBehaviour
{

    #region Fields
    [SerializeField] private Diode[] diodes;
    private StarPuzzleManager _puzzleManager;
    #endregion

    #region Main Functions
    private void FixedUpdate()
    {
        if (!StarPuzzleManager.Instance.isPuzzleActive) return;
        UpdateDiodes();
    }
    #endregion

    #region Diodes Update
    private void UpdateDiodes()
    {
        for (int i = 0; i < StarPuzzleManager.Instance.Circuits.Count; i++)
        {
            diodes[i].SetDiode(StarPuzzleManager.Instance.Circuits[i]);
        }
    }
    #endregion 
}
