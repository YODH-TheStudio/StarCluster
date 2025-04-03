using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UIElements;

public class ObjectFade: MonoBehaviour
{
    [SerializeField]
    private float _fadeSpeed = 0.5f;

    [SerializeField]
    private float _fadeAmount = 0.5f;

    private PlayerScript _player;
    private Camera _mainCamera;
    private float _distanceCamToPlayer;
    private Vector3 _directionCamToPlayer;
    private List<RaycastHit> _hits;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameManager.Instance.GetPlayer();
        _mainCamera = Camera.main;

        _hits = new List<RaycastHit>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _distanceCamToPlayer = Vector3.Distance(_player.transform.position, _mainCamera.transform.position);
        _directionCamToPlayer = (_player.transform.position -  _mainCamera.transform.position).normalized;

        RaycastHit hit;
        if (Physics.Raycast(_mainCamera.transform.position, _directionCamToPlayer, out hit, _distanceCamToPlayer))
        {
            if (hit.transform.gameObject.tag != "Player")
            {
                SendRaycast();
                //FadeObject(_hits);
            }
            else
            {
                //UnfadeObject(_hits);
            }
        }
    }

    private void SendRaycast()
    {
        RaycastHit[] hits = Physics.RaycastAll(_mainCamera.transform.position, _directionCamToPlayer, _distanceCamToPlayer);
        if (hits != null && _hits != null) 
        {
            foreach(RaycastHit hit in hits)
            {
                _hits.Add(hit);
            }
        } 
    }

    private void ResetHits()
    {

    }

    /*
    private void FadeObject(List<RaycastHit> hits)
    {
        foreach (RaycastHit hit in hits)
        {

            if (hit.transform.gameObject.tag != "Player")
            {
                Material hitMaterial = hit.transform.gameObject.GetComponent<MeshRenderer>().material;
                ToFadeMode(hitMaterial);
                hitMaterial.color = new Color(hitMaterial.color.r, hitMaterial.color.g, hitMaterial.color.b, Mathf.Lerp(hitMaterial.color.a, _fadeAmount, _fadeSpeed * Time.deltaTime));
                hit.transform.gameObject.GetComponent<MeshRenderer>().material = hitMaterial;
            }
        }
    }

    private void UnfadeObject(List<RaycastHit> hits)
    {
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.gameObject.tag != "Player")
            {
                Material hitMaterial = hit.transform.gameObject.GetComponent<MeshRenderer>().material;
                hitMaterial.color = new Color(hitMaterial.color.r, hitMaterial.color.g, hitMaterial.color.b, Mathf.Lerp(hitMaterial.color.a, 1f, _fadeSpeed * Time.deltaTime));
                hit.transform.gameObject.GetComponent<MeshRenderer>().material = hitMaterial;
                //ToOpaqueMode(hitMaterial);
            }
        }
    }
    */
    private void ToOpaqueMode(Material material)
    {
        material.SetOverrideTag("RenderType", "");
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        material.SetInt("_ZWrite", 1);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = -1;
    }

    private void ToFadeMode(Material material)
    {
        material.SetOverrideTag("RenderType", "Transparent");
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }

    private IEnumerator CoroutineFadeObject(RaycastHit hit)
    {
        Material hitMaterial = hit.transform.gameObject.GetComponent<MeshRenderer>().material;

        while (Mathf.Lerp(hitMaterial.color.a, 1f, _fadeSpeed * Time.deltaTime) > _fadeAmount)
        {
            hitMaterial.color = new Color(hitMaterial.color.r, hitMaterial.color.g, hitMaterial.color.b, Mathf.Lerp(hitMaterial.color.a, _fadeAmount, _fadeSpeed * Time.deltaTime));
            hit.transform.gameObject.GetComponent<MeshRenderer>().material = hitMaterial;
        }
        yield return null;
    }

    private IEnumerator CoroutineUnfadeObject(RaycastHit hit)
    {
        Material hitMaterial = hit.transform.gameObject.GetComponent<MeshRenderer>().material;

        while (Mathf.Lerp(hitMaterial.color.a, 1f, _fadeSpeed * Time.deltaTime) < 1f)
        {
            hitMaterial.color = new Color(hitMaterial.color.r, hitMaterial.color.g, hitMaterial.color.b, Mathf.Lerp(hitMaterial.color.a, 1f, _fadeSpeed * Time.deltaTime));
            hit.transform.gameObject.GetComponent<MeshRenderer>().material = hitMaterial;
        }
        yield return null;
    }
}
