using UnityEngine;


// interface, Parent class for Interactable Entities
public abstract class Interactable : MonoBehaviour
{
    #region Fields
    // transform of interact user
    protected Transform UserTransform;

    #endregion

    #region Properties
    public virtual void Interact()
    {
        //Debug.Log($"{gameObject.name} a été interagi.");
    }


    public void SetUserTransform(Transform newTransform)
    {
        UserTransform = newTransform;
    }

    #endregion
}
