using UnityEngine;

public class Diodes : MonoBehaviour
{
    [SerializeField] private Diode[] diodes;
    private StarPuzzleManager _puzzleManager;

    private void FixedUpdate()
    {
        if (!StarPuzzleManager.Instance.isPuzzleActive) return;
        UpdateDiodes();
    }

    private void UpdateDiodes()
    {
        for (int i = 0; i < StarPuzzleManager.Instance.Circuits.Count; i++)
        {
            diodes[i].SetDiode(StarPuzzleManager.Instance.Circuits[i]);
        }
    }
}
