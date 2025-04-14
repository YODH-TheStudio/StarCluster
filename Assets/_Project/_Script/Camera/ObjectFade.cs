using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UIElements;

public class ObjectFade: MonoBehaviour
{
    [SerializeField]
    private float _fadeSpeed = 2f;

    [SerializeField]
    private float _fadeAmount = 0.25f;

    private PlayerScript _player;
    private Camera _mainCamera;
    private float _distanceCamToPlayer;
    private Vector3 _directionCamToPlayer;

    private List<GameObject> _hits;
    private List<GameObject> _oldHits;
    private List<GameObject> _toRemove;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameManager.Instance.GetPlayer();
        _mainCamera = Camera.main;

        _hits = new List<GameObject>();
        _oldHits = new List<GameObject>();
        _toRemove = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        _distanceCamToPlayer = Vector3.Distance(_player.transform.position, _mainCamera.transform.position);
        _directionCamToPlayer = (_player.transform.position -  _mainCamera.transform.position).normalized;

        RaycastHit hit;
        if (Physics.Raycast(_mainCamera.transform.position, _directionCamToPlayer, out hit, _distanceCamToPlayer))
        {
            if (hit.transform.gameObject.tag != "Player")
            {
                SendRaycast();

                List<GameObject> toUnfade = new List<GameObject>();
                foreach (GameObject value in _oldHits)
                {
                    if(value.transform.gameObject.tag != "Player")
                    {
                        if (!_hits.Contains(value))
                        {
                            toUnfade.Add(value);
                        }
                    }
                    
                }

                UnfadeObject(toUnfade);

                foreach (GameObject obj in _toRemove)
                {
                    _hits.Remove(obj);
                }

                FadeObject(_hits);

                _toRemove.Clear();
            }
            else
            {
                UnfadeObject(_oldHits);

                foreach (GameObject obj in _toRemove)
                {
                    _hits.Remove(obj);
                }

                _toRemove.Clear();
            }
        }
    }

    private void SendRaycast()
    {
        _hits.Clear();
        RaycastHit[] hits = Physics.RaycastAll(_mainCamera.transform.position, _directionCamToPlayer, _distanceCamToPlayer);
        if (hits != null && _hits != null) 
        {
            foreach(RaycastHit hit in hits)
            {
                if(hit.transform.gameObject.tag != "Player")
                {
                    if (!_hits.Contains(hit.transform.gameObject))
                    {
                        _hits.Add(hit.transform.gameObject);

                        if (!_oldHits.Contains(hit.transform.gameObject))
                        {
                            _oldHits.Add(hit.transform.gameObject);
                        }
                        
                    }
                }
            }
        }
    }

    private void FadeObject(List<GameObject> hits)
    {
        foreach (GameObject hit in hits)
        {
            if (hit.tag != "Player")
            {
                Material hitMaterial = hit.GetComponent<MeshRenderer>().material;

                ToTransparentMode(hitMaterial);

                Color baseColor = hitMaterial.GetColor("_BaseColor");
                baseColor.a = Mathf.Lerp(baseColor.a, _fadeAmount, _fadeSpeed * Time.deltaTime);
                hitMaterial.SetColor("_BaseColor", baseColor);
            }
        }
    }

    private void UnfadeObject(List<GameObject> hits)
    {
        foreach (GameObject hit in hits)
        {
            if (hit.tag != "Player")
            {
                Material hitMaterial = hit.GetComponent<MeshRenderer>().material;

                Color baseColor = hitMaterial.GetColor("_BaseColor");
                baseColor.a = Mathf.Lerp(baseColor.a, 1f, _fadeSpeed * Time.deltaTime);
                hitMaterial.SetColor("_BaseColor", baseColor);

                if (baseColor.a >= 0.999f)
                {
                    ToOpaqueMode(hitMaterial);
                    _toRemove.Add(hit);
                }
            }
        }
    }

    private void ToOpaqueMode(Material material)
    {
        material.renderQueue = -1; // Auto
        material.SetFloat("_Surface", 0); // 0 = Opaque
        material.SetFloat("_Blend", 0); // 0 = Alpha (mais pas utilisé en opaque)
        material.SetFloat("_ZWrite", 1);
        material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
    }

    private void ToTransparentMode(Material material)
    {
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        material.SetFloat("_Surface", 1); // 1 = Transparent
        material.SetFloat("_Blend", 0); // 0 = Alpha
        material.SetFloat("_ZWrite", 0);
        material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
    }
}
