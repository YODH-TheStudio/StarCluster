using UnityEngine;


// interface, Parent class for Interactable Entities
public abstract class Interactable : MonoBehaviour
{
    // transform of interact user
    protected Transform UserTransform;

    public virtual void Interact()
    {
        //Debug.Log($"{gameObject.name} a été interagi.");
    }

    public void SetUserTransform(Transform newTransform)
    {
        UserTransform = newTransform;
    }
}
