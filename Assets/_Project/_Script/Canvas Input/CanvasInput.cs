using UnityEngine;

public class CanvasInput : MonoBehaviour
{
    [SerializeField] private GameObject interactionButton;
    
    private void Start()
    {
        GameManager.Instance.GetPlayer().gameObject.GetComponent<PlayerInteractionZone>().SetInteractionButton(interactionButton);
    }
    
}
