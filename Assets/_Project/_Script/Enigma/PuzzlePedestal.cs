using System;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePedestal : MonoBehaviour
{
    [SerializeField]
    private FusionPoint _fusionPoint;
    
    [SerializeField]
    private List<PedestalData> _pedestalDataList = new List<PedestalData>();

    [SerializeField]
    private float validationRadius = 1.5f;

    public bool IsSolved { get; private set; }

    [System.Serializable]
    private class PedestalData
    {
        public GameObject puzzleObject;
        [NonSerialized]
        public PushPullObject pushPullObject;
        public GameObject pedestalObject;
        public bool isOnPedestal = false;
    }
    
    private void Start()
    {
        foreach (var pair in _pedestalDataList)
        {
            if (pair.puzzleObject != null)
            {
                pair.pushPullObject = pair.puzzleObject.GetComponent<PushPullObject>();
            }
        }
    }

    private void FixedUpdate()
    {
        CheckObjectPosition();
    }

    private void CheckObjectPosition()
    {
        foreach (var pair in _pedestalDataList)
        {
            Vector3 pedestalPosition = pair.pedestalObject.transform.position;

            if (pair.puzzleObject != null)
            {

                float distanceXZ = Vector3.Distance(new Vector3(pair.puzzleObject.transform.position.x, 0, pair.puzzleObject.transform.position.z), new Vector3(pedestalPosition.x, 0, pedestalPosition.z));

                if (distanceXZ <= validationRadius)
                {
                    if (!pair.isOnPedestal) // Si l'objet n'etait pas deja valide
                    {
                        pair.isOnPedestal = true;
                        pair.pushPullObject.SetIsOnPedestal(true);

                        CheckPuzzleResolution();
                        
                        Debug.Log($"Objet {pair.puzzleObject.name} place correctement sur le socle.");
                    }
                }
                else
                {
                    if (pair.isOnPedestal) 
                    {
                        pair.isOnPedestal = false;
                        pair.pushPullObject.SetIsOnPedestal(false);

                        if (_fusionPoint )
                        {
                            _fusionPoint.SetState(false);
                        }
                        
                        Debug.Log($"Objet {pair.puzzleObject.name} retire du socle.");
                    }
                }
            }
        }
    }

    private void CheckPuzzleResolution()
    {
        foreach (var pedestal in _pedestalDataList)
        {
            if (!pedestal.isOnPedestal)
            {
                if (_fusionPoint)
                {
                    _fusionPoint.SetState(false);
                }

                return;
            }
        }

        OnEnigmeSolved();
    }

    protected virtual void OnEnigmeSolved()
    {
        Debug.Log("L'enigme est resolue!");
        if (_fusionPoint)
        {
            _fusionPoint.SetState(true);
        }
    }
}
