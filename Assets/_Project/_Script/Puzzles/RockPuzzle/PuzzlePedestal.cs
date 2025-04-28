using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PuzzlePedestal : MonoBehaviour
{

    #region Fields
    [SerializeField]
    private FusionPoint _fusionPoint;
    
    [SerializeField]
    private List<PedestalData> _pedestalDataList = new List<PedestalData>();

    [SerializeField]
    private float validationRadius = 1.5f;

    [SerializeField] private MeshRenderer _mainStoneMesh;

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
    
    #endregion

    #region Main Functions
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
    #endregion

    #region Puzzle
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
        if (_fusionPoint)
        {
            StartCoroutine(AtlassiumAnimation(3));
            
            _fusionPoint.SetState(true);
        }
    }
    #endregion

    #region Atlassium

    private void DeactivateAllAtlassium()
    {
        foreach (var pair in _pedestalDataList)
        {
            if (pair.pushPullObject != null)
            {
                pair.pushPullObject.DeactivateAtlassium();
            }
        }
        
        Material[] materials = _mainStoneMesh.materials;
        materials = new Material[1] { materials[0] };
        _mainStoneMesh.materials = materials;
    }
    
    

    #endregion

    #region Coroutine

    private IEnumerator AtlassiumAnimation(float duration)
    {
        float elapsedTime = 0f;
        Material[] materials = _mainStoneMesh.materials;
        _mainStoneMesh.materials = materials;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            float colorValue = Mathf.Lerp(0f, 1f, t);

            materials[1].SetFloat("_ColorSlider", colorValue);
            _mainStoneMesh.materials = materials;

            yield return null; // Wait for the next frame
        }

        // Ensure the final value is set
        materials[1].SetFloat("_ColorSlider", 1f);
        _mainStoneMesh.materials = materials;

        elapsedTime = 0f;
        
        float baseAlpha = materials[1].GetFloat("_Alpha");
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            float colorValue = Mathf.Lerp(baseAlpha, 0f, t);

            materials[1].SetFloat("_Alpha", colorValue);
            _mainStoneMesh.materials = materials;

            foreach (var pair in _pedestalDataList)
            {
                if (pair.pushPullObject != null)
                {
                    pair.pushPullObject.SetAtlassiumAlpha(colorValue);
                }
            }
            
            yield return null; // Wait for the next frame
        }
        
        // Ensure the final value is set
        materials[1].SetFloat("_Alpha", 0f);
        _mainStoneMesh.materials = materials;
        
        DeactivateAllAtlassium();
    }

    #endregion
}
