using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnigmeSocle : MonoBehaviour
{
    [SerializeField]
    private List<PedestalData> _pedestalDataList = new List<PedestalData>();
    // private Dictionary<GameObject, bool> objetsPlacementStatus = new Dictionary<GameObject, bool>();

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
        public bool isValid = false;
    }

    private void FixedUpdate()
    {

        foreach (var pair in _pedestalDataList)
        {
            Vector3 pedestalPosition = pair.pedestalObject.transform.position;

            if (pair.puzzleObject != null)
            {

                float distanceXZ = Vector3.Distance(new Vector3(pair.puzzleObject.transform.position.x, 0, pair.puzzleObject.transform.position.z), new Vector3(pedestalPosition.x, 0, pedestalPosition.z));

                if (distanceXZ <= validationRadius)
                {
                    if (!pair.isValid) // Si l'objet n'etait pas deja valide
                    {
                        pair.isValid = true;
                        Debug.Log($"Objet {pair.puzzleObject.name} place correctement sur le socle.");
                    }
                }
                else
                {
                    if (pair.isValid) 
                    {
                        pair.isValid = false;
                    }
                }
            }
        }

        CheckEnigmeResolution();
    }

    private void CheckEnigmeResolution()
    {
        foreach (var pedestal in _pedestalDataList)
        {
            if (!pedestal.isValid)
            {
                return;
            }
        }

        OnEnigmeSolved();
    }

    protected virtual void OnEnigmeSolved()
    {
        Debug.Log("L'�nigme est r�solue!");
    }
}
