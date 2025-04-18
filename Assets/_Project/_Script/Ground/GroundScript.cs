using UnityEngine;

public class GroundScript : MonoBehaviour
{
    #region Fields

    [SerializeField] private new GameObject particleSystem;

    public bool IsOnGrass { get; private set; }

    #endregion

    #region Triggers
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        
        IsOnGrass = true;
        other.gameObject.GetComponent<PlayerScript>().SpawnParticle(particleSystem);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        IsOnGrass = false;
        other.gameObject.GetComponent<PlayerScript>().DeleteParticle(particleSystem);
        
    }
    #endregion
}
