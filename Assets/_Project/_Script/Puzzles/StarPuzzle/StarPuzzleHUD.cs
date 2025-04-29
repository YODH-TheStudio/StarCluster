using System;
using UnityEngine;

public class StarPuzzleHUD : MonoBehaviour
{
    private Canvas _canvas;
    [SerializeField] private Diodes diodes;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        StarPuzzleManager.Instance.PuzzleCanvas = _canvas;
    }

    private void Start()
    {
        _canvas.gameObject.SetActive(false);
    }

    public void QuitPuzzle()
    {
        StarPuzzleManager.Instance.SwitchCamera();
    }
    
    
}
