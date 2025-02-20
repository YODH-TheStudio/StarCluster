using System.Collections.Generic;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    public PlatformSettings Settings; 
    private Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _renderer.material.color = Settings.DefaultColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        Activator activator = other.GetComponent<Activator>();
        if (activator != null && activator.Settings.ActivatorId == Settings.ActivatorId)
        {
            _renderer.material.color = Settings.ActiveColor;
            Debug.Log($"[PlatformTrigger] Activation par objet ID {activator.GetId()} - Couleur {Settings.ActiveColor}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Activator activator = other.GetComponent<Activator>();
        if (activator != null && activator.Settings.ActivatorId == Settings.ActivatorId)
        {
            _renderer.material.color = Settings.DefaultColor;
            Debug.Log($"[PlatformTrigger] Désactivation - Retour à {Settings.DefaultColor}");
        }
    }
}
