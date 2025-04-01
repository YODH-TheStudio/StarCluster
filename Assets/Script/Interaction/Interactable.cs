using UnityEngine;


// interface, Parent class for Interactable Entities
public abstract class Interactable : MonoBehaviour
{
    // transform of interact user
    protected Transform _userTransform;
    protected Vector3 _userInteractionNormal;

    public virtual void Interact()
    {
        Debug.Log($"{gameObject.name} a été interagi.");
    }

    //
    public void SetUserTransform(Transform newTransform)
    {
        _userTransform = newTransform;
    }
    public void SetUserInteractionNormal(Vector3 interactionNormal)
    {
        _userInteractionNormal = interactionNormal;
        Debug.Log($"Le Transform de l'utilisateur a été défini sur : {_userTransform.position}");
    }
}
