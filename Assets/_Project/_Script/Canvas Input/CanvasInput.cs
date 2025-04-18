using UnityEngine;

public class CanvasInput : MonoBehaviour
{
    #region Fields
    [SerializeField] private GameObject interactionButton;
    #endregion

    #region Main Functions
    private void Start()
    {
        GameManager.Instance.GetPlayer().gameObject.GetComponent<PlayerInteractionZone>().SetInteractionButton(interactionButton);
    }
    #endregion
}
