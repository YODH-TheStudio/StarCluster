using System.Collections.Generic;
using UnityEngine;

public class ObjectFade: MonoBehaviour
{
    #region Fields
    [SerializeField] private float _fadeSpeed = 2f;

    [SerializeField] private float _fadeAmount = 0.25f;

    private PlayerScript _player;
    private Camera _mainCamera;
    private float _distanceCamToPlayer;
    private Vector3 _directionCamToPlayer;

    private List<GameObject> _hits;
    private List<GameObject> _oldHits;
    private List<GameObject> _toRemove;

    #endregion

    #region Main Functions
    private void Start()
    {
        _player = GameManager.Instance.GetPlayer();
        _mainCamera = Camera.main;

        _hits = new List<GameObject>();
        _oldHits = new List<GameObject>();
        _toRemove = new List<GameObject>();
    }

    private void FixedUpdate()
    {
        _distanceCamToPlayer = Vector3.Distance(_player.transform.position, _mainCamera.transform.position);
        _directionCamToPlayer = (_player.transform.position -  _mainCamera.transform.position).normalized;

        RaycastHit hit;
        if (!Physics.Raycast(_mainCamera.transform.position, _directionCamToPlayer, out hit, _distanceCamToPlayer)) return;
        
        if (!hit.transform.gameObject.CompareTag("Player"))
        {
            SendRaycast();

            List<GameObject> toUnfade = new List<GameObject>();
            foreach (GameObject value in _oldHits)
            {
                if(!value.transform.gameObject.CompareTag("Player"))
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

    #endregion

    #region Raycast
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

    #endregion

    #region Fade Function
    private void FadeObject(List<GameObject> hits)
    {
        foreach (GameObject hit in hits)
        {
            if (!hit.CompareTag("Player") && hit.GetComponent<MeshRenderer>())
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
            if (!hit.CompareTag("Player") && hit.GetComponent<MeshRenderer>())
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
    #endregion

    #region Material Mode
    private void ToOpaqueMode(Material material)
    {
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        material.SetInt("_ZWrite", 1);
        material.SetInt("_Surface", 0);

        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;

        material.SetShaderPassEnabled("DepthOnly", true);
        material.SetShaderPassEnabled("SHADOWCASTER", true);

        material.SetOverrideTag("RenderType", "Opaque");

        material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
    }

    private void ToTransparentMode(Material material)
    {
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.SetInt("_Surface", 1);

        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        material.SetShaderPassEnabled("DepthOnly", false);
        material.SetShaderPassEnabled("SHADOWCASTER", false);

        material.SetOverrideTag("RenderType", "Transparent");

        material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
    }
    #endregion
}
