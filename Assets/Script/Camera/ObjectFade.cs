using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class ObjectFade: MonoBehaviour
{
    [SerializeField]
    private float _fadeSpeed = 0.5f;

    [SerializeField]
    private float _fadeAmount = 0.5f;

    private PlayerScript _player;
    private Camera _mainCamera;

    private RaycastHit[] _hits = null;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameManager.Instance.GetPlayer();
        _mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(_player.transform.position, _mainCamera.transform.position))
        {
            if(_hits == null)
            {
                SendRaycast();
            }
            FadeObject(_hits);
        }
        else
        {
            if (_hits != null)
            {
                UnfadeObject(_hits);
            }
        }
    }

    private void SendRaycast()
    {
        _hits = Physics.RaycastAll(_player.transform.position, _mainCamera.transform.position);
    }

    private void ResetHits()
    {
        _hits = null;
    }

    private void FadeObject(RaycastHit[] hits)
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

    private void UnfadeObject(RaycastHit[] hits)
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
}
