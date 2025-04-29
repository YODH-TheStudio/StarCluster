using UnityEngine;

public class InteractionButton : MonoBehaviour
{
    private PlayerScript _playerScript;
    
    private void Start()
    {
        _playerScript = GameManager.Instance.GetPlayer();
    }
    
    public void Interact()
    {
        _playerScript.OnInteract();
    }
}
