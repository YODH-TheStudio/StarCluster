using System;
using UnityEngine;

public class StarPuzzleHUD : MonoBehaviour
{
    [SerializeField] private Diodes diodes;
    [SerializeField] private Canvas canvas;

    private void Awake()
    {
        StarPuzzleManager.Instance.DiodesCanvas = canvas;
    }

    private void Start()
    {
        canvas.gameObject.SetActive(false);
    }

    public void QuitPuzzle()
    {
        StarPuzzleManager.Instance.SwitchCamera();
    }
    
    
}
