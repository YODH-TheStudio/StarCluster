using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    private static readonly Color PRIVATE_DEFAULT_COLOR = Color.white;
    private static readonly Color PRIVATE_ACTIVE_COLOR = Color.green;

    private Renderer _renderer;


    [SerializeField] private string TagToDetect = "Cube";

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _renderer.material.color = PRIVATE_DEFAULT_COLOR;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagToDetect))
        {
            _renderer.material.color = PRIVATE_ACTIVE_COLOR;
            Debug.Log("[PlatformTrigger] Cube détecté - Plateforme activée.");
        }
    }

}
