using UnityEngine;


// interface, Parent class for Interactable Entities
public abstract class Interactable : MonoBehaviour
{
    // transform of interact user
    protected Transform _userTransform;

    public virtual void Interact()
    {
        Debug.Log($"{gameObject.name} a �t� interagi.");
    }

    //
    public void SetUserTransform(Transform newTransform)
    {
        _userTransform = newTransform;
        Debug.Log($"Le Transform de l'utilisateur a �t� d�fini sur : {_userTransform.position}");
    }
}
