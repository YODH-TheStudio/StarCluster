using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlatformTrigger : MonoBehaviour
{
    public PlatformSettings Settings; 
    
    private Renderer _renderer;

    public UnityEvent OnPlatformActivated;
    public UnityEvent OnPlatformDeactivated;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _renderer.material.color = Settings.DefaultColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        Activator activator = other.GetComponent<Activator>();
        if (activator != null && activator.Settings != null && activator.Settings.ActivatorId == Settings.ActivatorId)
        {
            _renderer.material.color = Settings.ActiveColor;
            OnPlatformActivated.Invoke();
            Debug.Log($"[PlatformTrigger] Activation par objet ID {activator.GetId()} - Couleur {Settings.ActiveColor}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Activator activator = other.GetComponent<Activator>();
        if (activator != null && activator.Settings != null && activator.Settings.ActivatorId == Settings.ActivatorId)
        {
            _renderer.material.color = Settings.DefaultColor;
            OnPlatformDeactivated.Invoke();
            Debug.Log($"[PlatformTrigger] D�sactivation - Retour � {Settings.DefaultColor}");
        }
    }
}
